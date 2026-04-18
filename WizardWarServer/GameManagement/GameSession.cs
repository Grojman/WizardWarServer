using System.Text.Json;

public class GameSession
{
    PlayerConnection p1;
    PlayerConnection p2;

    GameState state;
    
    readonly GameManager manager;

    public GameSession(
        PlayerConnection a,
        PlayerConnection b,
        GameManager manager)
    {
        p1 = a;
        p2 = b;

        p1.Game = this;
        p2.Game = this;

        this.manager = manager;

        state = new GameState();
    }

    public async Task Start()
    {
        await p1.Send("start_game", null);
        await p2.Send("start_game", null);

        state.Initialize(p1, p2);

        await SendState();
    }

    public async Task HandleAction(
        PlayerConnection player,
        string json)
    {
        var action =
            JsonSerializer.Deserialize<PlayerAction>(json);

        state.ApplyAction(player, action);

        await SendState();

        if(state.GameActionResult.GameEnded)
        {
            await End(state.GameActionResult.Winner);
        }
    }

    async Task SendState()
    {
        await p1.Send("game_state", GameStateDto.Generate(state.Player1, state.Player2, state.CurrentTurn == 1));
        await p2.Send("game_state", GameStateDto.Generate(state.Player2, state.Player1, state.CurrentTurn == 2));

        await p1.Send("game_events", state.GameActionResult.Events.Select(GameEventDto.Generate));
        await p2.Send("game_events", state.GameActionResult.Events.Select(GameEventDto.Generate));

        state.GameActionResult.Events.Clear();
    }

    public async Task End(Guid? winner, bool forced = false)
    {
        var msg = new
        {
            winner,
            forced
        };
        await p1.Send("end_game", msg);

        await p2.Send("end_game", msg);

        manager.RemoveGameSession(this, p1, p2);
    }

    
}