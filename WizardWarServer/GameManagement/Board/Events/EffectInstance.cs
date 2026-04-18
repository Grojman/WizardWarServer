public class EffectInstance : IdentificableObject, ICloneable<EffectInstance>
{
    public EffectInstance()
    {
    }

    public EffectInstance(TriggerType trigger, IEffect effect, EffectDuration duration, EffectCondition? condition)
    {
        Trigger = trigger;
        Effect = effect;
        Duration = duration;
        Condition = condition;
    }

    public TriggerType Trigger { get; set; }
    public IEffect Effect { get; set; }

    public EffectDuration Duration { get; set; }
    public EffectCondition? Condition { get; set; } = null;

    public Guid SourceCardId { get; set; }

    public Guid PlayerSourceId { get; set; }
    public bool Expired { get => Duration.IsExpired(); }


    public EffectInstance Clone()
    {
        return new()
        {
            Trigger = Trigger,
            SourceCardId = SourceCardId,
            PlayerSourceId = PlayerSourceId,
            Duration = Duration.Clone(),
            Condition = Condition?.Clone(),
            Effect = Effect
        };
    }

    public void TryExecute(
        GameState state,
        GameEvent? ev)
    {
        if (Expired)
            return;

        if (!(Condition?.Check(PlayerSourceId, SourceCardId, state, ev) ?? true))
            return;

        if (ev is not null && ev.Source.Id == Id)
            return;

        Effect.Execute(PlayerSourceId, SourceCardId, state, ev);

        Duration.NotifyExecution();
    }
}