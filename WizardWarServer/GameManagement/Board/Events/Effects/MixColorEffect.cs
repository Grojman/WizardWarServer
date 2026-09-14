// Si no hay color activo, crea un color base al azar.
// Si el color activo es básico, añade un color mixto por encima (sin quitar el
// básico), en vez de reemplazarlo.
// Si el color activo ya es un mixto, añade un segundo mixto distinto por encima
// -> al haber dos mixtos activos a la vez, ambos (y el básico) se reemplazan de
// golpe por un único efecto Blanco.
// Si ya es Blanco, es un estado terminal y no hace nada más.
public class MixColorEffect : IEffect
{
    readonly ChromaticColor[] BASE = [ChromaticColor.Rojo, ChromaticColor.Verde, ChromaticColor.Azul];
    readonly ChromaticColor[] MIXED = [ChromaticColor.Amarillo, ChromaticColor.Celeste, ChromaticColor.Morado];
    public IEffect Clone() => new MixColorEffect();

    public void Execute(Guid playerId, Guid rivalId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        var player = state.GetState(playerId);
        var currents = ChromaticColorHelper.TryGetColors(player);
        var current = ChromaticColorHelper.TryGetSingleColor(player);
        if (!currents.Any())
        {
            ChromaticColorHelper.SetColor(state, cardId, player, BASE.GetRandom());
            return;
        }

        if (ChromaticColorHelper.IsBase(current.Value))
        {
            current = currents.FirstOrDefault(c => !ChromaticColorHelper.IsBase(c.Value));
        }

        switch (currents.Count(n => n != null && MIXED.Contains(n.Value)))
        {
            case 0:
                ChromaticColorHelper.AddColor(state, cardId, player, MIXED.GetRandom());
                return;
            case 1:
                ChromaticColorHelper.AddColor(state, cardId, player, MIXED.Where((c) => c != current).GetRandom());
                return;
            case 2:
                ChromaticColorHelper.SetColor(state, cardId, player, ChromaticColor.Blanco);
                return;
            default:
                return;
        }
    }
}
