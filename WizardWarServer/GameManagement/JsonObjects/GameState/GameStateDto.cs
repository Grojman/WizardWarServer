// HuntedPlayer: the player everyone targets this round in a 3+ player game,
// null in a 1v1 (see GameState.UsesHuntedTargeting).
public record GameStateDto(PlayerStateDto Me, PlayerStateDto[] Rivals, int CurrentTurn, bool IsReconnect, Guid Id, Guid? HuntedPlayer)
{
    public static GameStateDto Generate(PlayerState Me, PlayerState[] Rivals, GameState state, string language, Guid gameId, bool isReconnect = false)
    {
        return new (
            PlayerStateDto.Generate(Me, false, state, language),
            [.. Rivals.Select(n => PlayerStateDto.Generate(n, true, state, language))], state.TurnCounter, isReconnect, gameId, state.HuntedPlayer?.Id);
    }
}