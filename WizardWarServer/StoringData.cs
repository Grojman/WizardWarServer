using System.Diagnostics.Eventing.Reader;
using System.Text.Json;
using Serilog;

public class DeckStats
{
    public int Wins { get; set; }
    public int Losses { get; set; }
    public int TotalGames { get; set; }
    public int TotalTurns { get; set; }
    public int TotalSeconds { get; set; }

    public Dictionary<int, DeckStats> VsDeck { get; set; } = new();

    public double AverageTurn => TotalGames == 0 ? 0 : (double)TotalTurns / TotalGames;
    public double AverageSeconds => TotalSeconds == 0 ? 0 : (double)TotalSeconds / TotalGames;
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

    private static void UpdateStats(int deckId, int rivalDeck, bool isWinner, int turns, int seconds)
    {
        if(!Data.TryGetValue(deckId, out var stats))
        {
            stats = new DeckStats();
            Data[deckId] = stats;
        }

        if(!Data.TryGetValue(rivalDeck, out var rivalStats))
        {
            rivalStats = new DeckStats();
            Data[rivalDeck] = rivalStats;
        }
        stats.TotalGames++;
        stats.TotalTurns += turns;
        stats.TotalSeconds += seconds;
        rivalStats.TotalGames++;
        rivalStats.TotalSeconds += seconds;
        rivalStats.TotalTurns += turns;

        if (!stats.VsDeck.TryGetValue(rivalDeck, out var sR))
        {
            sR = new();
            stats.VsDeck[rivalDeck] = sR;
        }

        if (!rivalStats.VsDeck.TryGetValue(deckId, out var sI))
        {
            sI = new();
            rivalStats.VsDeck[deckId] = sI;
        }


        if (isWinner)
        {
            stats.Wins++;
            rivalStats.Losses++;


            sR.Wins++;
            sI.Losses++;
        } else
        {
            stats.Losses++;
            rivalStats.Wins++;

            sR.Losses++;
            sI.Wins++;
        }
    }

    public static void SaveData(GameState state, bool forced)
    {
        if (state.GameActionResult.Winner is null || state.Players.Count != 2) return;
        if (state.Players[0].Deck!.Id == state.Players[1].Deck!.Id) return;

        Guid winnerId = (Guid)state.GameActionResult.Winner;
        int turns = state.TurnCounter;

        lock (_lock)
        {
            TotalGamesPlayed++;

            var deckId1 = state.Players[0].Deck!.Id;
            var deckId2 = state.Players[1].Deck!.Id;

            UpdateStats(deckId1, deckId2, state.Players[0].Id  == winnerId, turns, state.TotalInSeconds);
        }
    }

    public static StatsDto GetStats(string language)
    {
        lock (_lock)
        {
            var allDecks = CardManager.Decks.Select(d => TranslationManager.TranslateDeck(d, language)).ToList();

            var decks = allDecks
                .Select(d => DeckStatsDto.Generate(d, Data.TryGetValue(d.id, out var s) ? s : null, allDecks))
                .ToList();

            return new StatsDto(TotalGamesPlayed, decks);
        }
    }
}
