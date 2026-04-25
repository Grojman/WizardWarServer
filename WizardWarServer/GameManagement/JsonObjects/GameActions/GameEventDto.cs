
using System.Text.Json.Serialization;
using System.Threading.Channels;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(CardDrawnEvent), nameof(CardDrawnEvent))]
[JsonDerivedType(typeof(PlayerHealthChanged), nameof(PlayerHealthChanged))]
[JsonDerivedType(typeof(UnitPlayed), nameof(UnitPlayed))]
[JsonDerivedType(typeof(SpellPlayed), nameof(SpellPlayed))]
[JsonDerivedType(typeof(UnitHealthChanged), nameof(UnitHealthChanged))]
[JsonDerivedType(typeof(UnitDamageChanged), nameof(UnitDamageChanged))]
[JsonDerivedType(typeof(UnitDeath), nameof(UnitDeath))]
[JsonDerivedType(typeof(CardAttacked), nameof(CardAttacked))]
[JsonDerivedType(typeof(DeckOutOfCards), nameof(DeckOutOfCards))]
[JsonDerivedType(typeof(AddedCardToDeck), nameof(AddedCardToDeck))]
[JsonDerivedType(typeof(DeckModifiedStats), nameof(DeckModifiedStats))]
public record GameEventDto(Guid Source, Guid PlayerSource)
{
    public static GameEventDto Generate(GameEvent e, GameState state)
    {
        return e switch
        {
            GameEvent.CardDrawnEvent cde => new CardDrawnEvent(cde.Source.Id, cde.PlayerSource.Id, CardDto.Generate(cde.CardInstance, state)),
            GameEvent.PlayerHealthChanged phc => new PlayerHealthChanged(phc.Source.Id, phc.PlayerSource.Id, phc.Amount),
            GameEvent.UnitHealthChanged uhc => new UnitHealthChanged(uhc.Source.Id, uhc.PlayerSource.Id, uhc.Card.Id, uhc.Amount),
            GameEvent.UnitDamageChanged udc => new UnitDamageChanged(udc.Source.Id, udc.PlayerSource.Id, udc.Card.Id, udc.Amount),
            GameEvent.UnitPlayed up => new UnitPlayed(up.Source.Id, up.PlayerSource.Id, CardDto.Generate(up.Unit, state), up.BoardPosition),
            GameEvent.SpellPlayed sp => new SpellPlayed(sp.Source.Id, sp.PlayerSource.Id, CardDto.Generate(sp.Spell, state)),
            GameEvent.UnitDeath ud => new UnitDeath(ud.Source.Id, ud.PlayerSource.Id, ud.Unit.Id),
            GameEvent.DeckOutOfCards doc => new DeckOutOfCards(doc.Source.Id, doc.PlayerSource.Id),
            GameEvent.CardAttacked ca => new CardAttacked(ca.Source.Id, ca.PlayerSource.Id, ca.Attacker.Id, ca.TargetType.ToString(), ca.TargetIndex),
            GameEvent.AddedCardToDeck acd => new AddedCardToDeck(acd.Source.Id, acd.PlayerSource.Id, acd.TargetedPlayer.Id),
            GameEvent.DeckModifiedStats dms => new DeckModifiedStats(dms.Source.Id, dms.PlayerSource.Id, dms.TargetedPlayer.Id),
            _ => throw new ArgumentException($"Unknown event type: {e.GetType().Name}")
        };
    }

    public record CardDrawnEvent(Guid Source, Guid PlayerSource, CardDto Card) : GameEventDto(Source, PlayerSource){}
    public record PlayerHealthChanged(Guid Source, Guid PlayerSource, int Amount) : GameEventDto(Source, PlayerSource){}
    public record UnitHealthChanged(Guid Source, Guid PlayerSource, Guid Card, int Amount) : GameEventDto(Source, PlayerSource){}
    public record UnitDamageChanged(Guid Source, Guid PlayerSource, Guid Card, int Amount) : GameEventDto(Source, PlayerSource){}
    public record UnitPlayed(Guid Source, Guid PlayerSource, CardDto Unit, int BoardPosition) : GameEventDto(Source, PlayerSource){}
    public record SpellPlayed(Guid Source, Guid PlayerSource, CardDto Spell) : GameEventDto(Source, PlayerSource){}
    public record UnitDeath(Guid Source, Guid PlayerSource, Guid Unit) : GameEventDto(Source, PlayerSource){}
    public record DeckOutOfCards(Guid Source, Guid PlayerSource) : GameEventDto(Source, PlayerSource){}
    public record CardAttacked(Guid Source, Guid PlayerSource, Guid Attacker, string TargetType, int TargetIndex) : GameEventDto(Source, PlayerSource){}

    public record AddedCardToDeck(Guid Source, Guid PlayerSource, Guid TargetedPlayer) : GameEventDto(Source, PlayerSource) {}

    public record DeckModifiedStats(Guid Source, Guid PlayerSource, Guid TargetedPlayer) : GameEventDto(Source, PlayerSource);
    
}