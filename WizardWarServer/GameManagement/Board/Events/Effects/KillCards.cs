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
        var list = gameFilter.GetMeetingCardsOnBoard(state, playerId, rivalId);
        foreach(var a in list) state.KillUnit(cardId, a);
    }
}