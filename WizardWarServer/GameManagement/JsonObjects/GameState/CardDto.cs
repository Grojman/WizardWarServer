using System.ComponentModel.Design;

public record CardDto(string id, string serverId, string name, string description, List<string> families, int attack, int health, string type, string imageUrl, bool canPlay, bool hasEffect, int effectTimes, int? conditionProgress, int? conditionTarget)
{
    public static CardDto Generate(CardInstance card, GameState state, bool dontCheck)
    {
        int? conditionProgress = null;
        int? conditionTarget = null;

        if (card.CanPlay is CountPlayedCardsCondition cpc)
        {
            var target = cpc.Target == PlayerType.PLAYER ? card.Player.Id : card.Player.PlayerTarget!.Id;
            conditionProgress = state.GetState(target).PlayedCards.Count(cpc.Filter.Check);
            conditionTarget = cpc.Value.Amount;
        } else if (card.CanPlay is MultiEffectCondition mec)
        {
            foreach(var a in mec.Conditions)
            {
                if (a is CountPlayedCardsCondition acpc)
                {
                    var target = acpc.Target == PlayerType.PLAYER ? card.Player.Id : card.Player.PlayerTarget!.Id;
                    conditionProgress = state.GetState(target).PlayedCards.Count(acpc.Filter.Check);
                    conditionTarget = acpc.Value.Amount;

                    break;
                }
            }
        }

        return new CardDto(card.Id.ToString(), card.Definition.Id, card.Definition.Name, card.Definition.Description, card.CurrentFamilies, card.CurrentAttack, card.CurrentHealth, card.Definition.Type.ToString(), string.IsNullOrWhiteSpace(card.Definition.imageUrl) ? $"{card.Definition.Id}.webp" : card.Definition.imageUrl, dontCheck || (card.CanPlay?.Check(card.Player.Id, card.Player.PlayerTarget!.Id, card, state, null) ?? true), card.SpecialEffects is not null && card.SpecialEffects.Count > 0, card.MaxSpecialEffectTimes, conditionProgress, conditionTarget);
    }

    public static CardDto Generate(CardDefinition card)
    {
        return new CardDto(card.Id, card.Id, card.Name, card.Description, card.Families.ToList(), card.BaseAttack, card.BaseHealth, card.Type.ToString(), string.IsNullOrWhiteSpace(card.imageUrl) ? $"{card.Id}.webp" : card.imageUrl, true, card.PlayEffects is not null && card.PlayEffects.Length > 0, card.PlayEffectTriggerTimes, null, null);
    }
}