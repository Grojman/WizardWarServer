// The rivals a player's effects currently aim at. In a 1v1 it always holds the
// single opponent, so everything behaves exactly as with a plain target. In
// games with more than two players the "hunted" player of the round holds
// every other alive player, and everyone else holds just the hunted player
// (see GameState.RecomputeTargets).
//
// Effects never see the group: EffectInstance/GameState run them once per
// member (see EffectFanOut), passing that member as the usual `rivalId`.
public class TargetGroup
{
    readonly List<PlayerState> players = new();

    public IReadOnlyList<PlayerState> Players => players;

    // First member: the one a single-target consumer (DTOs, bots, condition
    // progress) should look at.
    public PlayerState? Primary => players.Count > 0 ? players[0] : null;

    public bool Contains(PlayerState player) => players.Contains(player);

    // Returns whether the membership actually changed.
    public bool Set(IEnumerable<PlayerState> newPlayers)
    {
        var list = newPlayers.ToList();
        if (list.SequenceEqual(players)) return false;

        players.Clear();
        players.AddRange(list);
        return true;
    }

    public void Clear() => players.Clear();
}
