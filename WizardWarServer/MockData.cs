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
        "Flautista de Hamelin", CardType.Unit, "Cuando se juega una rata en la mesa rival, consigo +1/+1", 2, 4, [
            new EffectInstance(
                TriggerType.UnitPlayed,
                new GrowStatsBasedOnCardPlayed("Rata", 1, 1, true),
                new Always(),
                new PlayerCardCondition(false, null)
            )
        ],
        [], "", null, null, 0),
        new CardDefinition("5",
        "Mr bombastic", CardType.Unit, "Solo se puede jugar en turnos pares", 4, 4, [], ["Rata"], "/images/cards/1.jpg", new EvenTurnCondition(), null, 0),
        new CardDefinition("6", "Flow rata", CardType.Spell, "Curo al jugador uno  por cada rata que tenga el rival en el mazo.", -1, -1, [
            new EffectInstance(
                TriggerType.SpellPlayed,
                new DamagePlayerBasedOnCards(
                    true,
                    1,
                    PlayerType.PLAYER,
                    new GameFilter()
                    {
                        Filter = new CardFilter()
                        {
                            CurrentFamilies = ["Rata"]
                        },
                        WhichDeckToSearch = PlayerType.RIVAL
                    }
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
            new EffectInstance(TriggerType.TurnEnd, new AlterMySelf(0, -2, false), new Always(), null)
        ],
        ["Rata"], "", null, null, 0),
        new CardDefinition("9", "Matarratas defectuoso", CardType.Spell, "+1/+1 a todas tus ratas de la mesa y el mazo", -1, -1, [
            new EffectInstance(
                TriggerType.SpellPlayed,
                new AlterUnitStatsEffect(1, 1, 
                    new GameFilter()
                    {
                        Filter = new CardFilter()
                        {
                            CurrentFamilies = ["Rata"]
                        },
                        WhichBoardToSearch = PlayerType.PLAYER,
                        WhichDeckToSearch = PlayerType.PLAYER
                    }
                ),
                new DurationByExecutions(1),
                null
            )
        ], [], "", null, null, 0),
        new CardDefinition("10", "Primo de Remi", CardType.Unit, "Cuando soy jugado, planto 3 Quesos en el mazo de mi jugador", 2, 3, [
            new EffectInstance(
                TriggerType.UnitPlayed,
                new AppendCardToDeck(3, "7", false),
                new DurationByExecutions(1),
                new IHaveBeenPlayedCondition()
            ),
        ], [ "Rata" ], "", null, null, 0),
        new CardDefinition("11", "El poder de los sumideros", CardType.Spell, "Has tenido que jugar al menos 3 ratas para poder jugarme. A partir de ahora, cada vez que juegues una rata consigue +1/+1", -1, -1, [
            new EffectInstance(
                TriggerType.SpellPlayed,
                new AppendGlobalEffect(
                    new EffectInstance(
                        TriggerType.UnitPlayed,
                        new AlterMySelf(1, 1, true),
                        new Always(),
                        new PlayerCardCondition(true, new CardFilter()
                        {
                            CurrentFamilies = ["Rata"]
                        })
                    )
                ),
                new DurationByExecutions(1),
                new IHaveBeenPlayedCondition()
            )
        ], [], "", new CountPlayedCardsCondition(
                    new CardFilter()
                    {
                        CurrentFamilies = ["Rata"]
                    },
                    PlayerType.PLAYER,
                    3,
                    CountType.AT_LEAST
                ), null,0),
        new CardDefinition("12", "Arañazos", CardType.Spell, "0/-1 a la mesa enemiga ahora, y al final de este turno", -1, -1, [
            new EffectInstance(
                TriggerType.SpellPlayed,
                new AlterUnitStatsEffect(-1, 0, 
                    new GameFilter()
                    {
                        WhichBoardToSearch = PlayerType.RIVAL,
                        Filter = new CardFilter()
                    }
                ),
                new DurationByExecutions(1),
                new IHaveBeenPlayedCondition()
            ),
            new EffectInstance(
                TriggerType.TurnEnd,
                new AlterUnitStatsEffect(-1, 0, 
                    new GameFilter()
                    {
                        WhichBoardToSearch = PlayerType.RIVAL,
                        Filter = new CardFilter()
                    }
                ),
                new DurationByExecutions(1),
                new IHaveBeenPlayedCondition()
            )
        ], [ "Rata" ], "", null, null, 0),
        new CardDefinition("13", "Mordedura de rata", CardType.Spell, "0/-3 a un enemigo en la mesa (izq). HABILIDAD: Roba una carta", -1, -1, [
            new EffectInstance(
                TriggerType.SpellPlayed,
                new AlterUnitStatsEffect(
                    -3, 0, new GameFilter()
                    {
                        WhichBoardToSearch = PlayerType.RIVAL,
                        MaxLength = 1,
                        Filter = new CardFilter()
                    }
                ),
                new DurationByExecutions(1),
                new IHaveBeenPlayedCondition()
            )
        ], [ "Rata" ], "", null, new DrawCardEffect(), 1),


        new CardDefinition("14", "Libro de Caballería", CardType.Spell, "Roba una carta y cura 2 de vida al jugador", -1, -1, [
            new EffectInstance(TriggerType.SpellPlayed, new DrawCardEffect(), new DurationByExecutions(1),new IHaveBeenPlayedCondition()),
            new EffectInstance(TriggerType.SpellPlayed, new AlterPlayerHealthEffect(2, false), new DurationByExecutions(1), new IHaveBeenPlayedCondition())
        ], ["Libro"], "", null, null, 0),
        new CardDefinition("15", "La venta", CardType.Unit, "Cuando se juega una unidad, se le otorga +2/-1", 2, 5, [
            new EffectInstance(
                TriggerType.UnitPlayed, new AlterMySelf(2, -1, true), new Always(), new PlayerCardCondition(true, null)
            )
        ], [], "", null, null, 0),
        new CardDefinition("16", "Molino de viento", CardType.Unit, "", 3, 3, [], ["Paranoia"], "", null, null, 0),
        new CardDefinition("17", "Rebaño de corderos", CardType.Unit, "", 2, 2, [], ["Paranoia"], "", null, null, 0),
        new CardDefinition("18", "Dulcinea del Toboso", CardType.Unit, "Final de ronda: +2/+3 a Don Quijote, si se encuentra en mesa", 3, 6, [
            new EffectInstance(TriggerType.TurnEnd, new AlterUnitStatsEffect(3, 2, new()
            {
                WhichBoardToSearch = PlayerType.PLAYER,
                Filter = new() {DefinitionId = "19"}
            }), new Always(), null)
        ], ["Paranoia"], "", null, null, 0),
        new CardDefinition("19", "El Ingenioso Hidalgo Don Quijote de la Mancha", CardType.Unit, "Has jugado al menos 5 libros de caballería. Las Paranoias ganan +4/+4. Cuando se juega un libro de caballería, curo otros 2 de vida a mi jugador", 8, 8, [
            new EffectInstance(TriggerType.UnitPlayed, new AlterUnitStatsEffect(4, 4, 
                new GameFilter()
                {
                    WhichBoardToSearch = PlayerType.PLAYER,
                    WhichDeckToSearch = PlayerType.PLAYER,
                    Filter = new CardFilter()
                    {
                        CurrentFamilies = ["Paranoia"]
                    }
                }
            ), new DurationByExecutions(1), new IHaveBeenPlayedCondition()),
            new EffectInstance(
                TriggerType.SpellPlayed,
                new AlterPlayerHealthEffect(2, false), new Always(), new FilterPlayerCardCondition(new ()
                {
                    DefinitionId = "14"
                })
            )
        ], ["Caballero"], "", new CountPlayedCardsCondition(new() {DefinitionId = "14"}, PlayerType.PLAYER, 5 ,CountType.AT_LEAST), null, 0)
        
    ];
    public static Dictionary<DeckDto, Dictionary<string, int>> Decks = new()
    {
        // {
        //     new DeckDto(1,
        //     "Gran mago", 
        //     "Mago serio, mago confiable. Sus cartas son seguras, no se anda con tonterías. Nunca bebe en las fiestas porque sabe que le tocará conducir después."),
        //     new()
        //     {
                
        //     }
        // },
        // {
        //     new DeckDto(2,
        //     "Mago silly",
        //     "Señor y dueño de todas las tontunas. Rinde tu alma ante él y serás recompensado con la frustración de tus rivales."),
        //     new()
        //     {
                
        //     }
        // },
        // {
        //     new DeckDto(3,
        //     "Don Bola de Fuego Jr",
        //     "Reduce a cenizas a quienes se enfrentan a él, y con suerte sus aliados escapan a su cólera. Eso sí: que nadie le pregunte qué le pasó a Don Bola de Fuego padre."),
        //     new()
        //     {
                
        //     }
        // },
        // {
        //     new DeckDto(4,
        //     "???",
        //     "Nadie conoce realmente el origen de esta criatura, si es una o varias; pero es majo y tranquilo, así que se le hace un hueco. Es cuestión tuya si aceptar sus dudosos ofrecimientos o no."),
        //     new()
        //     {
                
        //     }
        // },
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
                { "7", 1},
                { "8", 3},
                { "9", 2},
                { "10", 3},
                { "11", 2},
                { "12", 3},
                { "13", 2},
            }
        }
        
          
    };

}