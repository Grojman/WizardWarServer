// Reactiva los efectos de tipo TriggerType.UnitPlayed de las unidades vivas en el
// tablero indicado, sin gastar su presupuesto de ejecuciones (igual que
// RetriggerSpellEffect hace con los efectos de un hechizo). Al usar ForceExecute, no
// se comprueba la Condition de cada EffectInstance ni si ya está Expired.
public class RetriggerUnitPlayedEffect : IEffect
{
    public RetriggerUnitPlayedEffect(PlayerType whichBoard)
    {
        WhichBoard = whichBoard;
    }

    public PlayerType WhichBoard { get; set; }

    public IEffect Clone() => new RetriggerUnitPlayedEffect(WhichBoard);

    public void Execute(Guid playerId, Guid rivalId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        if (WhichBoard is PlayerType.PLAYER or PlayerType.BOTH)
            Retrigger(state, state.GetState(playerId));

        if (WhichBoard is PlayerType.RIVAL or PlayerType.BOTH)
            Retrigger(state, state.GetState(rivalId));
    }

    static void Retrigger(GameState state, PlayerState player)
    {
        foreach (var unit in player.Board)
        {
            if (unit is null) continue;

            foreach (var e in unit.Effects)
                if (e.Trigger == TriggerType.UnitPlayed)
                    e.ForceExecute(state, null, false);
        }
    }
}
