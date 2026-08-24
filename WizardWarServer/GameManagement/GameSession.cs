using System.Globalization;
using System.Text;
using System.Text.Json;
using Serilog;
public class GameSession
{
    bool botSession = false;
    List<PlayerConnection> Connections;

    GameState state;
    readonly GameManager manager;
    readonly MatchSeries? series;
    readonly Guid? forcedStarterId;

    readonly object _sync = new();
    readonly Dictionary<Guid, CancellationTokenSource> disconnectGraceTimers = new();

    public bool HasEnded { get => state.GameActionResult.GameEnded; }

    public GameSession(
        List<PlayerConnection> connections,
        GameManager manager, bool botSession = false,
        MatchSeries? series = null,
        Guid? forcedStarterId = null)
    {
        Connections = connections;

        foreach(var c in Connections) c.Game = this;

        this.manager = manager;

        state = new GameState();

        this.botSession = botSession;
        this.series = series;
        this.forcedStarterId = forcedStarterId;
    }

    public async Task Start()
    {
        Log.Information("Game session starting with {PlayerCount} players (bot session: {IsBotSession})", Connections.Count, botSession);

        foreach(var c in Connections) await c.Send("start_game", new { });

        state.Initialize(Connections, forcedStarterId);

        

        await SendState();
    }

    public async Task HandleAction(PlayerConnection player, string json)
    {
        if (player is null || string.IsNullOrWhiteSpace(json))
        {
            return;
        }

        PlayerAction? action;

        try
        {
            action = JsonSerializer.Deserialize<PlayerAction>(json);
        }
        catch (JsonException ex)
        {
            Log.Warning(ex, "Received invalid in-game JSON from player {PlayerId}", player.Guid);
            await player.SendError("Your last action could not be understood by the server.");
            return;
        }

        await HandleAction(player, action);
    }

    public async Task HandleAction(
        PlayerConnection player,
        PlayerAction? action)
    {
        if (player is null || action is null || !Connections.Contains(player))
        {
            return;
        }

        try
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
        catch (Exception ex)
        {
            Log.Error(ex, "Unexpected error while handling in-game action from player {PlayerId}", player.Guid);
            await player.SendError("Something went wrong processing your action. Please try again.");
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
            forced,
            isSeriesRound = series is not null
        };

        Log.Information("Game session ended. Winner: {Winner} Forced: {Forced}", winner, forced);

        StoringData.SaveData(state, forced);
        StoringData.SaveInFile();

        foreach(var c in Connections) await c.Send("end_game", msg);
        state.ClearState();
        manager.RemoveGameSession(this, Connections);

        if (series is not null) await series.OnRoundEnded(winner, forced);
    }

    public async Task RemovePlayer(PlayerConnection c)
    {
        if (c is null || !Connections.Contains(c))
        {
            return;
        }

        state.KillPlayer(state.GetState(c.Guid), true);
        c.Game = null;
        Connections.Remove(c);

        if(botSession)
        {
            manager.RemoveGameSession(this, Connections);
            Connections.Clear();
            state.ClearState();
            return;
        }

        if(state.GameActionResult.GameEnded)
        {
            // End() already broadcasts end_game and clears state; sending
            // state afterward would look up players in an already-cleared list.
            await End(state.GameActionResult.Winner);
            return;
        }

        await SendState();
    }

    public async Task HandleDisconnect(PlayerConnection player)
    {
        if (player is null || !Connections.Contains(player))
        {
            return;
        }

        if (botSession)
        {
            // No other human is waiting on a bot game, so there's nothing to gain from a grace period.
            await RemovePlayer(player);
            return;
        }

        var cts = new CancellationTokenSource();

        lock (_sync)
        {
            disconnectGraceTimers[player.Guid] = cts;
        }

        manager.RegisterPendingResume(player.ClientId, this, player.Guid);

        var secondsToWait = manager.Options.DisconnectGracePeriodSeconds;

        foreach (var c in Connections.Where(c => c != player))
        {
            await c.Send("opponent_disconnected", new { playerId = player.Guid, secondsToWait });
        }

        _ = GraceTimeoutAsync(player, secondsToWait, cts.Token);
    }

    async Task GraceTimeoutAsync(PlayerConnection player, int secondsToWait, CancellationToken token)
    {
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(secondsToWait), token);
        }
        catch (TaskCanceledException)
        {
            return;
        }

        bool stillPending;
        lock (_sync)
        {
            stillPending = disconnectGraceTimers.Remove(player.Guid);
        }

        if (!stillPending) return;

        manager.ClearPendingResume(player.ClientId);
        series?.MarkDisconnected(player.Guid);

        await RemovePlayer(player);
    }

    public async Task<bool> TryReconnect(PlayerConnection newConnection, Guid targetPlayerGuid)
    {
        CancellationTokenSource? cts;
        int index;

        lock (_sync)
        {
            if (!disconnectGraceTimers.TryGetValue(targetPlayerGuid, out cts))
            {
                return false;
            }

            index = Connections.FindIndex(c => c.Guid == targetPlayerGuid);
            if (index == -1)
            {
                disconnectGraceTimers.Remove(targetPlayerGuid);
                return false;
            }

            var oldConnection = Connections[index];

            newConnection.Guid = oldConnection.Guid;
            newConnection.Name = oldConnection.Name;
            newConnection.ClientId = oldConnection.ClientId;
            newConnection.SelectedDeckId = oldConnection.SelectedDeckId;
            newConnection.NumberOfPlayersInGame = oldConnection.NumberOfPlayersInGame;
            newConnection.Game = this;
            newConnection.CurrentSeries = series;

            Connections[index] = newConnection;

            disconnectGraceTimers.Remove(targetPlayerGuid);
        }

        cts.Cancel();

        Log.Information("Player {PlayerId} reconnected", newConnection.Guid);

        foreach (var c in Connections.Where(c => c != newConnection))
        {
            await c.Send("opponent_reconnected", new { playerId = newConnection.Guid });
        }

        await SendState();

        return true;
    }

    public override string ToString()
    {
        StringBuilder sr = new($"[GAME] Bot? : {botSession} Nº Players: {Connections.Count} Ended: {HasEnded}\n");

        foreach(var p in Connections) sr.AppendLine(p.ToString()); 

        return sr.ToString();
    }
}