public class EffectInstance : IdentificableObject, ICloneable<EffectInstance>
{
    public EffectInstance()
    {
    }

    public EffectInstance(TriggerType trigger, List<IEffect> effect, EffectDuration duration, EffectCondition? condition)
    {
        Trigger = trigger;
        Effects = effect;
        Duration = duration;
        Condition = condition;
    }

    public TriggerType Trigger { get; set; }
    public List<IEffect> Effects { get; set; }

    public EffectDuration Duration { get; set; }
    public EffectCondition? Condition { get; set; } = null;

    public CardInstance SourceCard { get; set; }

    public PlayerState Player { get; set; }
    public bool Expired { get => Duration.IsExpired(); }

    public string Description { get; set; }

    public EffectInstance Clone()
    {
        return new()
        {
            Trigger = Trigger,
            SourceCard = SourceCard,
            Player = Player,
            Duration = Duration.Clone(),
            Condition = Condition?.Clone(),
            Effects = [.. Effects.Select(n => n.Clone())]
        };
    }

    public void TryExecute(
        GameState state,
        GameEvent? ev)
    {
        if (Expired)
            return;

        if (!(Condition?.Check(Player.Id, Player.PlayerTarget.Id, SourceCard, state, ev) ?? true))
            return;

        if (ev is not null && ev.Source.Id == Id)
            return;

        Effects.ForEach(n => n.Execute(Player.Id, Player.PlayerTarget.Id, SourceCard, state, ev));

        Duration.NotifyExecution();
    }
}