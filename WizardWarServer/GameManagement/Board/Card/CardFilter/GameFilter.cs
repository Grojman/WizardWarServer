public class GameFilter
{
    //TODO: HAY QEU REFACTORIZAR ESTO. HAY FUNCIONES QUE TIENEN EL MISMO CÓDIGO EN REALIDAD
    public required CardFilter Filter { get; set; }
    public PlayerType WhichDeckToSearch { get; set; } = PlayerType.NONE;
    public PlayerType WhichBoardToSearch { get; set; } = PlayerType.NONE;
    public PlayerType WhichHandToSearch { get; set; } = PlayerType.NONE;
    public PlayerType WhichPlayedCardsToSearch { get; set; } = PlayerType.NONE;
    public PlayerType WhichDeadCardsToSearch { get; set; } = PlayerType.NONE;
    public int MaxLength { get; set; } = 0;

    // The players a search covers. ALL looks at every alive player in the game,
    // regardless of who `playerId` targets; PLAYER/RIVAL/BOTH keep their usual
    // meaning relative to (playerId, rivalId).
    static IEnumerable<PlayerState> PlayersFor(PlayerType type, GameState state, Guid playerId, Guid rivalId)
    {
        switch (type)
        {
            case PlayerType.ALL:
                return state.AlivePlayersView;
            case PlayerType.PLAYER:
                return [state.GetState(playerId)];
            case PlayerType.RIVAL:
                return [state.GetState(rivalId)];
            case PlayerType.BOTH:
                return [state.GetState(playerId), state.GetState(rivalId)];
            default:
                return [];
        }
    }

    // Same search, but only over whatever it looks at on the rival's side: what
    // an extra target adds once the full search already ran against the first
    // one (see IEffect.ForAdditionalTarget). Null if it doesn't look at the rival.
    public GameFilter? RivalOnly()
    {
        static PlayerType Narrow(PlayerType t) => t is PlayerType.RIVAL or PlayerType.BOTH ? PlayerType.RIVAL : PlayerType.NONE;

        var narrowed = new GameFilter()
        {
            Filter = Filter,
            WhichDeckToSearch = Narrow(WhichDeckToSearch),
            WhichBoardToSearch = Narrow(WhichBoardToSearch),
            WhichHandToSearch = Narrow(WhichHandToSearch),
            WhichPlayedCardsToSearch = Narrow(WhichPlayedCardsToSearch),
            WhichDeadCardsToSearch = Narrow(WhichDeadCardsToSearch),
            MaxLength = MaxLength
        };

        return narrowed.LooksAtRival ? narrowed : null;
    }

    [System.Text.Json.Serialization.JsonIgnore]
    public bool LooksAtRival =>
        new[] { WhichDeckToSearch, WhichBoardToSearch, WhichHandToSearch, WhichPlayedCardsToSearch, WhichDeadCardsToSearch }
            .Any(t => t is PlayerType.RIVAL or PlayerType.BOTH);

    IEnumerable<CardInstance> Limit(IEnumerable<CardInstance> result) => MaxLength > 0 ? result.Take(MaxLength) : result;

    public IEnumerable<CardInstance> GetMeetingCardsOnRivalBoard(GameState state, Guid rivalPlayer)
    {
        var result = WhichBoardToSearch is PlayerType.RIVAL or PlayerType.BOTH
            ? state.GetState(rivalPlayer).Board.Where(n => n is not null).Select(n => n!).Where(Filter.Check)
            : [];
        return Limit(result);
    }

    public IEnumerable<CardInstance> GetMeetingCardsOnPlayerBoard(GameState state, Guid playerId)
    {
        var result = WhichBoardToSearch is PlayerType.PLAYER or PlayerType.BOTH
            ? state.GetState(playerId).Board.Where(n => n is not null).Select(n => n!).Where(Filter.Check)
            : [];
        return Limit(result);
    }

    public IEnumerable<CardInstance> GetMeetingCardsOnRivalDeck(GameState state, Guid rivalId)
    {
        var result = WhichDeckToSearch is PlayerType.RIVAL or PlayerType.BOTH ? state.GetState(rivalId).Deck!.cards.Where(Filter.Check) : [];
        return Limit(result);
    }

    public IEnumerable<CardInstance> GetMeetingCardsOnPlayerDeck(GameState state, Guid playerId)
    {
        var result = WhichDeckToSearch is PlayerType.PLAYER or PlayerType.BOTH ? state.GetState(playerId).Deck!.cards.Where(Filter.Check) : [];
        return Limit(result);
    }

    public IEnumerable<CardInstance> GetMeetingCardsOnBoard(GameState state, Guid playerId, Guid rivalId)
    {
        var result = WhichBoardToSearch == PlayerType.ALL
            ? PlayersFor(PlayerType.ALL, state, playerId, rivalId).SelectMany(p => p.Board).Where(n => n is not null).Select(n => n!).Where(Filter.Check)
            : GetMeetingCardsOnPlayerBoard(state, playerId).Concat(GetMeetingCardsOnRivalBoard(state, rivalId));
        return Limit(result);
    }

    public IEnumerable<CardInstance> GetMeetingCardsOffBoard(GameState state, Guid playerId, Guid rivalId)
    {
        var result = WhichDeckToSearch == PlayerType.ALL
            ? PlayersFor(PlayerType.ALL, state, playerId, rivalId).SelectMany(p => p.Deck!.cards).Where(Filter.Check)
            : GetMeetingCardsOnPlayerDeck(state, playerId).Concat(GetMeetingCardsOnRivalDeck(state, rivalId));
        return Limit(result);
    }

    public IEnumerable<CardInstance> GetCardsOnHand(GameState state, Guid playerId, Guid rivalId)
    {
        return PlayersFor(WhichHandToSearch, state, playerId, rivalId).SelectMany(p => p.Hand.Where(Filter.Check));
    }

    public IEnumerable<CardInstance> GetPlayedCards(GameState state, Guid playerId, Guid rivalId)
    {
        return PlayersFor(WhichPlayedCardsToSearch, state, playerId, rivalId).SelectMany(p => p.PlayedCards.Where(Filter.Check));
    }

    public IEnumerable<CardInstance> GetDeadCards(GameState state, Guid playerId, Guid rivalId)
    {
        return PlayersFor(WhichDeadCardsToSearch, state, playerId, rivalId).SelectMany(p => p.DeadCards.Where(Filter.Check));
    }

    public IEnumerable<CardInstance> GetMeetingCards(GameState state, Guid playerId, Guid rivalId)
    {
        var result = GetMeetingCardsOnBoard(state, playerId, rivalId).
                        Concat(GetMeetingCardsOffBoard(state, playerId, rivalId)).
                        Concat(GetCardsOnHand(state, playerId, rivalId)).
                        Concat(GetPlayedCards(state, playerId, rivalId)).
                        Concat(GetDeadCards(state, playerId, rivalId));
        return Limit(result);
    }
}
