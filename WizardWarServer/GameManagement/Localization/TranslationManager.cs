using Serilog;

public static class TranslationManager
{
    private static string CsvPath = Path.Combine(AppContext.BaseDirectory, "Data/Translations/translations.csv");

    private static Dictionary<string, Dictionary<string, string>> Entries = new();

    private static readonly HashSet<string> WarnedMissingKeys = new();

    public static string[] SupportedLanguages { get; private set; } = ["es"];

    public static string DefaultLanguage { get; private set; } = "es";

    public static void Configure(string csvPath, string defaultLanguage)
    {
        CsvPath = Path.IsPathRooted(csvPath)
            ? csvPath
            : Path.Combine(AppContext.BaseDirectory, csvPath);

        DefaultLanguage = defaultLanguage;
    }

    public static void Initialize()
    {
        if (!File.Exists(CsvPath))
            throw new TranslationManagerException($"Translations file not found at: {CsvPath}");

        var rows = ParseCsv(File.ReadAllText(CsvPath));

        if (rows.Count == 0)
            throw new TranslationManagerException($"Translations file at {CsvPath} is empty (no header row)");

        var header = rows[0];
        var languages = header.Skip(1).ToArray();

        if (languages.Length == 0)
            throw new TranslationManagerException("Translations file has no language columns after the key column");

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

        SupportedLanguages = languages;
        Entries = entries;
        WarnedMissingKeys.Clear();

        Log.Information("Loaded {KeyCount} translation keys for languages: {Languages}", Entries.Count, string.Join(", ", languages));
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
        if (!Entries.TryGetValue(key, out var values))
        {
            WarnMissingKey(key);
            return key;
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

    public static Dictionary<string, string> GetAll(string language)
    {
        var result = new Dictionary<string, string>();
        foreach (var key in Entries.Keys) result[key] = Get(key, language);
        return result;
    }

    private static void WarnMissingKey(string key)
    {
        if (WarnedMissingKeys.Add(key))
        {
            Log.Warning("Missing translation for key {Key}", key);
        }
    }

    private static List<List<string>> ParseCsv(string text)
    {
        return [.. text.Split('\n').Select(n => n.Split(',').ToList())];
    }

    public class TranslationManagerException : Exception
    {
        public TranslationManagerException(string message) : base(message) { }
    }
}
