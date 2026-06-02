
public class ReviveLastPlayed : IEffect
{
    public ReviveLastPlayed(CardFilter? filter, Dictionary<Destination, int> destinations, bool toMe)
    {
        Filter = filter;
        Destinations = destinations;
        ToMe = toMe;
    }

    public CardFilter? Filter { get; set; } = null;
    public Dictionary<Destination, int> Destinations { get; set; } = new(); 
    public bool ToMe { get; set; } = true;
    public IEffect Clone() => new ReviveLastPlayed(Filter, Destinations, ToMe);

    public void Execute(Guid playerId, Guid rivalId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        var cards = new List<CardInstance>();

        int total = Destinations.Values.Sum();

        var player = ToMe ? cardId.Player : cardId.Player.PlayerTarget;

        for(int i = cardId.Player.PlayedCards.Count - 1; i > 0 && cards.Count < total; i--)
        {
            if (Filter is null || Filter.Check(cardId.Player.PlayedCards[i]))
            {
                cards.Add(cardId.Player.PlayedCards[i]);
            }
        }

        foreach(var kvp in Destinations)
        {
            bool stop = false;
            int counter = 0;
            
            while(cards.Count != 0 && !stop && counter < kvp.Value)
            {
                counter++;
                CardInstance card = new(cards[0].Definition, player) ;
                cards.RemoveAt(0);

                switch (kvp.Key)
                {
                    case Destination.HAND:
                        state.DrawCard(player, card);
                        break;
                    case Destination.BOARD:
                        var boardPosition = player.Board.FindFirstNullPosition();
                        if(boardPosition != -1)
                        {
                            state.PlayCard(player, card, boardPosition);
                        } else
                        {
                            cards.Insert(0, card);
                            stop = true;
                        }
                        break;
                    case Destination.DECK:
                        state.AddCard(player, cardId.Player, card, cardId);
                        break;
                }
            }

        }

    }
}