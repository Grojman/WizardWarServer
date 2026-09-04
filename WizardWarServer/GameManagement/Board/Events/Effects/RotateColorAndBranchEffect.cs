// Rota el color (ver RotateColorEffect) y, si tras rotar el color activo es
// exactamente un color básico (Rojo/Verde/Azul), ejecuta la lista de efectos
// asociada a ese color. Si la rotación no hizo nada (había un color mixto o
// Blanco activo), no se ejecuta ninguna rama.
public class RotateColorAndBranchEffect : IEffect
{
    public RotateColorAndBranchEffect(IEffect[] ifRed, IEffect[] ifGreen, IEffect[] ifBlue)
    {
        IfRed = ifRed;
        IfGreen = ifGreen;
        IfBlue = ifBlue;
    }

    public IEffect[] IfRed { get; set; }
    public IEffect[] IfGreen { get; set; }
    public IEffect[] IfBlue { get; set; }

    public IEffect Clone() => new RotateColorAndBranchEffect(
        [.. IfRed.Select(e => e.Clone())],
        [.. IfGreen.Select(e => e.Clone())],
        [.. IfBlue.Select(e => e.Clone())]);

    public void Execute(Guid playerId, Guid rivalId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        new RotateColorEffect().Execute(playerId, rivalId, cardId, state, ev);

        var player = state.GetState(playerId);
        var current = ChromaticColorHelper.TryGetSingleColor(player);
        if (current is null) return;

        IEffect[]? branch = current.Value switch
        {
            ChromaticColor.Rojo => IfRed,
            ChromaticColor.Verde => IfGreen,
            ChromaticColor.Azul => IfBlue,
            _ => null
        };

        if (branch is null) return;

        foreach (var e in branch) e.Execute(playerId, rivalId, cardId, state, ev);
    }
}
