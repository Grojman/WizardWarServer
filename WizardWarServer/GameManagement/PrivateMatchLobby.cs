public class PrivateMatchLobby
{
    public required string Code { get; init; }
    public required PlayerConnection Host { get; set; }
    public required int NumberOfPlayers { get; init; }
    public MatchFormat Format { get; init; } = MatchFormat.Single;
    public List<PlayerConnection> Players { get; } = new();
}
