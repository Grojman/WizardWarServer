public class IDefendedCondition : EffectCondition
{
    public override bool Check(Guid playerId, Guid rivalId, CardInstance sourceCard, GameState state, GameEvent? ev)
    {
        return ev is GameEvent.CardAttacked a && a.Deffender?.Id == sourceCard.Id;
    }

    public override EffectCondition Clone() => new IAttackedCondition();
}