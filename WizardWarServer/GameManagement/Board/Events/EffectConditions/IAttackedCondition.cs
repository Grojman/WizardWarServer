public class IAttackedCondition : EffectCondition
{
    public override bool Check(Guid playerId, Guid rivalId, CardInstance sourceCard, GameState state, GameEvent? ev)
    {
        return ev is GameEvent.CardAttacked a && a.Attacker.Id == sourceCard.Id;
    }

    public override EffectCondition Clone() => new IAttackedCondition();
}