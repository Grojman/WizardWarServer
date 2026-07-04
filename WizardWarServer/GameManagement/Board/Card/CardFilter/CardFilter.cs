public class CardFilter
{
    public NumberFilter? CurrentAttack { get; set; } = null;
    public NumberFilter? CurrentHealth { get; set; } = null;
    public NumberFilter? Attack { get; set; } = null;
    public NumberFilter? Health { get; set; } = null;
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
        if (CurrentAttack is not null)
        {
            criteria.Add(CurrentAttack.Compare(card.CurrentAttack));
        }
        if (CurrentHealth is not null)
        {
            criteria.Add(CurrentHealth.Compare(card.CurrentHealth));
        }
        if (Attack is not null)
        {
            criteria.Add(Attack.Compare(card.Definition.BaseAttack));
        }
        if (Health is not null)
        {
            criteria.Add(Health.Compare(card.Definition.BaseHealth));
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
            criteria.Add(card.SpecialEffects is not null && card.SpecialEffects.Count > 0);
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