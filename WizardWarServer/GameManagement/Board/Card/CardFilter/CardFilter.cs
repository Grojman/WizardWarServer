public class CardFilter
{
    public int? CurrentAttack { get; set; } = null;
    public int? CurrentHealth { get; set; } = null;
    public int? Attack { get; set; } = null;
    public int? Health { get; set; } = null;
    public CardType? CardType { get; set; } = null;
    public string[]? Families { get; set; } = null;
    public string[]? CurrentFamilies { get; set; } = null;
    public string? DefinitionId { get; set; } = null;
    public Guid? Id { get; set; } = null;
    public bool? HasEffect { get; set; } = null;
    public int? NumberOfTriggersLeft { get; set; } = null;
    
    public bool MustMeetAllCriteria { get; set; } = false;

    public bool Check(CardInstance card)
    {
        var criteria = new List<bool>();

        if(CardType.HasValue)
        {
            criteria.Add(card.Definition.Type == CardType.Value);
        }
        if (CurrentAttack.HasValue)
        {
            criteria.Add(card.CurrentAttack == CurrentAttack.Value);
        }
        if (CurrentHealth.HasValue)
        {
            criteria.Add(card.CurrentHealth == CurrentHealth.Value);
        }
        if (Attack.HasValue)
        {
            criteria.Add(card.Definition.BaseAttack == Attack.Value);
        }
        if (Health.HasValue)
        {
            criteria.Add(card.Definition.BaseHealth == Health.Value);
        }
        if (Families != null)
        {
            if (MustMeetAllCriteria)
            {
                criteria.Add(Families.All(f => card.Definition.Families.Contains(f)));
            }
            else
            {
                criteria.Add(Families.Any(f => card.Definition.Families.Contains(f)));
            }
        }
        if (CurrentFamilies != null)
        {
            if (MustMeetAllCriteria)
            {
                criteria.Add(CurrentFamilies.All(f => card.CurrentFamilies.Contains(f)));
            }
            else
            {
                criteria.Add(CurrentFamilies.Any(f => card.CurrentFamilies.Contains(f)));
            }
        }
        if (DefinitionId != null)
        {
            criteria.Add(card.Definition.Id == DefinitionId);
        }
        if (Id.HasValue)
        {
            criteria.Add(card.Id == Id.Value);
        }
        if (HasEffect.HasValue)
        {
            criteria.Add(card.SpecialEffect is not null);
        }
        if (NumberOfTriggersLeft.HasValue)
        {
            criteria.Add(card.MaxSpecialEffectTimes == NumberOfTriggersLeft.Value);
        }

        if (criteria.Count == 0)
        {
            return true;
        }

        if (MustMeetAllCriteria)
        {
            return criteria.All(c => c);
        }
        else
        {
            return criteria.Any(c => c);
        }
    }
}