
public class AlterMySelf : IEffect
{
    public AlterMySelf(int damage, int health)
    {
        Damage = damage;
        Health = health;
    }

    public int Damage { get; set; }
    public int Health { get; set; }
    public IEffect Clone() => new AlterMySelf(Damage, Health);

    public void Execute(Guid playerId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        if(Damage != 0)
        {
            state.AlterUnitDamage(cardId, cardId, Damage);
        }

        if(Health != 0)
        {
            state.AlterUnitDamage(cardId, cardId, Health);
        }
    }
}