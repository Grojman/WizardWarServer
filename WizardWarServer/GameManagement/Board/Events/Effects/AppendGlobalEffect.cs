
public class AppendGlobalEffect : IEffect
{
    public string Description { get; set; }
    public AppendGlobalEffect(EffectInstance effect, string description)
    {
        Effect = effect;
        Effect.Description = description;
        Description = description;
    }

    public EffectInstance Effect { get; set; }
    public IEffect Clone() => new AppendGlobalEffect(Effect.Clone(), Description);

    public void Execute(Guid playerId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        cardId.AssignEffect(Effect);
        state.GetState(playerId).GlobalEffects.Add(Effect);
    }
}