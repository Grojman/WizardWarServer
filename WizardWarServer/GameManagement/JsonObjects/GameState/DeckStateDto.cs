public record DeckStateDto(string name, int cardAmount)
{
    public static DeckStateDto Generate(Deck deck)
    {
        return new(deck.Name, deck.Count);
    }
}