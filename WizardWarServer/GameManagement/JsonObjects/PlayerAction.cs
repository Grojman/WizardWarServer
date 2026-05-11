using System.Text.Json.Serialization;

[JsonPolymorphic(TypeDiscriminatorPropertyName ="$type")]
[JsonDerivedType(typeof(DrawCardAction), nameof(DrawCardAction))]
[JsonDerivedType(typeof(PlayCardAction), nameof(PlayCardAction))]
[JsonDerivedType(typeof(AttackAction), nameof(AttackAction))]
[JsonDerivedType(typeof(CardEffectActivated), nameof(CardEffectActivated))]
[JsonDerivedType(typeof(TextMessage), nameof(TextMessage))]
[JsonDerivedType(typeof(ChangeTarget), nameof(ChangeTarget))]
public interface PlayerAction
{
    public class ChangeTarget : PlayerAction
    {
        public required Guid NewTarget { get; set; }
    }
    public class TextMessage : PlayerAction
    {
        public required string Message { get; set; }
    }
    public class CardEffectActivated : PlayerAction
    {
        public required int CardIndex { get; set; }
    }
    public class DrawCardAction : PlayerAction {}
    public class PlayCardAction : PlayerAction
    {
        public required int CardIndex { get; set; }
        public required int BoardIndex { get; set; }
    }

    public class AttackAction : PlayerAction
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required TargetType TargetType { get; set;}
        public required int AttackerIndex { get; set; }
        public required int TargetIndex { get; set; }
        public required Guid PlayerTarget { get; set; }

    }
}