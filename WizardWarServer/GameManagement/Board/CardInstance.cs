public class CardInstance : IdentificableObject
{
    public Guid PlayerGuid { get; }

    public CardDefinition Definition { get; }

    public int CurrentAttack { get; set; }

    public int CurrentHealth { get; set; }

    public List<EffectInstance> Effects { get; set; }

    public CardInstance(CardDefinition def, Guid PlayerId)
    {
        PlayerGuid = PlayerId;
        Definition = def;

        CurrentAttack = def.BaseAttack;
        CurrentHealth = def.BaseHealth;

        Effects = new();
        foreach(EffectInstance e in def.Effects) Effects.Add(e.Clone());

        foreach(EffectInstance e in Effects)
        {
            e.PlayerSourceId = PlayerGuid;
            e.SourceCard = this;
        }
    }
}