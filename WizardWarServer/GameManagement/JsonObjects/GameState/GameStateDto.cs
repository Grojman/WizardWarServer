public record GameStateDto(PlayerStateDto Me, PlayerStateDto[] Rivals)
{
    public static GameStateDto Generate(PlayerState Me, PlayerState[] Rivals, GameState state)
    {
        return new (
            PlayerStateDto.Generate(Me, false, state),
            [.. Rivals.Select(n => PlayerStateDto.Generate(n, true, state))]);
    }
}