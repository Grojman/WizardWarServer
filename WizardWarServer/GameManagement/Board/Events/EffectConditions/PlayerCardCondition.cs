
public class PlayerCardCondition : EffectCondition
{
    public bool Me { get; set; } = false;
    public PlayerCardCondition(bool me)
    {
        Me = me;
    }
    public override bool Check(Guid playerId, CardInstance sourceCard, GameState state, GameEvent? ev)
    {
        if (ev is GameEvent.UnitPlayed u) return Me ? u.PlayerSource.Id == sourceCard.PlayerGuid : u.PlayerSource.Id != sourceCard.PlayerGuid;
        if (ev is GameEvent.SpellPlayed s) return Me ? s.PlayerSource.Id == sourceCard.PlayerGuid : s.PlayerSource.Id != sourceCard.PlayerGuid;

        return false;
    }

    public override EffectCondition Clone()
    {
        return new PlayerCardCondition(Me);
    }
}