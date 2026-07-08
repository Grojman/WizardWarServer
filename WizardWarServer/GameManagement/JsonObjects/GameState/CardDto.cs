public record CardDto(string id, string name, string description, List<string> families, int attack, int health, string type, string imageUrl, bool canPlay, bool hasEffect, int effectTimes)
{
    public static CardDto Generate(CardInstance card, GameState state, bool dontCheck)
    {
        return new CardDto(card.Id.ToString(), card.Definition.Name, card.Definition.Description, card.CurrentFamilies, card.CurrentAttack, card.CurrentHealth, card.Definition.Type.ToString(), string.IsNullOrWhiteSpace(card.Definition.imageUrl) ? $"{card.Definition.Id}.png" : card.Definition.imageUrl, dontCheck || (card.CanPlay?.Check(card.Player.Id, card.Player.PlayerTarget!.Id, card, state, null) ?? true), card.SpecialEffects is not null && card.SpecialEffects.Count > 0, card.MaxSpecialEffectTimes);
    }

    public static CardDto Generate(CardDefinition card)
    {
        return new CardDto(card.Id, card.Name, card.Description, card.Families.ToList(), card.BaseAttack, card.BaseHealth, card.Type.ToString(), string.IsNullOrWhiteSpace(card.imageUrl) ? $"{card.Id}.png" : card.imageUrl, true, card.PlayEffects is not null && card.PlayEffects.Length > 0, card.PlayEffectTriggerTimes);
    }
}