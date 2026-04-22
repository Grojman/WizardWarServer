public record DeckStateDto(int cardAmount, int id)
{
    public static DeckStateDto Generate(Deck deck)
    {
        return new(deck.Count, deck.Id);
    }
}