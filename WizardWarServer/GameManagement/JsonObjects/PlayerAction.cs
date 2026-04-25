using System.Text.Json.Serialization;

[JsonPolymorphic(TypeDiscriminatorPropertyName ="$type")]
[JsonDerivedType(typeof(DrawCardAction), nameof(DrawCardAction))]
[JsonDerivedType(typeof(PlayCardAction), nameof(PlayCardAction))]
[JsonDerivedType(typeof(AttackAction), nameof(AttackAction))]
[JsonDerivedType(typeof(CardEffectActivated), nameof(CardEffectActivated))]
public interface PlayerAction
{
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
    }
}