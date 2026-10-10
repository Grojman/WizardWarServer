// Lee el color activo del jugador SIN rotarlo y ejecuta la(s) rama(s) de todos los
// colores básicos que estén "contenidos" en el color activo (un mixto cuenta como
// sus dos componentes; Blanco cuenta como los tres). Pueden dispararse varias ramas
// a la vez si el color activo es uno mixto o Blanco.
public class BranchOnColorInclusiveEffect : IEffect
{
    public BranchOnColorInclusiveEffect(IEffect[] ifRed, IEffect[] ifGreen, IEffect[] ifBlue)
    {
        IfRed = ifRed;
        IfGreen = ifGreen;
        IfBlue = ifBlue;
    }

    public IEffect[] IfRed { get; set; }
    public IEffect[] IfGreen { get; set; }
    public IEffect[] IfBlue { get; set; }

    public IEffect Clone() => new BranchOnColorInclusiveEffect(
        [.. IfRed.Select(e => e.Clone())],
        [.. IfGreen.Select(e => e.Clone())],
        [.. IfBlue.Select(e => e.Clone())]);

    public void Execute(Guid playerId, Guid rivalId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        var player = state.GetState(playerId);
        var active = ChromaticColorHelper.GetActiveBaseComponents(player);

        var branches = new List<IEffect>();

        if (active.Contains(ChromaticColor.Rojo)) branches.AddRange(IfRed);

        if (active.Contains(ChromaticColor.Verde)) branches.AddRange(IfGreen);

        if (active.Contains(ChromaticColor.Azul)) branches.AddRange(IfBlue);

        lastExecuted = branches;
        foreach (var e in branches) e.Execute(playerId, rivalId, cardId, state, ev);
    }

    // Extra targets get the branches chosen for the first one, even if a
    // branch changed the active color in between.
    List<IEffect> lastExecuted = new();

    public IEffect? ForAdditionalTarget() => EffectSequence.From(lastExecuted.Select(e => e.ForAdditionalTarget()));
}
