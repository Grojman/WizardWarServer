using System.Net.WebSockets;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        StoringData.GetFromFile();
        var builder = WebApplication.CreateBuilder(args);

        // builder.WebHost.UseUrls("http://10.158.7.72:5182");
        builder.WebHost.UseUrls($"https://localhost:5182");
        var app = builder.Build();

        app.UseWebSockets();


        GameManager gameManager = new();

        CardManager.SerializeCards(MockData.Cards);

        foreach (var p in MockData.Decks) CardManager.SerializeDeck(p.Key, p.Value);

        CardManager.Initialize();

        app.Map("/", async context =>
        {
           context.Response.StatusCode = 200;
        });

        app.Map("/ws", async context =>
        {
            Console.WriteLine($"Connection found");
            if (context.WebSockets.IsWebSocketRequest)
            {
                Console.WriteLine($"Is web socket");

                var socket = await context.WebSockets.AcceptWebSocketAsync();

                var player = new PlayerConnection(socket);

                gameManager.AddPlayer(player);

                Console.WriteLine($"Current connections: {gameManager.PlayerCount}");

                await ReceiveLoop(player, gameManager);

            }
            else
            {
                context.Response.StatusCode = 400;
            }
        });

        new Thread(app.Run).Start();

        // app.Run();

        async Task ReceiveLoop(PlayerConnection player, GameManager manager)
        {
            try
            {
                var buffer = new byte[4096];

                while (player.Socket.State == WebSocketState.Open)
                {
                    var result = await player.Socket.ReceiveAsync(
                        new ArraySegment<byte>(buffer),
                        CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close || result.Count == 0)
                    {
                        Console.WriteLine("Socket is closed, removing...");
                        break;
                    }

                    var json = Encoding.UTF8.GetString(buffer, 0, result.Count);

                    Console.WriteLine($"Json recieved: {json}");

                    await manager.HandleMessage(player, json);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Socket session has ended because of an exception: {e}");
            }

            manager.RemovePlayer(player);
        }

        while(true)
        {
            string res = Console.ReadLine() ?? "";

            switch(res)
            {
                case "players":
                    gameManager.PrintPlayers();       
                break;
                case "games":
                    gameManager.PrintGames();
                break;
                case "cards":
                    MockData.PrintData();
                    break;
            }
        }
    }
}