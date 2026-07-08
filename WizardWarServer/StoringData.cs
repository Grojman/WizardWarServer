using System.Text.Json;

public class GameData
{
    public GameData()
    {
    }

    public GameData(List<int> playedDecks, int winnerDeck, bool forced)
    {
        PlayedDecks = playedDecks;
        WinnerDeck = winnerDeck;
        Forced = forced;
    }

    public List<int> PlayedDecks { get; set; } = new();
    public int WinnerDeck { get; set; }
    public bool Forced { get; set; }
    
}

public static class StoringData
{
    public static List<GameData> Data { get; private set; } = new();

    public const string FILE_PATH = "data.json";

    private static string GetFilePath() => Path.Combine(AppContext.BaseDirectory, FILE_PATH);

    public static void GetFromFile()
    {
        var filePath = GetFilePath();
        if (!File.Exists(filePath))
        {
            Data = new();
            return;
        }

        try
        {
            var json = File.ReadAllText(filePath);
            var res = JsonSerializer.Deserialize<List<GameData>>(json);
            Data = res ?? new();
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"No se pudo leer el archivo de datos: {ex.Message}");
            Data = new();
        }
    }

    public static void SaveInFile()
    {
        var filePath = GetFilePath();
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var json = JsonSerializer.Serialize(Data ?? new(), new JsonSerializerOptions()
        {
            WriteIndented = true
        });

        File.WriteAllText(filePath, json);
    }

    public static void SaveData(GameState state, bool forced)
    {
        Console.WriteLine("Saving new data");
        var data = new GameData
        {
            PlayedDecks = state.Players.Select(n => n.Deck!.Id).ToList()
        };
        if (state.GameActionResult.Winner is not null)
        {
            data.WinnerDeck = state.GetState((Guid)state.GameActionResult.Winner).Deck!.Id;
        }
        data.Forced = forced;

        Data.Add(data);
    }

}