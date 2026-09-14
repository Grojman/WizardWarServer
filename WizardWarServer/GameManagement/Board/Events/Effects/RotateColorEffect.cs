// Rota el color cromático del jugador: Rojo -> Verde -> Azul -> Rojo. Todo marcador
// activo se rota a la vez, incluidos los mixtos (Amarillo -> Celeste -> Morado ->
// Amarillo), de modo que un básico y sus mixtos añadidos encima roten juntos.
// Si no hay color activo, lo crea (Rojo). Si el color activo es Blanco, la rotación
// no tiene efecto (es un estado terminal).
public class RotateColorEffect : IEffect
{
    public IEffect Clone() => new RotateColorEffect();

    public void Execute(Guid playerId, Guid rivalId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        var player = state.GetState(playerId);
        var currents = ChromaticColorHelper.TryGetColors(player)
            .Where(c => c is not null)
            .Select(c => c!.Value)
            .ToList();

        if (currents.Count == 0)
        {
            ChromaticColorHelper.SetColor(state, cardId, player, ChromaticColor.Rojo);
            return;
        }

        if (currents.Contains(ChromaticColor.Blanco)) return;

        var rotated = currents
            .Select(c => ChromaticColorHelper.IsBase(c) ? ChromaticColorHelper.NextBase(c) : ChromaticColorHelper.NextMixed(c))
            .ToList();

        ChromaticColorHelper.SetColor(state, cardId, player, rotated[0]);
        foreach (var c in rotated.Skip(1))
            ChromaticColorHelper.AddColor(state, cardId, player, c);
    }
}
