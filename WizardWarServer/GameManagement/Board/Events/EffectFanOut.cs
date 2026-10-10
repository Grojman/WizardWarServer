// Runs a list of effects against every member of a player's TargetGroup. The
// first target gets each effect in full; every other target only gets the
// part of it aimed at the rival (IEffect.ForAdditionalTarget), so whatever an
// effect does to its own player (draw, heal, buff my board...) happens once
// while whatever it does to "the rival" happens once per targeted player.
public static class EffectFanOut
{
    public static void Execute(
        IEnumerable<IEffect> effects,
        PlayerState player,
        IReadOnlyList<PlayerState> targets,
        CardInstance source,
        GameState state,
        GameEvent? ev)
    {
        var list = effects as IList<IEffect> ?? effects.ToList();

        for (int i = 0; i < targets.Count; i++)
        {
            // Killed by an earlier pass: nothing left to hit on that side.
            if (i > 0 && !state.IsAlive(targets[i])) continue;

            foreach (var effect in list)
            {
                // Resolved lazily so effects that branch (random, color...)
                // can hand back the branch the first target actually took.
                var toRun = i == 0 ? effect : effect.ForAdditionalTarget();
                toRun?.Execute(player.Id, targets[i].Id, source, state, ev);
            }
        }
    }

    // Moves the targets that satisfy `condition` to the front, so the first
    // one that passes is the one receiving the effects in full.
    public static List<PlayerState> PassingTargets(
        EffectCondition? condition,
        PlayerState player,
        IReadOnlyList<PlayerState> targets,
        CardInstance source,
        GameState state,
        GameEvent? ev)
    {
        if (condition is null) return [.. targets];
        return [.. targets.Where(t => condition.Check(player.Id, t.Id, source, state, ev))];
    }
}

// Runtime-only effect used to hand back several effects as one, e.g. the
// rival-facing part of a branch that has already been chosen. Never serialized.
public class EffectSequence : IEffect
{
    public EffectSequence(IEnumerable<IEffect> effects)
    {
        Effects = [.. effects];
    }

    public List<IEffect> Effects { get; }

    public IEffect Clone() => new EffectSequence(Effects.Select(e => e.Clone()));

    public IEffect? ForAdditionalTarget() => From(Effects.Select(e => e.ForAdditionalTarget()));

    public void Execute(Guid playerId, Guid rivalId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        foreach (var e in Effects) e.Execute(playerId, rivalId, cardId, state, ev);
    }

    public static IEffect? From(IEnumerable<IEffect?> effects)
    {
        var list = effects.Where(e => e is not null).Select(e => e!).ToList();
        return list.Count == 0 ? null : new EffectSequence(list);
    }
}
