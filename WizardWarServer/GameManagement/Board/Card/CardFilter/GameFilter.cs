public class GameFilter
{
    public required CardFilter Filter { get; set; }
    public PlayerType WhichDeckToSearch { get; set; }
    public PlayerType WhichBoardToSearch { get; set; }

    public IEnumerable<CardInstance> GetMeetingCardsOnRivalBoard(GameState state, Guid playerId)
    {
        return WhichBoardToSearch is PlayerType.RIVAL or PlayerType.BOTH ? state.GetRival(playerId).Board.Where(n => n is not null && Filter.Check(n)) : [];
    }

    public IEnumerable<CardInstance> GetMeetingCardsOnPlayerBoard(GameState state, Guid playerId)
    {
        return WhichBoardToSearch is PlayerType.PLAYER or PlayerType.BOTH ? state.GetState(playerId).Board.Where(n => n is not null && Filter.Check(n)) : [];
    }

    public IEnumerable<CardInstance> GetMeetingCardsOnRivalDeck(GameState state, Guid playerId)
    {
        return WhichBoardToSearch is PlayerType.RIVAL or PlayerType.BOTH ? state.GetRival(playerId).Deck.cards.Where(Filter.Check) : [];
    }

    public IEnumerable<CardInstance> GetMeetingCardsOnPlayerDeck(GameState state, Guid playerId)
    {
        return WhichBoardToSearch is PlayerType.PLAYER or PlayerType.BOTH ? state.GetState(playerId).Deck.cards.Where(Filter.Check) : [];
    }

    public IEnumerable<CardInstance> GetMeetingCardsOnBoard(GameState state, Guid playerId)
    {
        return GetMeetingCardsOnPlayerBoard(state, playerId).Concat(GetMeetingCardsOnRivalBoard(state, playerId));
    }

    public IEnumerable<CardInstance> GetMeetingCardsOffBoard(GameState state, Guid playerId)
    {
        return GetMeetingCardsOnPlayerDeck(state, playerId).Concat(GetMeetingCardsOnRivalDeck(state, playerId));
    }

    public IEnumerable<CardInstance> GetMeetingCards(GameState state, Guid playerId)
    {
        return GetMeetingCardsOnBoard(state, playerId).Concat(GetMeetingCardsOffBoard(state, playerId));
    }
}