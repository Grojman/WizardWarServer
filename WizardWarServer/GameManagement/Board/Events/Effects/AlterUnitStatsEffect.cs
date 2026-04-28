

public class AlterUnitStatsEffect : IEffect
{
    public AlterUnitStatsEffect(int health, int damage, GameFilter filter)
    {
        Health = health;
        Damage = damage;
        Filter = filter;
    }

    public int Health { get; set; }
    public int Damage { get; set; }

    public GameFilter Filter { get; set; }

    public IEffect Clone() =>  new AlterUnitStatsEffect(Health, Damage, Filter);

    public void Execute(Guid playerId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        var rivalDeck = Filter.GetMeetingCardsOnRivalDeck(state, playerId);
        if(rivalDeck.Count() != 0)
        {
            foreach(var c in rivalDeck)
            {
                c.CurrentAttack += Damage;
                c.CurrentHealth += Health;
            }

            state.AlterDeck(state.GetRival(playerId), cardId, state.GetState(playerId), rivalDeck);
        }

        
    }
}