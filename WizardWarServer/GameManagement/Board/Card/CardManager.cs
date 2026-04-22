using System.Text.Json;

public static class CardManager
{
    private const string DECKS_FILE_PATH = "Data/Decks/decks.json";
    private const string CARDS_FILE_PATH = "Data/Decks/cards.json";

    public static List<DeckDto> Decks { get; set; } = new();

    // Cartas globales
    public static Dictionary<string, CardDefinition> Cards { get; set; }
        = new();

    private static Dictionary<int, Dictionary<string, int>> DeckCards
        = new();

    public static DeckDto GetById(int id)
        => Decks.First(n => n.id == id);

    public static void Initialize()
    {
        if (!File.Exists(DECKS_FILE_PATH))
            throw new CardManagerException(
                $"Decks file not found at: {DECKS_FILE_PATH}");

        if (!File.Exists(CARDS_FILE_PATH))
            throw new CardManagerException(
                $"Cards file not found at: {CARDS_FILE_PATH}");

        // =========================
        // CARGAR CARTAS GLOBALES
        // =========================

        string cardsJson =
            File.ReadAllText(CARDS_FILE_PATH);

        var cardList =
            JsonSerializer.Deserialize<List<CardDefinition>>(cardsJson)
            ?? new();

        Cards = cardList.ToDictionary(c => c.Id);

        // =========================
        // CARGAR MAZOS + CARTAS
        // =========================

        string decksJson =
            File.ReadAllText(DECKS_FILE_PATH);

        var deckWrappers =
            JsonSerializer.Deserialize<List<DeckWithCardsDto>>(decksJson)
            ?? new();

        Decks.Clear();
        DeckCards.Clear();

        foreach (var wrapper in deckWrappers)
        {
            Decks.Add(wrapper.Deck);

            DeckCards[wrapper.Deck.id]
                = wrapper.Cards;
        }
    }

    public class CardManagerException : Exception
    {
        public CardManagerException(string message)
            : base(message) { }
    }


    public static CardDefinition GetCardById(string id)
    {
        if (!Cards.ContainsKey(id))
            throw new CardManagerException(
                $"Card with id {id} not found");

        return Cards[id];
    }


    public static Dictionary<CardDefinition, int> GetDefinitionsByDeck(int deckId)
    {
        Dictionary<CardDefinition, int> data = new();
        foreach(var k in DeckCards[deckId]) data.Add(GetCardById(k.Key), k.Value);
        return data;
    }

    public static void SerializeCards(
        List<CardDefinition> cards)
    {
        string cardsJson =
            JsonSerializer.Serialize(
                cards,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

        File.WriteAllText(
            CARDS_FILE_PATH,
            cardsJson);

        Cards = cards.ToDictionary(c => c.Id);
    }

    // =========================
    // SERIALIZAR MAZOS + CARTAS
    // =========================

    public static void SerializeDeck(
        DeckDto deck,
        Dictionary<string, int> cardCopies)
    {
        var existing =
            Decks.FirstOrDefault(d => d.id == deck.id);

        if (existing != null)
        {
            Decks.Remove(existing);
            DeckCards.Remove(deck.id);
        }

        Decks.Add(deck);
        DeckCards[deck.id] = cardCopies;

        SaveAllDecks();
    }

    private static void SaveAllDecks()
    {
        var wrappers =
            Decks.Select(d => new DeckWithCardsDto
            {
                Deck = d,
                Cards = DeckCards.ContainsKey(d.id)
                    ? DeckCards[d.id]
                    : new()
            }).ToList();

        string decksJson =
            JsonSerializer.Serialize(
                wrappers,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

        File.WriteAllText(
            DECKS_FILE_PATH,
            decksJson);
    }
}