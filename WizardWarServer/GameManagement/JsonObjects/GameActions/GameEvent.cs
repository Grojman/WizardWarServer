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

}