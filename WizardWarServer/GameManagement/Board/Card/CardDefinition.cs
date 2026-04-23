using Microsoft.AspNetCore.Mvc.Routing;

public class CardDefinition
{
    public CardDefinition()
    {
    }

    public CardDefinition(string id, string name, CardType type, string description, int baseAttack, int baseHealth, List<EffectInstance> effects, string[] families, string imagUrl)
    {
        Id = id;
        Name = name;
        Type = type;
        Description = description;
        BaseAttack = baseAttack;
        BaseHealth = baseHealth;
        Effects = effects;
        Families = families;
        this.imageUrl = imageUrl;
    }

    public string[] Families { get; set; } = [];
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public CardType Type { get; set; } = CardType.Unit;

    public string Description { get; set; } = string.Empty;

    public int BaseAttack { get; set; } = 0;

    public int BaseHealth { get; set; } = 0;

    public List<EffectInstance> Effects { get; set; } = [];

    public string imageUrl { get; set; } = string.Empty;

    
}