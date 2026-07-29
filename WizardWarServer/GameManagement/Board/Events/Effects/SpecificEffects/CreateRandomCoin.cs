
public class CreateRandomCoin : IEffect
{
    readonly string[] COINS = ["96", "98", "99", "100", "101", "102"];
    public IEffect Clone() => new CreateRandomCoin();

    public void Execute(Guid playerId, Guid rivalId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        var player = state.GetState(playerId);
        var card = new CardInstance(CardManager.GetCardById(COINS.GetRandom()), player);

        state.AddCard(player, player, card, cardId);
    }
}