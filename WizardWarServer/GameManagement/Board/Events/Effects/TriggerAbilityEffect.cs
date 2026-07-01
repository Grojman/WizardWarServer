public class TriggerAbilityEffect : IEffect
{
    public TriggerAbilityEffect(PlayerType playerType, CardFilter? filter)
    {
        PlayerType = playerType;
        Filter = filter;
    }

    public PlayerType PlayerType { get; set; }

    public CardFilter? Filter { get; set; } = null;

    public IEffect Clone() => new TriggerAbilityEffect(PlayerType, Filter);
    public void Execute(Guid playerId, Guid rivalId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        if(PlayerType is PlayerType.PLAYER or PlayerType.BOTH)
        {
            for (int i = 0; i < cardId.Player.Board.Length; i++)
            {
                CardInstance? c = cardId.Player.Board[i];
                if (c is not null && (Filter?.Check(c) ??  true))
                {
                    state.ApplyEffect(cardId.Player.Connection, i);
                }
            }
        }

        if(PlayerType is PlayerType.RIVAL or PlayerType.BOTH)
        {
            for (int i = 0; i < cardId.Player.PlayerTarget.Board.Length; i++)
            {
                CardInstance? c = cardId.Player.PlayerTarget.Board[i];
                if (c is not null && (Filter?.Check(c) ??  true))
                {
                    state.ApplyEffect(cardId.Player.PlayerTarget.Connection, i);
                }
            }
        }
    }
}