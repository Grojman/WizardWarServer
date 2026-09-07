
// Como ReviveLastPlayed, pero toma las cartas de PlayerState.DeadCards (cartas que
// realmente han muerto) en vez de PlayedCards (cartas jugadas, vivas o muertas).
public class ReviveLastDied : IEffect
{
    public ReviveLastDied(CardFilter? filter, Dictionary<Destination, int> destinations, bool toMe)
    {
        Filter = filter;
        Destinations = destinations;
        ToMe = toMe;
    }

    public CardFilter? Filter { get; set; } = null;
    public Dictionary<Destination, int> Destinations { get; set; } = new();
    public bool ToMe { get; set; } = true;
    public IEffect Clone() => new ReviveLastDied(Filter, Destinations, ToMe);

    public void Execute(Guid playerId, Guid rivalId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        var cards = new List<CardInstance>();

        int total = Destinations.Values.Sum();

        var owner = ToMe ? cardId.Player : cardId.Player.PlayerTarget!;
        var graveyard = owner.DeadCards;

        for (int i = graveyard.Count - 1; i >= 0 && cards.Count < total; i--)
        {
            if (Filter is null || Filter.Check(graveyard[i]))
            {
                cards.Add(graveyard[i]);
            }
        }

        foreach (var kvp in Destinations)
        {
            bool stop = false;
            int counter = 0;

            while (cards.Count != 0 && !stop && counter < kvp.Value)
            {
                counter++;
                CardInstance card = new(cards[0].Definition, owner);
                cards.RemoveAt(0);

                switch (kvp.Key)
                {
                    case Destination.HAND:
                        state.DrawCard(owner, card, null);
                        break;
                    case Destination.BOARD:
                        var boardPosition = owner.Board.FindFirstNullPosition();
                        if (boardPosition != -1)
                        {
                            state.PlayCard(owner, card, boardPosition);
                        }
                        else
                        {
                            cards.Insert(0, card);
                            stop = true;
                        }
                        break;
                    case Destination.DECK:
                        state.AddCard(owner, cardId.Player, card, cardId);
                        break;
                }
            }
        }
    }
}
