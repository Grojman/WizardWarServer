// Rota el color cromático del jugador: Rojo -> Verde -> Azul -> Rojo.
// Si no hay color activo, lo crea (Rojo). Si el color activo es uno mixto o Blanco,
// la rotación no tiene efecto (solo se rota entre colores básicos).
public class RotateColorEffect : IEffect
{
    public IEffect Clone() => new RotateColorEffect();

    public void Execute(Guid playerId, Guid rivalId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        var player = state.GetState(playerId);
        var current = ChromaticColorHelper.TryGetSingleColor(player);

        if (current is null)
        {
            ChromaticColorHelper.SetColor(state, cardId, player, null, ChromaticColor.Rojo);
            return;
        }

        if (!ChromaticColorHelper.IsBase(current.Value)) return;

        ChromaticColorHelper.SetColor(state, cardId, player, current.Value, ChromaticColorHelper.NextBase(current.Value));
    }
}
