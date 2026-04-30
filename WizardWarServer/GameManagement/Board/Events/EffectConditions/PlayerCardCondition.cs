
public class PlayerCardCondition : EffectCondition
{
    public bool Me { get; set; } = false;
    public CardFilter? Filter { get; set; }
    public PlayerCardCondition(bool me, CardFilter? filter)
    {
        Me = me;
        Filter = filter;
    }
    public override bool Check(Guid playerId, CardInstance sourceCard, GameState state, GameEvent? ev)
    {
        CardInstance? card = null;

        if (ev is GameEvent.UnitPlayed u) card = u.Unit;
        if (ev is GameEvent.SpellPlayed s) card = s.Spell;

        return (Me ? card?.PlayerGuid == sourceCard.PlayerGuid : card?.PlayerGuid != sourceCard.PlayerGuid)  &&
                (Filter?.Check(card) ?? true);
    }

    public override EffectCondition Clone()
    {
        return new PlayerCardCondition(Me, Filter);
    }
}