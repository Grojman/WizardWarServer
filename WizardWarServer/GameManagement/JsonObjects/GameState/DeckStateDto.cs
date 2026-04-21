public record DeckStateDto(string name, int cardAmount, int id)
{
    public static DeckStateDto Generate(Deck deck)
    {
        return new(deck.Name, deck.Count, deck.Id);
    }
}