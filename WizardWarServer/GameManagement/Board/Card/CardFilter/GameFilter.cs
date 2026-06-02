using Microsoft.AspNetCore.Mvc.Razor;

public class GameFilter
{
    //TODO: HAY QEU REFACTORIZAR ESTO. HAY FUNCIONES QUE TIENEN EL MISMO CÓDIGO EN REALIDAD
    public required CardFilter Filter { get; set; }
    public PlayerType WhichDeckToSearch { get; set; } = PlayerType.NONE;
    public PlayerType WhichBoardToSearch { get; set; } = PlayerType.NONE;
    public PlayerType WhichHandToSearch { get; set; } = PlayerType.NONE;
    public int MaxLength { get; set; } = 0;


    public IEnumerable<CardInstance> GetMeetingCardsOnRivalBoard(GameState state, Guid rivalPlayer)
    {
        var result = WhichBoardToSearch is PlayerType.RIVAL or PlayerType.BOTH ? state.GetState(rivalPlayer).Board.Where(n => n is not null && Filter.Check(n)) : [];
        return MaxLength > 0 ? result.Take(MaxLength) : result;
    }

    public IEnumerable<CardInstance> GetMeetingCardsOnPlayerBoard(GameState state, Guid playerId)
    {
        var result = WhichBoardToSearch is PlayerType.PLAYER or PlayerType.BOTH ? state.GetState(playerId).Board.Where(n => n is not null && Filter.Check(n)) : [];
        return MaxLength > 0 ? result.Take(MaxLength) : result;
    }

    public IEnumerable<CardInstance> GetMeetingCardsOnRivalDeck(GameState state, Guid rivalId)
    {
        var result = WhichDeckToSearch is PlayerType.RIVAL or PlayerType.BOTH ? state.GetState(rivalId).Deck.cards.Where(Filter.Check) : [];
        return MaxLength > 0 ? result.Take(MaxLength) : result;
    }

    public IEnumerable<CardInstance> GetMeetingCardsOnPlayerDeck(GameState state, Guid playerId)
    {
        var result = WhichDeckToSearch is PlayerType.PLAYER or PlayerType.BOTH ? state.GetState(playerId).Deck.cards.Where(Filter.Check) : [];
        return MaxLength > 0 ? result.Take(MaxLength) : result;
    }

    public IEnumerable<CardInstance> GetMeetingCardsOnBoard(GameState state, Guid playerId, Guid rivalId)
    {
        var result = GetMeetingCardsOnPlayerBoard(state, playerId).Concat(GetMeetingCardsOnRivalBoard(state, rivalId));
        return MaxLength > 0 ? result.Take(MaxLength) : result;
    }

    public IEnumerable<CardInstance> GetMeetingCardsOffBoard(GameState state, Guid playerId, Guid rivalId)
    {
        var result = GetMeetingCardsOnPlayerDeck(state, playerId).Concat(GetMeetingCardsOnRivalDeck(state, rivalId));
        return MaxLength > 0 ? result.Take(MaxLength) : result;
    }

    public IEnumerable<CardInstance> GetCardsOnHand(GameState state, Guid playerId, Guid rivalId)
    {
        IEnumerable<CardInstance> cards = new List<CardInstance>();
        if (WhichHandToSearch is PlayerType.PLAYER or PlayerType.BOTH)
        {
            cards = cards.Concat(state.GetState(playerId).Hand.Where(Filter.Check));
        }
        if (WhichHandToSearch is PlayerType.RIVAL or PlayerType.BOTH)
        {
            cards = cards.Concat(state.GetState(rivalId).Hand.Where(Filter.Check));
        }

        return cards;
    }

    public IEnumerable<CardInstance> GetMeetingCards(GameState state, Guid playerId, Guid rivalId)
    {
        var result = GetMeetingCardsOnBoard(state, playerId, rivalId).
                        Concat(GetMeetingCardsOffBoard(state, playerId, rivalId)).
                        Concat(GetCardsOnHand(state, playerId, rivalId));
        return MaxLength > 0 ? result.Take(MaxLength) : result;
    }
}