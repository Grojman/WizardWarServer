public class CardInstance : IdentificableObject
{
    public Guid PlayerGuid { get; }

    public CardDefinition Definition { get; }

    public int CurrentAttack { get; set; }

    public int CurrentHealth { get; set; }

    public List<string> CurrentFamilies { get; set; }

    public List<EffectInstance> Effects { get; set; }
    public EffectCondition? CanPlay { get; set; }
    public IEffect? SpecialEffect { get; set; }
    public int MaxSpecialEffectTimes { get; set; }
    public bool DeathChecked { get; set; } = false;

    public CardInstance(CardDefinition def, Guid PlayerId)
    {
        PlayerGuid = PlayerId;
        Definition = def;

        CurrentAttack = def.BaseAttack;
        CurrentHealth = def.BaseHealth;

        CurrentFamilies = [.. def.Families];

        CanPlay = def.ConditionToPlay?.Clone();
        MaxSpecialEffectTimes = def.PlayEffectTriggerTimes;
        SpecialEffect = def.PlayEffect?.Clone();

        Effects = new();
        foreach(EffectInstance e in def.Effects) Effects.Add(e.Clone());

        foreach(EffectInstance e in Effects) AssignEffect(e);
    }

    public void AssignEffect(EffectInstance e)
    {
        e.PlayerSourceId = PlayerGuid;
        e.SourceCard = this;
    }
}