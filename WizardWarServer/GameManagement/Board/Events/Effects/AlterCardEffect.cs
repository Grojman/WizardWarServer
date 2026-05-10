
public class AlterCardEffect : IEffect
{
    public AlterCardEffect(int health, int damage, string newFamily)
    {
        Health = health;
        Damage = damage;
        NewFamily = newFamily;
    }

    public int Health { get; set; }
    public int Damage { get; set; }
    public string NewFamily { get; set; }
    public IEffect Clone() => new AlterCardEffect(Health, Damage, NewFamily);

    public void Execute(Guid playerId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        CardInstance? card = (ev as GameEvent.GameEventCard)?.Card;

        if(card is not null)
        {
            if(!string.IsNullOrWhiteSpace(NewFamily)) card.CurrentFamilies.Add(NewFamily);

            state.AlterUnitDamage(cardId, card, Damage);
            state.AlterUnitHealth(cardId, card, Health);
        }
    }
}