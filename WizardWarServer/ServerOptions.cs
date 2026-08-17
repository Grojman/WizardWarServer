public class ServerOptions
{
    public const string SectionName = "ServerSettings";

    /// <summary>Origins allowed to open a WebSocket connection (CSWSH protection). Empty = no allowlist enforced.</summary>
    public string[] AllowedOrigins { get; set; } = [];

    public int MaxMessageSizeBytes { get; set; } = 65536;
    public int MessageRateLimitPerSecond { get; set; } = 20;
    public int ConnectionRateLimitPerMinute { get; set; } = 30;
    public int MaxSuggestionLength { get; set; } = 2000;
    public int MinPlayersPerGame { get; set; } = 1;
    public int MaxPlayersPerGame { get; set; } = 8;
    public bool SeedCardDataIfMissing { get; set; } = true;
    public string DataDirectory { get; set; } = "Data/Decks";
    public int DisconnectGracePeriodSeconds { get; set; } = 30;

    public bool IsValidPlayerCount(int count) => count >= MinPlayersPerGame && count <= MaxPlayersPerGame;
}
