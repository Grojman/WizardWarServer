public class KillMySelf : IEffect
{
    public IEffect Clone() => new KillMySelf();

    public void Execute(Guid playerId, Guid rivalId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        state.KillUnit(cardId, cardId);
    }

    // Only touches its own player/card.
    public IEffect? ForAdditionalTarget() => null;
}