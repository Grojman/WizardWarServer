public record GameStateDto(PlayerStateDto Me, PlayerStateDto Rival, bool IsMyTurn)
{
    public static GameStateDto Generate(PlayerState Me, PlayerState Rival, bool IsMyTurn)
    {
        return new (
            PlayerStateDto.Generate(Me, false),
            PlayerStateDto.Generate(Rival, true),
            IsMyTurn);
    }
}