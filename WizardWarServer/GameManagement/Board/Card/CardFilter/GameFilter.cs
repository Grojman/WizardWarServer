public class GameFilter
{
    public CardFilter Filter { get; set; }
    public PlayerType WhichDeckToSearch { get; set; }
    public PlayerType WhichBoardToSearch { get; set; }

    public IEnumerable<CardInstance> GetMeetingCards(GameState state, Guid playerId)
    {
        List<CardInstance> list = new();

        var rival = state.GetRival(playerId);
        var player = state.GetState(playerId);

        if (WhichDeckToSearch is PlayerType.PLAYER or PlayerType.BOTH)
        {
            foreach (var item in player.Deck.cards) if(Filter.Check(item)) list.Add(item);
        }

        if (WhichDeckToSearch is PlayerType.RIVAL or PlayerType.BOTH)
        {
            foreach (var item in rival.Deck.cards) if(Filter.Check(item)) list.Add(item);
        }


        if (WhichBoardToSearch is PlayerType.PLAYER or PlayerType.BOTH)
        {
            foreach (var item in player.Board) if(item is not null && Filter.Check(item)) list.Add(item);
        }

        if (WhichBoardToSearch is PlayerType.RIVAL or PlayerType.BOTH)
        {
            foreach (var item in rival.Board) if(item is not null && Filter.Check(item)) list.Add(item);
        }

        return list;
    }
}