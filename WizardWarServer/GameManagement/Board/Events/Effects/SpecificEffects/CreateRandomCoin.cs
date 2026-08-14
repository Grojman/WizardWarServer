
public class CreateRandomCoin : IEffect
{
    readonly string[] COINS = ["96", "98", "99", "100", "101", "102"];
    public int Amount { get; set; } = 1;
    public CreateRandomCoin(int amount)
    {
        Amount = amount;
    }
    public IEffect Clone() => new CreateRandomCoin(Amount);

    public void Execute(Guid playerId, Guid rivalId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        var player = state.GetState(playerId);

        for (int i = 0; i < Amount; i++)
        {
            var card = new CardInstance(CardManager.GetCardById(COINS.GetRandom()), player);
            state.AddCard(player, player, card, cardId);
        }

    }
}