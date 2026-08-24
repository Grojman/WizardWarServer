using System.Text.Json;
using Serilog;

public class MatchSeries
{
    public enum SeriesPhase { Selecting, RoundInProgress, Finished }

    public const int RoundsToWin = 2;

    public Guid Id { get; } = Guid.NewGuid();
    public List<PlayerConnection> Connections { get; }
    public SeriesPhase Phase { get; private set; } = SeriesPhase.Selecting;
    public int RoundNumber { get; private set; } = 1;

    readonly Dictionary<Guid, int> Scores;
    Dictionary<Guid, int?> PendingSelection;
    readonly HashSet<int> UsedDeckIds = new();

    Guid? currentPicker;
    Guid? disconnectedPlayerId;
    GameSession? CurrentRound;

    readonly GameManager manager;

    public MatchSeries(List<PlayerConnection> connections, GameManager manager)
    {
        Connections = connections;
        this.manager = manager;

        foreach (var c in Connections) c.CurrentSeries = this;

        Scores = Connections.ToDictionary(p => p.Guid, p => 0);
        PendingSelection = Connections.ToDictionary(p => p.Guid, p => (int?)null);
        currentPicker = Connections.GetRandom().Guid;
    }

    PlayerConnection GetRival(PlayerConnection player) => Connections.First(p => p.Guid != player.Guid);

    public async Task Start()
    {
        Log.Information("Match series {SeriesId} starting between {Players}", Id, string.Join(", ", Connections.Select(c => c.Guid)));

        Phase = SeriesPhase.Selecting;
        RoundNumber = 1;

        await BroadcastSeriesState();
    }

    public async Task HandleAction(PlayerConnection player, string json)
    {
        if (player is null || string.IsNullOrWhiteSpace(json)) return;

        PlayerAction? action;

        try
        {
            action = JsonSerializer.Deserialize<PlayerAction>(json);
        }
        catch (JsonException ex)
        {
            Log.Warning(ex, "Received invalid series JSON from player {PlayerId}", player.Guid);
            await player.SendError("Your last action could not be understood by the server.");
            return;
        }

        await HandleAction(player, action);
    }

