
public class AlterPlayerHealthEffect : IEffect
{
    public int Amount { get; set; }
    public bool ToRival { get; set; }

    public AlterPlayerHealthEffect(int amount, bool toRival)
    {
        Amount = amount;
        ToRival = toRival;
    }

    public void Execute(Guid playerId, Guid rivalId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        var player = state.GetState( ToRival ?  rivalId : playerId);

        state.AlterPlayerHealth(cardId, player, Amount);
    }

    public IEffect Clone() => new AlterPlayerHealthEffect(Amount, ToRival);
}