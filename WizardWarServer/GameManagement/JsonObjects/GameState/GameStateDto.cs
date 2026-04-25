public record GameStateDto(PlayerStateDto Me, PlayerStateDto Rival, bool IsMyTurn)
{
    public static GameStateDto Generate(PlayerState Me, PlayerState Rival, bool IsMyTurn, GameState state)
    {
        return new (
            PlayerStateDto.Generate(Me, false, state),
            PlayerStateDto.Generate(Rival, true, state),
            IsMyTurn);
    }
}