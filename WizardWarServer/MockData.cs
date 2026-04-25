public static class MockData
{
    public static List<CardDefinition> Cards =
    [
        new CardDefinition(
            "1",
            "Truco Gitano",
            CardType.Spell,
            "Roba tres cartas",
            -1,
            -1,
            [
                new EffectInstance(
                    TriggerType.SpellPlayed,
                    new DrawCardEffect(3),
                    new DurationByExecutions(1),
                    new IHaveBeenPlayedCondition()
                )
            ],
            [],
            "",
            null,
            null,
            0
        ),
        new CardDefinition("2",
        "Rata", CardType.Unit, "", 1, 1, [], ["Rata"], "", null, null, 0),
        new CardDefinition("3",
        "Familia de ratas", CardType.Unit, "Cuando muero, añado dos ratas al mazo del rival.", 2, 1, [
            new EffectInstance(
                TriggerType.UnitDeath,
                new AppendRatas(2),
                new DurationByExecutions(1),
                new IHaveDiedCondition()
            ),
        ],
        ["Rata"], "", null, null, 0),
        new CardDefinition("4",
        "Flautista de Hamelin", CardType.Unit, "Cuando se juega una rata en mesa, consigo +1/+1", 2, 4, [
            new EffectInstance(
                TriggerType.UnitPlayed,
                new GrowStatsBasedOnCardPlayed("Rata", 1, 1, true),
                new Always(),
                new PlayerCardCondition(false)
            )
        ],
        [], "", null, null, 0),
        new CardDefinition("5",
        "Mr bombastic", CardType.Unit, "", 4, 4, [], ["Rata"], "/images/cards/1.jpg", null, null, 0),
        new CardDefinition("6", "Flow rata", CardType.Spell, "Curo al jugador uno  por cada rata que tenga el rival en el mazo.", -1, -1, [
            new EffectInstance(
                TriggerType.SpellPlayed,
                new DamagePlayerBasedOnCards(
                    ["Rata"],
                    true,
                    1,
                    PlayerType.RIVAL,
                    PlayerType.NONE,
                    true
                ),
                new DurationByExecutions(1),
                new IHaveBeenPlayedCondition()
            )
        ], [], "", null, null, 0)
        
    ];
    public static Dictionary<DeckDto, Dictionary<string, int>> Decks = new()
    {
        {
            new DeckDto(1,
            "Gran mago", 
            "Mago serio, mago confiable. Sus cartas son seguras, no se anda con tonterías. Nunca bebe en las fiestas porque sabe que le tocará conducir después."),
            new()
            {
                
            }
        },
        {
            new DeckDto(2,
            "Mago silly",
            "Señor y dueño de todas las tontunas. Rinde tu alma ante él y serás recompensado con la frustración de tus rivales."),
            new()
            {
                { "1", 2},
                { "3", 3},
                { "4", 2},
                { "5", 4},
                { "6", 1},
            }
        },
        {
            new DeckDto(3,
            "Don Bola de Fuego Jr",
            "Reduce a cenizas a quienes se enfrentan a él, y con suerte sus aliados escapan a su cólera. Eso sí: que nadie le pregunte qué le pasó a Don Bola de Fuego padre."),
            new()
            {
                
            }
        },
        {
            new DeckDto(4,
            "???",
            "Nadie conoce realmente el origen de esta criatura, si es una o varias; pero es majo y tranquilo, así que se le hace un hueco. Es cuestión tuya si aceptar sus dudosos ofrecimientos o no."),
            new()
            {
                
            }
        }
        
          
    };

}