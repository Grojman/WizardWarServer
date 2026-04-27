
public class DamagePlayerBasedOnCards : IEffect
{
    public DamagePlayerBasedOnCards()
    {
    }

    public DamagePlayerBasedOnCards(bool roundedUp, float relation, PlayerType targetPlayer, GameFilter filter)
    {
        RoundedUp = roundedUp;
        Relation = relation;
        TargetPlayer = targetPlayer;
        Filter = filter;
    }

    public bool RoundedUp { get; set; }
    public float Relation { get; set; }
    public PlayerType TargetPlayer { get; set; }
    public GameFilter Filter { get; set; }


    public IEffect Clone() => new DamagePlayerBasedOnCards(RoundedUp, Relation, TargetPlayer, Filter);

    public void Execute(Guid playerId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        int cardCounter = Filter.GetMeetingCards(state, playerId).Count();

        var rival = state.GetRival(playerId);
        var player = state.GetState(playerId);

        float calculatedDamage = cardCounter * Relation;
        int result = RoundedUp ? (int)MathF.Ceiling(calculatedDamage) : (int)MathF.Floor(calculatedDamage);


        if (TargetPlayer is PlayerType.PLAYER or PlayerType.BOTH)
        {
            state.AlterPlayerHealth(cardId, player, result);
        }

        if (TargetPlayer is PlayerType.RIVAL or PlayerType.BOTH)
        {
            state.AlterPlayerHealth(cardId, rival, result);
        }
    }
}