// Cierto si el color activo del jugador es, o contiene como componente, el color
// indicado (p.ej. Rojo también es válido cuando el color activo es Amarillo,
// Morado o Blanco, por formarse a partir del rojo).
public class PlayerHasColorCondition : EffectCondition
{
    public PlayerHasColorCondition(ChromaticColor color)
    {
        Color = color;
    }

    public ChromaticColor Color { get; set; }

    public override bool Check(Guid playerId, Guid rivalId, CardInstance sourceCard, GameState state, GameEvent? ev)
    {
        var player = state.GetState(playerId);
        return ChromaticColorHelper.GetActiveBaseComponents(player).Contains(Color);
    }

    public override EffectCondition Clone() => new PlayerHasColorCondition(Color);
}
