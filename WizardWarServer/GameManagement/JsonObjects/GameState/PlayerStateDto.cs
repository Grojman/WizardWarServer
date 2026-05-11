public record PlayerStateDto(Guid Id, Guid TargetPlayer, string Name, CardDto?[] Board, int Health, int HandSize, CardDto[] HandData, DeckStateDto Deck, IEnumerable<string> GlobalEffects, bool IsMyTurn, CardDto? LastSpellPlayed)
{
    public static PlayerStateDto Generate(PlayerState state, bool hidden, GameState gameState)
    {
        return new (state.Id,
                    state.PlayerTarget.Id,
                    state.Name,
                    [.. state.Board.Select(n => n is null ? null : CardDto.Generate(n, gameState, true))],
                    state.Health,
                    state.Hand.Count,
                    hidden ? [] : [.. state.Hand.Select(n => CardDto.Generate(n, gameState, false))],
                    DeckStateDto.Generate(state.Deck),
                    state.GlobalEffects.Select(n => n.Description),
                    state.IsMyTurn,
                    state.LastSpellPlayed is null ? null : CardDto.Generate(state.LastSpellPlayed, gameState, true));
    }
}