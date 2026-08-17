using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.RateLimiting;
using Serilog;
internal class Program
{
    private const string ConnectRateLimiterPolicy = "ws-connect";

    private static async Task<int> Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        try
        {
            Log.Information("Starting WizardWarServer");

            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((context, services, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration));

            var serverOptions = builder.Configuration
                .GetSection(ServerOptions.SectionName)
                .Get<ServerOptions>() ?? new ServerOptions();

            builder.Services.AddSingleton(serverOptions);

            var certificatePath = Path.Combine(builder.Environment.ContentRootPath, "certificate.pfx");
            if (File.Exists(certificatePath))
            {
                var certificatePassword = Environment.GetEnvironmentVariable("CERT_PASSWORD");

                builder.WebHost.ConfigureKestrel(kestrelOptions =>
                {
                    kestrelOptions.ConfigureHttpsDefaults(httpsOptions =>
                    {
                        httpsOptions.ServerCertificate = X509CertificateLoader.LoadPkcs12FromFile(certificatePath, certificatePassword);
                    });
                });

                Log.Information("Loaded HTTPS certificate from {CertificatePath}", certificatePath);
            }
            else
            {
                Log.Information("No certificate.pfx found at {CertificatePath}; using default Kestrel HTTPS configuration", certificatePath);
            }

            builder.Services.AddRateLimiter(rateLimiterOptions =>
            {
                rateLimiterOptions.OnRejected = (context, _) =>
                {
                    Log.Warning(
                        "Rejected WebSocket connection attempt from {RemoteIp} (connection rate limit exceeded)",
                        context.HttpContext.Connection.RemoteIpAddress);
                    return ValueTask.CompletedTask;
                };

                rateLimiterOptions.AddPolicy(ConnectRateLimiterPolicy, httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = Math.Max(1, serverOptions.ConnectionRateLimitPerMinute),
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0
                        }));
            });

            var app = builder.Build();

            app.UseRateLimiter();

            app.UseWebSockets();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            StoringData.Configure(serverOptions);
            StoringData.GetFromFile();

            CardManager.Configure(serverOptions.DataDirectory);

            var forceSeed = args.Contains("--seed");
            if (forceSeed || !CardManager.DataFilesExist())
            {
                Log.Information(
                    "Seeding card/deck data from MockData ({Reason})",
                    forceSeed ? "explicit --seed flag" : "data files missing");

                CardManager.SerializeCards(MockData.Cards);
                foreach (var p in MockData.Decks) CardManager.SerializeDeck(p.Key, p.Value);
            }
            else
            {
                Log.Information("Existing card/deck data files found; skipping MockData seed (pass --seed to force regeneration)");
            }

            CardManager.Initialize();

            GameManager gameManager = new(serverOptions);

            app.Map("/ws", async context =>
            {
                if (!context.WebSockets.IsWebSocketRequest)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }

                if (!IsOriginAllowed(context, serverOptions))
                {
                    Log.Warning(
                        "Rejected WebSocket upgrade from disallowed origin {Origin} ({RemoteIp})",
                        context.Request.Headers.Origin.ToString(),
                        context.Connection.RemoteIpAddress);
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return;
                }

                var socket = await context.WebSockets.AcceptWebSocketAsync();

                Guid.TryParse(context.Request.Query["clientId"].ToString(), out var clientId);

                var player = new PlayerConnection(socket) { ClientId = clientId };

                await gameManager.AddPlayer(player);

                var resumed = await gameManager.TryResumeGame(player);

                Log.Information(
                    "Player {PlayerId} connected from {RemoteIp} (resumed: {Resumed})",
                    player.Guid, context.Connection.RemoteIpAddress, resumed);

                await ReceiveLoop(player, gameManager, serverOptions, app.Lifetime.ApplicationStopping);
            }).RequireRateLimiting(ConnectRateLimiterPolicy);

            app.Lifetime.ApplicationStopping.Register(() =>
            {
                Log.Information("Server is stopping, closing active connections...");
                gameManager.CloseAllConnectionsAsync().GetAwaiter().GetResult();
            });

            if (app.Environment.IsDevelopment())
            {
                _ = Task.Run(() => RunDebugConsole(gameManager, app.Lifetime.ApplicationStopping));
            }

            await app.RunAsync();

            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "WizardWarServer terminated unexpectedly");
            return 1;
        }
        finally
        {
            Log.Information("WizardWarServer stopped");
            await Log.CloseAndFlushAsync();
        }
    }

    private static bool IsOriginAllowed(HttpContext context, ServerOptions options)
    {
        if (options.AllowedOrigins is null || options.AllowedOrigins.Length == 0) return true;

        var origin = context.Request.Headers.Origin.ToString();

        // Non-browser clients (native game clients) typically don't send an Origin header at all.
        if (string.IsNullOrEmpty(origin)) return true;

        return options.AllowedOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase);
    }

    private static async Task ReceiveLoop(
        PlayerConnection player,
        GameManager manager,
        ServerOptions options,
        CancellationToken stoppingToken)
    {
        using var rateLimiter = new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions
        {
            TokenLimit = Math.Max(1, options.MessageRateLimitPerSecond),
            TokensPerPeriod = Math.Max(1, options.MessageRateLimitPerSecond),
            ReplenishmentPeriod = TimeSpan.FromSeconds(1),
            AutoReplenishment = true,
            QueueLimit = 0
        });

        try
        {
            var buffer = new byte[8192];

            using var messageStream = new MemoryStream();

            while (player.Socket.State == WebSocketState.Open)
            {
                messageStream.SetLength(0);

                WebSocketReceiveResult result;
                do
                {
                    result = await player.Socket.ReceiveAsync(new ArraySegment<byte>(buffer), stoppingToken);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        Log.Information("Player {PlayerId} closed the connection", player.Guid);
                        return;
                    }

                    if (messageStream.Length + result.Count > options.MaxMessageSizeBytes)
                    {
                        Log.Warning(
                            "Player {PlayerId} sent a message exceeding the {MaxSize} byte limit; closing connection",
                            player.Guid, options.MaxMessageSizeBytes);
                        await player.Socket.CloseAsync(WebSocketCloseStatus.MessageTooBig, "Message too large", CancellationToken.None);
                        return;
                    }

                    messageStream.Write(buffer, 0, result.Count);
                } while (!result.EndOfMessage);

                if (messageStream.Length == 0) continue;

                using var lease = rateLimiter.AttemptAcquire();
                if (!lease.IsAcquired)
                {
                    Log.Warning("Player {PlayerId} exceeded the message rate limit; closing connection", player.Guid);
                    await player.Socket.CloseAsync(WebSocketCloseStatus.PolicyViolation, "Rate limit exceeded", CancellationToken.None);
                    return;
                }

                var json = Encoding.UTF8.GetString(messageStream.GetBuffer(), 0, (int)messageStream.Length);

                await manager.HandleMessage(player, json);
            }
        }
        catch (OperationCanceledException)
        {
            Log.Information("Receive loop for player {PlayerId} cancelled (server shutting down)", player.Guid);
        }
        catch (WebSocketException wsEx)
        {
            Log.Information("WebSocket session for player {PlayerId} ended: {Message}", player.Guid, wsEx.Message);
        }
        catch (Exception e)
        {
            Log.Error(e, "Socket session for player {PlayerId} ended because of an unexpected exception", player.Guid);
        }
        finally
        {
            if (player.Socket.State == WebSocketState.Open)
            {
                try
                {
                    await player.Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                }
                catch (Exception e)
                {
                    Log.Debug(e, "Failed to close socket cleanly for player {PlayerId}", player.Guid);
                }
            }

            await manager.RemovePlayer(player);
        }
    }

    private static void RunDebugConsole(GameManager gameManager, CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            string res = Console.ReadLine() ?? "";

            switch (res)
            {
                case "players":
                    gameManager.PrintPlayers();
                    break;
                case "games":
                    gameManager.PrintGames();
                    break;
                case "series":
                    gameManager.PrintSeries();
                    break;
                case "cards":
                    MockData.PrintData();
                    break;
            }
        }
    }
}
