
public class EvenTurnCondition : EffectCondition
{
    public override bool Check(Guid playerId, CardInstance sourceCard, GameState state, GameEvent? ev)
    {
        return state.TurnCounter % 2 == 0;
    }

    public override EffectCondition Clone() => new EvenTurnCondition();
}