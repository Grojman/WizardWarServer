using System.Globalization;
using System.Text;
using System.Text.Json;
using Serilog;
public class GameSession
{
    bool botSession = false;
    List<PlayerConnection> Connections;
    readonly Guid Id;
    GameState state;
    readonly GameManager manager;
    readonly MatchSeries? series;
    readonly Guid? forcedStarterId;

    readonly object _sync = new();
    readonly Dictionary<Guid, CancellationTokenSource> disconnectGraceTimers = new();

    // Every player's receive loop (plus bots and grace-period timers) calls into
    // this session on its own task. Anything that reads or mutates `state` or
    // broadcasts it goes through this lock, so two players acting at the same
    // time can't interleave inside the rules engine. Methods suffixed *Core
    // expect the caller to already hold it (SemaphoreSlim isn't reentrant).
    readonly SemaphoreSlim actionLock = new(1, 1);

    public bool HasEnded { get => state.GameActionResult.GameEnded; }

    public GameSession(
        List<PlayerConnection> connections,
        GameManager manager, bool botSession = false,
        MatchSeries? series = null,
        Guid? forcedStarterId = null)
    {
        Id = Guid.NewGuid();
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

        await actionLock.WaitAsync();
        try
        {
            foreach(var c in Connections) await c.Send("start_game", new { Id });

            state.Initialize(Connections, forcedStarterId);

            await SendState();
        }
        finally
        {
            actionLock.Release();
        }
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
            await player.SendError(TranslationKeys.ErrActionNotUnderstood);
            return;
        }

        await HandleAction(player, action);
    }

    public async Task HandleAction(
        PlayerConnection player,
        PlayerAction? action)
    {
        if (player is null || action is null)
        {
            return;
        }

        await actionLock.WaitAsync();
        try
        {
            await HandleActionCore(player, action);
        }
        finally
        {
            actionLock.Release();
        }
    }

    async Task HandleActionCore(PlayerConnection player, PlayerAction action)
    {
        if (!Connections.Contains(player) || state.GameActionResult.GameEnded)
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
                // Same signal a grace-period timeout gives MatchSeries (see
                // GraceTimeoutAsync): without it, OnRoundEnded would treat this
                // as a normal round loss and move the series on to the next
                // round, leaving this player's CurrentSeries pointed at a
                // series it just walked away from instead of ending it outright.
                series?.MarkDisconnected(player.Guid);
                await RemovePlayerCore(player);
                return;
            } else if (action is PlayerAction.Surrender)
            {
                state.Surrender(state.GetState(player.Guid));

                await SendState();

                if (state.GameActionResult.GameEnded)
                {
                    await EndCore(state.GameActionResult.Winner);
                }
                return;
            } else if (action is PlayerAction.ChangeLanguageAction la)
            {
                TranslationManager.TryResolveLanguage(la.Language, out var resolvedLanguage);
                player.Language = resolvedLanguage;
                await player.SendTranslations();
                await player.Send("game_state", GameStateDto.Generate(state.GetState(player.Guid), [.. state.GetRivals(player.Guid)], state, player.Language, Id));
                return;
            }

            if (!state.ApplyAction(player, action))
            {
                // Rejected (e.g. not their turn): nothing changed, but the
                // client is waiting on a game_state before it lets the player
                // act again, and evidently has a stale view of the match.
                await player.Send("game_state", GameStateDto.Generate(state.GetState(player.Guid), [.. state.GetRivals(player.Guid)], state, player.Language, Id));
                return;
            }

            await SendState();

            if(state.GameActionResult.GameEnded)
            {
                await EndCore(state.GameActionResult.Winner);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unexpected error while handling in-game action from player {PlayerId}", player.Guid);

            // Whatever part of the action did get applied, everyone must see it
            // or their clients drift apart from the server (and each other).
            try
            {
                await SendState();
            }
            catch (Exception sendEx)
            {
                Log.Error(sendEx, "Could not resync game state after a failed action");
                state.GameActionResult.Events.Clear();
            }

            await player.SendError(TranslationKeys.ErrActionFailed);
        }
    }

    async Task SendState(bool isReconnect = false)
    {
        var rawEvents = state.GameActionResult.Events.ToList();
        state.GameActionResult.Events.Clear();

        foreach(var c in Connections.ToList())
        {
            // One player's snapshot failing to build must not leave everyone
            // after them in the list without the update.
            try
            {
                await c.Send("game_state", GameStateDto.Generate(state.GetState(c.Guid), [.. state.GetRivals(c.Guid)], state, c.Language, Id, isReconnect));
                await c.Send("game_events", rawEvents.Select(e => GameEventDto.Generate(e, state, c.Language)).ToList());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Could not send game state to player {PlayerId}", c.Guid);
            }
        }
    }

    public async Task End(Guid? winner, bool forced = false)
    {
        await actionLock.WaitAsync();
        try
        {
            await EndCore(winner, forced);
        }
        finally
        {
            actionLock.Release();
        }
    }

    async Task EndCore(Guid? winner, bool forced = false)
    {
        Log.Information("Game session ended. Winner: {Winner} Forced: {Forced}", winner, forced);

        StoringData.SaveData(state, forced);
        StoringData.SaveInFile();

        // Built before ClearState(), which empties the player list the
        // defeat reasons live on.
        var msg = new
        {
            winner,
            forced,
            isSeriesRound = series is not null,
            defeats = Defeats()
        };

        foreach(var c in Connections) await c.Send("end_game", msg);
        state.ClearState();
        manager.RemoveGameSession(this, Connections);

        if (series is not null) await series.OnRoundEnded(winner, forced);
    }

    // Why every knocked-out player lost, so both the loser and their rivals
    // can be told.
    List<object> Defeats() => state.Players
        .Where(p => p.DefeatReason is not null)
        .Select(p => (object)new { playerId = p.Id, reason = p.DefeatReason })
        .ToList();

    public async Task RemovePlayer(PlayerConnection c)
    {
        await actionLock.WaitAsync();
        try
        {
            await RemovePlayerCore(c);
        }
        finally
        {
            actionLock.Release();
        }
    }

    async Task RemovePlayerCore(PlayerConnection c)
    {
        if (c is null || !Connections.Contains(c))
        {
            return;
        }

        state.KillPlayer(state.GetState(c.Guid), DefeatReason.Disconnect, true);
        c.Game = null;
        Connections.Remove(c);

        // c is already out of Connections by this point, so it won't be
        // included in the SendState()/End() broadcasts below — tell it
        // directly that it's out (e.g. a player cancelling a resumed match
        // from the home page needs its own client-side ack to re-enable
        // the "start new game" buttons).
        await c.Send("end_game", new
        {
            winner = state.GameActionResult.Winner,
            forced = true,
            isSeriesRound = series is not null,
            defeats = Defeats()
        });

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
            await EndCore(state.GameActionResult.Winner);
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
            newConnection.Language = oldConnection.Language;
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

        await actionLock.WaitAsync();
        try
        {
            foreach (var c in Connections.Where(c => c != newConnection))
            {
                await c.Send("opponent_reconnected", new { playerId = newConnection.Guid });
            }

            await SendState(isReconnect: true);
        }
        finally
        {
            actionLock.Release();
        }

        return true;
    }

    public override string ToString()
    {
        StringBuilder sr = new($"[GAME] Bot? : {botSession} Nº Players: {Connections.Count} Ended: {HasEnded}\n");

        foreach(var p in Connections) sr.AppendLine(p.ToString()); 

        return sr.ToString();
    }
}