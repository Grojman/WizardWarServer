
public class IHaveBeenPlayedCondition : EffectCondition
{
    public override bool Check(Guid playerId, Guid cardId, GameState state, GameEvent? ev)
    {
        Guid? myCard = null;
        if(ev is GameEvent.UnitPlayed e)
        {
            myCard = e.Unit.Id;
        } else if (ev is GameEvent.SpellPlayed a)
        {
            myCard = a.Spell.Id;
        }

        return myCard == cardId;
    }

    public override EffectCondition Clone()
    {
        return new IHaveBeenPlayedCondition();
    }
    
}