using Microsoft.Extensions.Options;

public class GeneratePaqueteEffect : IEffect
{
    const int SMALLEST_PAQUETE_ID = 71;
    public IEffect Clone() => new GeneratePaqueteEffect();

    public void Execute(Guid playerId, Guid rivalId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        Console.WriteLine("It executes");
        if (ev is GameEvent.SpellPlayed s)
        {
            Console.WriteLine("Searching options");

            List<int> idOptions = new();
            int last = int.Parse(s.Card.Definition.Id);
            Console.WriteLine($"Starting in: {last}");

            while(last >= SMALLEST_PAQUETE_ID)
            {
        Console.WriteLine("Adding option");
                idOptions.Add(last);
                last -= 2;
            }
            if(idOptions.Count != 0)
            {    
                Console.WriteLine("There are more than 0");

                var def = CardManager.GetCardById(idOptions.GetRandom().ToString());
                state.AddCard(cardId.Player, cardId.Player, new(def, cardId.Player), cardId);
            }
        }
    }
}