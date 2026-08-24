public class ForcePlayCardInHandEffect : IEffect
{
    public ForcePlayCardInHandEffect(PlayerType player, CardFilter? filter, int maxCards)
    {
        Player = player;
        Filter = filter;
        MaxCards = maxCards;
    }

    public PlayerType Player { get; set; }
    public CardFilter? Filter { get; set; }
    public int MaxCards { get; set; }

    public IEffect Clone() => new ForcePlayCardInHandEffect(Player, Filter, MaxCards);

    public void Execute(Guid playerId, Guid rivalId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        PlayerState[] players = Player switch
        {
            PlayerType.RIVAL => [cardId.Player.PlayerTarget],
            PlayerType.BOTH => [cardId.Player.PlayerTarget, cardId.Player],
            PlayerType.NONE => [],
            PlayerType.PLAYER => [cardId.Player],
            _ => []
        };

        int played = 0;

        foreach(var player in players)
        {
            for (int i = 0; i < player.Hand.Count; i++)
            {
                if (played >= MaxCards) return;

                CardInstance c = player.Hand[i];
                if (Filter?.Check(c) ?? true)
                {
                    var boardIndexes = player.Board.Select((a,b) => new {a, b}).Where(n => n.a is null).Select(n => n.b);

                    if(c.Definition.Type == CardType.Spell || boardIndexes.Any())
                    {
                        var targetIndex = boardIndexes.Any() ? boardIndexes.GetRandom() : -1;
                        state.PlayCard(player.Connection, c.Id, targetIndex);
                        played++;
                    }
                }
            }
        }


    }
}