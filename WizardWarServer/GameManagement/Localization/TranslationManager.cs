using Serilog;

public static class TranslationManager
{
    private static string CsvPath = Path.Combine(AppContext.BaseDirectory, "Data/Translations/translations.csv");
    private static string CardCsvPath = Path.Combine(AppContext.BaseDirectory, "Data/Translations/cards.csv");

    private static Dictionary<string, Dictionary<string, string>> UiEntries = new();
    private static Dictionary<string, Dictionary<string, string>> CardEntries = new();

    private static readonly HashSet<string> WarnedMissingKeys = new();

    public static string[] SupportedLanguages { get; private set; } = ["es"];

    public static string DefaultLanguage { get; private set; } = "es";

    public static void Configure(string csvPath, string cardCsvPath, string defaultLanguage)
    {
        CsvPath = Path.IsPathRooted(csvPath)
            ? csvPath
            : Path.Combine(AppContext.BaseDirectory, csvPath);

        CardCsvPath = Path.IsPathRooted(cardCsvPath)
            ? cardCsvPath
            : Path.Combine(AppContext.BaseDirectory, cardCsvPath);

        DefaultLanguage = defaultLanguage;
    }

    public static void Initialize()
    {
        var (uiLanguages, uiEntries) = LoadCsv(CsvPath);
        var (_, cardEntries) = LoadCsv(CardCsvPath);

        SupportedLanguages = uiLanguages;
        UiEntries = uiEntries;
        CardEntries = cardEntries;
        WarnedMissingKeys.Clear();

        Log.Information(
            "Loaded {UiKeyCount} UI translation keys and {CardKeyCount} card/deck translation keys for languages: {Languages}",
            UiEntries.Count, CardEntries.Count, string.Join(", ", uiLanguages));
    }

    private static (string[] languages, Dictionary<string, Dictionary<string, string>> entries) LoadCsv(string path)
    {
        if (!File.Exists(path))
            throw new TranslationManagerException($"Translations file not found at: {path}");

        var rows = ParseCsv(File.ReadAllText(path));

        if (rows.Count == 0)
            throw new TranslationManagerException($"Translations file at {path} is empty (no header row)");

        var header = rows[0];
        var languages = header.Skip(1).ToArray();

        if (languages.Length == 0)
            throw new TranslationManagerException($"Translations file at {path} has no language columns after the key column");

        var entries = new Dictionary<string, Dictionary<string, string>>();

        foreach (var row in rows.Skip(1))
        {
            if (row.Count == 0 || string.IsNullOrWhiteSpace(row[0])) continue;

            var key = row[0];
            var values = new Dictionary<string, string>();

            for (int i = 0; i < languages.Length; i++)
            {
                values[languages[i]] = i + 1 < row.Count ? row[i + 1] : string.Empty;
            }

            entries[key] = values;
        }

        return (languages, entries);
    }

    public static bool IsSupportedLanguage(string language) => SupportedLanguages.Contains(language);

    public static bool TryResolveLanguage(string? requested, out string resolved)
    {
        if (!string.IsNullOrWhiteSpace(requested) && IsSupportedLanguage(requested))
        {
            resolved = requested;
            return true;
        }

        resolved = DefaultLanguage;
        return false;
    }

    public static string Get(string key, string language)
    {
        if (!UiEntries.TryGetValue(key, out var values) && !CardEntries.TryGetValue(key, out values))
        {
            WarnMissingKey(key);
            return string.Empty;
        }

        if (values.TryGetValue(language, out var value) && !string.IsNullOrEmpty(value))
        {
            return value;
        }

        if (language != DefaultLanguage && values.TryGetValue(DefaultLanguage, out var defaultValue) && !string.IsNullOrEmpty(defaultValue))
        {
            return defaultValue;
        }

        WarnMissingKey(key);
        return key;
    }

    // Only UI/error strings are meant to be pushed to the client wholesale on a language
    // change; card/deck text is looked up on demand via Get() when generating card/deck DTOs.
    public static Dictionary<string, string> GetAll(string language)
    {
        var result = new Dictionary<string, string>();
        foreach (var key in UiEntries.Keys) result[key] = Get(key, language);
        return result;
    }

    public static DeckDto TranslateDeck(DeckDto deck, string language)
    {
        return deck with
        {
            name = Get($"DECK_{deck.id}_NAME", language),
            description = Get($"DECK_{deck.id}_DESC", language)
        };
    }

    public static List<string> TranslateFamilies(IEnumerable<string> familyIds, string language)
        => familyIds.Select(id => Get($"FAMILY_{id}", language)).ToList();

    private static void WarnMissingKey(string key)
    {
        if (WarnedMissingKeys.Add(key))
        {
            Log.Warning("Missing translation for key {Key}", key);
        }
    }

    private static List<List<string>> ParseCsv(string text)
    {
        var lines = text.Replace("\r\n", "\n").Split('\n');
        return [.. lines.Select(ParseCsvLine)];
    }

    private static List<string> ParseCsvLine(string line)
    {
        var fields = new List<string>();
        var current = new System.Text.StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (inQuotes)
            {
                if (c == '"')
                {
                    if (i + 1 < line.Length && line[i + 1] == '"')
                    {
                        current.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = false;
                    }
                }
                else
                {
                    current.Append(c);
                }
            }
            else if (c == '"')
            {
                inQuotes = true;
            }
            else if (c == ',')
            {
                fields.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        fields.Add(current.ToString());
        return fields;
    }

    public class TranslationManagerException : Exception
    {
        public TranslationManagerException(string message) : base(message) { }
    }
}
