
public class DrawCardEffect : IEffect
{
    public DrawCardEffect()
    {
    }

    public DrawCardEffect(int cardAmount, CardFilter? filter)
    {
        CardAmount = cardAmount;
        Filter = filter;
    }

    public int CardAmount { get; set; }
    public CardFilter? Filter { get; set; }

    public IEffect Clone() => new DrawCardEffect(CardAmount, Filter);
    public void Execute(Guid player, CardInstance card, GameState state, GameEvent? ev)
    {
        for (int i = 0; i < CardAmount; i++)
        {
            state.DrawCard(state.GetState(player).Connection, Filter);
        }
    }
}