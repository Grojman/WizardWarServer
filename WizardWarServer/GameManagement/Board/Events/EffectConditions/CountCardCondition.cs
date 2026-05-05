
public class CountCardCondition : EffectCondition
{
    public CountCardCondition(GameFilter filter, int amount, CountType countType)
    {
        Filter = filter;
        Amount = amount;
        CountType = countType;
    }


    public GameFilter Filter { get; set; }
    public int Amount { get; set; }
    public CountType CountType { get; set; }

    public override bool Check(Guid playerId, CardInstance sourceCard, GameState state, GameEvent? ev)
    {
        var count = Filter.GetMeetingCards(state, playerId).Count();
        return CountType switch
        {
            CountType.AT_LEAST => count >= Amount,
            CountType.AT_MAX => count <= Amount,
            CountType.EXACTLY => count == Amount,
            CountType.AT_LEAST_OVER => count > Amount,
            CountType.AT_MAX_UNDER => count < Amount,
            _ => false
        };
    }

    public override EffectCondition Clone() => new CountCardCondition(Filter, Amount, CountType);
}