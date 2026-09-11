public abstract class GameEvent
{
    public required PlayerState PlayerSource { get; set; }
    public required IdentificableObject Source { get; set; }


    public abstract class GameEventCard : GameEvent
    {
        public required CardInstance Card { get; set;}
    }
    public class TargetPlayerChanged : GameEvent
    {
        public required Guid NewTarget { get; set; }
    }

    public class PlayerDeath : GameEvent
    {
        
    }

    public class TextMessage : GameEvent
    {
        public required string Message { get; set; }
    }
    public class CardDrawnEvent : GameEventCard
    {
        public required Guid PlayerId { get; set; }
        public required bool FromDeck { get; set; }
    }

    public class PlayerHealthChanged : GameEvent
    {
        public required Guid PlayerId { get; set; }
        public required int Amount { get; set; }
    }

    public class UnitHealthChanged : GameEventCard
    {

        public required int Amount { get; set; }
    }

    public class UnitDamageChanged : GameEventCard
    {
        public required int Amount { get; set; }
    }

    public class UnitPlayed : GameEventCard
    {
        public required int BoardPosition { get; set; }

        // Card's own on-play effects (e.g. "when played, +1/+1 to my board")
        // run synchronously right after this event is queued but before it's
        // ever serialized, and Card is a live reference — so by the time the
        // DTO is built at broadcast time, Card.CurrentAttack/CurrentHealth
        // already include that effect's change. Captured here, at
        // construction time (see GameState.PlayCard), these hold the stats
        // as the card was actually placed, so the client can place it at its
        // pre-effect stats and let the effect's own UnitHealthChanged/
        // UnitDamageChanged event animate the change on top, instead of
        // double-applying it.
        public required int PlacedAttack { get; set; }
        public required int PlacedHealth { get; set; }
    }

    public class SpellPlayed : GameEventCard
    {
    }

    public class UnitDeath : GameEventCard
    {
        public required int BoardPosition { get; set; }
    }

    public class DeckOutOfCards : GameEvent {}

    public class CardAttacked : GameEvent
    {
        public required CardInstance Attacker { get; set; }
        public required CardInstance? Deffender { get; set; }
        public required TargetType TargetType { get; set; }
        public required int TargetIndex { get; set; }
        public required PlayerState PlayerTarget { get; set; }

        // Post-attack effects (e.g. "after attacking, +1/+1") run synchronously
        // right after this event is queued but before it's ever serialized, and
        // Attacker/Deffender are live references — so by the time the DTO is
        // built at broadcast time, CurrentAttack may already include that
        // effect's change. Captured here, at construction time (see
        // GameState.Attack), these hold the attack values actually used to
        // compute the damage dealt, so the client's health change matches
        // what the server actually applied.
        public required int AttackerDamage { get; set; }
        public required int DefenderDamage { get; set; }
    }

    public class AddedCardToDeck : GameEventCard
    {
        public required PlayerState TargetedPlayer { get; set; }
    }

    public class DeckModifiedStats : GameEvent
    {
        public required PlayerState TargetedPlayer { get; set; }
        public required IEnumerable<CardInstance> AffectedCards { get; set; }
    }

    public class CardEventPlayed : GameEventCard
    {
    }

    // Internal-only signal (never broadcast via GameActionResult/GameEventDto): fired
    // whenever any EffectInstance is appended to a player's PlayerState.GlobalEffects,
    // for whichever deck. Conditions that care about a specific kind of global effect
    // (e.g. ColorChangedToCondition) upcast Effect.Effects to find it.
    public class GlobalEffectAdded : GameEvent
    {
        public required Guid PlayerId { get; set; }
        public required EffectInstance Effect { get; set; }
    }

}