
public class CountPlayedCardsCondition : EffectCondition
{
    public CountPlayedCardsCondition(CardFilter filter, PlayerType target, NumberFilter value)
    {
        Filter = filter;
        Target = target;
        Value = value;
    }


    public CardFilter Filter { get; set; }
    public PlayerType Target { get; set; }
    public NumberFilter Value { get; set; }
    public override bool Check(Guid playerId, Guid rivalId, CardInstance sourceCard, GameState state, GameEvent? ev)
    {
        var count = state.GetState( Target == PlayerType.PLAYER ? playerId : rivalId).PlayedCards.Count(Filter.Check);

        return Value.Compare(count);
    }

    public override EffectCondition Clone() => new CountPlayedCardsCondition(Filter, Target, Value);
}