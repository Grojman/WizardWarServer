// Roba una carta por cada unidad que cumpla el filtro de tablero indicado.
// Con ExcludeSelf = true, la propia carta que ejecuta el efecto no cuenta
// (útil para "una carta por cada OTRA unidad en mesa").
public class DrawCardPerBoardUnitEffect : IEffect
{
    public DrawCardPerBoardUnitEffect(PlayerType whichBoard, bool excludeSelf)
    {
        WhichBoard = whichBoard;
        ExcludeSelf = excludeSelf;
    }

    public PlayerType WhichBoard { get; set; }
    public bool ExcludeSelf { get; set; }

    public IEffect Clone() => new DrawCardPerBoardUnitEffect(WhichBoard, ExcludeSelf);

    public void Execute(Guid playerId, Guid rivalId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        var player = state.GetState(playerId);
        var rival = state.GetState(rivalId);

        int count = 0;
        if (WhichBoard is PlayerType.PLAYER or PlayerType.BOTH)
            count += player.Board.Count(c => c is not null && (!ExcludeSelf || c.Id != cardId.Id));
        if (WhichBoard is PlayerType.RIVAL or PlayerType.BOTH)
            count += rival.Board.Count(c => c is not null && (!ExcludeSelf || c.Id != cardId.Id));

        for (int i = 0; i < count; i++)
            state.DrawCard(player.Connection, cardId);
    }
}
