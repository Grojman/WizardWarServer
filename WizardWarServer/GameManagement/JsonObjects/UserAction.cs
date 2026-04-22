using System.Text.Json.Serialization;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(ChangeNameAction), "ChangeNameAction")]
[JsonDerivedType(typeof(JoinQueueAction), "JoinQueueAction")]
[JsonDerivedType(typeof(LeaveQueueAction), "LeaveQueueAction")]
[JsonDerivedType(typeof(GetDecksAction), "GetDecksAction")]
[JsonDerivedType(typeof(GetAllCardsAction), "GetAllCardsAction")]
public interface UserAction
{
    public class ChangeNameAction : UserAction
    {
        public required string NewName { get; set;}
    }

    public class JoinQueueAction : UserAction
    {
        public required int DeckId { get; set; }
    }
    
    public class LeaveQueueAction : UserAction {}

    public class GetDecksAction : UserAction {}

    public class GetAllCardsAction : UserAction {}

}