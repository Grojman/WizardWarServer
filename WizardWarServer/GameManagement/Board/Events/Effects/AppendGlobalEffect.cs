
public class AppendGlobalEffect : IEffect
{
    public AppendGlobalEffect(EffectInstance effect)
    {
        Effect = effect;
    }

    public EffectInstance Effect { get; set; }
    public IEffect Clone() => new AppendGlobalEffect(Effect);

    public void Execute(Guid playerId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        cardId.AssignEffect(Effect);
        state.GlobalEffects.Add(Effect);
    }
}