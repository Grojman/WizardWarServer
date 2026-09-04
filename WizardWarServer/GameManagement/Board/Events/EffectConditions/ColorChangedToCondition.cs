// Cierto solo cuando el evento disparador es un GameEvent.PlayerColorChanged del
// PROPIO jugador (no del rival) cuyo NewColor sea exactamente el color indicado.
public class ColorChangedToCondition : EffectCondition
{
    public ColorChangedToCondition(ChromaticColor color)
    {
        Color = color;
    }

    public ChromaticColor Color { get; set; }

    public override bool Check(Guid playerId, Guid rivalId, CardInstance sourceCard, GameState state, GameEvent? ev)
    {
        return ev is GameEvent.PlayerColorChanged pc && pc.NewColor == Color && pc.PlayerId == playerId;
    }

    public override EffectCondition Clone() => new ColorChangedToCondition(Color);
}
