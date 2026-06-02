using Microsoft.Extensions.Options;

public class GeneratePaqueteEffect : IEffect
{
    const int SMALLEST_PAQUETE_ID = 75;
    public IEffect Clone() => new GeneratePaqueteEffect();

    public void Execute(Guid playerId, Guid rivalId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        if (ev is GameEvent.SpellPlayed s)
        {
            List<int> idOptions = new();
            int last = int.Parse(s.Card.Definition.Id);
            while(last >= SMALLEST_PAQUETE_ID)
            {
                idOptions.Add(last);
                last -= 2;
            }
            var def = CardManager.GetCardById(idOptions.GetRandom().ToString());
            state.AddCard(cardId.Player, cardId.Player, new(def, cardId.Player), cardId);
        }
    }
}