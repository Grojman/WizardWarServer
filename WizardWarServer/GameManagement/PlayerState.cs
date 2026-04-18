public class PlayerState : IdentificableObject
{
    public PlayerConnection Connection { get; set; }
    public required string Name { get; set; }
    public int Health { get; set; } = 20;

    public Deck Deck { get; set; }

    public List<CardInstance> Hand { get; set; } = new();

    public CardInstance?[] Board { get; set; } = new CardInstance[4];

    public CardInstance GetFromHand(int index)
    {
        var card = Hand[index];
        Hand.RemoveAt(index);
        return card;
    }

}