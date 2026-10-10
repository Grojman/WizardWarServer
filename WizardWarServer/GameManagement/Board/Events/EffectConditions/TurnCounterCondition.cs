
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

    // Se puede comprobar varias veces en el mismo turno (una por cada objetivo,
    // ver EffectFanOut): todas tienen que devolver lo mismo que la primera.
    bool lastResult = true;

    public override bool Check(Guid playerId, Guid rivalId, CardInstance sourceCard, GameState state, GameEvent? ev)
    {
        if (currentTurn != state.TurnCounter)
        {
            if(currentTurn != -1) AmountCounter++; //la primera vez que se comprueba no debe de contar
            lastResult = AmountCounter >= Amount;
            currentTurn = state.TurnCounter;
        }

        return lastResult;
    }

    public override EffectCondition Clone() => new TurnCounterCondition(Amount);
}
