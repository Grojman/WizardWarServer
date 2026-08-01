using System.Text.Json;
using Serilog;

public class DeckStats
{
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int TotalGames { get; set; }
    public int TotalTurns { get; set; }

    public Dictionary<int, DeckStats> VsDeck { get; set; } = new();

    public double AverageTurn => TotalGames == 0 ? 0 : (double)TotalTurns / TotalGames;
}

internal class PersistedStats
{
    public int TotalGamesPlayed { get; set; }
    public Dictionary<int, DeckStats> DeckStats { get; set; } = new();
}

public static class StoringData
{
    private static readonly object _lock = new();

    public static Dictionary<int, DeckStats> Data { get; private set; } = new();

    public static int TotalGamesPlayed { get; private set; }

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
                TotalGamesPlayed = 0;
                return;
            }

            try
            {
                var json = File.ReadAllText(_filePath);
                var res = JsonSerializer.Deserialize<PersistedStats>(json);
                Data = res?.DeckStats ?? new();
                TotalGamesPlayed = res?.TotalGamesPlayed ?? 0;
            }
            catch (JsonException ex)
            {
                Log.Error(ex, "Could not read data file at {Path}, starting with empty stats", _filePath);
                Data = new();
                TotalGamesPlayed = 0;
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

            var persisted = new PersistedStats
            {
                TotalGamesPlayed = TotalGamesPlayed,
                DeckStats = Data
            };

            var json = JsonSerializer.Serialize(persisted, new JsonSerializerOptions()
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
        if (state.GameActionResult.Winner is null) return;

        Guid winnerId = (Guid)state.GameActionResult.Winner;
        int turns = state.TurnCounter;
        var registeredDecks = new List<int>();

        lock (_lock)
        {
            TotalGamesPlayed++;

            foreach (var player in state.Players)
            {
                var deckId = player.Deck!.Id;
                bool won = player.Id == winnerId;

                if (!Data.TryGetValue(deckId, out var stats))
                {
                    stats = new DeckStats();
                    Data[deckId] = stats;
                }

                if (registeredDecks.Contains(deckId) && !won)
                {
                    return;
                } else {
                    if (!won)
                    {
                        registeredDecks.Add(deckId);
                    }
                }

                stats.TotalGames++;
                stats.TotalTurns += turns;


                if (won)
                {
                    stats.Wins++;
                }
                else
                {
                    stats.Losses++;
                }
            }
        }
    }

    public static StatsDto GetStats()
    {
        lock (_lock)
        {
            var allDecks = CardManager.Decks.ToList();

            var decks = allDecks
                .Select(d => DeckStatsDto.Generate(d, Data.TryGetValue(d.id, out var s) ? s : null, allDecks))
                .ToList();

            return new StatsDto(TotalGamesPlayed, decks);
        }
    }
}
