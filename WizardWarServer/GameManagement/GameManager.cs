using System.Text.Json;

public class GameManager
{
    public int PlayerCount { get => players.Count; }
    public const int MAX_LENGHT_PLAYER_NAME = 40;
    List<PlayerConnection> players = new();
    Dictionary<int, List<PlayerConnection>> queue = new();

    List<GameSession> games = new();

    public Task AddPlayer(PlayerConnection player)
    {
        players.Add(player);
        return Task.CompletedTask;
    }

    async Task CheckQueue(int n, List<PlayerConnection> players)
    {
        if (players.Count >= n)
        {
            var playersList = players.GetRange(0, n);

            players.RemoveRange(0, n);

            var game = new GameSession(playersList, this);

            games.Add(game);

            await game.Start();
        }
    }

    public async Task QueuePlayer(PlayerConnection player)
    {
        if (!queue.ContainsKey(player.NumberOfPlayersInGame)) queue[player.NumberOfPlayersInGame] = new();

        queue[player.NumberOfPlayersInGame].Add(player);

        await CheckQueue(player.NumberOfPlayersInGame, queue[player.NumberOfPlayersInGame]);
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

        games.Add(game);

        await game.Start();
    }

    public async Task RemovePlayer(PlayerConnection player)
    {
        players.Remove(player);

        if(queue.TryGetValue(player.NumberOfPlayersInGame, out List<PlayerConnection>? value)) value.Remove(player);

        if (player.Game is not null) await player.Game.RemovePlayer(player);
    }
    public async Task UnqueuePlayer(PlayerConnection player)
    {
        queue[player.NumberOfPlayersInGame].Remove(player);

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
            Console.WriteLine("Not a game action");
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
                        player.SelectedDeckId = c.DeckId;
                        player.NumberOfPlayersInGame = c.NumberOfPlayers;
                        await AddBotGame(player);
                        break;
                    case UserAction.JoinQueueAction b:
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
                    default:
                        Console.WriteLine($"Unauthorized message!! {json}");
                        break;
                }
            }
            catch (JsonException)
            {
                Console.WriteLine("JSON inválido");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            
        }
    }

    public void RemoveGameSession(GameSession session, IEnumerable<PlayerConnection> connections)
    {
        games.Remove(session);
        foreach (var c in connections) c.Game = null;
    }

    public void PrintPlayers()
    {
        Console.WriteLine($"Player count: {PlayerCount}");
        foreach(var g in players) Console.WriteLine(g);
    }

    public void PrintGames()
    {
        Console.WriteLine($"Games count: {games.Count()}");
        foreach(var g in games) Console.WriteLine(g);
    }
}