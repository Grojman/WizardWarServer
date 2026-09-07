
// Cierto cuando el evento disparador es un GameEvent.PlayerHealthChanged que afectó
// concretamente al jugador indicado (Me = mi jugador, Me = false = el rival), y el
// Amount del cambio cumple el NumberFilter (p.ej. AT_MAX_UNDER 0 para detectar daño).
public class PlayerHealthChangedCondition : EffectCondition
{
    public PlayerHealthChangedCondition(bool me, NumberFilter amount)
    {
        Me = me;
        Amount = amount;
    }

    public bool Me { get; set; }
    public NumberFilter Amount { get; set; }

    public override bool Check(Guid playerId, Guid rivalId, CardInstance sourceCard, GameState state, GameEvent? ev)
    {
        if (ev is not GameEvent.PlayerHealthChanged e) return false;

        var expectedPlayer = Me ? sourceCard.Player : sourceCard.Player.PlayerTarget;

        return e.PlayerId == expectedPlayer?.Id && Amount.Compare(e.Amount);
    }

    public override EffectCondition Clone() => new PlayerHealthChangedCondition(Me, Amount);
}
