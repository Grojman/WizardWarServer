using Serilog;

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

    // The condition is checked once per target; the effects run in full
    // against the first target that passes it and only their rival-facing
    // part against the rest (see EffectFanOut).
    public void TryExecute(
        GameState state,
        GameEvent? ev,
        bool checkExpiration = true)
    {
        if (Expired && checkExpiration)
            return;

        var targets = EffectFanOut.PassingTargets(Condition, Player, Player.Targets.Players, SourceCard, state, ev);
        if (targets.Count == 0)
            return;

        if (ev is not null && ev.Source.Id == Id)
            return;

        Run(state, ev, targets, notifyExec: true);
    }

    public void ForceExecute(
        GameState state,
        GameEvent? ev,
        bool notifyExec
    )
    {
        Run(state, ev, [.. Player.Targets.Players], notifyExec);
    }

    void Run(GameState state, GameEvent? ev, IReadOnlyList<PlayerState> targets, bool notifyExec)
    {
        var previousGlobalSource = state.CurrentGlobalEffectSource;
        if (IsGlobal) state.CurrentGlobalEffectSource = this;
        try
        {
            EffectFanOut.Execute(Effects, Player, targets, SourceCard, state, ev);
            if (notifyExec) Duration.NotifyExecution();
        }
        catch (Exception e)
        {
            // A single broken effect must not abort the rest of the action
            // (and with it the turn change / state broadcast).
            Log.Error(e, "Effect of card {CardId} failed while executing", SourceCard?.Definition.Id);
        }
        finally
        {
            state.CurrentGlobalEffectSource = previousGlobalSource;
        }
    }
}
