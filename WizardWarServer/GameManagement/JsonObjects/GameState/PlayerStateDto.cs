public record PlayerStateDto(Guid Id, string Name, CardDto?[] Board, int Health, int HandSize, CardDto[] HandData, DeckStateDto Deck)
{
    public static PlayerStateDto Generate(PlayerState state, bool hidden, GameState gameState)
    {
        return new (state.Id,
                    state.Name,
                    [.. state.Board.Select(n => n is null ? null : CardDto.Generate(n, gameState, true))],
                    state.Health,
                    state.Hand.Count,
                    hidden ? [] : [.. state.Hand.Select(n => CardDto.Generate(n, gameState, false))],
                    DeckStateDto.Generate(state.Deck));
    }
}