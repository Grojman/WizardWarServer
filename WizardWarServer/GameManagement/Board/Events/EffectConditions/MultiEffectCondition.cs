
public class MultiEffectCondition : EffectCondition
{
    public MultiEffectCondition(List<EffectCondition> conditions, bool isOr)
    {
        Conditions = conditions;
        IsOr = isOr;
    }

    public List<EffectCondition> Conditions { get; set; }
    public bool IsOr { get; set; }
    public override bool Check(Guid playerId, CardInstance sourceCard, GameState state, GameEvent? ev)
    {
        return IsOr ? Conditions.Any(n => n.Check(playerId, sourceCard, state, ev)) : Conditions.All(n => n.Check(playerId, sourceCard, state, ev));
    }

    public override EffectCondition Clone() => new MultiEffectCondition([.. Conditions.Select(n => n.Clone())], IsOr);
}