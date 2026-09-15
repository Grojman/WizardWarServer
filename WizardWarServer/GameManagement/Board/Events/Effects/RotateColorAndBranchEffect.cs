// Rota el color (ver RotateColorEffect) y, tras rotar, ejecuta la lista de
// efectos asociada a cada componente básico (Rojo/Verde/Azul) de los colores
// activos - un color mixto o Blanco dispara varias ramas a la vez (sus
// componentes), pero cada rama básica se ejecuta como máximo una vez aunque
// varios marcadores activos compartan ese componente.
public class RotateColorAndBranchEffect : IEffect
{
    public RotateColorAndBranchEffect(IEffect[] ifRed, IEffect[] ifGreen, IEffect[] ifBlue)
    {
        IfRed = ifRed;
        IfGreen = ifGreen;
        IfBlue = ifBlue;
    }

    public IEffect[] IfRed { get; set; }
    public IEffect[] IfGreen { get; set; }
    public IEffect[] IfBlue { get; set; }

    public IEffect Clone() => new RotateColorAndBranchEffect(
        [.. IfRed.Select(e => e.Clone())],
        [.. IfGreen.Select(e => e.Clone())],
        [.. IfBlue.Select(e => e.Clone())]);

    public void Execute(Guid playerId, Guid rivalId, CardInstance cardId, GameState state, GameEvent? ev)
    {
        var player = state.GetState(playerId);




        var currents = ChromaticColorHelper.TryGetColors(player)
            .Where(c => c is not null)
            .Select(c => c!.Value);
        
        if(!currents.Any())
        {
            new RotateColorEffect().Execute(playerId, rivalId, cardId, state, ev);
        }

        var addedComponents = new HashSet<ChromaticColor>();
        List<IEffect> effects = new();
        foreach (var c in currents)
        {
            foreach (var component in ChromaticColorHelper.Components(c))
            {
                if (!addedComponents.Add(component)) continue;

                var branch = component switch
                {
                    ChromaticColor.Rojo => IfRed,
                    ChromaticColor.Verde => IfGreen,
                    ChromaticColor.Azul => IfBlue,
                    _ => null
                };

                if (branch is not null)
                    effects.AddRange(branch);
            }
        }

        foreach (var effect in effects)
            effect.Execute(playerId, rivalId, cardId, state, ev);
        
        if(effects.Any())
        {
            new RotateColorEffect().Execute(playerId, rivalId, cardId, state, ev);
            
        }
    }
}
