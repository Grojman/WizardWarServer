
public class AlterUnitStatsEffect : IEffect
{
    public int AffectedCards;
    int counter = 0;
    public Direction WhichWay;
    public AffectedStats AffectedStats;
    public int Damage;
    public int Healht;


    public AlterUnitStatsEffect(Direction whichWay, int damage, int affectedCards)
    {
        AffectedCards = affectedCards;
        WhichWay = whichWay;
        Damage = damage;
        AffectedStats = AffectedStats.DAMAGE;
    }

    public AlterUnitStatsEffect(int healht, Direction whichWay, int affectedCards)
    {
        AffectedCards = affectedCards;
        Healht = healht;
        WhichWay = whichWay;
        AffectedStats = AffectedStats.HEALTH;
    }

    public AlterUnitStatsEffect(Direction whichWay, int damage, int health, int affectedCards)
    {
        AffectedCards = affectedCards;
        WhichWay = whichWay;
        Damage = damage;
        Healht = health;
    }

    public void Execute(Guid playerId, CardInstance source, GameState state, GameEvent? ev)
    {
        var rival = state.GetRival(playerId);
        counter = 0;

        int start = WhichWay == Direction.LEFT_TO_RIGHT ? 0 : rival.Board.Length;
        int end = WhichWay == Direction.LEFT_TO_RIGHT ? rival.Board.Length : -1;
        int step = WhichWay == Direction.LEFT_TO_RIGHT ? 1 : -1;

        for(int i = start; i != end && counter < AffectedCards; i += step)
        {
            if (rival.Board[i] is not null)
            {
                if (AffectedStats is AffectedStats.DAMAGE or AffectedStats.BOTH)
                {
                    state.AlterUnitDamage(source, rival.Board[i], Damage);
                }
                if (AffectedStats is AffectedStats.HEALTH or AffectedStats.BOTH)
                {
                    state.AlterUnitHealth(source, rival.Board[i], Healht);
                }
                counter++;
            }
        }
    }
}