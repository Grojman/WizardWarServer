public class CardInstance : IdentificableObject
{
    public PlayerState Player { get; set; }

    public CardDefinition Definition { get; }

    public int CurrentAttack { get; set; }

    public int CurrentHealth { get; set; }

    public List<string> CurrentFamilies { get; set; }

    public List<EffectInstance> Effects { get; set; }
    public EffectCondition? CanPlay { get; set; }
    public List<IEffect>? SpecialEffects { get; set; }
    public int MaxSpecialEffectTimes { get; set; }
    public bool DeathChecked { get; set; } = false;

    public CardInstance(CardDefinition def, PlayerState player)
    {
        Player = player;
        Definition = def;

        CurrentAttack = def.BaseAttack;
        CurrentHealth = def.BaseHealth;

        CurrentFamilies = [.. def.Families];

        CanPlay = def.ConditionToPlay?.Clone();
        MaxSpecialEffectTimes = def.PlayEffectTriggerTimes;

        SpecialEffects = new();
        foreach(IEffect e in def.PlayEffects ?? []) SpecialEffects.Add(e.Clone());

        Effects = new();
        foreach(EffectInstance e in def.Effects) Effects.Add(e.Clone());

        foreach(EffectInstance e in Effects) AssignEffect(e);
    }

    public void AssignEffect(EffectInstance e)
    {
        e.Player = Player;
        e.SourceCard = this;
    }
}