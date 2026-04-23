public record CardDto(string id, string name, string description, List<string> families, int attack, int health, string type, string imageUrl)
{
    public static CardDto Generate(CardInstance card)
    {
        return new CardDto(card.Id.ToString(), card.Definition.Name, card.Definition.Description, card.CurrentFamilies, card.CurrentAttack, card.CurrentHealth, card.Definition.Type.ToString(), card.Definition.imageUrl);
    }

    public static CardDto Generate(CardDefinition card)
    {
        return new CardDto(card.Id, card.Name, card.Description, card.Families.ToList(), card.BaseAttack, card.BaseHealth, card.Type.ToString(), card.imageUrl);
    }
}