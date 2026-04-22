using System.Text.Json;

public class GameManager
{
    public int PlayerCount { get => players.Count; }
    List<PlayerConnection> players = new();
    List<PlayerConnection> queue = new();

    List<GameSession> games = new();

    public async void AddPlayer(PlayerConnection player)
    {
        players.Add(player);
    }

    public async Task QueuePlayer(PlayerConnection player)
    {
        queue.Add(player);

        if (queue.Count >= 2)
        {
            var p1 = queue[0];
            var p2 = queue[1];

            queue.RemoveRange(0, 2);

            var game = new GameSession(p1, p2, this);

            games.Add(game);

            await game.Start();
        }
    }

    public async void RemovePlayer(PlayerConnection player)
    {
        if (players.Contains(player)) players.Remove(player);
        queue.Remove(player);

        if (player.Game != null)
        {
            await player.Game.End(null, true);
        }
    }

    public async Task UnqueuePlayer(PlayerConnection player)
    {
        queue.Remove(player);

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
            var action = JsonSerializer.Deserialize<UserAction>(json);

            switch(action)
            {
                case UserAction.ChangeNameAction a:
                    player.Name = a.NewName;
                    break;
                case UserAction.JoinQueueAction b:
                    player.SelectedDeckId = b.DeckId;
                    QueuePlayer(player);
                    break;
                case UserAction.LeaveQueueAction:
                    UnqueuePlayer(player);
                    break;
                case UserAction.GetDecksAction:
                    await player.Send("get_decks", CardManager.Decks);
                    break;
                case UserAction.GetAllCardsAction:
                    await player.Send("get_cards", CardManager.Cards.Select(n => CardDto.Generate(n.Value)));
                    break;
                default:
                    Console.WriteLine($"Unauthorized message!! {json}");
                    break;
            }
        }
    }

    public async void RemoveGameSession(GameSession session, PlayerConnection c1, PlayerConnection c2)
    {
        games.Remove(session);
        c1.Game = c2.Game = null;
    }
}