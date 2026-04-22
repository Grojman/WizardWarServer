public record CardDto(string id, string name, string description, List<string> families, int attack, int health, string type)
{
    public static CardDto Generate(CardInstance card)
    {
        return new CardDto(card.Id.ToString(), card.Definition.Name, card.Definition.Description, card.CurrentFamilies, card.CurrentAttack, card.CurrentHealth, card.Definition.Type.ToString());
    }

    public static CardDto Generate(CardDefinition card)
    {
        return new CardDto(card.Id, card.Name, card.Description, card.Families.ToList(), card.BaseAttack, card.BaseHealth, card.Type.ToString());
    }
}