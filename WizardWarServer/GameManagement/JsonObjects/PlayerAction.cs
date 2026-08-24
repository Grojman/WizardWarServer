using System.Text.Json.Serialization;

[JsonPolymorphic(TypeDiscriminatorPropertyName ="$type")]
[JsonDerivedType(typeof(DrawCardAction), nameof(DrawCardAction))]
[JsonDerivedType(typeof(PlayCardAction), nameof(PlayCardAction))]
[JsonDerivedType(typeof(AttackAction), nameof(AttackAction))]
[JsonDerivedType(typeof(CardEffectActivated), nameof(CardEffectActivated))]
[JsonDerivedType(typeof(TextMessage), nameof(TextMessage))]
[JsonDerivedType(typeof(ChangeTarget), nameof(ChangeTarget))]
[JsonDerivedType(typeof(LeaveGame), nameof(LeaveGame))]
[JsonDerivedType(typeof(SelectSeriesDeckAction), nameof(SelectSeriesDeckAction))]
[JsonDerivedType(typeof(RequestSeriesStateAction), nameof(RequestSeriesStateAction))]
[JsonDerivedType(typeof(GetDecksAction), nameof(GetDecksAction))]
public interface PlayerAction
{
    public class LeaveGame : PlayerAction {}
    public class GetDecksAction : PlayerAction {}

    public class SelectSeriesDeckAction : PlayerAction
    {
        public required int DeckId { get; set; }
    }
    public class RequestSeriesStateAction : PlayerAction {}
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
        public required Guid CardId { get; set; }
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