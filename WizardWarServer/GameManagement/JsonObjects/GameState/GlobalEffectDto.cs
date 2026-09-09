// Color is the active chromatic color's server-side ChromaticColor enum name
// (e.g. "Rojo") when this global effect is the player's color marker (see
// ChromaticColorHelper.SetColor), or null for every other global effect —
// lets the client color/highlight chromatic text without having to
// string-match translated descriptions.
public record GlobalEffectDto(Guid Id, string Text, string? Color)
{
    public static GlobalEffectDto Generate(EffectInstance effect, string language)
    {
        var color = effect.Effects.OfType<ColorMarkerEffect>().FirstOrDefault()?.Color;

        return new(effect.Id, TranslationManager.Get(effect.Description, language), color?.ToString());
    }
}
