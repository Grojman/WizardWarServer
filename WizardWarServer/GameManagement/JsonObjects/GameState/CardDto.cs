public record CardDto(Guid id, string name, string description, int attack, int health, string type)
{
    public static CardDto Generate(CardInstance card)
    {
        return new CardDto(card.Id, card.Definition.Name, card.Definition.Description, card.CurrentAttack, card.CurrentHealth, card.Definition.Type.ToString());
    }
}