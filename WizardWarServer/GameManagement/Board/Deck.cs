public class Deck
{
    public int Id {get; }
    public string Name { get; }
    public Guid PlayerId { get; }
    public List<CardInstance> cards;

    public Deck(string name, IEnumerable<CardDefinition> definitions, Guid playerId, int deckId)
    {
        Id = deckId;
        Name = name;
        PlayerId = playerId;
        cards = definitions
            .Select(d => new CardInstance(d, PlayerId))
            .ToList();

        Shuffle(cards);
    }

    public CardInstance? Draw()
    {
        if (cards.Count == 0)
            return null;

        var card = cards[0];

        cards.RemoveAt(0);

        return card;
    }

    public int Count => cards.Count;

    private void Shuffle(List<CardInstance> list)
    {
        var rng = new Random();

        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);

            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    public void AddCard(CardInstance card)
    {
        var index = new Random().Next(cards.Count);
        cards.Insert(index, card);
    }

    public void AddCard(CardDefinition card) => AddCard(new CardInstance(card, PlayerId));
}