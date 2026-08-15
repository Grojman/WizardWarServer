public class CreateCardInHand : IEffect
{
    public string CardId { get; set; } = string.Empty;
    public int CardsAmount { get; set; } = 1;
    public bool ToRival { get; set; } = false;

    public CreateCardInHand() {}
    public CreateCardInHand(int cardsAmount, string cardId, bool toRival)
    {
        CardsAmount = cardsAmount;
        CardId = cardId;
        ToRival = toRival;
    }

    public IEffect Clone() => new CreateCardInHand(CardsAmount, CardId, ToRival);

    public void Execute(Guid playerId, Guid rivalId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        var cardDefinition = CardManager.GetCardById(CardId);

        var rival = state.GetState(rivalId);
        var player = cardId.Player;

        var target = ToRival ? rival : player;

        for (int i = 0; i < CardsAmount; i++)
        {
            state.DrawCard(target, new CardInstance(cardDefinition, target), cardId, false);
        }
    }
}
