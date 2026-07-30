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
        }

        Log.Information("Player {PlayerId} disconnected", player.Guid);

        if (player.Game is not null) await player.Game.RemovePlayer(player);
    }
    public async Task UnqueuePlayer(PlayerConnection player)
    {
        lock (_sync)
        {
            if (queue.TryGetValue(player.NumberOfPlayersInGame, out var value)) value.Remove(player);
        }

        if (player.Game != null)
        {
            await player.Game.End(null, true);
        }
    }

    public async Task HandleMessage(
        PlayerConnection player,
        string json)
    {
        if (player.Game != null)
        {
            await player.Game.HandleAction(
                player,
                json);
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
                            await player.Send("error", new { message = "Invalid number of players." });
                            break;
                        }
                        if (!CardManager.Decks.Any(d => d.id == c.DeckId))
                        {
                            await player.Send("error", new { message = "Unknown deck id." });
                            break;
                        }
                        player.SelectedDeckId = c.DeckId;
                        player.NumberOfPlayersInGame = c.NumberOfPlayers;
                        await AddBotGame(player);
                        break;
                    case UserAction.JoinQueueAction b:
                        if (!options.IsValidPlayerCount(b.NumberOfPlayers))
                        {
                            await player.Send("error", new { message = "Invalid number of players." });
                            break;
                        }
                        if (!CardManager.Decks.Any(d => d.id == b.DeckId))
                        {
                            await player.Send("error", new { message = "Unknown deck id." });
                            break;
                        }
                        player.SelectedDeckId = b.DeckId;
                        player.NumberOfPlayersInGame = b.NumberOfPlayers;
                        await QueuePlayer(player);
                        break;
                    case UserAction.LeaveQueueAction:
                        await UnqueuePlayer(player);
                        break;
                    case UserAction.GetDecksAction:
                        await player.Send("get_decks", CardManager.Decks);
                        break;
                    case UserAction.GetAllCardsAction:
                        await player.Send("get_cards", CardManager.Decks.Select(n => new {n.name, cards = CardManager.GetDefinitionsByDeck(n.id).Select(l =>new KeyValuePair<CardDto, int>(CardDto.Generate(l.Key), l.Value))}));
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
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error while handling message from player {PlayerId}", player.Guid);
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
}
