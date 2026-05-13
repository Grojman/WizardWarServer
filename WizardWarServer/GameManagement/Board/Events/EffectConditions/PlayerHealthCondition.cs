
public class PlayerHealthCondition : EffectCondition
{
    public PlayerHealthCondition(bool me, CountType type, int amount)
    {
        Me = me;
        Type = type;
        Amount = amount;
    }

    public bool Me { get; set; }
    public CountType Type { get; set; }
    public int Amount { get; set; }
    public override bool Check(Guid playerId, Guid rivalId, CardInstance sourceCard, GameState state, GameEvent? ev)
    {
        var player = Me ? sourceCard.Player : sourceCard.Player.PlayerTarget;

        return Type.Evaluate(player.Health, Amount);
    }

    public override EffectCondition Clone() => new PlayerHealthCondition(Me, Type, Amount);
}