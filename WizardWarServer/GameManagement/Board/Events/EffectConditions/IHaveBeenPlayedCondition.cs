
public class IHaveBeenPlayedCondition : EffectCondition
{
    public override bool Check(Guid playerId, Guid rivalId, CardInstance card, GameState state, GameEvent? ev)
    {
        Guid? myCard = null;
        if(ev is GameEvent.GameEventCard e) myCard = e.Card.Id;

        return myCard == card.Id;
    }

    public override EffectCondition Clone()
    {
        return new IHaveBeenPlayedCondition();
    }
    
}