
public class AlterPlayerHealthEffect : IEffect
{
    public int Amount { get; set; }
    public bool ToRival { get; set; }

    public AlterPlayerHealthEffect(int amount, bool toRival)
    {
        Amount = amount;
        ToRival = toRival;
    }

    public void Execute(Guid playerId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        var player = ToRival ? state.GetRival(cardId.PlayerGuid) :  state.GetState(cardId.PlayerGuid);

        state.AlterPlayerHealth(cardId, player, Amount);
    }
}