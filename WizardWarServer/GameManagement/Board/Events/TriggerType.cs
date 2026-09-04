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
    ColorChanged,
    // Nunca se dispara: usado por los marcadores inertes de ChromaticColorHelper.
    None

}