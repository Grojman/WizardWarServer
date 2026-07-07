public class NumericEventCondition : EffectCondition
{
    public NumericEventCondition(NumberFilter filter)
    {
        Filter = filter;
    }

    public NumberFilter Filter { get; set; }


    public override bool Check(Guid playerId, Guid rivalId, CardInstance sourceCard, GameState state, GameEvent? ev)
    {
        int value = ev switch
        {
            GameEvent.PlayerHealthChanged e => e.Amount,
            GameEvent.UnitDamageChanged e => e.Amount,
            GameEvent.UnitHealthChanged e => e.Amount,
            _ => 0
        };

        return Filter.Compare(value);
    }

    public override EffectCondition Clone() => new NumericEventCondition(Filter);
}