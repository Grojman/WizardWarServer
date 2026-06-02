public class AlterPlayerHealthBasedOnMyStats : IEffect
{

    public AlterPlayerHealthBasedOnMyStats(AffectedStats stats, float relatio, bool rival)
    {
        Stats = stats;
        Relatio = relatio;
        Rival = rival;
    }
    public bool Rival { get; set; }
    public AffectedStats Stats { get; set; }
    public float Relatio { get; set; }
    public IEffect Clone() => new AlterPlayerHealthBasedOnMyStats(Stats, Relatio, Rival);

    public void Execute(Guid playerId, Guid rivalId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        int total = 0;

        if(Stats is AffectedStats.DAMAGE or AffectedStats.BOTH)
        {
            total += cardId.CurrentAttack;
        }
        if(Stats is AffectedStats.HEALTH or AffectedStats.BOTH)
        {
            total += cardId.CurrentHealth;
        }

        total = (int)(total * Relatio);

        state.AlterPlayerHealth(cardId, Rival ? cardId.Player.PlayerTarget : cardId.Player, total);
        
    }
}