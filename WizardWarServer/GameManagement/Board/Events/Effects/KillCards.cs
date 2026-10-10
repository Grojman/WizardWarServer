public class KillCards : IEffect
{
    public KillCards(CardFilter filter, PlayerType boardSearch, int count)
    {
        BoardSearch = boardSearch;
        Filter = filter;
        Count = count;
        gameFilter = new()
        {
            Filter = filter,
            WhichBoardToSearch = BoardSearch,
            MaxLength = count
        };
    }
    GameFilter gameFilter;
    public PlayerType BoardSearch { get; set; }

    public CardFilter Filter { get; set; }
    public int Count { get; set; }
    public IEffect Clone() => new KillCards(Filter, BoardSearch, Count);

    public void Execute(Guid playerId, Guid rivalId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        // Materialized first: units placed by death effects while this runs
        // weren't on the board when it was cast.
        var list = gameFilter.GetMeetingCardsOnBoard(state, playerId, rivalId).ToList();
        foreach(var a in list) state.KillUnit(cardId, a);
    }

    public IEffect? ForAdditionalTarget() => BoardSearch switch
    {
        PlayerType.RIVAL => this,
        PlayerType.BOTH => new KillCards(Filter, PlayerType.RIVAL, Count),
        // PLAYER: my own board was already searched; ALL: every board was.
        _ => null
    };
}