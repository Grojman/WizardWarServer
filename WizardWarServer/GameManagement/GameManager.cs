using System.Text.Json;
using Serilog;
public class GameManager
{
    public int PlayerCount { get => players.Count; }
    public const int MAX_LENGHT_PLAYER_NAME = 40;

    private readonly object _sync = new();

    readonly List<PlayerConnection> players = new();
    readonly Dictionary<int, List<PlayerConnection>> queue = new();

    readonly List<GameSession> games = new();

    readonly List<PlayerConnection> seriesQueue = new();
    readonly List<MatchSeries> activeSeries = new();

    readonly Dictionary<string, PrivateMatchLobby> privateMatches = new();
    static readonly char[] PrivateMatchCodeAlphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789".ToCharArray();
    const int PrivateMatchCodeLength = 6;

    readonly ServerOptions options;

    public GameManager(ServerOptions options)
    {
        this.options = options;
    }

    public Task AddPlayer(PlayerConnection player)
    {
        lock (_sync)
        {
            players.Add(player);
        }
        return Task.CompletedTask;
    }

    async Task CheckQueue(int n, List<PlayerConnection> queuedPlayers)
    {
        List<PlayerConnection>? playersList = null;

        lock (_sync)
        {
            if (queuedPlayers.Count >= n)
            {
                playersList = queuedPlayers.GetRange(0, n);
                queuedPlayers.RemoveRange(0, n);
            }
        }

        if (playersList is null) return;

        var game = new GameSession(playersList, this);

        lock (_sync)
        {
            games.Add(game);
        }

        await game.Start();
    }

    public async Task QueuePlayer(PlayerConnection player)
    {
        List<PlayerConnection> playerQueue;

        lock (_sync)
        {
            if (!queue.TryGetValue(player.NumberOfPlayersInGame, out var value))
            {
                value = new();
                queue[player.NumberOfPlayersInGame] = value;
            }

            value.Add(player);
            playerQueue = value;
        }

        await CheckQueue(player.NumberOfPlayersInGame, playerQueue);
    }

    async Task CheckSeriesQueue()
    {
        List<PlayerConnection>? playersList = null;

        lock (_sync)
        {
            if (seriesQueue.Count >= 2)
            {
                playersList = seriesQueue.GetRange(0, 2);
                seriesQueue.RemoveRange(0, 2);
            }
        }

        if (playersList is null) return;

        var series = new MatchSeries(playersList, this);

        lock (_sync)
        {
            activeSeries.Add(series);
        }

        await series.Start();
    }

    public async Task QueueSeriesPlayer(PlayerConnection player)
    {
        lock (_sync)
        {
            seriesQueue.Add(player);
        }

        await CheckSeriesQueue();
    }

    public void RegisterGameSession(GameSession session)
    {
        lock (_sync)
        {
            games.Add(session);
        }
    }

    public void RemoveSeries(MatchSeries series, IEnumerable<PlayerConnection> connections)
    {
        lock (_sync)
        {
            activeSeries.Remove(series);
        }
        foreach (var c in connections) c.CurrentSeries = null;
    }

    public async Task AddBotGame(PlayerConnection player)
    {
        var botList = new List<PlayerConnection>
        {
            player
        };
        for (int i = 0; i < player.NumberOfPlayersInGame - 1; i++)
        {
            botList.Add(new BotConnection());
        }

        var game = new GameSession(botList, this, true);

        lock (_sync)
        {
            games.Add(game);
        }

        await game.Start();
    }

    public async Task RemovePlayer(PlayerConnection player)
    {
        lock (_sync)
        {
            players.Remove(player);

            if (queue.TryGetValue(player.NumberOfPlayersInGame, out List<PlayerConnection>? value)) value.Remove(player);
            seriesQueue.Remove(player);
        }

        Log.Information("Player {PlayerId} disconnected", player.Guid);

        await LeavePrivateMatch(player);

        // A series player's round-in-progress GameSession is also reachable via player.Game,
        // but MatchSeries.RemovePlayer must be the entry point so it can flag the disconnect
        // and terminate the whole series once the round it delegates to has ended.
        if (player.CurrentSeries is not null) await player.CurrentSeries.RemovePlayer(player);
        else if (player.Game is not null) await player.Game.RemovePlayer(player);
    }

