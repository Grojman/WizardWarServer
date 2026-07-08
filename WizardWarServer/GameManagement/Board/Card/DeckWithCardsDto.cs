public class DeckWithCardsDto
{
    public required DeckDto Deck { get; set; }

    public Dictionary<string, int> Cards { get; set; }
        = new();
}