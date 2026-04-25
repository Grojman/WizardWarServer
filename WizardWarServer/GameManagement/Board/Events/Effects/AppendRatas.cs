
public class AppendRatas : IEffect
{
    const string RATA_ID = "2";
    int ratasAmount  { get; set; } = 1;

    public AppendRatas() {}
    public AppendRatas(int ratasAmount)
    {
        this.ratasAmount = ratasAmount;
    }

    public void Execute(Guid playerId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        var rataInstance = CardManager.GetCardById(RATA_ID);

        var rival = state.GetRival(playerId);
        var player = state.GetState(playerId);

        for (int i = 0; i < ratasAmount; i++)
        {
            state.AddCard(rival, player, new CardInstance(rataInstance, rival.Id), cardId);
        }
    }

    public IEffect Clone() => new AppendRatas(ratasAmount);
}