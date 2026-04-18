public static class MockData
{
    public static Dictionary<DeckDto, List<CardDefinition>> data = new(){
        {
            new DeckDto(1,
            "Gran mago", 
            "Mago serio, mago confiable. Sus cartas son seguras, no se anda con tonterías. Nunca bebe en las fiestas porque sabe que le tocará conducir después."), [
            new CardDefinition("1", "Aprendiz de mago", CardType.Unit, "", 3, 3, []),
            new CardDefinition("1", "Aprendiz de mago", CardType.Unit, "", 3, 3, []),
            new CardDefinition("1", "Aprendiz de mago", CardType.Unit, "", 3, 3, []),
            new CardDefinition("1", "Aprendiz de mago", CardType.Unit, "", 3, 3, []),
            new CardDefinition("1", "Aprendiz de mago", CardType.Unit, "", 3, 3, []),
            new CardDefinition("3", "Mago consumado", CardType.Unit, "", 6, 6, []),
            new CardDefinition("2", "Truco gitano", CardType.Spell, "Roba dos cartas", -1, -1, [
                new EffectInstance(
                    TriggerType.SpellPlayed,
                    new DrawCardEffect(2),
                    new DurationByExecutions(false, 1, 0),
                    new IHaveBeenPlayedCondition()
                )
            ])
        ]},
        {
            new DeckDto(2,
            "Mago silly",
            "Señor y dueño de todas las tontunas. Rinde tu alma ante él y serás recompensado con la frustración de tus rivales."),
            [
                
            ]
        },
        {
            new DeckDto(3,
            "Don Bola de Fuego Jr",
            "Reduce a cenizas a quienes se enfrentan a él, y con suerte sus aliados escapan a su cólera. Eso sí: que nadie le pregunte qué le pasó a Don Bola de Fuego padre."),
            [
                
            ]
        },
        {
            new DeckDto(4,
            "???",
            "Nadie conoce realmente el origen de esta criatura, si es una o varias; pero es majo y tranquilo, así que se le hace un hueco. Es cuestión tuya si aceptar sus dudosos ofrecimientos o no."),
            [
                
            ]
        }
    };
}