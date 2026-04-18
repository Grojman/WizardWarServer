public class GameContext
{
    public PlayerState Player1 { get; }

    public PlayerState Player2 { get; }

    public GameSession Session { get; }

    public GameContext(
        PlayerState p1,
        PlayerState p2,
        GameSession session)
    {
        Player1 = p1;
        Player2 = p2;
        Session = session;
    }

    public PlayerState GetOpponent(PlayerState player)
    {
        return player == Player1
            ? Player2
            : Player1;
    }
}