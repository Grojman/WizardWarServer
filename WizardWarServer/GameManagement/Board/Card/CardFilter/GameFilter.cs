public class GameFilter
{
    public required CardFilter Filter { get; set; }
    public PlayerType WhichDeckToSearch { get; set; } = PlayerType.NONE;
    public PlayerType WhichBoardToSearch { get; set; } = PlayerType.NONE;
    public int MaxLength { get; set; } = 0;

    public IEnumerable<CardInstance> GetMeetingCardsOnRivalBoard(GameState state, Guid playerId)
    {
        var result = WhichBoardToSearch is PlayerType.RIVAL or PlayerType.BOTH ? state.GetRival(playerId).Board.Where(n => n is not null && Filter.Check(n)) : [];
        return MaxLength > 0 ? result.Take(MaxLength) : result;
    }

    public IEnumerable<CardInstance> GetMeetingCardsOnPlayerBoard(GameState state, Guid playerId)
    {
        var result = WhichBoardToSearch is PlayerType.PLAYER or PlayerType.BOTH ? state.GetState(playerId).Board.Where(n => n is not null && Filter.Check(n)) : [];
        return MaxLength > 0 ? result.Take(MaxLength) : result;
    }

    public IEnumerable<CardInstance> GetMeetingCardsOnRivalDeck(GameState state, Guid playerId)
    {
        var result = WhichBoardToSearch is PlayerType.RIVAL or PlayerType.BOTH ? state.GetRival(playerId).Deck.cards.Where(Filter.Check) : [];
        return MaxLength > 0 ? result.Take(MaxLength) : result;
    }

    public IEnumerable<CardInstance> GetMeetingCardsOnPlayerDeck(GameState state, Guid playerId)
    {
        var result = WhichBoardToSearch is PlayerType.PLAYER or PlayerType.BOTH ? state.GetState(playerId).Deck.cards.Where(Filter.Check) : [];
        return MaxLength > 0 ? result.Take(MaxLength) : result;
    }

    public IEnumerable<CardInstance> GetMeetingCardsOnBoard(GameState state, Guid playerId)
    {
        var result = GetMeetingCardsOnPlayerBoard(state, playerId).Concat(GetMeetingCardsOnRivalBoard(state, playerId));
        return MaxLength > 0 ? result.Take(MaxLength) : result;
    }

    public IEnumerable<CardInstance> GetMeetingCardsOffBoard(GameState state, Guid playerId)
    {
        var result = GetMeetingCardsOnPlayerDeck(state, playerId).Concat(GetMeetingCardsOnRivalDeck(state, playerId));
        return MaxLength > 0 ? result.Take(MaxLength) : result;
    }

    public IEnumerable<CardInstance> GetMeetingCards(GameState state, Guid playerId)
    {
        var result = GetMeetingCardsOnBoard(state, playerId).Concat(GetMeetingCardsOffBoard(state, playerId));
        return MaxLength > 0 ? result.Take(MaxLength) : result;
    }
}