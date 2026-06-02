public class RetriggerSpellEffect : IEffect
{
    public IEffect Clone() => new RetriggerSpellEffect();

    public void Execute(Guid playerId, Guid rivalId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        if (ev is GameEvent.SpellPlayed s)
        {
            foreach(var a in s.Card.Effects)
            {
                a.TryExecute(state, null, false);
            }
        }
    }
}