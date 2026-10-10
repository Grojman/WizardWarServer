// Gives the card running the effect [Damage/Health] once for every card the
// filter finds, not counting the card itself (e.g. "+1/+1 for each other copy
// of me my player has played before").
public class AlterMySelfPerCardCount : IEffect
{
    public AlterMySelfPerCardCount(int damage, int health, GameFilter filter)
    {
        Damage = damage;
        Health = health;
        Filter = filter;
    }

    public int Damage { get; set; }
    public int Health { get; set; }
    public GameFilter Filter { get; set; }

    public IEffect Clone() => new AlterMySelfPerCardCount(Damage, Health, Filter);

    public void Execute(Guid playerId, Guid rivalId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        var count = Filter.GetMeetingCards(state, playerId, rivalId).Count(c => c.Id != cardId.Id);
        if (count == 0) return;

        if (Damage != 0) state.AlterUnitDamage(cardId, cardId, Damage * count);
        if (Health != 0) state.AlterUnitHealth(cardId, cardId, Health * count);
    }

    // Only touches the card itself.
    public IEffect? ForAdditionalTarget() => null;
}
