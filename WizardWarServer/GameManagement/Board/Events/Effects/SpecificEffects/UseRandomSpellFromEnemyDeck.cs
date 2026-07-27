
public class UseRandomSpellFromEnemyDeck : IEffect
{
    public IEffect Clone() => new UseRandomSpellFromEnemyDeck();
    public void Execute(Guid playerId, Guid rivalId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        var cards = state.GetState(rivalId).Deck!.cards.Where(n => n.Definition.Type == CardType.Spell);
        if (cards.Any())
        {
            var player = state.GetState(playerId);
            var copy = new CardInstance(cards.GetRandom().Definition, player);
            state.PlayCard(player, copy, -1);
        }
    }
}