    public async Task HandleAction(PlayerConnection player, PlayerAction? action)
    {
        if (player is null || action is null || !Connections.Contains(player)) return;

        try
        {
            switch (action)
            {
                case PlayerAction.GetDecksAction:
                    await player.Send("get_decks", CardManager.Decks);
                    break;
                case PlayerAction.SelectSeriesDeckAction a:
                    await TrySelectDeck(player, a.DeckId);
                    break;
                case PlayerAction.RequestSeriesStateAction:
                    await SendSeriesStateTo(player);
                    break;
                case PlayerAction.LeaveGame:
                    await RemovePlayer(player);
                    break;
                default:
                    // In-round actions are routed straight to the round's GameSession
                    // (player.Game is checked before player.CurrentSeries), so anything
                    // else arriving here is unexpected while a series is in progress.
                    break;
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Unexpected error while handling series action from player {PlayerId}", player.Guid);
            await player.SendError("Something went wrong processing your action. Please try again.");
        }
    }

    async Task TrySelectDeck(PlayerConnection player, int deckId)
    {
        if (Phase != SeriesPhase.Selecting) return;

        if (!CardManager.Decks.Any(d => d.id == deckId))
        {
            await player.SendError("Unknown deck id.");
            return;
        }

        if (UsedDeckIds.Contains(deckId))
        {
            await player.SendError("Ese mazo ya ha sido usado en esta serie.");
            return;
        }

        if (PendingSelection.TryGetValue(player.Guid, out var existing) && existing.HasValue) return;

        if (RoundNumber < 3)
        {
            var rival = GetRival(player);
            if (PendingSelection.TryGetValue(rival.Guid, out var rivalPick) && rivalPick == deckId)
            {
                await player.SendError("Tu rival ya ha elegido ese mazo.");
                return;
            }
        }

        PendingSelection[player.Guid] = deckId;
        currentPicker = Connections[0].Guid == currentPicker ? Connections[1].Guid : Connections[0].Guid;

        await BroadcastSeriesState();

        if (Connections.All(p => PendingSelection.TryGetValue(p.Guid, out var v) && v.HasValue))
        {
            await StartRound();
        }
    }

    async Task StartRound()
    {
        foreach (var p in Connections)
        {
            p.SelectedDeckId = PendingSelection[p.Guid]!.Value;
        }

        Phase = SeriesPhase.RoundInProgress;

        CurrentRound = new GameSession([.. Connections], manager, series: this);

        manager.RegisterGameSession(CurrentRound);

        await CurrentRound.Start();
    }

    public async Task OnRoundEnded(Guid? winner, bool forced)
    {
        if (Phase != SeriesPhase.RoundInProgress) return;

        foreach (var p in Connections)
        {
            if (PendingSelection.TryGetValue(p.Guid, out var d) && d.HasValue) UsedDeckIds.Add(d.Value);
        }

        if (currentPicker.HasValue)
        {
            currentPicker = Connections[0].Guid == currentPicker ? Connections[1].Guid : Connections[0].Guid; 
        }

        CurrentRound = null;

        if (disconnectedPlayerId.HasValue)
        {
            var remaining = Connections.FirstOrDefault(p => p.Guid != disconnectedPlayerId.Value);
            await EndSeries(remaining?.Guid ?? winner ?? Guid.Empty, forfeited: true);
            return;
        }

        if (winner.HasValue) Scores[winner.Value] = Scores.GetValueOrDefault(winner.Value) + 1;

        if (winner.HasValue && Scores[winner.Value] >= RoundsToWin)
        {
            await EndSeries(winner.Value, forfeited: false);
            return;
        }

        RoundNumber++;
        PendingSelection = Connections.ToDictionary(p => p.Guid, p => (int?)null);
        Phase = SeriesPhase.Selecting;

        await BroadcastSeriesState();
    }

    // Only reached while no round is running (still picking a deck) - GameManager routes a
    // disconnect during a round straight to the round's GameSession.HandleDisconnect instead,
    // which grants a grace period and calls MarkDisconnected only if it truly expires.
    public async Task RemovePlayer(PlayerConnection player)
    {
        if (!Connections.Contains(player) || Phase == SeriesPhase.Finished) return;

        var rival = GetRival(player);

        await EndSeries(rival.Guid, forfeited: true);
    }

    public void MarkDisconnected(Guid playerId) => disconnectedPlayerId = playerId;

    async Task EndSeries(Guid winnerId, bool forfeited)
    {
        Phase = SeriesPhase.Finished;

        Log.Information("Match series {SeriesId} ended. Winner: {Winner} Forfeited: {Forfeited}", Id, winnerId, forfeited);

        var msg = new
        {
            seriesId = Id,
            winnerId,
            forfeited,
            scores = Connections.Select(p => new { playerId = p.Guid, name = p.Name, score = Scores.GetValueOrDefault(p.Guid) })
        };

        foreach (var p in Connections) await p.Send("series_end", msg);

        manager.RemoveSeries(this, Connections);
    }

    async Task BroadcastSeriesState()
    {
        foreach (var p in Connections) await SendSeriesStateTo(p);
    }

    async Task SendSeriesStateTo(PlayerConnection recipient)
    {
        var rival = GetRival(recipient);

        int? mySelected = PendingSelection.TryGetValue(recipient.Guid, out var mine) ? mine : null;
        int? rivalSelected = PendingSelection.TryGetValue(rival.Guid, out var theirs) ? theirs : null;

        string rivalStatus;
        int? rivalDeckId;
        string myStatus;

        if (RoundNumber >= 3)
        {
            rivalStatus = "hidden";
            rivalDeckId = null;
            myStatus = mySelected.HasValue ? "waiting_you" : "selecting";
        }
        else
        {
            if (currentPicker == rival.Guid)
            {
                rivalStatus = rivalSelected.HasValue ? "waiting" : "selecting";
            } else
            {
                rivalStatus = "waiting_you";
            }
            rivalDeckId = rivalSelected;

            if (currentPicker == recipient.Guid)
            {
                myStatus = mySelected.HasValue ? "waiting_you" : "selecting";
            } else
            {
                myStatus = "waiting";
            }
        }


        var reserved = new HashSet<int>(UsedDeckIds);
        if (RoundNumber < 3 && rivalSelected.HasValue) reserved.Add(rivalSelected.Value);

        var availableDecks = CardManager.Decks.Where(d => !reserved.Contains(d.id));

        var msg = new
        {
            seriesId = Id,
            round = RoundNumber,
            roundsToWin = RoundsToWin,
            scores = Connections.Select(p => new { playerId = p.Guid, name = p.Name, score = Scores.GetValueOrDefault(p.Guid) }),
            you = new { playerId = recipient.Guid, name = recipient.Name, status = myStatus, deckId = mySelected },
            rival = new { playerId = rival.Guid, name = rival.Name, status = rivalStatus, deckId = rivalDeckId },
            availableDecks
        };

        await recipient.Send("series_state", msg);
    }

    public override string ToString()
    {
        return $"[SERIES] Id: {Id} Round: {RoundNumber}/3 Phase: {Phase} Scores: {string.Join(", ", Scores.Select(kv => $"{kv.Key}={kv.Value}"))}";
    }
}
