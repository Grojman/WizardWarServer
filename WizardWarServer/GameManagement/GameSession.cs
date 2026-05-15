using System.Text.Json;

public class GameSession
{  
    bool botSession = false;
    List<PlayerConnection> Connections;

    GameState state;
    
    readonly GameManager manager;

    public bool HasEnded { get => state.GameActionResult.GameEnded; }

    public GameSession(
        List<PlayerConnection> connections,
        GameManager manager, bool botSession = false)
    {
        Connections = connections;

        foreach(var c in Connections) c.Game = this;

        this.manager = manager;

        state = new GameState();

        this.botSession = botSession;
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
        } else if (action is PlayerAction.LeaveGame)
        {
            await RemovePlayer(player);
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
        var events = state.GameActionResult.Events.ToList();
        foreach(var c in Connections)
        {
            await c.Send("game_state", GameStateDto.Generate(state.GetState(c.Guid), [.. state.GetRivals(c.Guid)], state));
            await c.Send("game_events", events);
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
        state.ClearState();
        manager.RemoveGameSession(this, Connections);
    }

    public async Task RemovePlayer(PlayerConnection c)
    {
        state.KillPlayer(state.GetState(c.Guid), true);
        c.Game = null;
        Connections.Remove(c);

        if(botSession)
        {
            manager.RemoveGameSession(this, Connections);
            Connections.Clear();
            state.ClearState();
        } else if(state.GameActionResult.GameEnded)
        {
            await End(state.GameActionResult.Winner);
        }

        await SendState();
    }
}