// Cierto solo cuando el evento disparador es un GameEvent.GlobalEffectAdded del PROPIO
// jugador (no del rival) cuyo efecto añadido sea, mediante upcast, un ColorMarkerEffect
// con exactamente el color indicado (no hay un evento dedicado a cambios de color: se
// detecta a partir del mecanismo genérico de PlayerState.GlobalEffects).
public class ColorChangedToCondition : EffectCondition
{
    public ColorChangedToCondition(ChromaticColor color)
    {
        Color = color;
    }

    public ChromaticColor Color { get; set; }

    public override bool Check(Guid playerId, Guid rivalId, CardInstance sourceCard, GameState state, GameEvent? ev)
    {
        return ev is GameEvent.GlobalEffectAdded gea
            && gea.PlayerId == playerId
            && gea.Effect.Effects.OfType<ColorMarkerEffect>().Any(m => m.Color == Color);
    }

    public override EffectCondition Clone() => new ColorChangedToCondition(Color);
}
