
// Rolls once and executes exactly one of the two branches — never both, never neither.
public class RandomBranchEffect : IEffect
{
    public RandomBranchEffect(int probability, IEffect[] ifSuccess, IEffect[] ifFailure)
    {
        Probability = probability;
        IfSuccess = ifSuccess;
        IfFailure = ifFailure;
    }

    public int Probability { get; set; }
    public IEffect[] IfSuccess { get; set; }
    public IEffect[] IfFailure { get; set; }

    public IEffect Clone() => new RandomBranchEffect(
        Probability,
        [.. IfSuccess.Select(e => e.Clone())],
        [.. IfFailure.Select(e => e.Clone())]);

    public void Execute(Guid playerId, Guid rivalId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        var branch = new Random().Next(0, 100) < Probability ? IfSuccess : IfFailure;
        foreach (var e in branch) e.Execute(playerId, rivalId, cardId, state, ev);
    }
}
