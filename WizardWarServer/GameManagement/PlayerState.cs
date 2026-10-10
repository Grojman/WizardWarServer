using System.Text.Json.Serialization;

public class PlayerState : IdentificableObject
{
    public TargetGroup Targets { get; } = new();
    public bool IsMyTurn { get; set; } = false;
    public List<EffectInstance> GlobalEffects { get; set; } = new();
    public List<CardInstance> PlayedCards { get; set; } = new();
    public List<CardInstance> DeadCards { get; set; } = new();
    public PlayerConnection Connection { get; set; } = null!;
    public required string Name { get; set; }
    public int Health { get; set; } = 20;

    // Set when the player is knocked out of the match (see GameState.KillPlayer).
    public DefeatReason? DefeatReason { get; set; }

    public Deck? Deck { get; set; }

    public List<CardInstance> Hand { get; set; } = new();

    public CardInstance?[] Board { get; set; } = new CardInstance[4];

    public CardInstance? LastSpellPlayed = null;

    public CardInstance? GetFromHand(Guid cardId)
    {
        var index = Hand.FindIndex(c => c.Id == cardId);
        if (index == -1) return null;

        var card = Hand[index];
        Hand.RemoveAt(index);
        return card;
    }

}
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DefeatReason
{
    NoHealth,
    OutOfCards,
    Surrender,
    Disconnect
}
