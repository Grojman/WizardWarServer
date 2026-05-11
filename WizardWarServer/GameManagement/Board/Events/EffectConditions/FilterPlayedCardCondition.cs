public class FilterPlayerCardCondition : EffectCondition
{
    public FilterPlayerCardCondition(CardFilter filter)
    {
        Filter = filter;
    }

    public CardFilter Filter { get; set; }

    public override bool Check(Guid playerId, Guid rivalId, CardInstance sourceCard, GameState state, GameEvent? ev)
    {
        CardInstance? card = null;

        if(ev is GameEvent.GameEventCard e) card = e.Card;

        return card is not null && Filter.Check(card);
    }

    public override EffectCondition Clone() => new FilterPlayerCardCondition(Filter);
}