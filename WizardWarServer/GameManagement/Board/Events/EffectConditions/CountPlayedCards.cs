
public class CountPlayedCardsCondition : EffectCondition
{
    public CountPlayedCardsCondition(CardFilter filter, PlayerType target, int amount, CountType countType)
    {
        Filter = filter;
        Target = target;
        Amount = amount;
        CountType = countType;
    }


    public CardFilter Filter { get; set; }
    public PlayerType Target { get; set; }
    public int Amount { get; set; }
    public CountType CountType { get; set; }

    public override bool Check(Guid playerId, Guid rivalId, CardInstance sourceCard, GameState state, GameEvent? ev)
    {
        var count = state.GetState( Target == PlayerType.PLAYER ? playerId : rivalId).PlayedCards.Count(Filter.Check);

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

    public override EffectCondition Clone() => new CountPlayedCardsCondition(Filter, Target, Amount, CountType);
}