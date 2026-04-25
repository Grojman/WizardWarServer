
public class DamagePlayerBasedOnCards : IEffect
{
    public DamagePlayerBasedOnCards()
    {
    }

    public DamagePlayerBasedOnCards(string[] familiesToSearch, bool roundedUp, float relation, PlayerType whichDeckToSearch, PlayerType whichBoardToSearch, bool mustHaveAllFamilies)
    {
        FamiliesToSearch = familiesToSearch;
        RoundedUp = roundedUp;
        Relation = relation;
        WhichDeckToSearch = whichDeckToSearch;
        WhichBoardToSearch = whichBoardToSearch;
        MustHaveAllFamilies = mustHaveAllFamilies;
    }

    public string[] FamiliesToSearch { get; set;}

    public bool MustHaveAllFamilies { get; set; }
    public bool RoundedUp { get; set; }
    public float Relation { get; set; }

    public PlayerType WhichDeckToSearch { get; set; }
    public PlayerType WhichBoardToSearch { get; set; }
    public PlayerType TargetPlayer { get; set; }

    public IEffect Clone() => new DamagePlayerBasedOnCards(FamiliesToSearch, RoundedUp, Relation, WhichDeckToSearch, WhichBoardToSearch, MustHaveAllFamilies);

    public void Execute(Guid playerId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        int cardCounter = 0;

        var rival = state.GetRival(playerId);
        var player = state.GetState(playerId);

        if (WhichDeckToSearch is PlayerType.PLAYER or PlayerType.BOTH)
        {
            foreach (var item in player.Deck.cards) if (MustHaveAllFamilies ? FamiliesToSearch.All(item.CurrentFamilies.Contains) : FamiliesToSearch.Any(item.CurrentFamilies.Contains)) cardCounter++;
        }

        if (WhichDeckToSearch is PlayerType.RIVAL or PlayerType.BOTH)
        {
            foreach (var item in rival.Deck.cards) if (MustHaveAllFamilies ? FamiliesToSearch.All(item.CurrentFamilies.Contains) : FamiliesToSearch.Any(item.CurrentFamilies.Contains)) cardCounter++;
        }


        if (WhichBoardToSearch is PlayerType.PLAYER or PlayerType.BOTH)
        {
            foreach (var item in player.Board) if (item is not null && (MustHaveAllFamilies ? FamiliesToSearch.All(item.CurrentFamilies.Contains) : FamiliesToSearch.Any(item.CurrentFamilies.Contains))) cardCounter++;
        }

        if (WhichBoardToSearch is PlayerType.RIVAL or PlayerType.BOTH)
        {
            foreach (var item in rival.Board) if (item is not null && (MustHaveAllFamilies ? FamiliesToSearch.All(item.CurrentFamilies.Contains) : FamiliesToSearch.Any(item.CurrentFamilies.Contains))) cardCounter++;
        }

        float calculatedDamage = cardCounter * Relation;
        int result = RoundedUp ? (int)MathF.Ceiling(calculatedDamage) : (int)MathF.Floor(calculatedDamage);


        if (TargetPlayer is PlayerType.PLAYER or PlayerType.BOTH)
        {
            state.AlterPlayerHealth(cardId, player, result);
        }

        if (TargetPlayer is PlayerType.RIVAL or PlayerType.BOTH)
        {
            state.AlterPlayerHealth(cardId, rival, result);
        }
    }
}