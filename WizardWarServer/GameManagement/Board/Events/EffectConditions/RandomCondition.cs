
public class RandomCondition : EffectCondition
{
    public RandomCondition(int probabilty)
    {
        Probabilty = probabilty;
    }

    public int Probabilty { get; set; } = 0;
    public override bool Check(Guid playerId, Guid rivalId, CardInstance sourceCard, GameState state, GameEvent? ev)
    {
        return new Random().Next(0, 100) < Probabilty;
    }

    public override EffectCondition Clone() => new RandomCondition(Probabilty);
}