    string GenerateUniquePrivateMatchCode()
    {
        Span<char> buffer = stackalloc char[PrivateMatchCodeLength];
        string code;

        do
        {
            for (int i = 0; i < PrivateMatchCodeLength; i++)
            {
                buffer[i] = PrivateMatchCodeAlphabet[Random.Shared.Next(PrivateMatchCodeAlphabet.Length)];
            }
            code = new string(buffer);
        } while (privateMatches.ContainsKey(code));

        return code;
    }

    public async Task CreatePrivateMatch(PlayerConnection player, MatchFormat format = MatchFormat.Single)
    {
        await UnqueuePlayer(player);
        await LeavePrivateMatch(player);

        PrivateMatchLobby lobby;

        lock (_sync)
        {
            var code = GenerateUniquePrivateMatchCode();

            lobby = new PrivateMatchLobby
            {
                Code = code,
                Host = player,
                NumberOfPlayers = player.NumberOfPlayersInGame,
                Format = format
            };
            lobby.Players.Add(player);

            privateMatches[code] = lobby;
            player.PendingPrivateMatch = lobby;
        }

        Log.Information("Player {PlayerId} created private match {Code}", player.Guid, lobby.Code);

        await player.Send("private_match_created", new
        {
            code = lobby.Code,
            current = lobby.Players.Count,
            total = lobby.NumberOfPlayers,
            isHost = true
        });
    }

    public async Task JoinPrivateMatch(PlayerConnection player, string? rawCode)
    {
        var code = rawCode?.Trim().ToUpperInvariant() ?? string.Empty;

        if (code.Length == 0)
        {
            await player.SendError("Introduce un código de partida.");
            return;
        }

        await UnqueuePlayer(player);
        await LeavePrivateMatch(player);

        List<PlayerConnection>? startGameList = null;
        PrivateMatchLobby? lobby;
        List<PlayerConnection>? notifyList = null;
        bool notFound = false;

        lock (_sync)
        {
            if (!privateMatches.TryGetValue(code, out lobby))
            {
                notFound = true;
            }
            else if (lobby.Players.Contains(player))
            {
                // Already in this lobby (e.g. duplicate message); nothing to do.
            }
            else
            {
                lobby.Players.Add(player);
                player.PendingPrivateMatch = lobby;

                if (lobby.Players.Count >= lobby.NumberOfPlayers)
                {
                    privateMatches.Remove(lobby.Code);
                    startGameList = new List<PlayerConnection>(lobby.Players);
                    foreach (var p in startGameList) p.PendingPrivateMatch = null;
                }
                else
                {
                    notifyList = new List<PlayerConnection>(lobby.Players);
                }
            }
        }

        if (notFound)
        {
            await player.SendError("No existe ninguna partida privada con ese código.");
            return;
        }

        if (startGameList is not null)
        {
            if (lobby!.Format == MatchFormat.BestOfThree)
            {
                Log.Information("Private match {Code} filled up, starting best-of-3 series", code);

                var series = new MatchSeries(startGameList, this);

                lock (_sync)
                {
                    activeSeries.Add(series);
                }

                await series.Start();
                return;
            }

            Log.Information("Private match {Code} filled up, starting game", code);

            var game = new GameSession(startGameList, this);

            lock (_sync)
            {
                games.Add(game);
            }

            await game.Start();
            return;
        }

        if (notifyList is not null && lobby is not null)
        {
            foreach (var p in notifyList)
            {
                await p.Send("private_match_update", new
                {
                    code = lobby.Code,
                    current = lobby.Players.Count,
                    total = lobby.NumberOfPlayers,
                    isHost = p == lobby.Host
                });
            }
        }
    }

