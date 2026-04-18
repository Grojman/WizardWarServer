
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
    public void Execute(Guid player, Guid card, GameState state, GameEvent? ev)
    {
        for (int i = 0; i < CardAmount; i++)
        {
            state.DrawCard(state.GetState(player).Connection);
        }
    }
}