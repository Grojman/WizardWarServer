public class GameActionResult
{
    public Queue<GameEventDto> Events { get; } = new();
    public bool GameEnded { get; set; }
    public Guid? Winner { get; set; }
    public required GameState State { get; set; }

    public void AddEvent(GameEvent e)
    {
        Events.Enqueue(GameEventDto.Generate(e, State));
    }
}