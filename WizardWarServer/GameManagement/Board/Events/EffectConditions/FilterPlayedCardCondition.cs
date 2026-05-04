public class FilterPlayerCardCondition : EffectCondition
{
    public FilterPlayerCardCondition(CardFilter filter)
    {
        Filter = filter;
    }

    public CardFilter Filter { get; set; }

    public override bool Check(Guid playerId, CardInstance sourceCard, GameState state, GameEvent? ev)
    {
        CardInstance? card = null;

        if(ev is GameEvent.UnitPlayed e) card = e.Unit;
        if(ev is GameEvent.SpellPlayed f) card = f.Spell;

        return card is not null && Filter.Check(card);
    }

    public override EffectCondition Clone() => new FilterPlayerCardCondition(Filter);
}