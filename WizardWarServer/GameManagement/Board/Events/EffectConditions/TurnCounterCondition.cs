
//TODO: TERMINAR
public class TurnCounterCondition : EffectCondition
{
    public TurnCounterCondition(int amount)
    {
        Amount = amount;
    }

    public int Amount { get; set; }
    public int AmountCounter { get; set; } = 0;

    int currentTurn = -1;

    public override bool Check(Guid playerId, Guid rivalId, CardInstance sourceCard, GameState state, GameEvent? ev)
    {
        var result = true;

        if (currentTurn != state.TurnCounter)
        {
            if(currentTurn != -1) AmountCounter++; //la primera vez que se comprueba no debe de contar
            result = AmountCounter >= Amount;
            currentTurn = state.TurnCounter;   
        }


        return result;
    }

    public override EffectCondition Clone() => new TurnCounterCondition(Amount);
}
