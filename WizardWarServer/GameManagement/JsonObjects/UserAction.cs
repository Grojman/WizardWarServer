using System.Text.Json.Serialization;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(ChangeNameAction), "ChangeNameAction")]
[JsonDerivedType(typeof(JoinQueueAction), "JoinQueueAction")]
[JsonDerivedType(typeof(LeaveQueueAction), "LeaveQueueAction")]
[JsonDerivedType(typeof(GetDecksAction), "GetDecksAction")]
[JsonDerivedType(typeof(GetAllCardsAction), "GetAllCardsAction")]
[JsonDerivedType(typeof(StartBotGameAction), "StartBotGameAction")]
[JsonDerivedType(typeof(SendSuggestion), "SendSuggestion")]
[JsonDerivedType(typeof(GetStatsAction), "GetStatsAction")]
[JsonDerivedType(typeof(CreatePrivateMatchAction), "CreatePrivateMatchAction")]
[JsonDerivedType(typeof(JoinPrivateMatchAction), "JoinPrivateMatchAction")]
[JsonDerivedType(typeof(LeavePrivateMatchAction), "LeavePrivateMatchAction")]
public interface UserAction
{
    public class SendSuggestion : UserAction
    {
        public required string Suggestion { get; set; }
    }
    public class ChangeNameAction : UserAction
    {
        public required string NewName { get; set;}
    }

    public class JoinQueueAction : UserAction
    {
        public required int DeckId { get; set; }

        public required int NumberOfPlayers { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MatchFormat Format { get; set; } = MatchFormat.Single;
    }

    public class StartBotGameAction : UserAction
    {
        public required int DeckId { get; set; }

        public required int NumberOfPlayers { get; set; }
    }
    
    public class LeaveQueueAction : UserAction {}

    public class GetDecksAction : UserAction {}

    public class GetAllCardsAction : UserAction {}

    public class GetStatsAction : UserAction {}

    public class CreatePrivateMatchAction : UserAction
    {
        public required int DeckId { get; set; }

        public required int NumberOfPlayers { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MatchFormat Format { get; set; } = MatchFormat.Single;
    }

    public class JoinPrivateMatchAction : UserAction
    {
        public required int DeckId { get; set; }

        public required string Code { get; set; }
    }

    public class LeavePrivateMatchAction : UserAction {}

}