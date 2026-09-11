public record GameStateDto(PlayerStateDto Me, PlayerStateDto[] Rivals, int CurrentTurn, bool IsReconnect)
{
    public static GameStateDto Generate(PlayerState Me, PlayerState[] Rivals, GameState state, string language, bool isReconnect = false)
    {
        return new (
            PlayerStateDto.Generate(Me, false, state, language),
            [.. Rivals.Select(n => PlayerStateDto.Generate(n, true, state, language))], state.TurnCounter, isReconnect);
    }
}