public class Deck
{
    public string Name { get; }
    public Guid PlayerId { get; }
    private Stack<CardInstance> cards;

    public Deck(string name, IEnumerable<CardDefinition> definitions, Guid playerId)
    {
        Name = name;
        PlayerId = playerId;
        var list = definitions
            .Select(d => new CardInstance(d, PlayerId))
            .ToList();

        Shuffle(list);

        cards = new Stack<CardInstance>(list);
    }

    public CardInstance? Draw()
    {
        if (cards.Count == 0)
            return null;

        return cards.Pop();
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
}