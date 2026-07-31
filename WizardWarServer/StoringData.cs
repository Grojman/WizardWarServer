using System.Text.Json;

public class GameData
{
    public GameData()
    {
    }

    public GameData(int wins, int loses)
    {
        Wins = wins;
        Loses = loses;
    }

    public int Wins { get; set; }
    public int Loses { get; set; }

}

public static class StoringData
{
    private static readonly object _lock = new();

    public static Dictionary<int, Dictionary<int, GameData>> Data { get; private set; } = new();

    public const string FILE_PATH = "data.json";
    public const string SUGGESTIONS_TEXT = "suggestions.txt";
    public const string SUGGESTION_HEADER = "===================================================";

    private static string _filePath = Path.Combine(AppContext.BaseDirectory, FILE_PATH);
    private static string _suggestionsPath = Path.Combine(AppContext.BaseDirectory, SUGGESTIONS_TEXT);
    private static int _maxSuggestionLength = 2000;

    public static void Configure(ServerOptions options)
    {
        _maxSuggestionLength = options.MaxSuggestionLength;
    }

    public static void SaveSuggestion(string suggestion)
    {
        if (string.IsNullOrEmpty(suggestion)) return;

        if (suggestion.Length > _maxSuggestionLength)
        {
            suggestion = suggestion[.._maxSuggestionLength];
        }

        lock (_lock)
        {
            try
            {
                using StreamWriter sr = new(_suggestionsPath, true);
                sr.WriteLine(SUGGESTION_HEADER);
                sr.WriteLine(suggestion);
                sr.WriteLine(SUGGESTION_HEADER);
            }
            catch (IOException ex)
            {
                Log.Error(ex, "Failed to save suggestion to {Path}", _suggestionsPath);
            }
        }
    }

    public static void GetFromFile()
    {
        lock (_lock)
        {
            if (!File.Exists(_filePath))
            {
                Data = new();
                return;
            }

            try
            {
                var json = File.ReadAllText(_filePath);
                var res = JsonSerializer.Deserialize<Dictionary<int, Dictionary<int, GameData>>>(json);
                Data = res ?? new();
            }
            catch (JsonException ex)
            {
                Log.Error(ex, "Could not read data file at {Path}, starting with empty stats", _filePath);
                Data = new();
            }
        }
    }

    public static void SaveInFile()
    {
        lock (_lock)
        {
            var directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var json = JsonSerializer.Serialize(Data ?? [], new JsonSerializerOptions()
            {
                WriteIndented = true,
            });

            try
            {
                File.WriteAllText(_filePath, json);
            }
            catch (IOException ex)
            {
                Log.Error(ex, "Failed to persist data file at {Path}", _filePath);
            }
        }
    }

    public static void SaveData(GameState state, bool forced)
    {
        if (state.GameActionResult.Winner is not null)
        {
            Guid winnerId = (Guid)state.GameActionResult.Winner;
            int winerDeck = state.GetState(winnerId).Deck!.Id;

            lock (_lock)
            {
                Dictionary<int, GameData> values;

                if (!Data.ContainsKey(winerDeck))
                {
                    values = [];
                    Data.Add(winerDeck, values);
                }
                else
                {
                    values = Data[winerDeck];
                }

                HashSet<int> appendedIds = [];

                foreach (var player in state.Players.Where(n => n.Id != winnerId))
                {
                    var id = player.Deck!.Id;
                    if (appendedIds.Add(id))
                    {
                        if (!Data.ContainsKey(id))
                        {
                            Data.Add(id, []);
                        }

                        if (!Data[id].ContainsKey(winerDeck))
                        {
                            Data[id].Add(winerDeck, new());
                        }

                        if (!Data[winerDeck].ContainsKey(id))
                        {
                            Data[winerDeck].Add(id, new());
                        }

                        Data[winerDeck][id].Wins++;
                        Data[id][winerDeck].Loses++;
                    }
                }
            }
        }

    }

}
