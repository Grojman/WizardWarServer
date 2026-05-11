using System.Text.Json;

public class GameSession
{  
    IEnumerable<PlayerConnection> Connections;

    GameState state;
    
    readonly GameManager manager;

    public GameSession(
        IEnumerable<PlayerConnection> connections,
        GameManager manager)
    {
        Connections = connections;

        foreach(var c in Connections) c.Game = this;

        this.manager = manager;

        state = new GameState();
    }

    public async Task Start()
    {
        foreach(var c in Connections) await c.Send("start_game", null);

        state.Initialize(Connections);

        await SendState();
    }

    public async Task HandleAction(PlayerConnection player, string json)
    {
        var action =
            JsonSerializer.Deserialize<PlayerAction>(json);
        await HandleAction(player, action);
    }

    public async Task HandleAction(
        PlayerConnection player,
        PlayerAction action)
    {
        if (action is PlayerAction.TextMessage m)
        {
            foreach(var c in Connections) await c.Send("text_message", new {
                player = player.Guid,
                message = m.Message
            });

            return;
        }

        state.ApplyAction(player, action);

        await SendState();

        if(state.GameActionResult.GameEnded)
        {
            await End(state.GameActionResult.Winner);
        }
    }

    async Task SendState()
    {
        foreach(var c in Connections)
        {
            await c.Send("game_state", GameStateDto.Generate(state.GetState(c.Guid), [.. state.GetRivals(c.Guid)], state));
            await c.Send("game_events", state.GameActionResult.Events);
        }

        state.GameActionResult.Events.Clear();
    }

    public async Task End(Guid? winner, bool forced = false)
    {
        var msg = new
        {
            winner,
            forced
        };

        foreach(var c in Connections) await c.Send("end_game", msg); 

        manager.RemoveGameSession(this, Connections);
    }

    
}