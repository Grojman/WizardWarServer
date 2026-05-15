public class PlayCardEffect : IEffect
{
    public PlayCardEffect(string cardId, bool fromDeck)
    {
        CardId = cardId;
        FromDeck = fromDeck;
    }

    public string CardId { get; set; }
    public bool FromDeck { get; set; }

    public IEffect Clone() => new PlayCardEffect(CardId, FromDeck);

    public void Execute(Guid playerId, Guid rivalId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        CardInstance? card;

        if(FromDeck)
        {
            card = cardId.Player.Deck.Draw(new(){DefinitionId = CardId});
        } else
        {
            card = new (CardManager.GetCardById(CardId), cardId.Player);
        }

        if(card is not null)
        {
            var boardIndex = cardId.Player.Board.Select((n, i) => new{n,  i}).Where(n => n.n is null);
            if(boardIndex.Any())
            {
                state.PlayCard(cardId.Player, card, boardIndex.ElementAt(0).i);
            }
        }
    }
}