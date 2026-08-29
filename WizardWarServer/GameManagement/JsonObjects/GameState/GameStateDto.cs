public record GameStateDto(PlayerStateDto Me, PlayerStateDto[] Rivals, int CurrentTurn)
{
    public static GameStateDto Generate(PlayerState Me, PlayerState[] Rivals, GameState state, string language)
    {
        return new (
            PlayerStateDto.Generate(Me, false, state, language),
            [.. Rivals.Select(n => PlayerStateDto.Generate(n, true, state, language))], state.TurnCounter);
    }
}