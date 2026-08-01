public record DeckMatchupDto(int OpponentDeckId, string OpponentDeckName, int Wins, int Losses, int TotalGames, double AverageTurn)
{
    public static DeckMatchupDto Generate(DeckDto opponent, DeckStats? stats)
    {
        return new(
            opponent.id,
            opponent.name,
            stats?.Wins ?? 0,
            stats?.Losses ?? 0,
            stats?.TotalGames ?? 0,
            stats?.AverageTurn ?? 0);
    }
}

public record DeckStatsDto(int DeckId, string DeckName, int Wins, int Losses, int TotalGames, double AverageTurn, IEnumerable<DeckMatchupDto> Matchups)
{
    public static DeckStatsDto Generate(DeckDto deck, DeckStats? stats, IEnumerable<DeckDto> allDecks)
    {
        DeckStats? GetMatchupStats(int opponentDeckId) =>
            stats != null && stats.VsDeck.TryGetValue(opponentDeckId, out var matchup) ? matchup : null;

        var matchups = allDecks
            .Where(d => d.id != deck.id)
            .Select(d => DeckMatchupDto.Generate(d, GetMatchupStats(d.id)))
            .ToList();

        return new(
            deck.id,
            deck.name,
            stats?.Wins ?? 0,
            stats?.Losses ?? 0,
            stats?.TotalGames ?? 0,
            stats?.AverageTurn ?? 0,
            matchups);
    }
}

public record StatsDto(int TotalGames, IEnumerable<DeckStatsDto> Decks);
