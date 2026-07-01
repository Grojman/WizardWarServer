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

    public List<int> PlayedDecks { get; set; }
    public int WinnerDeck { get; set; }
    public bool Forced { get; set; }
    
}

public static class StoringData
{
    public static List<GameData> Data;

    public const string FILE_PATH = "data.json";
    public static void GetFromFile()
    {
        if (!File.Exists(FILE_PATH))
        {
            Data = new();
            return;
        }

        var res = JsonSerializer.Deserialize<List<GameData>>(FILE_PATH);
        if (res is null)
        {
            Console.WriteLine("No data saved");
        }
        Data = res ?? new();
    }

    public static void SaveInFile()
    {
        JsonSerializer.Serialize(Data, new JsonSerializerOptions()
        {
            WriteIndented = true
        });
    }

    public static void SaveData(GameState state, bool forced)
    {
        var data = new GameData
        {
            PlayedDecks = state.Players.Select(n => n.Deck.Id).ToList()
        };
        if (state.GameActionResult.Winner is not null)
        {
            data.WinnerDeck = state.GetState((Guid)state.GameActionResult.Winner).Deck.Id;
        }
        data.Forced = forced;

        Data.Add(data);
    }

}