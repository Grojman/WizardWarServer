
public class AlterMySelf : IEffect
{
    public AlterMySelf(int damage, int health, bool useGameEvent)
    {
        Damage = damage;
        Health = health;
        UseGameEvent = useGameEvent;
    }

    public int Damage { get; set; }
    public int Health { get; set; }
    public bool UseGameEvent { get; set; }
    public IEffect Clone() => new AlterMySelf(Damage, Health, UseGameEvent);

    public void Execute(Guid playerId, Guid rivalId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        var card = UseGameEvent ? ((ev as GameEvent.GameEventCard)?.Card ?? cardId) : cardId;

        state.AlterUnitDamage(cardId, card, Damage);
        state.AlterUnitHealth(cardId, card, Health);
    }
}