    public async Task LeavePrivateMatch(PlayerConnection player)
    {
        PrivateMatchLobby? lobby;
        List<PlayerConnection>? notifyList = null;

        lock (_sync)
        {
            lobby = player.PendingPrivateMatch;
            if (lobby is null) return;

            player.PendingPrivateMatch = null;
            lobby.Players.Remove(player);

            if (lobby.Players.Count == 0)
            {
                privateMatches.Remove(lobby.Code);
            }
            else
            {
                if (lobby.Host == player)
                {
                    lobby.Host = lobby.Players[0];
                }
                notifyList = new List<PlayerConnection>(lobby.Players);
            }
        }

        if (notifyList is null || lobby is null) return;

        foreach (var p in notifyList)
        {
            await p.Send("private_match_update", new
            {
                code = lobby.Code,
                current = lobby.Players.Count,
                total = lobby.NumberOfPlayers,
                isHost = p == lobby.Host
            });
        }
    }
    public async Task UnqueuePlayer(PlayerConnection player)
    {
        lock (_sync)
        {
            if (queue.TryGetValue(player.NumberOfPlayersInGame, out var value)) value.Remove(player);
            seriesQueue.Remove(player);
        }

        // A player already inside a series (selecting or mid-round) cannot reach this
        // path in practice, since HandleMessage routes them to MatchSeries.HandleAction
        // instead of the lobby UserAction switch that calls UnqueuePlayer.
        if (player.Game != null)
        {
            await player.Game.End(null, true);
        }
    }

    public async Task HandleMessage(
        PlayerConnection player,
        string json)
    {
        var game = player.Game;
        var series = player.CurrentSeries;

        if (game != null)
        {
            try
            {
                await game.HandleAction(
                    player,
                    json);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error while handling in-game message from player {PlayerId}", player.Guid);
                await player.SendError("Something went wrong processing your action. Please try again.");
            }
        } else if (series != null)
        {
            try
            {
                await series.HandleAction(player, json);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error while handling series message from player {PlayerId}", player.Guid);
                await player.SendError("Something went wrong processing your action. Please try again.");
            }
        } else
        {
            UserAction? action = null;

            try
            {
                action = JsonSerializer.Deserialize<UserAction>(json);

                switch(action)
                {
                    case UserAction.ChangeNameAction a:
                        if (a.NewName.Length > MAX_LENGHT_PLAYER_NAME)
                        {
                            a.NewName = a.NewName.Substring(0, MAX_LENGHT_PLAYER_NAME);
                        }
                        player.Name = a.NewName;
                        break;
                    case UserAction.StartBotGameAction c:
                        if (!options.IsValidPlayerCount(c.NumberOfPlayers))
                        {
                            await player.SendError("Invalid number of players.");
                            break;
                        }
                        if (!CardManager.Decks.Any(d => d.id == c.DeckId))
                        {
                            await player.SendError("Unknown deck id.");
                            break;
                        }
                        player.SelectedDeckId = c.DeckId;
                        player.NumberOfPlayersInGame = c.NumberOfPlayers;
                        await AddBotGame(player);
                        break;
                    case UserAction.JoinQueueAction b:
                        if (b.Format == MatchFormat.BestOfThree)
                        {
                            if (b.NumberOfPlayers != 2)
                            {
                                await player.SendError("Best-of-3 matches require exactly 2 players.");
                                break;
                            }
                        }
                        else if (!options.IsValidPlayerCount(b.NumberOfPlayers))
                        {
                            await player.SendError("Invalid number of players.");
                            break;
                        }
                        if (b.Format != MatchFormat.BestOfThree && !CardManager.Decks.Any(d => d.id == b.DeckId))
                        {
                            await player.SendError("Unknown deck id.");
                            break;
                        }
                        player.SelectedDeckId = b.DeckId;
                        player.NumberOfPlayersInGame = b.NumberOfPlayers;
                        if (b.Format == MatchFormat.BestOfThree) await QueueSeriesPlayer(player);
                        else await QueuePlayer(player);
                        break;
                    case UserAction.LeaveQueueAction:
                        await UnqueuePlayer(player);
                        break;
                    case UserAction.CreatePrivateMatchAction e:
                        if (e.Format == MatchFormat.BestOfThree)
                        {
                            if (e.NumberOfPlayers != 2)
                            {
                                await player.SendError("Best-of-3 matches require exactly 2 players.");
                                break;
                            }
                        }
                        else if (!options.IsValidPlayerCount(e.NumberOfPlayers))
                        {
                            await player.SendError("Invalid number of players.");
                            break;
                        }
                        if (!CardManager.Decks.Any(d => d.id == e.DeckId))
                        {
                            await player.SendError("Unknown deck id.");
                            break;
                        }
                        player.SelectedDeckId = e.DeckId;
                        player.NumberOfPlayersInGame = e.NumberOfPlayers;
                        await CreatePrivateMatch(player, e.Format);
                        break;
                    case UserAction.JoinPrivateMatchAction f:
                        if (!CardManager.Decks.Any(d => d.id == f.DeckId))
                        {
                            await player.SendError("Unknown deck id.");
                            break;
                        }
                        player.SelectedDeckId = f.DeckId;
                        await JoinPrivateMatch(player, f.Code);
                        break;
                    case UserAction.LeavePrivateMatchAction:
                        await LeavePrivateMatch(player);
                        break;
                    case UserAction.GetDecksAction:
                        await player.Send("get_decks", CardManager.Decks);
                        break;
                    case UserAction.GetAllCardsAction:
                        await player.Send("get_cards", CardManager.Decks.Select(n => new {n.name, cards = CardManager.GetDefinitionsByDeck(n.id).Select(l =>new KeyValuePair<CardDto, int>(CardDto.Generate(l.Key), l.Value))}));
                        break;
                    case UserAction.GetStatsAction:
                        await player.Send("get_stats", StoringData.GetStats());
                        break;
                    case UserAction.SendSuggestion d:
                        StoringData.SaveSuggestion(d.Suggestion);
                        break;
                    default:
                        Log.Warning("Unrecognized message from player {PlayerId}: {Json}", player.Guid, json);
                        break;
                }
            }
            catch (JsonException ex)
            {
                Log.Warning(ex, "Received invalid JSON from player {PlayerId}", player.Guid);
                await player.SendError("Your last message could not be understood by the server.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while handling message from player {PlayerId}", player.Guid);
                await player.SendError("Something went wrong processing your request. Please try again.");
            }


        }
    }

