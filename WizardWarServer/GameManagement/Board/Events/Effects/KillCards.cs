public class KillCards : IEffect
{
    public KillCards(CardFilter filter, PlayerType boardSearch)
    {
        BoardSearch = boardSearch;
        Filter = filter;
        gameFilter = new()
        {
            Filter = filter,
            WhichBoardToSearch = BoardSearch
        };
    }
    GameFilter gameFilter;
    public PlayerType BoardSearch { get; set; }

    public CardFilter Filter { get; set; }
    public IEffect Clone() => new KillCards(Filter, BoardSearch);

    public void Execute(Guid playerId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        var list = gameFilter.GetMeetingCardsOnBoard(state, playerId);
        foreach(var a in list) state.KillUnit(cardId, a);
    }
}