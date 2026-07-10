using System.Text.Json.Serialization;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(ChangeNameAction), "ChangeNameAction")]
[JsonDerivedType(typeof(JoinQueueAction), "JoinQueueAction")]
[JsonDerivedType(typeof(LeaveQueueAction), "LeaveQueueAction")]
[JsonDerivedType(typeof(GetDecksAction), "GetDecksAction")]
[JsonDerivedType(typeof(GetAllCardsAction), "GetAllCardsAction")]
[JsonDerivedType(typeof(StartBotGameAction), "StartBotGameAction")]
public interface UserAction
{
    public class ChangeNameAction : UserAction
    {
        public required string NewName { get; set;}
    }

    public class JoinQueueAction : UserAction
    {
        public required int DeckId { get; set; }

        public required int NumberOfPlayers { get; set; }
    }

    public class StartBotGameAction : UserAction
    {
        public required int DeckId { get; set; }

        public required int NumberOfPlayers { get; set; }
    }
    
    public class LeaveQueueAction : UserAction {}

    public class GetDecksAction : UserAction {}

    public class GetAllCardsAction : UserAction {}

}