    public void RemoveGameSession(GameSession session, IEnumerable<PlayerConnection> connections)
    {
        lock (_sync)
        {
            games.Remove(session);
        }
        foreach (var c in connections) c.Game = null;
    }

    public async Task CloseAllConnectionsAsync()
    {
        List<PlayerConnection> snapshot;
        lock (_sync)
        {
            snapshot = new List<PlayerConnection>(players);
        }

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

        foreach (var player in snapshot)
        {
            try
            {
                if (player.Socket.State == System.Net.WebSockets.WebSocketState.Open)
                {
                    await player.Socket.CloseAsync(
                        System.Net.WebSockets.WebSocketCloseStatus.NormalClosure,
                        "Server is shutting down",
                        cts.Token);
                }
            }
            catch (Exception ex)
            {
                Log.Debug(ex, "Failed to close socket cleanly for player {PlayerId} during shutdown", player.Guid);
            }
        }
    }

    public void PrintPlayers()
    {
        List<PlayerConnection> snapshot;
        lock (_sync)
        {
            snapshot = new List<PlayerConnection>(players);
        }

        Console.WriteLine($"Player count: {snapshot.Count}");
        foreach(var g in snapshot) Console.WriteLine(g);
    }

    public void PrintGames()
    {
        List<GameSession> snapshot;
        lock (_sync)
        {
            snapshot = new List<GameSession>(games);
        }

        Console.WriteLine($"Games count: {snapshot.Count}");
        foreach(var g in snapshot) Console.WriteLine(g);
    }

    public void PrintSeries()
    {
        List<MatchSeries> snapshot;
        lock (_sync)
        {
            snapshot = new List<MatchSeries>(activeSeries);
        }

        Console.WriteLine($"Active series count: {snapshot.Count}");
        foreach(var s in snapshot) Console.WriteLine(s);
    }
}
