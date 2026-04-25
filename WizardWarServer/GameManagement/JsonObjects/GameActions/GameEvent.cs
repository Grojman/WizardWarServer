public abstract class GameEvent
{
    public required PlayerState PlayerSource { get; set; }
    public required IdentificableObject Source { get; set; }
    public class CardDrawnEvent : GameEvent
    {
        public required Guid PlayerId { get; set; }
        public required CardInstance CardInstance { get; set; }
    }

    public class PlayerHealthChanged : GameEvent
    {
        public required Guid PlayerId { get; set; }
        public required int Amount { get; set; }
    }

    public class UnitHealthChanged : GameEvent
    {
        public required CardInstance Card { get; set;}

        public required int Amount { get; set; }
    }

    public class UnitDamageChanged : GameEvent
    {
        public required CardInstance Card { get; set; }
        public required int Amount { get; set; }
    }

    public class UnitPlayed : GameEvent
    {
        public required int BoardPosition { get; set; }
        public required CardInstance Unit { get; set; }
    }

    public class SpellPlayed : GameEvent
    {
        public required CardInstance Spell { get; set; }
    }

    public class UnitDeath : GameEvent
    {
        public required int BoardPosition { get; set; }
        public required CardInstance Unit { get; set; }
    }

    public class DeckOutOfCards : GameEvent {}

    public class CardAttacked : GameEvent
    {
        public required CardInstance Attacker { get; set; }
        public required CardInstance? Deffender { get; set; }
        public required TargetType TargetType { get; set; }
        public required int TargetIndex { get; set; }
    }

    public class AddedCardToDeck : GameEvent
    {
        public required PlayerState TargetedPlayer { get; set; }
        public required CardInstance AddedCard { get; set; }
    }

    public class DeckModifiedStats : GameEvent
    {
        public required PlayerState TargetedPlayer { get; set; }
        public required List<CardInstance> AffectedCards { get; set; }
    }

    public class CardEventPlayed : GameEvent
    {
        public required CardInstance Card { get; set; }
    }

}