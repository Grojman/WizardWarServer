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
    public List<IEffect> Effects { get; set; } = new();

    public EffectDuration Duration { get; set; } = new Always();
    public EffectCondition? Condition { get; set; } = null;

    public CardInstance SourceCard { get; set; } = null!;

    public PlayerState Player { get; set; } = null!;
    public bool Expired { get => Duration.IsExpired(); }

    public string Description { get; set; } = string.Empty;

    // Set (only) by GameState.AddGlobalEffect, once this instance actually
    // lives in a PlayerState.GlobalEffects list. Lets TryExecute/ForceExecute
    // below attribute whatever this effect does back to itself (via
    // GameState.CurrentGlobalEffectSource) instead of to SourceCard — the
    // card that originally granted the aura may be long dead/off the board
    // by the time the aura fires again, so it's not a meaningful animation
    // source for the client.
    public bool IsGlobal { get; set; } = false;

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
        GameEvent? ev,
        bool checkExpiration = true)
    {
        if (Expired && checkExpiration)
            return;

        if (!(Condition?.Check(Player.Id, Player.PlayerTarget!.Id, SourceCard, state, ev) ?? true))
            return;

        if (ev is not null && ev.Source.Id == Id)
            return;

        var previousGlobalSource = state.CurrentGlobalEffectSource;
        if (IsGlobal) state.CurrentGlobalEffectSource = this;
        try
        {
            Effects.ForEach(n => n.Execute(Player.Id, Player.PlayerTarget!.Id, SourceCard, state, ev));
            Duration.NotifyExecution();
        }
        finally
        {
            state.CurrentGlobalEffectSource = previousGlobalSource;
        }
    }

    public void ForceExecute(
        GameState state,
        GameEvent? ev,
        bool notifyExec
    )
    {
        var previousGlobalSource = state.CurrentGlobalEffectSource;
        if (IsGlobal) state.CurrentGlobalEffectSource = this;
        try
        {
            Effects.ForEach(n => n.Execute(Player.Id, Player.PlayerTarget!.Id, SourceCard, state, ev));
            if (notifyExec) Duration.NotifyExecution();
        }
        finally
        {
            state.CurrentGlobalEffectSource = previousGlobalSource;
        }
    }
}