using Microsoft.Extensions.Logging.Abstractions;

public static class MockData
{
    public static List<CardDefinition> Cards =
    [
        new CardDefinition(
            "1",
            "Truco Ratero",
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
        "Rata", CardType.Unit, "HABILIDAD: Inflinge uno de daño a su propio jugador", 1, 1, [], ["Rata"], "", null, new AlterPlayerHealthEffect(-1, false), 1),
        new CardDefinition("3",
        "Familia de ratas", CardType.Unit, "Cuando muero, añado dos ratas al mazo del rival.", 2, 1, [
            new EffectInstance(
                TriggerType.UnitDeath,
                new AppendCardToDeck(2, "2", true   ),
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
                    
                ),
                new DurationByExecutions(1),
                new IHaveBeenPlayedCondition()
            )
        ], [], "", null, null, 0),
        new CardDefinition("7", "Queso", CardType.Spell, "Curo 3 de vida al usuario", -1, -1, [
            new EffectInstance(TriggerType.SpellPlayed, new AlterPlayerHealthEffect(3, false), new DurationByExecutions(1), new IHaveBeenPlayedCondition())
        ], [], "", null, null, -1),
        new CardDefinition("8", "Madriguera de ratas", CardType.Unit, "Fin de ronda: me inflinjo 2 de daño y añado 1 rata al mazo rival", 0, 6, [
            new EffectInstance(TriggerType.TurnEnd, new AppendCardToDeck(1, "2", true), new Always(), null),
            new EffectInstance(TriggerType.TurnEnd, new AlterMySelf(0, -2), new Always(), null)
        ],
        ["Rata"], "", null, null, 0),
        new CardDefinition("9", "Matarratas defectuoso", CardType.Spell, "+1/+1 a todas las ratas de la mesa y el mazo", -1, -1, [
            new EffectInstance(
                TriggerType.SpellPlayed,
                
            )
        ])
        
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
        },
        {
            new DeckDto(5,
            "Rata mágica",
            "¿Pero qué? ¿Quién ha dejado una rata entrar en la guerra? Si además lleva sombrero y todo, y parece que se ha traído a todos sus parientes. En fin, no seré yo quien la juzgue, pero no parece que trame nada bueno."),
            new()
            {
                { "1", 2},
                { "3", 3},
                { "4", 2},
                { "5", 3},
                { "6", 1},
            }
        }
        
          
    };

}