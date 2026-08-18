
public class SetUnitToCeroEffect : IEffect
{
    public SetUnitToCeroEffect(int amount, bool toPlayer)
    {
        Amount = amount;
        ToPlayer = toPlayer;
    }

    public int Amount { get; set; }
    public bool ToPlayer { get; set; }

    public IEffect Clone() => new SetUnitToCeroEffect(Amount, ToPlayer);

    public void Execute(Guid playerId, Guid rivalId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        var cardsTocheck = state.GetState(ToPlayer ? playerId : rivalId).Board;

        int counter = 0;
        foreach(var c in cardsTocheck)
        {
            if (c is not null && counter < Amount)
            {
                state.AlterUnitDamage(cardId, c, -c.CurrentAttack);
                counter++;
            }
        }
    }
}