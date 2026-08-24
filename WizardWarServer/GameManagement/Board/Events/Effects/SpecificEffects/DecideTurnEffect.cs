public class DecideTurnEffect : IEffect
{
    private CardInstance card = null!;
    public IEffect Clone() => new DecideTurnEffect();

    public void Execute(Guid playerId, Guid rivalId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        List<Action<GameState>> options = new();
        card = cardId;

        if(cardId.Player.CanPlayCard(state)) options.Add(PlayCard);
        if(cardId.Player.CanAttack()) options.Add(Attack);
        if(cardId.Player.CanDraw(options.Count)) options.Add(DrawCard);

        options.GetRandom()(state);
    }

    void Attack(GameState state)
    {
        var rivalPositions = card.Player.PlayerTarget!.Board.Select((a, b) => new {a, b}).Where(n => n.a is not null).Select(n => n.b);
        var playerPositions = card.Player.Board.Select((a, b) => new {a, b}).Where(n => n.a is not null).Select(n => n.b);
        TargetType[] options = !rivalPositions.Any() ? [TargetType.PLAYER] : [TargetType.PLAYER, TargetType.BOARD];

        state.Attack(card.Player.Connection, card.Player.PlayerTarget.Id, playerPositions.GetRandom(), options.GetRandom(), rivalPositions.GetRandom());
    }

    void DrawCard(GameState state)
    {
        state.DrawCard(card.Player.Connection, null);
    }

    void PlayCard(GameState state)
    {
        var isPlace = card.Player.Board.Any(n => n is null);
        var boardIndexes = card.Player.Board.Select((a,b) => new {a, b}).Where(n => n.a is null).Select(n => n.b);
        var playableCards = card.Player.Hand.Where(n => (n.CanPlay?.Check(n.Player.Id, n.Player.PlayerTarget!.Id, n, state, null) ?? true) && (n.Definition.Type == CardType.Spell || isPlace));

        state.PlayCard(card.Player.Connection, playableCards.GetRandom().Id, isPlace ? -1 : boardIndexes.GetRandom());
    }
}