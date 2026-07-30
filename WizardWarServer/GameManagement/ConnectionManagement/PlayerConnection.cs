using System.Net.WebSockets;
using Serilog;

public class PlayerConnection
{
    public int SelectedDeckId { get; set; } = -1;
    public Guid Guid = Guid.NewGuid();
    public WebSocket Socket { get; }

    public string Name { get; set; } = string.Empty;

    public GameSession? Game { get; set; }

    public int NumberOfPlayersInGame { get; set; } = -1;

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

            await Socket.SendAsync(
                bytes,
                WebSocketMessageType.Text,
                true,
                CancellationToken.None);    
        } catch(Exception e)
        {
            Log.Warning(e, "Couldn't send message to player {PlayerId}", Guid);
        }
        
    }
}