public class DeckWithCardsDto
{
    public DeckDto Deck { get; set; }

    public Dictionary<string, int> Cards { get; set; }
        = new();
}