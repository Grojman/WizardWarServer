public class DrawCardsBasedOnFilter : IEffect
{
    public DrawCardsBasedOnFilter(float relation, GameFilter filter, bool rival, CardFilter? filterDrawedCards)
    {
        Relation = relation;
        Filter = filter;
        Rival = rival;
        FilterDrawedCards = filterDrawedCards;
    }

    public bool Rival { get; set; }
    public float Relation { get; set;}
    public GameFilter Filter { get; set; }
    public CardFilter? FilterDrawedCards { get; set; }
    public IEffect Clone() => new DrawCardsBasedOnFilter(Relation, Filter, Rival, FilterDrawedCards);

    public void Execute(Guid playerId, Guid rivalId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        var count = (int)(Filter.GetMeetingCards(state, playerId, rivalId).Count() * Relation);
        var p = Rival ? cardId.Player.PlayerTarget : cardId.Player;

        for (int i = 0; i < count; i++)
        {
            state.DrawCard( p.Connection, cardId, FilterDrawedCards);
        }        
    }
}