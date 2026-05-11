
public static class MockData
{
    public static List<CardDefinition> Cards =
[
    new CardDefinition
    {
        Id = "1",
        Name = "Truco Ratero",
        Type = CardType.Spell,
        Description = "Roba tres cartas",
        Effects =
        [
            new EffectInstance(
                TriggerType.SpellPlayed,
                [new DrawCardEffect(){CardAmount = 3}],
                new DurationByExecutions(1),
                new IHaveBeenPlayedCondition()
            )
        ]
    },

    new CardDefinition
    {
        Id = "2",
        Name = "Rata",
        Description = "HABILIDAD: Inflinge uno de daño a su propio jugador",
        BaseAttack = 1,
        BaseHealth = 1,
        Families = ["Rata"],
        PlayEffect = new AlterPlayerHealthEffect(-1, false),
        PlayEffectTriggerTimes = 1
    },

    new CardDefinition
    {
        Id = "3",
        Name = "Familia de ratas",
        Description = "Cuando muero, añado dos ratas al mazo del rival.",
        BaseAttack = 2,
        BaseHealth = 1,
        Families = ["Rata"],
        Effects =
        [
            new EffectInstance(
                TriggerType.UnitDeath,
                [new AppendCardToDeck(2, "2", true)],
                new DurationByExecutions(1),
                new IHaveDiedCondition()
            )
        ]
    },

    new CardDefinition
    {
        Id = "4",
        Name = "Flautista de Hamelin",
        Description = "Cuando se juega una rata en la mesa rival, consigo +1/+1",
        BaseAttack = 2,
        BaseHealth = 4,
        Effects =
        [
            new EffectInstance(
                TriggerType.UnitPlayed,
                [
                    new AlterMySelf(1, 1,false)
                ],
                new Always(),
                new PlayerCardCondition(false, new() { CurrentFamilies = ["Rata"]})
            )
        ]
    },

    new CardDefinition
    {
        Id = "5",
        Name = "Mr bombastic",
        Description = "Solo se puede jugar en turnos pares",
        BaseAttack = 4,
        BaseHealth = 4,
        Families = ["Rata"],
        imageUrl = "/images/cards/1.jpg",
        ConditionToPlay = new EvenTurnCondition()
    },

    new CardDefinition
    {
        Id = "6",
        Name = "Flow rata",
        Type = CardType.Spell,
        Description = "Curo al jugador uno por cada rata que tenga el rival en el mazo.",
        Effects =
        [
            new EffectInstance(
                TriggerType.SpellPlayed,
                [
                    new DamagePlayerBasedOnCards(
                        true,
                        1,
                        PlayerType.PLAYER,
                        new GameFilter
                        {
                            Filter = new CardFilter { CurrentFamilies = ["Rata"] },
                            WhichDeckToSearch = PlayerType.RIVAL
                        }
                    )
                ],
                new DurationByExecutions(1),
                new IHaveBeenPlayedCondition()
            )
        ]
    },

    new CardDefinition
    {
        Id = "7",
        Name = "Queso",
        Type = CardType.Spell,
        Description = "Curo 3 de vida al usuario",
        Effects =
        [
            new EffectInstance(
                TriggerType.SpellPlayed,
                [new AlterPlayerHealthEffect(3, false)],
                new DurationByExecutions(1),
                new IHaveBeenPlayedCondition()
            )
        ],
    },

    new CardDefinition
    {
        Id = "8",
        Name = "Madriguera de ratas",
        Description = "Fin de ronda: me inflinjo 2 de daño y añado 1 rata al mazo rival",
        BaseAttack = 0,
        BaseHealth = 6,
        Families = ["Rata"],
        Effects =
        [
            new EffectInstance(
                TriggerType.TurnEnd,
                [new AppendCardToDeck(1, "2", true)],
                new Always(),
                null
            ),
            new EffectInstance(
                TriggerType.TurnEnd,
                [new AlterMySelf(0, -2, false)],
                new Always(),
                null
            )
        ]
    },

    new CardDefinition
    {
        Id = "9",
        Name = "Matarratas defectuoso",
        Type = CardType.Spell,
        Description = "+1/+1 a todas tus ratas de la mesa y el mazo",
        Effects =
        [
            new EffectInstance(
                TriggerType.SpellPlayed,
                [
                    new AlterUnitStatsEffect(
                        1, 1,
                        new GameFilter
                        {
                            Filter = new CardFilter { CurrentFamilies = ["Rata"] },
                            WhichBoardToSearch = PlayerType.PLAYER,
                            WhichDeckToSearch = PlayerType.PLAYER
                        }
                    )
                ],
                new DurationByExecutions(1),
                new IHaveBeenPlayedCondition()
            )
        ]
    },

    new CardDefinition
    {
        Id = "10",
        Name = "Primo de Remi",
        Description = "Cuando soy jugado, planto 3 Quesos en el mazo de mi jugador",
        BaseAttack = 2,
        BaseHealth = 3,
        Families = ["Rata"],
        Effects =
        [
            new EffectInstance(
                TriggerType.UnitPlayed,
                [new AppendCardToDeck(3, "7", false)],
                new DurationByExecutions(1),
                new IHaveBeenPlayedCondition()
            )
        ]
    },

    new CardDefinition
    {
        Id = "11",
        Name = "El poder de los sumideros",
        Type = CardType.Spell,
        Description = "Has tenido que jugar al menos 3 ratas...",
        Effects =
        [
            new EffectInstance(
                TriggerType.SpellPlayed,
                [
                    new AppendGlobalEffect(
                        new EffectInstance(
                            TriggerType.UnitPlayed,
                            [new AlterMySelf(1, 1, true)],
                            new Always(),
                            new PlayerCardCondition(true,
                                new CardFilter { CurrentFamilies = ["Rata"] })
                        ),
                        "+1/+1 Si la carta es una Rata"
                    )
                ],
                new DurationByExecutions(1),
                new IHaveBeenPlayedCondition()
            )
        ],
        ConditionToPlay = new CountPlayedCardsCondition(
            new CardFilter { CurrentFamilies = ["Rata"] },
            PlayerType.PLAYER,
            3,
            CountType.AT_LEAST
        )
    },

    new CardDefinition
    {
        Id = "12",
        Name = "Arañazos",
        Type = CardType.Spell,
        Description = "0/-1 a la mesa enemiga ahora y al final de este turno",
        Families = ["Rata"],
        Effects =
        [
            new EffectInstance(
                TriggerType.SpellPlayed,
                [
                    new AlterUnitStatsEffect(-1, 0,
                        new GameFilter
                        {
                            WhichBoardToSearch = PlayerType.RIVAL,
                            Filter = new CardFilter()
                        }),
                    new AppendGlobalEffect(
                        new EffectInstance(
                            TriggerType.TurnEnd,
                            [
                                new AlterUnitStatsEffect(-1, 0,
                                    new GameFilter
                                    {
                                        WhichBoardToSearch = PlayerType.RIVAL,
                                        Filter = new CardFilter()
                                    })
                            ],
                            new DurationByExecutions(1),
                            null
                        ),
                        "0/-1 en el próximo final de turno"
                    )
                ],
                new DurationByExecutions(1),
                new IHaveBeenPlayedCondition()
            )
        ]
    },

    new CardDefinition
    {
        Id = "13",
        Name = "Mordedura de rata",
        Type = CardType.Spell,
        Description = "0/-3 a un enemigo en la mesa (izq). HABILIDAD: Roba una carta",
        Families = ["Rata"],
        Effects =
        [
            new EffectInstance(
                TriggerType.SpellPlayed,
                [
                    new AlterUnitStatsEffect(-3, 0,
                        new GameFilter
                        {
                            WhichBoardToSearch = PlayerType.RIVAL,
                            MaxLength = 1,
                            Filter = new CardFilter()
                        })
                ],
                new DurationByExecutions(1),
                new IHaveBeenPlayedCondition()
            )
        ],
        PlayEffect = new DrawCardEffect(),
        PlayEffectTriggerTimes = 1
    },
    new()
    {
        Id = "13_1",
        Name = "Exterminador de plagas",
        BaseAttack = 1,
        BaseHealth = 2,
        Description = "Cuando el jugador rival roba una carta, y es una rata, le inflinjo 1 de daño",
        Type = CardType.Unit,
        Effects = [
            new(
                TriggerType.DrawCard,
                [
                    new AlterPlayerHealthEffect(-1, true)
                ],
                new Always(),
                new PlayerCardCondition(false, new() { CurrentFamilies = ["Rata"]})
            )
        ]  
    },
        new CardDefinition
    {
        Id = "14",
        Name = "Libro de Caballería",
        Type = CardType.Spell,
        Description = "Necesitas al menos 1 caballero en mesa. Roba una carta y cura 2 de vida al jugador",
        Families = ["Libro"],
        Effects =
        [
            new EffectInstance(
                TriggerType.SpellPlayed,
                [new DrawCardEffect(){CardAmount = 1}, new AlterPlayerHealthEffect(2, false)],
                new DurationByExecutions(1),
                new IHaveBeenPlayedCondition()
            )
        ],
        ConditionToPlay = 
            new CountCardCondition(
                new GameFilter()
                {
                    WhichBoardToSearch = PlayerType.PLAYER,
                    Filter = new()
                    {
                        CurrentFamilies = ["Caballero"]
                    }
                },
                1,
                CountType.AT_LEAST
            )
    },

    new CardDefinition
    {
        Id = "15",
        Name = "La venta",
        Description = "Cuando se juega una unidad, se le otorga +2/-1",
        BaseAttack = 2,
        BaseHealth = 5,
        Effects =
        [
            new EffectInstance(
                TriggerType.UnitPlayed,
                [new AlterMySelf(2, -1, true)],
                new Always(),
                new PlayerCardCondition(true, null)
            )
        ]
    },

    new CardDefinition
    {
        Id = "16",
        Name = "Molino de viento",
        BaseAttack = 3,
        BaseHealth = 3,
        Families = ["Paranoia"]
    },

    new CardDefinition
    {
        Id = "17",
        Name = "Rebaño de corderos",
        BaseAttack = 2,
        BaseHealth = 2,
        Families = ["Paranoia"]
    },

    new CardDefinition
    {
        Id = "18",
        Name = "Dulcinea del Toboso",
        Description = "Final de ronda: +2/+3 a Don Quijote, si se encuentra en mesa",
        BaseAttack = 3,
        BaseHealth = 6,
        Families = ["Paranoia", "Castellano"],
        Effects =
        [
            new EffectInstance(
                TriggerType.TurnEnd,
                [
                    new AlterUnitStatsEffect(3, 2,
                        new GameFilter
                        {
                            WhichBoardToSearch = PlayerType.PLAYER,
                            Filter = new CardFilter { DefinitionId = "19" }
                        })
                ],
                new Always(),
                null
            )
        ]
    },

    new CardDefinition
    {
        Id = "19",
        Name = "El Ingenioso Hidalgo Don Quijote de la Mancha",
        Description = "Has jugado al menos 5 libros de caballería o una Dulcinea del Toboso. Las Paranoias ganan +4/+4. Cuando se juega un libro de caballería, curo otros 2 de vida a mi jugador",
        BaseAttack = 8,
        BaseHealth = 8,
        Families = ["Caballero", "Castellano"],
        Effects =
        [
            new EffectInstance(
                TriggerType.UnitPlayed,
                [
                    new AlterUnitStatsEffect(4, 4,
                        new GameFilter
                        {
                            WhichBoardToSearch = PlayerType.PLAYER,
                            WhichDeckToSearch = PlayerType.PLAYER,
                            Filter = new CardFilter { CurrentFamilies = ["Paranoia"] }
                        })
                ],
                new DurationByExecutions(1),
                new IHaveBeenPlayedCondition()
            ),
            new EffectInstance(
                TriggerType.SpellPlayed,
                [new AlterPlayerHealthEffect(2, false)],
                new Always(),
                new FilterPlayerCardCondition(
                    new CardFilter { DefinitionId = "14" }
                )
            )
        ],
        ConditionToPlay = new MultiEffectCondition(
            [
                new CountPlayedCardsCondition(
                    new CardFilter { DefinitionId = "14" },
                    PlayerType.PLAYER,
                    5,
                    CountType.AT_LEAST
                ),
                new CountPlayedCardsCondition(
                    new CardFilter { DefinitionId = "18" },
                    PlayerType.PLAYER,
                    1,
                    CountType.AT_LEAST
                )
            ],
            true
        )
    },

    new CardDefinition
    {
        Id = "20",
        Name = "Locuras de Hidalgo",
        Type = CardType.Spell,
        Description = "Añade una paranoia de cada tipo en el mazo",
        Effects =
        [
            new EffectInstance(
                TriggerType.SpellPlayed,
                [
                    new AppendCardToDeck(1, "16", false),
                    new AppendCardToDeck(1, "17", false),
                    new AppendCardToDeck(1, "18", false)
                ],
                new DurationByExecutions(1),
                null
            )
        ]
    },

    new CardDefinition
    {
        Id = "21",
        Name = "Rocinante",
        BaseAttack = 3,
        BaseHealth = 3,
        Families = ["Caballo"]
    },

    new CardDefinition
    {
        Id = "22",
        Name = "How Hungry",
        Type = CardType.Spell,
        Description = "Si hay algún caballo en tu mesa o más, los mato y el jugador se cura el total de la vida de todos",
        Effects =
        [
            new EffectInstance(
                TriggerType.SpellPlayed,
                [
                    new AlterPlayerBasedOnCardStats(
                        PlayerType.PLAYER,
                        new CardFilter { CurrentFamilies = ["Caballo"] },
                        AffectedStats.HEALTH,
                        PlayerType.PLAYER,
                        1
                    )
                ],
                new DurationByExecutions(1),
                new IHaveBeenPlayedCondition()
            )
        ],
        ConditionToPlay = new CountCardCondition(
            new GameFilter
            {
                WhichBoardToSearch = PlayerType.PLAYER,
                Filter = new CardFilter { CurrentFamilies = ["Caballo"] }
            },
            1,
            CountType.AT_LEAST
        )
    },
    new CardDefinition()
    {
        Id = "23",
        Type = CardType.Unit,
        Name = "Honoroso Caballero",
        Families = ["Caballero"],
        BaseAttack = 2,
        BaseHealth = 3
    },
    new CardDefinition()
    {
        Id = "24",
        Name = "Cura Pero Pérez",
        Type = CardType.Unit,
        BaseAttack = 1,
        BaseHealth = 4,
        Description = "Fin de ronda: Creo un Libro de Caballería y lo añado en el mazo",
        Effects = [
            new EffectInstance(
                TriggerType.TurnEnd,
                [
                    new AppendCardToDeck(1, "14", false)
                ],
                new Always(),
                null
            )
        ]
    },
    new CardDefinition()
    {
        Id = "25",
        Name = "El Loco de Sierra Morena",
        Type = CardType.Unit,
        BaseAttack = 4,
        BaseHealth = 5,
        Families = ["Caballero"],
        Description = "Si entra Don Quijote o Sancho Panza en el juego, le inflinjo 4 de daño",
        Effects = [
            new EffectInstance(
                TriggerType.UnitPlayed,
                [
                    new AlterMySelf(0, -4, true)
                ],
                new Always(),
                new MultiEffectCondition(
                    [
                        new PlayerCardCondition(true, new() { DefinitionId = "26" }),
                        new PlayerCardCondition(true, new() { DefinitionId = "19" }),
                    ],
                    true
                )
            )
        ]
    },
    new CardDefinition()
    {
        Id = "26",
        Name = "Sancho Panza",
        Type = CardType.Unit,
        BaseAttack = 1,
        BaseHealth = 6,
        Families = ["Castellano"],
        Description = "Fin de Ronda: Si no hay otros Castellanos en juego, 0/-1 a las cartas en el campo. HABILIDAD: +2/+2 a las cartas de la mesa",
        Effects = [
            new EffectInstance(
                TriggerType.TurnEnd,
                [
                    new AlterUnitStatsEffect(
                        -1, 0, new(){
                            WhichBoardToSearch = PlayerType.PLAYER,
                            Filter = new()
                        }
                    )
                ],
                new Always(),
                new CountCardCondition(
                    new()
                    {
                        WhichBoardToSearch = PlayerType.PLAYER,
                        Filter = new() {CurrentFamilies = ["Castellano"]}
                    },
                    1,
                    CountType.AT_MAX
                )
            )
        ],
        PlayEffect = new AlterUnitStatsEffect(2, 2, new(){WhichBoardToSearch = PlayerType.PLAYER, Filter = new()}),
        PlayEffectTriggerTimes = 2
    },
    new()
    {
        Id = "26_1",
        Name = "Llamada de auxilio",
        Type = CardType.Spell,
        Description = "Roba dos caballeros",
        Effects = [
            new(TriggerType.SpellPlayed, [new DrawCardEffect(2, new(){CurrentFamilies = ["Caballero"]})], new DurationByExecutions(1), null)
        ]
    },
    new()
    {
        Id = "27",
        Name = "Bola de fuego",
        Families = ["Ceniza"],
        Type = CardType.Spell,
        Description = "0/-4 A un enemigo (izq)",
        Effects = [
            new()
            {
                Trigger = TriggerType.SpellPlayed,
                Effects = [
                    new AlterUnitStatsEffect(-4, 0, new(){
                        WhichBoardToSearch = PlayerType.RIVAL,
                        MaxLength = 1,
                        Filter = new()
                    })
                ],
                Condition = new IHaveBeenPlayedCondition(),
                Duration = new DurationByExecutions(1)
            }
        ]
    },
    new()
    {
        Id = "28",
        Name = "Aprendiz de mago",
        Families = ["Brujo"],
        Type = CardType.Unit,
        BaseAttack = 3,
        BaseHealth = 3,
        Description = "Cuando me juegas, creo una bola de fuego",
        Effects = [
            new()
            {
                Trigger = TriggerType.UnitPlayed,
                Effects = [
                    new AppendCardToDeck(1, "27", false)
                ],
                Condition = new IHaveBeenPlayedCondition(),
                Duration = new DurationByExecutions(1)
            }
        ]
    },
    new()
    {
        Id = "29",
        Name = "Maestro de la ceniza",
        Type = CardType.Unit,
        Families = ["Brujo", "Ceniza"],
        BaseAttack = 6,
        BaseHealth = 7,
        Description = "Has jugado 6 bolas de fuego o más. Fin de Ronda: creo una bola de fuego en el mazo",
        Effects = [
            new()
            {
                Trigger = TriggerType.TurnEnd,
                Effects = [
                    new AppendCardToDeck(1, "27", false)
                ],
                Duration = new Always(),
            }
        ],
        ConditionToPlay = new CountPlayedCardsCondition(
            new() {DefinitionId = "27"},
            PlayerType.PLAYER,
            6,
            CountType.AT_LEAST
        )
    },
    new()
    {
        Id = "30",
        Name = "Espíritu del fuego",
        BaseAttack = 2,
        BaseHealth = 4,
        Description = "Cuando juegas una bola de fuego, inflinjo 1 de daño al rival",
        Type = CardType.Unit,
        Families = ["Ceniza"],
        Effects = [
            new(
                TriggerType.SpellPlayed,
                [new AlterPlayerHealthEffect(-1, true)],
                new Always(),
                new PlayerCardCondition(true, new() { DefinitionId = "27"})
            )
        ]
    },
    new()
    {
        Id = "31",
        Name = "Tierras yermas",
        Families = ["Destrucción"],
        Type = CardType.Spell,
        Description = "-2/-2 a la mesa enemiga",
        Effects = [
            new(TriggerType.SpellPlayed, [new AlterUnitStatsEffect(-2, -2, new() {
                WhichBoardToSearch = PlayerType.RIVAL,
                Filter = new()
            })], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
        ]
    },
    new()
    {
        Id = "32",
        Name = "Conjuro del dios del fuego",
        Type = CardType.Spell,
        Description = "Roba dos hechizos",
        Effects = [
            new(TriggerType.SpellPlayed, [new DrawCardEffect(2, new() {CardType = CardType.Spell})], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
        ]
    },
    new()
    {
        Id = "33",
        Name = "La llamada de la ceniza",
        Type = CardType.Spell,
        Description = "Roba una carta ceniza",
        Effects = [
            new(TriggerType.SpellPlayed, [new DrawCardEffect(1, new() {CurrentFamilies = ["Ceniza"]})], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
        ]
    },
    new()
    {
        Id = "34",
        Name = "La ira del dios del fuego",
        Families = ["Destrucción"],
        Type = CardType.Spell,
        Description = "MATA a TODAS las unidades en mesa",
        Effects = [
            new(TriggerType.SpellPlayed, [new KillCards(new(), PlayerType.BOTH)], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
        ]
    },
    new()
    {
        Id = "35",
        Name = "Chispas escurridizas",
        Families = ["Destrucción", "Maligno"],
        Type = CardType.Unit,
        BaseAttack = 1,
        BaseHealth = 1,
        Description = "El próximo hechizo que juegues, curo 1 y quito 1 al rival",
        Effects = [
            new EffectInstance(
                TriggerType.UnitPlayed,
                [
                    new AppendGlobalEffect(
                        new(
                            TriggerType.SpellPlayed,
                            [
                                new AlterPlayerHealthEffect(1, false),
                                new AlterPlayerHealthEffect(-1, true),
                            ],
                            new DurationByExecutions(1),
                            new PlayerCardCondition(true, null)
                        ),
                        "El próximo hechizo que juegue este jugador, consigue 1 de vida y el rival -1"
                    )
                ],
                new DurationByExecutions(1),
                null)
        ]
    },
    new ()
    {
        Id = "36",
        Name = "Instructor de las llamas",
        Families = ["Brujo"],
        Type = CardType.Unit,
        Description = "Las próximas dos unidades que juegues consiguen +1/+1",
        BaseAttack = 2,
        BaseHealth = 2,
        Effects = [
            new(
                TriggerType.UnitPlayed,
                [
                    new AppendGlobalEffect(
                        new(
                            TriggerType.UnitPlayed,
                            [
                                new AlterMySelf(1, 1, true)
                            ],
                            new DurationByExecutions(2),
                            new PlayerCardCondition(true, null)
                        ),
                        "Las próximas dos unidades que juegues consiguen +1/+1"
                    )
                ],
                new DurationByExecutions(1),
                new IHaveBeenPlayedCondition()
            )
            
        ]
    },
    new()
    {
        Id = "37",
        Type = CardType.Spell,
        Name = "Comunión Espiritual",
        Description = "Has jugado una llamada de la ceniza. Las unidades en mesa consiguen +1/+1.",
        Effects = [
            new(
                TriggerType.SpellPlayed,
                [
                    new AlterUnitStatsEffect(1, 1, new() {Filter = new(), WhichBoardToSearch = PlayerType.PLAYER})
                ],
                new DurationByExecutions(1),
                null
            )
        ],
        ConditionToPlay = new CountPlayedCardsCondition(new() {DefinitionId = "33"}, PlayerType.PLAYER, 1, CountType.AT_LEAST)
    },
    new()
    {
        Id = "38",
        Name = "Gran negador",
        Families = ["Maligno"],
        Description = "+3/+3 si no has jugado un hechizo antes",
        Type = CardType.Unit,
        BaseAttack = 1,
        BaseHealth = 1,
        Effects = [
            new
            (
                TriggerType.UnitPlayed,
                [new AlterMySelf(3, 3, false)],
                new DurationByExecutions(1),
                new CountPlayedCardsCondition(new() { CardType = CardType.Spell}, PlayerType.PLAYER, 0, CountType.AT_MAX)
            )
        ]
    },
    new()
    {
        Id = "39",
        Name = "Maquiavelo el maquiavélico",
        Families = ["Brujo", "Maligno"],
        Description = "Cuando juegas un hechizo, consigo +1/+1",
        BaseHealth = 1,
        BaseAttack = 1,
        Type = CardType.Unit,
        Effects = [
            new(
                TriggerType.SpellPlayed,
                [ new AlterMySelf(1, 1, false)],
                new Always(),
                new PlayerCardCondition(true, null)
            )
        ]
    }

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
        {
            new DeckDto(7,
            "Don Bola de Fuego Jr",
            "Reduce a cenizas a quienes se enfrentan a él, y con suerte sus aliados escapan a su cólera. Eso sí: que nadie le pregunte qué le pasó a Don Bola de Fuego padre."),
            new()
            {
                //34
                {"27", 4},
                {"28", 3},
                {"29", 1},
                {"30", 3},
                {"31", 4},
                {"32", 3},
                {"33", 2},
                {"34", 1},
                {"35", 4},
                {"36", 2},
                {"37", 2},
                {"38", 2},
                {"39", 3},
            }
        },
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
            "El mago del queso",
            "¿Pero qué? ¿Quién ha dejado una rata entrar en la guerra? Si además lleva sombrero y todo, y parece que se ha traído a todos sus parientes. En fin, no seré yo quien la juzgue, pero no parece que trame nada bueno."),
            new()
            {
                //32
                { "1", 2},
                { "3", 3},
                { "4", 2},
                { "5", 3},
                { "6", 2},
                { "7", 2},
                { "8", 3},
                { "9", 2},
                { "10", 3},
                { "11", 2},
                { "12", 3},
                { "13", 2},
                { "13_1", 3},
            }
        },
        {
            new DeckDto(6,
                "El Ingenioso Hidalgo Don Quijote de la Mancha",
                "Primero una rata, y ahora este tío. Habla de un modo muy extraño, nadie le entiende y... parece que se está pegando con unos molinos. No para de hablar de enemigos y de una tal Dulcinea; menos mal que esos monstruos no son reales, ¿Verdad?"
            ),
            new()
            {
                //33
                { "14", 5},
                { "15", 3},
                { "16", 3},
                { "26_1", 1},
                { "17", 3},
                { "18", 1},
                { "19", 1},
                { "20", 2},
                { "21", 3},
                { "22", 1},
                { "23", 4},
                { "24", 2},
                { "25", 2},
                { "26", 2},
            }
        }
        
          
    };

}