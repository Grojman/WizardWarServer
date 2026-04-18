using System.Text.Json;

public static class CardManager
{
    private const string DECKS_FILE_PATH = "Data/Decks/decks.json";
    private const string CARDS_FOLDER = "Data/Decks/Cards";

    public static List<DeckDto> Decks { get; set; } = new();
    public static DeckDto GetById(int id) => Decks.First(n => n.id == id);
    static Dictionary<int, List<CardDefinition>> Cards { get; set; } = new();
    
    public static void Initialize()
    {
            if (!File.Exists(DECKS_FILE_PATH))
                throw new CardManagerException($"Decks file not found at: {DECKS_FILE_PATH}");

            if (!Directory.Exists(CARDS_FOLDER))
                throw new CardManagerException($"Cards folder not found at: {CARDS_FOLDER}");

            string decksJson = File.ReadAllText(DECKS_FILE_PATH);
            Decks = JsonSerializer.Deserialize<List<DeckDto>>(decksJson) ?? new();

            foreach (var deck in Decks)
            {
                string cardFilePath = Path.Combine(CARDS_FOLDER, $"{deck.id}.json");
                if (!File.Exists(cardFilePath))
                    throw new CardManagerException($"Card file not found at: {cardFilePath}");

                string cardsJson = File.ReadAllText(cardFilePath);
                var cardDefinitions = JsonSerializer.Deserialize<List<CardDefinition>>(cardsJson) ?? new();
                Cards[deck.id] = cardDefinitions;
            }
    }

    public class CardManagerException : Exception
    {
        public CardManagerException(string message) : base(message) {}
    }

    public static List<CardDefinition> GetDefinitionsByDeck(DeckDto deck) => Cards[deck.id];
    public static List<CardDefinition> GetDefinitionsByDeck(int deckId) => Cards[deckId];
    public static void EmptyCardsFolder()
    {
        if (Directory.Exists(CARDS_FOLDER))
        {
            foreach (var file in Directory.GetFiles(CARDS_FOLDER))
            {
                File.Delete(file);
            }
        }
    }

    public static void SerializeDecks(List<DeckDto> decks)
    {
        string decksJson = JsonSerializer.Serialize(decks, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(DECKS_FILE_PATH, decksJson);
    }

    public static void SerializeCards(DeckDto deck, List<CardDefinition> newCards)
    {
        string cardFilePath = Path.Combine(CARDS_FOLDER, $"{deck.id}.json");
        string cardsJson = JsonSerializer.Serialize(newCards, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(cardFilePath, cardsJson);
    }
}