using System.Net.WebSockets;
using Serilog;

public class PlayerConnection
{
    public int SelectedDeckId { get; set; } = -1;
    public Guid Guid = Guid.NewGuid();
    public Guid ClientId { get; set; } = Guid.Empty;
    public WebSocket Socket { get; }

    public string Name { get; set; } = string.Empty;

    public string Language { get; set; } = TranslationManager.DefaultLanguage;

    public GameSession? Game { get; set; }

    public MatchSeries? CurrentSeries { get; set; }

    public PrivateMatchLobby? PendingPrivateMatch { get; set; }

    public int NumberOfPlayersInGame { get; set; } = -1;

    // A WebSocket allows only one SendAsync in flight at a time; a second
    // concurrent one throws and that message is silently lost. Sends to the
    // same player come from many tasks (their game, other players' receive
    // loops broadcasting the player count, bots, grace timers...), so they
    // queue up here instead.
    readonly SemaphoreSlim sendLock = new(1, 1);
    static readonly TimeSpan SendTimeout = TimeSpan.FromSeconds(15);

    public PlayerConnection(WebSocket socket)
    {
        Socket = socket;
    }

    public override string ToString()
    {
        return $"[CONECTION] Player: {Name} Id: {Guid} In-game: {Game is not null}";
    }

    public virtual async Task Send(string type, object obj)
    {

        try
        {
            var json = System.Text.Json.JsonSerializer.Serialize(
            new JsonMessage(
                type,
                obj
            )
            );

            var bytes = System.Text.Encoding.UTF8.GetBytes(json);

            if (Socket.State != WebSocketState.Open) return;

            await sendLock.WaitAsync();
            try
            {
                // A client that stopped reading must not stall everyone else
                // waiting on this lock (or on the game session's).
                using var timeout = new CancellationTokenSource(SendTimeout);
                await Socket.SendAsync(
                    bytes,
                    WebSocketMessageType.Text,
                    true,
                    timeout.Token);
            }
            finally
            {
                sendLock.Release();
            }
        } catch(Exception e)
        {
            Log.Warning(e, "Couldn't send message to player {PlayerId}", Guid);
        }

    }

    public Task SendError(string key)
    {
        return Send("error", new { message = TranslationManager.Get(key, Language) });
    }

    public Task SendTranslations()
    {
        return Send("translations", new { language = Language, values = TranslationManager.GetAll(Language) });
    }
}