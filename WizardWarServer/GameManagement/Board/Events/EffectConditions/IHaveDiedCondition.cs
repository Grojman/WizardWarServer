
public class IHaveDiedCondition : EffectCondition
{
    public override bool Check(Guid playerId, CardInstance sourceCard, GameState state, GameEvent? ev)
    {
        return ev is GameEvent.UnitDeath e && e.Card.Id == sourceCard.Id;
    }

    public override EffectCondition Clone() => new IHaveDiedCondition();
}