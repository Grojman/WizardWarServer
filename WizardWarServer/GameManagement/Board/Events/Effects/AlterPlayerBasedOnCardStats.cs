
public class AlterPlayerBasedOnCardStats : IEffect
{
    public int Relation { get; set; }
    public PlayerType Target { get; set; }
    public PlayerType BoardSearch { get; set; }
    public CardFilter Filter { get; set; }
    public AffectedStats Stats { get; set; }
    GameFilter boardFilter;

    public AlterPlayerBasedOnCardStats(PlayerType boardSearch, CardFilter filter, AffectedStats stats, PlayerType target, int relation)
    {
        Target = target;
        BoardSearch = boardSearch;
        Filter = filter;
        Stats = stats;
        Relation = relation; 
        boardFilter = new()
        {
            Filter = Filter,
            WhichBoardToSearch = BoardSearch  
        };
    }

    public IEffect Clone() => new AlterPlayerBasedOnCardStats(BoardSearch, Filter, Stats, Target, Relation);
    public void Execute(Guid playerId, Guid rivalId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        var lista = boardFilter.GetMeetingCardsOnBoard(state, playerId, rivalId);

        int counter = 0;

        foreach(var a in lista)
        {
            if(!a.DeathChecked)
            {
                counter += Stats switch
                {
                    AffectedStats.DAMAGE => a.CurrentAttack,
                    AffectedStats.HEALTH => a.CurrentHealth,
                    AffectedStats.BOTH => a.CurrentAttack + a.CurrentHealth,
                    _ => 0
                };
            }
        }

        counter *= Relation;

        if (Target is PlayerType.PLAYER or PlayerType.BOTH) state.AlterPlayerHealth(cardId, state.GetState(playerId), counter);
        if (Target is PlayerType.RIVAL or PlayerType.BOTH) state.AlterPlayerHealth(cardId, state.GetState(rivalId), counter);
    }
}