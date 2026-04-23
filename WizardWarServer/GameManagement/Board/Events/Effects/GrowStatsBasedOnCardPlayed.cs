
public class GrowStatsBasedOnCardPlayed : IEffect
{
    public GrowStatsBasedOnCardPlayed(string familyType, int health, int damage, bool applyToMyself)
    {
        FamilyType = familyType;
        Health = health;
        Damage = damage;
        ApplyToMyself = applyToMyself;
    }

    public string FamilyType { get; set; } = string.Empty;
    public int Health { get; set; } = 0;
    public int Damage { get; set; } = 0;

    public bool ApplyToMyself { get; set; } = true;

    public void Execute(Guid playerId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        if(ev is GameEvent.UnitPlayed e && (e.Unit.CurrentFamilies.Contains(FamilyType) || string.IsNullOrWhiteSpace(FamilyType)))
        {
            state.AlterUnitDamage(cardId, ApplyToMyself ? cardId : e.Unit, Damage);
            state.AlterUnitHealth(cardId, ApplyToMyself ? cardId : e.Unit, Health);
        }
    }
}