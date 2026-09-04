// Marcador inerte: representa "el color activo del jugador es Color" dentro de una
// entrada de PlayerState.GlobalEffects. No hace nada al ejecutarse; otros efectos y
// condiciones lo leen a través de ChromaticColorHelper.
public class ColorMarkerEffect : IEffect
{
    public ColorMarkerEffect(ChromaticColor color)
    {
        Color = color;
    }

    public ChromaticColor Color { get; set; }

    public IEffect Clone() => new ColorMarkerEffect(Color);

    public void Execute(Guid playerId, Guid rivalId, CardInstance cardId, GameState state, GameEvent? ev)
    {
    }
}
