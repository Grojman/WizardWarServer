
public class ForceBasicColorEffect : IEffect
{
    public ChromaticColor Color { get; set; }

    public ForceBasicColorEffect(ChromaticColor color)
    {
        Color = color;
    }

    public IEffect Clone() => new ForceBasicColorEffect(Color);

    public void Execute(Guid playerId, Guid rivalId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        if(!ChromaticColorHelper.IsBase(Color))
        {
            throw new InvalidDataException("Color is not a base color");
        }

        var player = state.GetState(playerId);

        var colors = ChromaticColorHelper.TryGetColors(player);

        if (colors.Any(n => n is not null && ChromaticColorHelper.IsMixed(n.Value)))
        {
            return;
        }

        ChromaticColorHelper.SetColor(state, cardId, player, Color);
    }
}