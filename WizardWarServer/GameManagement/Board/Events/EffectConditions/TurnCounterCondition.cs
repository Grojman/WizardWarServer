
//TODO: TERMINAR
public class TurnCounterCondition : EffectCondition
{
    public TurnCounterCondition(int amount)
    {
        Amount = amount;
    }

    public int Amount { get; set; }
    public int AmountCounter { get; set; } = 0;

    int currentTurn;

    public override bool Check(Guid playerId, Guid rivalId, CardInstance sourceCard, GameState state, GameEvent? ev)
    {
        var result = true;

        if (currentTurn == state.TurnCounter)
        {
            // Amount
        }

        currentTurn = state.TurnCounter;   

        return result;
    }

    public override EffectCondition Clone() => new TurnCounterCondition(Amount);
}
