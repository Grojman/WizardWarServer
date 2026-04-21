
public class IHaveBeenPlayedCondition : EffectCondition
{
    public override bool Check(Guid playerId, CardInstance card, GameState state, GameEvent? ev)
    {
        Guid? myCard = null;
        if(ev is GameEvent.UnitPlayed e)
        {
            myCard = e.Unit.Id;
        } else if (ev is GameEvent.SpellPlayed a)
        {
            myCard = a.Spell.Id;
        }

        return myCard == card.Id;
    }

    public override EffectCondition Clone()
    {
        return new IHaveBeenPlayedCondition();
    }
    
}