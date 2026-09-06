public enum TriggerType
{
    TurnEnd,
    DrawCard,
    PlayerHealthChanged,
    UnitHealthChanged,
    UnitDamageChanged,
    UnitPlayed,
    SpellPlayed,
    UnitDeath,
    CardAddedToDeck,
    DeckModified,
    CardAttacked,
    CardEffectPlayed,
    // Se dispara cada vez que se añade un EffectInstance a PlayerState.GlobalEffects
    // (de cualquier mazo). Ver GameState.AddGlobalEffect y GameEvent.GlobalEffectAdded.
    GlobalEffectAdded,
    // Nunca se dispara: usado por los marcadores inertes de ChromaticColorHelper.
    None

}