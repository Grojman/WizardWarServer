// Plays one random extra action (play a card, attack or draw) for the owner of
// the card, as if it were their turn. Attacks go to the rival the effect is
// being run against.
public class DecideTurnEffect : IEffect
{
    public IEffect Clone() => new DecideTurnEffect();

    public void Execute(Guid playerId, Guid rivalId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        var player = cardId.Player;
        if (!state.IsAlive(player)) return;

        List<Action> options = new();

        if (player.CanPlayCard(state)) options.Add(() => PlayCard(player, state));
        if (player.CanAttack() && state.IsAlive(state.GetState(rivalId))) options.Add(() => Attack(player, state.GetState(rivalId), state));
        if (player.CanDraw(options.Count)) options.Add(() => state.DrawCard(player.Connection, null));

        if (options.Count == 0) return;

        options.GetRandom()();
    }

    static void Attack(PlayerState player, PlayerState rival, GameState state)
    {
        var attackers = OccupiedPositions(player.Board, c => c.CurrentAttack > 0);
        if (attackers.Count == 0) return;

        var defenders = OccupiedPositions(rival.Board, _ => true);
        var targetType = defenders.Count == 0 || Random.Shared.Next(2) == 0 ? TargetType.PLAYER : TargetType.BOARD;
        var targetIndex = targetType == TargetType.BOARD ? defenders.GetRandom() : -1;

        state.Attack(player.Connection, rival.Id, attackers.GetRandom(), targetType, targetIndex);
    }

    static void PlayCard(PlayerState player, GameState state)
    {
        var freeSlots = player.Board.Select((card, index) => new { card, index }).Where(n => n.card is null).Select(n => n.index).ToList();
        var playableCards = player.Hand
            .Where(n => state.CanPlay(n) && (n.Definition.Type == CardType.Spell || freeSlots.Count > 0))
            .ToList();
        if (playableCards.Count == 0) return;

        var card = playableCards.GetRandom();
        var boardIndex = card.Definition.Type == CardType.Spell ? -1 : freeSlots.GetRandom();

        state.PlayCard(player.Connection, card.Id, boardIndex);
    }

    static List<int> OccupiedPositions(CardInstance?[] board, Func<CardInstance, bool> predicate) =>
        board.Select((card, index) => new { card, index }).Where(n => n.card is not null && predicate(n.card)).Select(n => n.index).ToList();

    // Only touches its own player/card.
    public IEffect? ForAdditionalTarget() => null;
}
