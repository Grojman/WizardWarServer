// Cierto si el color activo del jugador es EXACTAMENTE uno de los colores indicados.
// A diferencia de PlayerHasColorCondition (que comprueba componentes básicos, p.ej.
// "contiene el rojo"), esta condición compara el color activo tal cual: útil para
// restricciones como "amarillo o blanco" o "blanco", donde el color exigido puede
// ser en sí mismo un mixto y no tiene sentido descomponerlo en componentes.
public class PlayerColorIsCondition : EffectCondition
{
    public PlayerColorIsCondition(params ChromaticColor[] colors)
    {
        Colors = colors;
    }

    public ChromaticColor[] Colors { get; set; }

    public override bool Check(Guid playerId, Guid rivalId, CardInstance sourceCard, GameState state, GameEvent? ev)
    {
        var player = state.GetState(playerId);
        var current = ChromaticColorHelper.TryGetSingleColor(player);
        return current is not null && Colors.Contains(current.Value);
    }

    public override EffectCondition Clone() => new PlayerColorIsCondition(Colors);
}
