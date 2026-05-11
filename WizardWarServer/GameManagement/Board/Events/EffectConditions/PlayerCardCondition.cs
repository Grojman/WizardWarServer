
public class PlayerCardCondition : EffectCondition
{
    public bool Me { get; set; } = false;
    public CardFilter? Filter { get; set; }
    public PlayerCardCondition(bool me, CardFilter? filter)
    {
        Me = me;
        Filter = filter;
    }
    public override bool Check(Guid playerId, Guid rivalId, CardInstance sourceCard, GameState state, GameEvent? ev)
    {
        CardInstance? card = null;

        if (ev is GameEvent.GameEventCard u) card = u.Card;

        return (Me ? card?.Player.Id == sourceCard.Player.Id : card?.Player.Id != sourceCard.Player.Id)  &&
                (Filter?.Check(card) ?? true);
    }

    public override EffectCondition Clone()
    {
        return new PlayerCardCondition(Me, Filter);
    }
}