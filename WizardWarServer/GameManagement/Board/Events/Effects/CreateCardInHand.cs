public class CreateCardInHand : IEffect
{
    public string CardId { get; set; }
    public IEffect Clone()
    {
        throw new NotImplementedException();
    }

    public void Execute(Guid playerId, Guid rivalId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        
    }
}