
public class CountCardCondition : EffectCondition
{
    public CountCardCondition(GameFilter filter, NumberFilter value)
    {
        Filter = filter;
        Value = value;
    }


    public GameFilter Filter { get; set; }
    public NumberFilter Value { get; set; }

    public override bool Check(Guid playerId, Guid rivalId, CardInstance sourceCard, GameState state, GameEvent? ev)
    {
        var count = Filter.GetMeetingCards(state, playerId, rivalId).Count();
        return Value.Compare(count);
    }

    public override EffectCondition Clone() => new CountCardCondition(Filter, Value);
}