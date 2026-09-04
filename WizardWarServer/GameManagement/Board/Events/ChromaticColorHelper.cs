// Colores mixtos formados por sus dos componentes básicos (mezcla aditiva RGB):
// Amarillo = Rojo+Verde, Celeste = Verde+Azul, Morado = Azul+Rojo, Blanco = los tres.
// El color "activo" de un jugador se guarda como un único EffectInstance permanente
// en PlayerState.GlobalEffects cuyo Effects contiene un ColorMarkerEffect (marcador
// inerte, Trigger = TriggerType.None para que nunca se dispare por sí mismo).
public static class ChromaticColorHelper
{
    public static ChromaticColor? TryGetSingleColor(PlayerState player)
    {
        return player.GlobalEffects
            .SelectMany(e => e.Effects)
            .OfType<ColorMarkerEffect>()
            .Select(e => (ChromaticColor?)e.Color)
            .FirstOrDefault();
    }

    static bool IsColorMarkerInstance(EffectInstance e) => e.Effects.Any(f => f is ColorMarkerEffect);

    public static void SetColor(GameState state, CardInstance sourceCard, PlayerState player, ChromaticColor? oldColor, ChromaticColor newColor)
    {
        player.GlobalEffects.RemoveAll(IsColorMarkerInstance);

        var marker = new EffectInstance(TriggerType.None, [new ColorMarkerEffect(newColor)], new Always(), null)
        {
            Player = player,
            SourceCard = sourceCard
        };
        player.GlobalEffects.Add(marker);

        state.SetPlayerColor(sourceCard, player, oldColor, newColor);
    }

    public static bool IsBase(ChromaticColor c) => c is ChromaticColor.Rojo or ChromaticColor.Verde or ChromaticColor.Azul;
    public static bool IsMixed(ChromaticColor c) => c is ChromaticColor.Amarillo or ChromaticColor.Celeste or ChromaticColor.Morado;

    public static ChromaticColor NextBase(ChromaticColor c) => c switch
    {
        ChromaticColor.Rojo => ChromaticColor.Verde,
        ChromaticColor.Verde => ChromaticColor.Azul,
        ChromaticColor.Azul => ChromaticColor.Rojo,
        _ => throw new ArgumentOutOfRangeException(nameof(c))
    };

    public static ChromaticColor MixOf(ChromaticColor baseColor) => baseColor switch
    {
        ChromaticColor.Rojo => ChromaticColor.Amarillo,
        ChromaticColor.Verde => ChromaticColor.Celeste,
        ChromaticColor.Azul => ChromaticColor.Morado,
        _ => throw new ArgumentOutOfRangeException(nameof(baseColor))
    };

    public static HashSet<ChromaticColor> Components(ChromaticColor color) => color switch
    {
        ChromaticColor.Rojo => [ChromaticColor.Rojo],
        ChromaticColor.Verde => [ChromaticColor.Verde],
        ChromaticColor.Azul => [ChromaticColor.Azul],
        ChromaticColor.Amarillo => [ChromaticColor.Rojo, ChromaticColor.Verde],
        ChromaticColor.Celeste => [ChromaticColor.Verde, ChromaticColor.Azul],
        ChromaticColor.Morado => [ChromaticColor.Azul, ChromaticColor.Rojo],
        ChromaticColor.Blanco => [ChromaticColor.Rojo, ChromaticColor.Verde, ChromaticColor.Azul],
        _ => []
    };

    public static HashSet<ChromaticColor> GetActiveBaseComponents(PlayerState player)
    {
        var current = TryGetSingleColor(player);
        return current is null ? [] : Components(current.Value);
    }
}
