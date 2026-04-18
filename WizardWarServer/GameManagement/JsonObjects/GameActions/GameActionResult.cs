public class GameActionResult
{
    public Queue<GameEvent> Events { get; } = new();
    public bool GameEnded { get; set; }
    public Guid? Winner { get; set; }
}