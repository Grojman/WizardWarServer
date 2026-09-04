// Si no hay color activo, crea el primer color mixto (Amarillo).
// Si el color activo es básico, lo reemplaza por su color mixto derivado
// (Rojo->Amarillo, Verde->Celeste, Azul->Morado).
// Si el color activo ya es un mixto, ese sería el segundo mixto distinto que se
// añade -> ambos se reemplazan de golpe por un único efecto Blanco.
// Si ya es Blanco, es un estado terminal y no hace nada más.
public class MixColorEffect : IEffect
{
    public IEffect Clone() => new MixColorEffect();

    public void Execute(Guid playerId, Guid rivalId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        var player = state.GetState(playerId);
        var current = ChromaticColorHelper.TryGetSingleColor(player);

        if (current is null)
        {
            ChromaticColorHelper.SetColor(state, cardId, player, null, ChromaticColor.Amarillo);
            return;
        }

        if (ChromaticColorHelper.IsBase(current.Value))
        {
            ChromaticColorHelper.SetColor(state, cardId, player, current.Value, ChromaticColorHelper.MixOf(current.Value));
            return;
        }

        if (ChromaticColorHelper.IsMixed(current.Value))
        {
            ChromaticColorHelper.SetColor(state, cardId, player, current.Value, ChromaticColor.Blanco);
        }
    }
}
