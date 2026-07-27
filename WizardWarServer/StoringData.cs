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
    public static Dictionary<int, Dictionary<int, GameData>> Data { get; private set; } = new();

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
            var res = JsonSerializer.Deserialize<Dictionary<int, Dictionary<int, GameData>>>(json);
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

        var json = JsonSerializer.Serialize(Data ?? [], new JsonSerializerOptions()
        {
            WriteIndented = true,
        });

        File.WriteAllText(filePath, json);
    }

    public static void SaveData(GameState state, bool forced)
    {
        if (state.GameActionResult.Winner is not null)
        {
            Guid winnerId = (Guid)state.GameActionResult.Winner;
            int winerDeck = state.GetState(winnerId).Deck!.Id;

            Dictionary<int,GameData> values;

            if (!Data.ContainsKey(winerDeck))
            {
                values = [];
                Data.Add(winerDeck, values);
            } else
            {
                values = Data[winerDeck];
            }

            HashSet<int> appendedIds = [];

            foreach(var player in state.Players.Where(n => n.Id != winnerId))
            {
                var id = player.Deck!.Id;
                if (appendedIds.Add(id))
                {
                    if (!Data.ContainsKey(id))
                    {
                        Data.Add(id, []);
                    }

                    if(!Data[id].ContainsKey(winerDeck))
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