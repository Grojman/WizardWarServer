
public class AppendCardToDeck : IEffect
{
    public string CardId { get; set; } = string.Empty;
    int CardsAmount  { get; set; } = 1;
    bool ToRival { get; set; } = false;

    public AppendCardToDeck() {}
    public AppendCardToDeck(int cardsAmount, string cardId, bool toRival)
    {
        this.CardsAmount = cardsAmount;
        CardId = cardId;
        ToRival = toRival;
    }

    public void Execute(Guid playerId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        var cardInstance = CardManager.GetCardById(CardId);

        var rival = state.GetRival(playerId);
        var player = state.GetState(playerId);

        for (int i = 0; i < CardsAmount; i++)
        {
            state.AddCard(ToRival ? rival : player, ToRival ? player : rival, new CardInstance(cardInstance,(ToRival ? rival : player).Id), cardId);
        }
    }

    public IEffect Clone() => new AppendCardToDeck(CardsAmount, CardId, ToRival);
}