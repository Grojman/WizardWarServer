

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

    public void Execute(Guid playerId, Guid rivalId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        var rivalDeck = Filter.GetMeetingCardsOnRivalDeck(state, playerId);
        if(rivalDeck.Count() != 0)
        {
            foreach(var c in rivalDeck)
            {
                c.CurrentAttack += Damage;
                c.CurrentHealth += Health;
            }

            state.AlterDeck(state.GetState(rivalId), cardId, state.GetState(playerId), rivalDeck);
        }

        var playerDeck = Filter.GetMeetingCardsOnPlayerDeck(state, playerId);
        if(playerDeck.Count() != 0)
        {
            foreach(var c in playerDeck)
            {
                c.CurrentAttack += Damage;
                c.CurrentHealth += Health;
            }

            state.AlterDeck(state.GetState(playerId), cardId, state.GetState(playerId), playerDeck);
        }

        var rivalBoard = Filter.GetMeetingCardsOnRivalBoard(state, playerId);
        if(rivalBoard.Count() != 0)
        {
            foreach(var c in rivalBoard)
            {
                if(Damage != 0) state.AlterUnitDamage(cardId, c, Damage);
                if(Health != 0) state.AlterUnitHealth(cardId, c, Health); 
            }
        }

        var playerBoard = Filter.GetMeetingCardsOnPlayerBoard(state, playerId);
        if(playerBoard.Count() != 0)
        {
            foreach(var c in playerBoard)
            {
                if(Damage != 0) state.AlterUnitDamage(cardId, c, Damage);
                if(Health != 0) state.AlterUnitHealth(cardId, c, Health); 
            }
        }

        
    }
}