
public class DrawCardEffect : IEffect
{
    public DrawCardEffect()
    {
    }

    public DrawCardEffect(int cardAmount)
    {
        CardAmount = cardAmount;
    }

    public int CardAmount { get; set; }

    public IEffect Clone() => new DrawCardEffect(CardAmount);
    public void Execute(Guid player, CardInstance card, GameState state, GameEvent? ev)
    {
        for (int i = 0; i < CardAmount; i++)
        {
            state.DrawCard(state.GetState(player).Connection);
        }
    }
}