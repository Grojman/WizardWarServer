public class PlayCardEffect : IEffect
{
    public PlayCardEffect(string cardId, bool fromDeck, bool toHand)
    {
        CardId = cardId;
        FromDeck = fromDeck;
        ToHand = toHand;
    }

    public string CardId { get; set; }
    public bool FromDeck { get; set; }

    public bool ToHand { get; set; } 

    public IEffect Clone() => new PlayCardEffect(CardId, FromDeck, ToHand);

    public void Execute(Guid playerId, Guid rivalId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        CardInstance? card;

        if(FromDeck && ( (ToHand && cardId.Player.Hand.Count < GameState.MAX_HAND) || cardId.Player.Board.Any(n => n is null)))
        {
            card = cardId.Player.Deck?.Draw(new(){DefinitionId = CardId});
        } else
        {
            card = new (CardManager.GetCardById(CardId), cardId.Player);
        }

        if(card is not null)
        {
            if (ToHand)
            {
                state.DrawCard(cardId.Player, card, cardId, FromDeck);   
            } else
            {
                var boardIndex = cardId.Player.Board.Select((n, i) => new{n,  i}).Where(n => n.n is null);
                if(boardIndex.Any() || card.Definition.Type is CardType.Spell)
                {
                    state.PlayCard(cardId.Player, card, boardIndex.Any() ? boardIndex.ElementAt(0).i : -1);
                }
            }

        }
    }
}