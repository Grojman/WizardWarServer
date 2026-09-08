public class CreateCopyOfSelf : IEffect
{
    public bool CopyDamage { get; set; } = false;

    public CreateCopyOfSelf() {}
    public CreateCopyOfSelf(bool copyDamage)
    {
        CopyDamage = copyDamage;
    }

    public IEffect Clone() => new CreateCopyOfSelf(CopyDamage);

    public void Execute(Guid playerId, Guid rivalId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        var player = cardId.Player;
        var copy = new CardInstance(cardId.Definition, player);

        if (CopyDamage) copy.CurrentAttack = cardId.CurrentAttack;

        state.AddCard(player, player.PlayerTarget!, copy, cardId);
    }
}
