
public static class MockData
{
    public static void PrintData()
    {
        foreach(var c in Cards)
        {
            Console.WriteLine($"[CARD] Id: {c.Id}, Name: {c.Name}, Description: {c.Description}");
        }
    }
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
        PlayEffects = [new AlterPlayerHealthEffect(-1, false)],
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
                new IHaveBeenPlayedCondition()
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
        Name = "Mind Máster",
        Description = "HABILIDAD: Activa todas las habilidades de la mesa enemiga",
        BaseAttack = 1,
        BaseHealth = 1,
        Type = CardType.Unit,
        Families = ["Rata"],
        Effects = [
            new(TriggerType.UnitPlayed, [
                new TriggerAbilityEffect(PlayerType.RIVAL, null)
            ]
            ,new DurationByExecutions(1), new IHaveBeenPlayedCondition())
        ]
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
        Description = "Has tenido que jugar al menos 3 ratas. A partir de ahora, las cartas Rata que juegues ganan +1/+1",
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
            new(CountType.AT_LEAST, 3)
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
        PlayEffects = [new DrawCardEffect()],
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
        Description = "Roba una carta y cura 2 de vida al jugador",
        Families = ["Libro"],
        Effects =
        [
            new EffectInstance(
                TriggerType.SpellPlayed,
                [new DrawCardEffect(){CardAmount = 1}, new AlterPlayerHealthEffect(2, false)],
                new DurationByExecutions(1),
                new IHaveBeenPlayedCondition()
            )
        ]
    },

    new CardDefinition
    {
        Id = "15",
        Name = "La venta",
        Description = "Cuando se juega una unidad, se le otorga +2/-1",
        BaseAttack = 1,
        BaseHealth = 4,
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
        BaseHealth = 1,
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
        BaseAttack = 5,
        BaseHealth = 5,
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
                    new(CountType.AT_LEAST, 5)
                ),
                new CountPlayedCardsCondition(
                    new CardFilter { DefinitionId = "18" },
                    PlayerType.PLAYER,
                    new(CountType.AT_LEAST, 1)
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
        Families = ["Caballo"],
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
            new(CountType.AT_LEAST, 1)
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
        BaseAttack = 3,
        BaseHealth = 4,
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
        BaseHealth = 5,
        Families = ["Castellano"],
        Description = "Fin de Ronda: Si no hay otros Castellanos en juego, 0/-1 a las cartas en el campo. HABILIDAD: +1/+1 a las cartas de la mesa",
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
                    new(CountType.AT_MAX, 1)
                )
            )
        ],
        PlayEffects = [new AlterUnitStatsEffect(1, 1, new(){WhichBoardToSearch = PlayerType.PLAYER, Filter = new()})],
        PlayEffectTriggerTimes = 1
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
            new(CountType.AT_LEAST, 6)
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
        Description = "-2/-2 a la mesa enemiga. El jugador pierde 2 de vida",
        Effects = [
            new(TriggerType.SpellPlayed, 
                [new AlterUnitStatsEffect(-2, -2, new() {
                WhichBoardToSearch = PlayerType.RIVAL,
                Filter = new()
            }), new AlterPlayerHealthEffect(-2, false)], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
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
        Description = "Roba una carta ceniza. Curo 2 de vida al jugador.",
        Effects = [
            new(TriggerType.SpellPlayed, [new DrawCardEffect(1, new() {CurrentFamilies = ["Ceniza"]}), new AlterPlayerHealthEffect(2, false)], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
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
            new(TriggerType.SpellPlayed, [new KillCards(new(), PlayerType.BOTH, 8)], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
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
        ConditionToPlay = new CountPlayedCardsCondition(new() {DefinitionId = "33"}, PlayerType.PLAYER, new(CountType.AT_LEAST, 1))
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
                new CountPlayedCardsCondition(new() { CardType = CardType.Spell}, PlayerType.PLAYER, new(CountType.AT_MAX, 0))
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
    },


    new()
    {
        Id = "40",
        Name = "GymRat",
        Families = ["Gym"],
        Type = CardType.Unit,
        BaseAttack = 1,
        BaseHealth = 2,
        Description = "Cuando muero, creo un batido de proteínas en el mazo",
        Effects = [
            new(TriggerType.UnitDeath, [new AppendCardToDeck(1, "42", false)], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
        ]
    },

    new()
    {
        Id = "41",
        Name = "Día de pierna",
        Families = ["Entrenamiento"],
        Type = CardType.Spell,
        Description = "0/+2 a la mesa. Las cartas Gym en mesa pierden 3 de vida",
        Effects = [
            new(
                TriggerType.SpellPlayed,
                [
                    new AlterUnitStatsEffect(2, 0, new GameFilter()
                        {
                            Filter = new(){},
                            WhichBoardToSearch = PlayerType.PLAYER
                        }),
                    new AlterUnitStatsEffect(-3, 0, new GameFilter()
                        {
                            Filter = new(){ CurrentFamilies = ["Gym"]},
                            WhichBoardToSearch = PlayerType.PLAYER
                        }),
                ], new DurationByExecutions(1), new IHaveBeenPlayedCondition()
            )
        ]
    },
    new()
    {
        Id = "42",
        Name = "Batido de proteínas",
        Description = "La primera carta de la izquierda consigue +1/+2",
        Type = CardType.Spell,
        Effects = [
            new(TriggerType.SpellPlayed, [new AlterUnitStatsEffect(2, 1, new GameFilter()
                        {
                            Filter = new(){ },
                            WhichBoardToSearch = PlayerType.PLAYER,
                            MaxLength = 1
                        })], new DurationByExecutions(1), null)
        ],
        Families = ["Entrenamiento"]
    },
    new()
    {
        Id = "43",
        Name = "Entrenador personal",
        Description = "Crea dos batidos de proteínas en el mazo",
        Type = CardType.Unit,
        Effects = [
            new(TriggerType.UnitPlayed, [new AppendCardToDeck(2, "42", false)], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
        ],
        BaseHealth = 2,
        BaseAttack = 2
    },
    new()
    {
        Id = "44",
        Name = "El señor de las mancuernas",
        Type = CardType.Unit,
        BaseAttack = 0,
        BaseHealth = 2,
        Description = "Cuando el jugador usa un batido de proteínas, consigo +1/0",
        Effects = [
            new(TriggerType.SpellPlayed, [new AlterMySelf(0, 1, false)], new Always(), new PlayerCardCondition(true, new() {DefinitionId = "42"})),
        ]
    },
    new()
    {
        Id = "45",
        Name = "Gerente del Basic Fit",
        Description = "Si hay una carta Motivacion en la mesa, roba una carta",
        Families = ["Gym"],
        Type = CardType.Unit,
        BaseAttack = 1,
        BaseHealth = 1,
        Effects = [
            new(TriggerType.UnitPlayed, [new DrawCardEffect()], new DurationByExecutions(1), new CountCardCondition(new() {Filter = new() {CurrentFamilies = ["Motivacion"]}}, new(CountType.AT_LEAST, 1)))
        ]
    },
    new()
    {
        Id = "46",
        Name = "David Goggins",
        Families = ["Motivacion"],
        Type = CardType.Unit,
        Description = "Después de golpear, consigo +1/+1",
        Effects = [
            new (TriggerType.CardAttacked, [new AlterMySelf(1, 1, false)], new Always(), new IHaveBeenPlayedCondition())
        ],
        BaseAttack = 2,
        BaseHealth = 1
    },
    new()
    {
        Id = "47",
        Name = "Señor chino viejo motivado",
        Description = "Cuando recibo daño, consigo +1/+1",
        Type = CardType.Unit,
        Families = ["Oriental", "Motivacion"],
        BaseAttack = 0,
        BaseHealth = 2,
        Effects = [
            new(TriggerType.UnitHealthChanged, [new AlterMySelf(1, 1, false)], new Always(), new IHaveBeenPlayedCondition())
        ]
    },
    new()
    {
        Id = "48",
        Name = "Tik Tok motivacional",
        Families = ["Motivacion"],
        Description = "Necesitas una carta con 0 de daño en mesa. Las dos primeras unidades (IZQ) ganan +2/+2",
        Type = CardType.Spell,
        Effects = [
            new(TriggerType.SpellPlayed, [new AlterUnitStatsEffect(2, 2, new(){WhichBoardToSearch = PlayerType.PLAYER, Filter = new(), MaxLength = 2})], new DurationByExecutions(1), null)
        ],
        ConditionToPlay = new CountCardCondition(new(){WhichBoardToSearch = PlayerType.PLAYER, Filter = new(){CurrentAttack = new(CountType.EXACTLY, 0)}}, new(CountType.AT_LEAST, 1))
    },
    new()
    {
        Id = "49",
        Name = "No pain, no gain",
        Families = ["Motivacion"],
        Description = "Inflinge 1 de daño a 2 unidades y roba una carta. HABILIDAD: inflinge 1 de daño a todas las cartas en mesa y les otorgas +1/0",
        Type = CardType.Spell,
        Effects = [
          new(TriggerType.SpellPlayed, [new AlterUnitStatsEffect(-1, 0, new(){WhichBoardToSearch = PlayerType.PLAYER, MaxLength = 2, Filter = new()}), new DrawCardEffect()], new DurationByExecutions(1), null)  
        ],
        PlayEffects = [
            new AlterUnitStatsEffect(-1, 0, new(){WhichBoardToSearch = PlayerType.PLAYER, Filter = new()}),
            new AlterUnitStatsEffect(0, 2, new(){WhichBoardToSearch = PlayerType.PLAYER, Filter = new()}),
        ],
        PlayEffectTriggerTimes = 1,
        ConditionToPlay = new CountCardCondition(new(){WhichBoardToSearch = PlayerType.PLAYER, Filter = new()}, new(CountType.AT_LEAST, 1))
    },
    new()
    {
        Id = "50",
        Name = "Esteroides",
        Description = "Elimina mi mesa completa. Me curo la cantidad de vida total de mis tropas",
        Type = CardType.Spell,
        Effects = [
            new(TriggerType.SpellPlayed, [new AlterPlayerBasedOnCardStats(PlayerType.PLAYER, new(), AffectedStats.HEALTH, PlayerType.PLAYER, 1)], new DurationByExecutions(1), null)
        ]
    },
    new()
    {
        Id = "51",
        Name = "Biscuit Oliva",
        Families = ["Leyenda"],
        Type = CardType.Unit,
        BaseAttack = 3,
        BaseHealth = 3,
        Description = "Necesitas al menos 2 tropas de Gym. Cuando entro al campo, +1/+1 a la mesa.",
        Effects = [
            new(TriggerType.UnitPlayed, [new AlterUnitStatsEffect(1, 1, new(){WhichBoardToSearch = PlayerType.PLAYER, Filter = new()})], new DurationByExecutions(1), null),
        ],
        ConditionToPlay = new CountCardCondition(new(){WhichBoardToSearch = PlayerType.PLAYER, Filter = new(){CurrentFamilies = ["Gym"]}}, new(CountType.AT_LEAST, 2))
    },
    new()
    {
        Id = "52",
        Name = "Flexeo de músculos",
        Description = "El jugador tiene 10 de vida o más. Roba 2 cartas",
        Type = CardType.Spell,
        Effects = [new(TriggerType.SpellPlayed, [new DrawCardEffect(2, null)], new DurationByExecutions(1), null)],
        ConditionToPlay = new PlayerHealthCondition(true, new(CountType.AT_LEAST, 10))
    },

    new()
    {
        Id = "13_2",
        Name = "Rey rata",
        Type = CardType.Unit,
        Description = "Has jugado 6 ratas o más. Cuando golpeo, añado una rata al mazo rival. Cuando juegas un queso, consigo +1/+1",
        Families = ["Rata"],
        BaseAttack = 3,
        BaseHealth = 3,
        ConditionToPlay = new CountPlayedCardsCondition(new(){CurrentFamilies = ["Rata"]}, PlayerType.PLAYER, new(CountType.AT_LEAST, 6)),
        Effects = [
            new(TriggerType.SpellPlayed, [new AlterMySelf(1, 1, false)], new Always(), new PlayerCardCondition(true, new(){DefinitionId = "7"})),
            new(TriggerType.CardAttacked, [new AppendCardToDeck(1, "2", true)], new Always(), new IAttackedCondition())
        ]

    },

new()
{
    Id = "53",
    Name = "Fragmento helado",
    Type = CardType.Spell,
    Families = ["Hielo"],
    Description = "-2/0 a una unidad enemiga (izq)",
    Effects = [
        new(
            TriggerType.SpellPlayed,
            [
                new AlterUnitStatsEffect(0, -2, new()
                {
                    WhichBoardToSearch = PlayerType.RIVAL,
                    MaxLength = 1,
                    Filter = new()
                })
            ],
            new DurationByExecutions(1),
            new IHaveBeenPlayedCondition()
        )
    ]
},

new()
{
    Id = "54",
    Name = "Aprendiz del invierno",
    Type = CardType.Unit,
    Families = ["Brujo"],
    BaseAttack = 1,
    BaseHealth = 1,
    Description = "Al jugarme, -1/0 a dos enemigos (IZQ)",
    Effects = [
        new(
            TriggerType.SpellPlayed,
            [new AlterMySelf(1, 1, false)],
            new Always(),
            new PlayerCardCondition(true, new(){CurrentFamilies = ["Hielo"]})
        )
    ]
},

new()
{
    Id = "55",
    Name = "Ventisca cruel",
    Type = CardType.Spell,
    Families = ["Hielo"],
    Description = "-1/0 a toda la mesa enemiga. Roba una carta",
    Effects = [
        new(
            TriggerType.SpellPlayed,
            [
                new AlterUnitStatsEffect(0, -1, new()
                {
                    WhichBoardToSearch = PlayerType.RIVAL,
                    Filter = new()
                }),
                new DrawCardEffect()
            ],
            new DurationByExecutions(1),
            new IHaveBeenPlayedCondition()
        )
    ]
},

new()
{
    Id = "56",
    Name = "Gólem de escarcha",
    Type = CardType.Unit,
    Families = ["Hielo"],
    BaseAttack = 0,
    BaseHealth = 2,
    Description = "Cuando se altera el ataque de las unidades enemigas, consigo +1/0",
    Effects = [
        new (TriggerType.UnitDamageChanged, [new AlterMySelf(1, 0, false)], new Always(), new PlayerCardCondition(false, null))
    ]
},

new()
{
    Id = "57",
    Name = "Prisión glaciar",
    Type = CardType.Spell,
    Families = ["Hielo"],
    Description = "-4/0 a una unidad enemiga (izq)",
    Effects = [
        new(
            TriggerType.SpellPlayed,
            [
                new AlterUnitStatsEffect(0, -4, new()
                {
                    WhichBoardToSearch = PlayerType.RIVAL,
                    MaxLength = 1,
                    Filter = new()
                })
            ],
            new DurationByExecutions(1),
            new IHaveBeenPlayedCondition()
        )
    ]
},

new()
{
    Id = "58",
    Name = "Sabio de la nieve",
    Type = CardType.Unit,
    Families = ["Brujo"],
    BaseAttack = 1,
    BaseHealth = 2,
    Description = "Final de ronda: -1/0 a dos unidades enemigas (izq)",
    Effects = [
        new(
            TriggerType.TurnEnd,
            [
                new AlterUnitStatsEffect(0, -1, new()
                {
                    WhichBoardToSearch = PlayerType.RIVAL,
                    MaxLength = 2,
                    Filter = new()
                })
            ],
            new Always(),
            null
        )
    ]
},

new()
{
    Id = "59",
    Name = "Corazón congelado",
    Type = CardType.Spell,
    Families = ["Hielo"],
    Description = "Mata las unidades enemigas que tengan 0 de daño o menos",
    Effects = [
        new(
            TriggerType.SpellPlayed,
            [
                new KillCards(new () {CurrentAttack = new(CountType.AT_MAX, 0)}, PlayerType.RIVAL, 4)
            ],
            new DurationByExecutions(1),
            new IHaveBeenPlayedCondition()
        )
    ],
    ConditionToPlay = new CountCardCondition(new() {WhichBoardToSearch = PlayerType.RIVAL, Filter = new() {CurrentAttack = new(CountType.AT_MAX, 0)}}, new(CountType.AT_LEAST, 1))
},

new()
{
    Id = "60",
    Name = "Elemental de hielo",
    Type = CardType.Unit,
    Families = ["Hielo"],
    BaseAttack = 2,
    BaseHealth = 1,
    Description = "Al jugarme: -1/0 a la mesa rival, y curo a mi jugador 3 de vida.",
    Effects = [
        new(
            TriggerType.UnitPlayed,
            [new AlterUnitStatsEffect(0, -1, new(){WhichBoardToSearch = PlayerType.RIVAL, Filter = new()}), new AlterPlayerHealthEffect(3, false)],
            new DurationByExecutions(1),
            new IHaveBeenPlayedCondition()
        )
    ]
},

new()
{
    Id = "61",
    Name = "Invierno interminable",
    Type = CardType.Spell,
    Families = ["Hielo"],
    Description = "Durante los próximos dos finales de ronda, -1/0 a la mesa enemiga",
    Effects = [
        new(
            TriggerType.SpellPlayed,
            [
                new AppendGlobalEffect(
                    new(
                        TriggerType.TurnEnd,
                        [
                            new AlterUnitStatsEffect(0, -1, new()
                            {
                                WhichBoardToSearch = PlayerType.RIVAL,
                                Filter = new()
                            })
                        ],
                        new DurationByExecutions(2),
                        null
                    ),
                    "Durante los próximos dos finales de ronda, -1/0 a la mesa enemiga"
                )
            ],
            new DurationByExecutions(1),
            null
        )
    ]
},

new()
{
    Id = "62",
    Name = "Dragón boreal",
    Type = CardType.Unit,
    Families = ["Hielo", "Leyenda", "Dragón"],
    BaseAttack = 5,
    BaseHealth = 6,
    Description = "No hay criaturas en juego, y has jugado 10 cartas de Hielo o más. Final de ronda: si hay hueco, invoco una cría de dragón",
    Effects = [
        new(TriggerType.TurnEnd, [
            new PlayCardEffect("65", false)
        ], new Always(), null)
    ],
    ConditionToPlay = new MultiEffectCondition([
        new CountPlayedCardsCondition(
        new() { CurrentFamilies = ["Hielo"] },
        PlayerType.PLAYER,
        new(CountType.AT_LEAST, 10)
    ),
    new CountCardCondition(new(){WhichBoardToSearch = PlayerType.PLAYER, Filter = new()}, new(CountType.EXACTLY, 0))
    ], false)
},

new()
{
    Id = "63",
    Name = "Hechicero del cero absoluto",
    Type = CardType.Unit,
    Families = ["Hielo", "Brujo"],
    BaseAttack = 2,
    BaseHealth = 4,
    Description = "Cuando una unidad enemiga llega a 0 de ataque, le inflinjo 2 de daño",
    Effects = [
        new(
            TriggerType.UnitHealthChanged,
            [
                new AlterUnitStatsEffect(-2, 0, new()
                {
                    WhichBoardToSearch = PlayerType.RIVAL,
                    Filter = new(){ CurrentAttack = new(CountType.EXACTLY, 0) }
                })
            ],
            new Always(),
            new PlayerCardCondition(false, null)
        )
    ]
},

new()
{
    Id = "64",
    Name = "Avalancha",
    Type = CardType.Spell,
    Families = ["Hielo"],
    Description = "Has jugado 5 hechizos de hielo o más. -2/0 a toda la mesa enemiga. Las unidades con 0 de ataque reciben además -3/-3",
    Effects = [
        new(
            TriggerType.SpellPlayed,
            [
                new AlterUnitStatsEffect(0, -2, new()
                {
                    WhichBoardToSearch = PlayerType.RIVAL,
                    Filter = new()
                }),
                new AlterUnitStatsEffect(-3, -3, new()
                {
                    WhichBoardToSearch = PlayerType.RIVAL,
                    Filter = new(){ CurrentAttack = new(CountType.EXACTLY, 0) }
                })
            ],
            new DurationByExecutions(1),
            new IHaveBeenPlayedCondition()
        )
    ],
    ConditionToPlay = new CountPlayedCardsCondition(new(){CurrentFamilies = ["Hielo"], CardType = CardType.Spell}, PlayerType.PLAYER, new(CountType.AT_LEAST, 5))
},
new()
{
    Id = "65",
    Name = "Cría de dragón",
    Type = CardType.Unit,
    Families = ["Leyenda", "Dragón"],
    Description = "HABILIDAD: quito 1 de vida al rival y se lo otorgo a mi jugador",
    BaseAttack = 2,
    BaseHealth = 1,
    PlayEffectTriggerTimes = 1,
    PlayEffects = [
        new AlterPlayerHealthEffect(1, false),
        new AlterPlayerHealthEffect(-1, true),
    ]
},


new()
{
    Id = "66",
    Name = "Pilluelo",
    Families = ["Ladrón"],
    Description = "Cuando muero, roba una carta",
    BaseAttack = 1,
    BaseHealth = 1,
    Type = CardType.Unit,
    Effects =[
        new(TriggerType.UnitDeath, [new DrawCardEffect(1, null)], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
    ]
},
new()
{
    Id = "67",
    Name = "Fanático creyente",
    Families = ["Culto"],
    Description = "Cuando muero, inflinjo 1 de daño al rival",
    BaseAttack = 1,
    BaseHealth = 2,
    Type = CardType.Unit,
    Effects =[
        new(TriggerType.UnitDeath, [new AlterPlayerHealthEffect(-1, true)], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
    ]
},
new()
{
    Id = "68",
    Name = "Terrible sacrificio",
    Families = ["Culto"],
    Description = "Mata a una unidad en mesa (izq) para robar dos cartas",
    Type = CardType.Spell,
    Effects =[
        new(TriggerType.SpellPlayed, [new KillCards(new(), PlayerType.PLAYER, 1), new DrawCardEffect(2, null)], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
    ],
    ConditionToPlay = new CountCardCondition(new() {WhichBoardToSearch = PlayerType.PLAYER, Filter = new()}, new(CountType.AT_LEAST, 1))
},
new()
{
    Id = "69",
    Name = "Venganza sangrienta",
    Families = ["Culto"],
    Description = "Mata a una unidad en mesa (izq) para matar otra unidad (izq) en la mesa rival",
    Type = CardType.Spell,
    Effects =[
        new(TriggerType.SpellPlayed, [new KillCards(new(), PlayerType.PLAYER, 1),new KillCards(new(), PlayerType.RIVAL, 1), ], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
    ],
    ConditionToPlay = new MultiEffectCondition([
        new CountCardCondition(new() {WhichBoardToSearch = PlayerType.PLAYER, Filter = new()}, new(CountType.AT_LEAST, 1)),
        new CountCardCondition(new() {WhichBoardToSearch = PlayerType.RIVAL, Filter = new()}, new(CountType.AT_LEAST, 1))
    ], false)
},




new()
{
    Id = "70",
    Name = "RED",
    Families = ["Capa"],
    Type = CardType.Unit,
    BaseHealth = 1,
    BaseAttack = 1,
    Description = "Crea ENLACE y BITS en el mazo",
    Effects = [
        new(TriggerType.UnitPlayed, [
            new AppendCardToDeck(1, "71", false),
            new AppendCardToDeck(1, "72", false),
        ], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
    ]
},
new()
{
    Id = "71",
    Name = "BITS",
    Families = ["Paquete"],
    Type = CardType.Spell,
    Description = "Curo 1 de vida al jugador",
    Effects = [
        new(TriggerType.SpellPlayed, [new AlterPlayerHealthEffect(1, false)], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
    ]
},
new()
{
    Id = "72",
    Name = "ENLACE",
    Families = ["Capa"],
    Type = CardType.Unit,
    BaseHealth = 2,
    BaseAttack = 2,
    Description = "Crea RED y Trama en el mazo",
    Effects = [
        new(TriggerType.UnitPlayed, [
            new AppendCardToDeck(1, "73", false),
            new AppendCardToDeck(1, "74", false),
        ], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
    ]
},
new()
{
    Id = "73",
    Name = "Trama",
    Families = ["Paquete"],
    Type = CardType.Spell,
    Description = "Roba una carta",
    Effects = [
        new(TriggerType.SpellPlayed, [new DrawCardEffect()], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
    ]
},
new()
{
    Id = "74",
    Name = "RED",
    Families = ["Capa"],
    Type = CardType.Unit,
    BaseHealth = 3,
    BaseAttack = 3,
    Description = "Crea TRANSPORTE y Paquete en el mazo",
    Effects = [
        new(TriggerType.UnitPlayed, [
            new AppendCardToDeck(1, "75", false),
            new AppendCardToDeck(1, "76", false),
        ], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
    ]
},
new()
{
    Id = "75",
    Name = "Paquete",
    Families = ["Paquete"],
    Type = CardType.Spell,
    Description = "0/+1 a las unidades en mesa",
    Effects = [
        new(TriggerType.SpellPlayed, [new AlterUnitStatsEffect(1, 0, new(){WhichBoardToSearch = PlayerType.PLAYER, Filter = new()})], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
    ]
},
new()
{
    Id = "76",
    Name = "TRANSPORTE",
    Families = ["Capa"],
    Type = CardType.Unit,
    BaseHealth = 4,
    BaseAttack = 4,
    Description = "Crea SESIÓN y Datagrama en el mazo",
    Effects = [
        new(TriggerType.UnitPlayed, [
            new AppendCardToDeck(1, "77", false),
            new AppendCardToDeck(1, "78", false),
        ], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
    ]
},
new()
{
    Id = "77",
    Name = "Datagrama",
    Families = ["Paquete"],
    Type = CardType.Spell,
    Description = "+1/0 a las unidades en mesa",
    Effects = [
        new(TriggerType.SpellPlayed, [new AlterUnitStatsEffect(0, 1, new(){WhichBoardToSearch = PlayerType.PLAYER, Filter = new()})], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
    ]
},
new()
{
    Id = "78",
    Name = "SESIÓN",
    Families = ["Capa"],
    Type = CardType.Unit,
    BaseHealth = 5,
    BaseAttack = 5,
    Description = "Crea PRESENTACIÓN y Datos de sesión en el mazo",
    Effects = [
        new(TriggerType.UnitPlayed, [
            new AppendCardToDeck(1, "79", false),
            new AppendCardToDeck(1, "80", false),
        ], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
    ]
},
new()
{
    Id = "79",
    Name = "Datos de sesión",
    Families = ["Paquete"],
    Type = CardType.Spell,
    Description = "Curo 4 de vida al jugador",
    Effects = [
        new(TriggerType.SpellPlayed, [new AlterPlayerHealthEffect(4, false)], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
    ]
},
new()
{
    Id = "80",
    Name = "PRESENTACIÓN",
    Families = ["Capa"],
    Type = CardType.Unit,
    BaseHealth = 6,
    BaseAttack = 6,
    Description = "Crea APLICACIÓN y Datos de presentación en el mazo",
    Effects = [
        new(TriggerType.UnitPlayed, [
            new AppendCardToDeck(1, "81", false),
            new AppendCardToDeck(1, "82", false),
        ], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
    ]
},
new()
{
    Id = "81",
    Name = "Datos de presentación",
    Families = ["Paquete"],
    Type = CardType.Spell,
    Description = "+2/+2 a las dos próximas unidades que se jueguen",
    Effects = [
        new(TriggerType.SpellPlayed, [
            new AppendGlobalEffect(
                new(TriggerType.UnitPlayed, [new AlterMySelf(2, 2, true)], new DurationByExecutions(2), new PlayerCardCondition(true, null)),
                "+2/+2 a las dos próximas unidades jugadas"
            )
            ], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
    ]
},
new()
{
    Id = "82",
    Name = "APLICACIÓN",
    Families = ["Capa"],
    Type = CardType.Unit,
    BaseHealth = 7,
    BaseAttack = 7,
    Description = "Crea Ex Machina y Datos de aplicación en el mazo",
    Effects = [
        new(TriggerType.UnitPlayed, [
            new AppendCardToDeck(1, "83", false),
            new AppendCardToDeck(1, "84", false),
        ], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
    ]
},
new()
{
    Id = "83",
    Name = "Datos de aplicación",
    Families = ["Paquete"],
    Type = CardType.Spell,
    Description = "Roba 3 cartas",
    Effects = [
        new(TriggerType.SpellPlayed, [new DrawCardEffect(3, null)], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
    ]
},
new()
{
    Id = "84",
    Name = "Ex Machina",
    Families = ["Machina"],
    Type = CardType.Unit,
    BaseAttack = 0,
    BaseHealth = 12,
    Description = "Todas las capas han sido ensambladas. La máquina perfecta. Final de turno: decido por el jugador",
    Effects = [
        new(TriggerType.TurnEnd, [new DecideTurnEffect()], new Always(), null)
    ]
},
new()
{
    Id = "85",
    Name = "HUB",
    Families = ["Nodo"],
    Type = CardType.Unit,
    BaseAttack = 0,
    BaseHealth = 2,
    Description = "Cuando el jugador juega un hechizo paquete, se activa una vez más",
    Effects = [
        new(TriggerType.SpellPlayed, [new RetriggerSpellEffect()], new Always(), new PlayerCardCondition(true, new(){CurrentFamilies = ["Paquete"]}))
    ]
},
new()
{
    Id = "86",
    Name = "Router",
    Families = ["Nodo"],
    Type = CardType.Unit,
    BaseAttack = 1,
    BaseHealth = 3,
    Description = "Cuando el jugador juega un paquete, creo un paquete de capa menor o igual al jugado y lo añado al mazo",
    Effects = [
        new(TriggerType.SpellPlayed, [new GeneratePaqueteEffect()], new Always(), new PlayerCardCondition(true, new(){CurrentFamilies = ["Paquete"]}))
    ]
},
new()
{
    Id = "87",
    Name = "Electricista apañado",
    Description = "Cuando me juegas, roba una carta",
    Type = CardType.Unit,
    BaseAttack = 2,
    BaseHealth = 4,
    Families = ["Seguridad"],
    Effects = [
        new(TriggerType.UnitPlayed, [new DrawCardEffect()], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
    ]
},
new()
{
    Id = "88",
    Name = "Hacker",
    BaseAttack = 1,
    BaseHealth = 2,
    Type = CardType.Unit,
    Description = "Cuando se juega un paquete, creo un virus en el mazo del rival. HABILIDAD: daño al rival 1 por cada virus que tenga en la mano",
    Effects = [
        new(TriggerType.SpellPlayed, [new AppendCardToDeck(1, "89", true)], new Always(), new PlayerCardCondition(true, new(){CurrentFamilies = ["Paquete"]}))
    ],
    PlayEffectTriggerTimes = 1,
    PlayEffects = [
        new DamagePlayerBasedOnCards(true, -1, PlayerType.RIVAL, new() {WhichHandToSearch = PlayerType.RIVAL, Filter = new(){CurrentFamilies = ["Virus"]}})
    ]
},
new()
{
    Id = "89",
    Name = "Malware",
    Type = CardType.Spell,
    Families = ["Virus"],
    Description = "Inflinjo 1 de daño a mi jugador",
    Effects = [
        new(TriggerType.SpellPlayed, [new AlterPlayerHealthEffect(-1, false)], new DurationByExecutions(1), null)
    ]
},
new()
{
    Id = "90",
    Name = "Mensaje de broadcast",
    Families = ["Paquete"],
    Type = CardType.Spell,
    Description = "Juega todos los virus que haya en mano del rival",
    Effects = [
        new(TriggerType.SpellPlayed, [new ForcePlayCardInHandEffect(PlayerType.RIVAL, new(){CurrentFamilies = ["Virus"]})], new DurationByExecutions(1), null)
    ]
},
new()
{
    Id = "91",
    Name = "Denegación de servicio",
    Type = CardType.Spell,
    Effects = [
        new(TriggerType.SpellPlayed, [
            new AlterUnitStatsEffect(0, -3, new(){WhichBoardToSearch = PlayerType.RIVAL, Filter = new()}),
            new AppendGlobalEffect(
                new(TriggerType.TurnEnd, [
                    new AlterUnitStatsEffect(0, +1, new(){WhichBoardToSearch = PlayerType.RIVAL, Filter = new()}),
                    
                ], new DurationByExecutions(1), new TurnCounterCondition(2)),
                "Las unidades del rival conseguirán +1/0 dentro de dos rondas"
            )   
        ], new DurationByExecutions(1), null)
    ],
    Description = "-3/0 a la mesa del rival, +1/0 dentro de 2 rondas"
},
new()
{
    Id = "92",
    Name = "Troyano",
    Type = CardType.Unit,
    BaseAttack = 1,
    BaseHealth = 1,
    Families = ["Virus"],
    Description = "Cuando muero, me agrego al mazo rival",
    Effects = [
        new(
            TriggerType.UnitDeath,
            [
                new AppendCardToDeck(1, "92", true)
            ],
            new Always(),
            new IHaveBeenPlayedCondition()
        )
    ]
},
new()
{
    Id = "93",
    Name = "Firewall",
    Families = ["Seguridad"],
    Type = CardType.Unit,
    BaseAttack = 0,
    BaseHealth = 3,
    Description = "Cuando el rival juega un virus, consigo +1/+1",
    Effects = [
        new(
            TriggerType.SpellPlayed,
            [
                new AlterMySelf(1, 1, false)
            ],
            new Always(),
            new PlayerCardCondition(false, new(){CurrentFamilies = ["Virus"]})
        ),
        new(
            TriggerType.UnitPlayed,
            [
                new AlterMySelf(1, 1, false)
            ],
            new Always(),
            new PlayerCardCondition(false, new(){CurrentFamilies = ["Virus"]})
        )
    ]
},
new()
{
    Id = "94",
    Name = "Copia de seguridad",
    Families = ["Seguridad"],
    Type = CardType.Unit,
    BaseAttack = 2,
    BaseHealth = 4,
    Description = "HABILIDAD: Muero y curo a mi jugador el equivalente a mi vida",
    PlayEffectTriggerTimes = 1,
    PlayEffects = [
        new AlterPlayerHealthBasedOnMyStats(AffectedStats.HEALTH, 1, false),
        new KillMySelf(),
    ]
},
new()
{
    Id = "95",
    Name = "Prueba Unitaria",
    Type = CardType.Spell,
    Description = "Roba una carta. Las cartas de tipo Seguridad ganan 1 de vida",
    Effects = [
        new(TriggerType.SpellPlayed,
        [
            new AlterUnitStatsEffect(1, 0, new()
            {
                WhichBoardToSearch = PlayerType.PLAYER,
                WhichDeckToSearch = PlayerType.PLAYER,
                Filter = new()
                {
                    CurrentFamilies = ["Seguridad"]
                }
            }),
            new DrawCardEffect()
            
        ],new DurationByExecutions(1), null)
    ]
}




// Capa 2, Hechizo 2 (roba una carta)
// Capa 3, Hechizo 3 (uno de vida a las unidades en mesa)
// Capa 4, Hechizo 4 (uno de ataque a las unidades en mesa)
// Capa 5, Hechizo 5 (cura 4 de vida)
// Capa 6, Hechizo 6 (+2/+2 a las dos próximas unidades que se jueguen)
// Capa 7, Hechizo 7 (Roba 3 cartas)
// Ex machina (0/12 ???? FIN DE RONDA: DECIDE EL TURNO POR JUGADOR)

// TODAS LAS CAPAS tienen n/n de vida, y crean una capa de un nivel superior y su paquete. La última capa crea Ex machina.

// Media: 10 * 2.5 = 15
// Capa 1, Hechizo 1 (cura 1 de vida a su jugador)
// Hub (0/2 Vuelve a activar los hechizos de tipo paquete),
// Router (1/3 Cada vez que se lance un hechizo de tipo paquete, genera uno igual o menor y lo mete en el mazo),

// Electricista Apañado (SEGURIDAD 2/4, clase que tanquea y ya)
// Hacker (1/2, cuando se juega un paquete, crea un virus en el mazo del rival. HABILIDAD: inflinge 1 de daño por cada virus en mano)
// Mensaje de broadcast(paquete, juega todos los virus que haya en mano del rival)
// Denegación de servicio (-2 de ataque a los rivales durante 2 turnos)
// Troyano (VIRUS 1/1, cuando muero me coloco en el mazo del rival)
// Firewall (SEGURIDAD, 0/3, cuando el rival juega un VIRUS consigo +1/+1)
// Copia de seguridad (SEGURIDAD, 2/4, HABILIDAD: me mato para curar mi vida al jugador)
// Prueba Unitaria (Roba una carta. Las cartas de tipo SEGURIDAD ganan 1 de vida)

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
            new DeckDto(
                10,
                "Tecnomago",
                "Una criatura compuesta de metal, cables y energía. No tiene alma. No tiene comunión con el mundo y su energía. Y, de algún modo, es capaz de realizar sus propias obras. Ve por encima del resto, se ha declarado a sí misma como un hechicero, y nadie entiende cuáles son sus propósitos."
            ),
            new()
            {
                //30
                {"70", 2},
                {"85", 2},
                {"86", 2},
                {"87", 4},
                {"88", 3},
                {"90", 2},
                {"91", 3},
                {"92", 3},
                {"93", 3},
                {"94", 3},
                {"95", 3},
            }
        },
        {
            new DeckDto(
                9,
                "Archimago del invierno",
                "Enterrado bajo toneladas de hielo durante milenios, este ser ha decidido resurgir y traer consigo sus nuevas creaciones. Los escalofríos suceden allá por donde pasa, y nadie ha sido todavía capaz de descifrar esa mirada perdida y vacía de vida; no sin haber perdido el juicio antes.") ,
            new()
            {
                //34
                {"53", 4},
                {"54", 3},
                {"55", 4},
                {"56", 3},
                {"57", 3},
                {"58", 3},
                {"59", 2},
                {"60", 3},
                {"61", 2},
                {"62", 1},
                {"63", 3},
                {"64", 3}
            }
        },
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
                //34
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
                { "13_2", 2},
            }
        },
        {
            new DeckDto(6,
                "El Ingenioso Hidalgo Don Quijote de la Mancha",
                "Primero una rata, y ahora este tío. Habla de un modo muy extraño, nadie le entiende y... parece que se está pegando con unos molinos. No para de hablar de enemigos y de una tal Dulcinea; menos mal que esos monstruos no son reales, ¿Verdad?"
            ),
            new()
            {
                //34
                { "14", 5},
                { "15", 3},
                { "16", 3},
                { "26_1", 2},
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
        },
        {
            new DeckDto(8, "Mago cachas", "Sus artes son simlpes pero devastadoras. La mancuernamancia ha ganado no pocas guerras, y no se va a contentar con reunir una victoria más: ha apostado con el resto de magos que quien pierda tendrá que dar 5 vueltas al campo de batalla."),
            new(){
                //34
                {"40", 4},
                {"41", 2},
                {"42", 3},
                {"43", 2},
                {"44", 1},
                {"45", 3},
                {"46", 2},
                {"47", 2},
                {"48", 3},
                {"49", 4},
                {"50", 2},
                {"51", 3},
                {"52", 2},
            }
        }
        
          
    };

}