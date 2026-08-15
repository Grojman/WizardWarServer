
public class AppendCardToDeck : IEffect
{
    public string CardId { get; set; } = string.Empty;
    public int CardsAmount  { get; set; } = 1;
    public bool ToRival { get; set; } = false;
    public int TopDeckDepth { get; set; } = 0;

    public AppendCardToDeck() {}
    public AppendCardToDeck(int cardsAmount, string cardId, bool toRival, int topDeckDepth = 0)
    {
        this.CardsAmount = cardsAmount;
        CardId = cardId;
        ToRival = toRival;
        TopDeckDepth = topDeckDepth;
    }

    public void Execute(Guid playerId, Guid rivalId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        var cardInstance = CardManager.GetCardById(CardId);

        var rival = state.GetState(rivalId);
        var player = cardId.Player;

        for (int i = 0; i < CardsAmount; i++)
        {
            state.AddCard(ToRival ? rival : player, ToRival ? player : rival, new CardInstance(cardInstance, ToRival ? rival : player), cardId, TopDeckDepth);
        }
    }

    public IEffect Clone() => new AppendCardToDeck(CardsAmount, CardId, ToRival, TopDeckDepth);
}