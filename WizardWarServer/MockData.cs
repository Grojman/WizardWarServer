public static class MockData
{
    public static void PrintData()
    {
        foreach(var c in Cards)
        {
            Console.WriteLine($"[CARD] Id: {c.Id}");
        }
    }
    public static List<CardDefinition> Cards =
[
    new CardDefinition
    {
        Id = "1",
        Type = CardType.Spell,
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
        BaseAttack = 1,
        BaseHealth = 1,
        Families = ["RATA"],
        PlayEffects = [new AlterPlayerHealthEffect(-1, false)],
        PlayEffectTriggerTimes = 1
    },

    new CardDefinition
    {
        Id = "3",
        BaseAttack = 2,
        BaseHealth = 1,
        Families = ["RATA"],
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
                new PlayerCardCondition(false, new() { CurrentFamilies = ["RATA"]})
            )
        ]
    },

    new CardDefinition
    {
        Id = "5",
        BaseAttack = 1,
        BaseHealth = 1,
        Type = CardType.Unit,
        Families = ["RATA"],
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
        Type = CardType.Spell,
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
                            Filter = new CardFilter { CurrentFamilies = ["RATA"] },
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
        Type = CardType.Spell,
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
        BaseAttack = 0,
        BaseHealth = 6,
        Families = ["RATA"],
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
        Type = CardType.Spell,
        Effects =
        [
            new EffectInstance(
                TriggerType.SpellPlayed,
                [
                    new AlterUnitStatsEffect(
                        1, 1,
                        new GameFilter
                        {
                            Filter = new CardFilter { CurrentFamilies = ["RATA"] },
                            WhichBoardToSearch = PlayerType.PLAYER,
                            WhichDeckToSearch = PlayerType.PLAYER
                        }
                    ),
                    new AlterPlayerHealthEffect(-3, false)
                ],
                new DurationByExecutions(1),
                new IHaveBeenPlayedCondition()
            )
        ]
    },

    new CardDefinition
    {
        Id = "10",
        BaseAttack = 2,
        BaseHealth = 3,
        Families = ["RATA"],
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
        Type = CardType.Spell,
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
                                new CardFilter { CurrentFamilies = ["RATA"] })
                        ),
                        "CARD_11_GLOBAL_EFFECT"
                    )
                ],
                new DurationByExecutions(1),
                new IHaveBeenPlayedCondition()
            )
        ],
        ConditionToPlay = new CountPlayedCardsCondition(
            new CardFilter { CurrentFamilies = ["RATA"] },
            PlayerType.PLAYER,
            new(CountType.AT_LEAST, 8)
        )
    },

    new CardDefinition
    {
        Id = "12",
        Type = CardType.Spell,
        Families = ["RATA"],
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
                        "CARD_12_GLOBAL_EFFECT"
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
        Type = CardType.Spell,
        Families = ["RATA"],
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
        BaseAttack = 1,
        BaseHealth = 2,
        Type = CardType.Unit,
        Effects = [
            new(
                TriggerType.DrawCard,
                [
                    new AlterPlayerHealthEffect(-1, true)
                ],
                new Always(),
                new PlayerCardCondition(false, new() { CurrentFamilies = ["RATA"]})
            )
        ]  
    },

    new()
    {
        Id = "13_2",
        Type = CardType.Unit,
        Families = ["RATA"],
        BaseAttack = 3,
        BaseHealth = 3,
        ConditionToPlay = new CountPlayedCardsCondition(new(){CurrentFamilies = ["RATA"]}, PlayerType.PLAYER, new(CountType.AT_LEAST, 8)),
        Effects = [
            new(TriggerType.SpellPlayed, [new AlterMySelf(1, 1, false)], new Always(), new PlayerCardCondition(true, new(){DefinitionId = "7"})),
            new(TriggerType.CardAttacked, [new AppendCardToDeck(1, "2", true)], new Always(), new IAttackedCondition())
        ]

    },

    new CardDefinition
    {
        Id = "14",
        Type = CardType.Spell,
        Families = ["LIBRO"],
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
        BaseAttack = 2,
        BaseHealth = 2,
        Families = ["PARANOIA"]
    },

    new CardDefinition
    {
        Id = "17",
        BaseAttack = 1,
        BaseHealth = 1,
        Families = ["PARANOIA"]
    },

    new CardDefinition
    {
        Id = "18",
        BaseAttack = 3,
        BaseHealth = 1,
        Families = ["PARANOIA", "CASTELLANO"],
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
        BaseAttack = 5,
        BaseHealth = 5,
        Families = ["CABALLERO", "CASTELLANO"],
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
                            Filter = new CardFilter { CurrentFamilies = ["PARANOIA"] }
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
        Type = CardType.Spell,
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
        BaseAttack = 2,
        BaseHealth = 2,
        Families = ["CABALLO"],
        Effects = [
            new(TriggerType.UnitPlayed, [new AlterMySelf(2, 2, false)], new DurationByExecutions(1), new PlayerCardCondition(true, new() {DefinitionId = "19"}))
        ]
    },

    new CardDefinition
    {
        Id = "22",
        Type = CardType.Spell,
        Effects =
        [
            new EffectInstance(
                TriggerType.SpellPlayed,
                [
                    new AlterPlayerHealthEffect(7, false),
                    new AppendCardToDeck(1, "19", false)
                ],
                new DurationByExecutions(1),
                new CountCardCondition(
                    new GameFilter
                    {
                        WhichBoardToSearch = PlayerType.PLAYER,
                        Filter = new CardFilter { DefinitionId = "19" }
                    },
                    new(CountType.AT_LEAST, 1))
            ),
            new EffectInstance(
                TriggerType.SpellPlayed,
                [
                    new KillCards(new(){DefinitionId = "19"}, PlayerType.BOTH, 4)
                ],
                new DurationByExecutions(1),
                new IHaveBeenPlayedCondition()
            )
        ],
        ConditionToPlay = new CountCardCondition(
            new GameFilter
            {
                WhichBoardToSearch = PlayerType.BOTH,
                Filter = new CardFilter { DefinitionId = "19" }
            },
            new(CountType.AT_LEAST, 1)
        )
    },
    new CardDefinition()
    {
        Id = "23",
        Type = CardType.Unit,
        Families = ["CABALLERO"],
        Effects = [
            new(
                TriggerType.UnitDeath, [new AppendGlobalEffect(
                    new(TriggerType.UnitPlayed, [new AlterMySelf(1, -1, true)], new DurationByExecutions(2), new PlayerCardCondition(true, null))
                    , "CARD_23_GLOBAL_EFFECT"
                )], new DurationByExecutions(1), new IHaveBeenPlayedCondition()
            )
        ],
        BaseAttack = 2,
        BaseHealth = 3
    },
    new CardDefinition()
    {
        Id = "24",
        Type = CardType.Unit,
        BaseAttack = 1,
        BaseHealth = 4,
        Families = ["CASTELLANO"],
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
        Type = CardType.Unit,
        BaseAttack = 3,
        BaseHealth = 4,
        Families = ["CABALLERO"],
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
        Type = CardType.Unit,
        BaseAttack = 1,
        BaseHealth = 5,
        Families = ["CASTELLANO"],
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
                        Filter = new() {CurrentFamilies = ["CASTELLANO"]}
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
        Type = CardType.Spell,
        Effects = [
            new(TriggerType.SpellPlayed, [new DrawCardEffect(2, new(){CurrentFamilies = ["CABALLERO"]})], new DurationByExecutions(1), null)
        ]
    },
    new()
    {
        Id = "27",
        Families = ["CENIZA"],
        Type = CardType.Spell,
        Effects = [
            new()
            {
                Trigger = TriggerType.SpellPlayed,
                Effects = [
                    new AlterUnitStatsEffect(-4, 0, new(){
                        WhichBoardToSearch = PlayerType.RIVAL,
                        MaxLength = 1,
                        Filter = new()
                    }),
                ],
                Condition = new IHaveBeenPlayedCondition(),
                Duration = new DurationByExecutions(1)
            }
        ]
    },
    new()
    {
        Id = "28",
        Families = ["BRUJO"],
        Type = CardType.Unit,
        BaseAttack = 2,
        BaseHealth = 1,
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
        Type = CardType.Unit,
        Families = ["BRUJO", "CENIZA"],
        BaseAttack = 6,
        BaseHealth = 7,
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
            new(CountType.AT_LEAST, 5)
        )
    },
    new()
    {
        Id = "30",
        BaseAttack = 2,
        BaseHealth = 3,
        Type = CardType.Unit,
        Families = ["CENIZA"],
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
        Families = ["DESTRUCCION"],
        Type = CardType.Spell,
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
        Type = CardType.Spell,
        Effects = [
            new(TriggerType.SpellPlayed, [new DrawCardEffect(2, new() {CardType = CardType.Spell})], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
        ]
    },
    new()
    {
        Id = "33",
        Type = CardType.Spell,
        Effects = [
            new(TriggerType.SpellPlayed, [new DrawCardEffect(1, new() {CurrentFamilies = ["CENIZA"]}), new AlterPlayerHealthEffect(2, false)], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
        ]
    },
    new()
    {
        Id = "34",
        Families = ["DESTRUCCION"],
        Type = CardType.Spell,
        Effects = [
            new(TriggerType.SpellPlayed, [new KillCards(new(), PlayerType.BOTH, 8)], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
        ]
    },
    new()
    {
        Id = "35",
        Families = ["DESTRUCCION", "MALIGNO"],
        Type = CardType.Unit,
        BaseAttack = 2,
        BaseHealth = 1,
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
                        "CARD_35_GLOBAL_EFFECT"
                    )
                ],
                new DurationByExecutions(1),
                null)
        ]
    },
    new ()
    {
        Id = "36",
        Families = ["BRUJO"],
        Type = CardType.Unit,
        BaseAttack = 1,
        BaseHealth = 1,
        Effects = [
            new(
                TriggerType.UnitDeath,
                [
                    new PlayCardEffect("27", false, true)
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
        Effects = [
            new(
                TriggerType.SpellPlayed,
                [
                    new AlterUnitStatsEffect(1, 1, new() {Filter = new() {CurrentFamilies = ["BRUJO"]}, WhichDeckToSearch = PlayerType.PLAYER})
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
        Families = ["MALIGNO"],
        Type = CardType.Unit,
        BaseAttack = 1,
        BaseHealth = 1,
        Effects = [
            new
            (
                TriggerType.UnitPlayed,
                [new AlterMySelf(3, 0, false)],
                new DurationByExecutions(1),
                new MultiEffectCondition([
                new IHaveBeenPlayedCondition(),
                new CountPlayedCardsCondition(new() { CardType = CardType.Spell}, PlayerType.PLAYER, new(CountType.AT_MAX, 0))
                ], false)
            ),
            new
            (
                TriggerType.UnitPlayed,
                [new DrawCardEffect()],
                new DurationByExecutions(1),
                new MultiEffectCondition([
                new IHaveBeenPlayedCondition(),
                new CountPlayedCardsCondition(new() { CardType = CardType.Spell}, PlayerType.PLAYER, new(CountType.AT_LEAST, 1))
                ], false)
            )
        ]
    },
    new()
    {
        Id = "39",
        Families = ["BRUJO", "MALIGNO"],
        BaseHealth = 2,
        BaseAttack = 1,
        Type = CardType.Unit,
        Effects = [
            new(
                TriggerType.SpellPlayed,
                [ new AlterMySelf(1, 0, false)],
                new Always(),
                new PlayerCardCondition(true, null)
            )
        ]
    },


    new()
    {
        Id = "40",
        Families = ["GYM", "RATA"],
        Type = CardType.Unit,
        BaseAttack = 3,
        BaseHealth = 1,
        Effects = [
            new(TriggerType.UnitDeath, [new AppendCardToDeck(1, "42", false)], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
        ]
    },

    new()
    {
        Id = "41",
        Families = ["ENTRENAMIENTO"],
        Type = CardType.Spell,
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
                            Filter = new(){ CurrentFamilies = ["GYM"]},
                            WhichBoardToSearch = PlayerType.PLAYER
                        }),
                ], new DurationByExecutions(1), new IHaveBeenPlayedCondition()
            )
        ]
    },
    new()
    {
        Id = "42",
        Type = CardType.Spell,
        Effects = [
            new(TriggerType.SpellPlayed, [new AlterUnitStatsEffect(2, 1, new GameFilter()
                        {
                            Filter = new(){ },
                            WhichBoardToSearch = PlayerType.PLAYER,
                            MaxLength = 1
                        })], new DurationByExecutions(1), null)
        ],
        Families = ["ENTRENAMIENTO"]
    },
    new()
    {
        Id = "43",
        Type = CardType.Unit,
        Families = ["GYM"],
        Effects = [
            new(TriggerType.UnitPlayed, [new AppendCardToDeck(2, "42", false)], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
        ],
        BaseHealth = 2,
        BaseAttack = 2
    },
    new()
    {
        Id = "44",
        Type = CardType.Unit,
        BaseAttack = 0,
        BaseHealth = 4,
        Effects = [
            new(TriggerType.SpellPlayed, [new AlterMySelf(1, 0, false)], new Always(), new PlayerCardCondition(true, new() {DefinitionId = "42"})),
        ]
    },
    new()
    {
        Id = "45",
        Families = ["GYM"],
        Type = CardType.Unit,
        BaseAttack = 2,
        BaseHealth = 1,
        Effects = [
            new(TriggerType.UnitPlayed, [new DrawCardEffect()], new DurationByExecutions(1), new CountCardCondition(new() {Filter = new() {CurrentFamilies = ["GYM"]}, WhichBoardToSearch = PlayerType.PLAYER}, new(CountType.AT_LEAST, 1)))
        ]
    },
    new()
    {
        Id = "46",
        Families = ["MOTIVACION"],
        Type = CardType.Unit,
        Effects = [
            new (TriggerType.CardAttacked, [new AlterMySelf(1, 1, false)], new Always(), new IAttackedCondition())
        ],
        BaseAttack = 2,
        BaseHealth = 1
    },
    new()
    {
        Id = "47",
        Type = CardType.Unit,
        Families = ["MOTIVACION"],
        BaseAttack = 0,
        BaseHealth = 3, 
        Effects = [
            new(TriggerType.UnitHealthChanged, [new AlterMySelf(1, 1, true)], new Always(), 
            new MultiEffectCondition(
                [
                    new NumericEventCondition(new(CountType.AT_MAX_UNDER, 0)),
                    new IHaveBeenPlayedCondition()
                ],
                false
            ))
        ]
    },
    new()
    {
        Id = "48",
        Families = ["MOTIVACION"],
        Type = CardType.Spell,
        Effects = [
            new(TriggerType.SpellPlayed, [new AlterUnitStatsEffect(2, 2, new(){WhichBoardToSearch = PlayerType.PLAYER, Filter = new(), MaxLength = 2})], new DurationByExecutions(1), null)
        ],
        ConditionToPlay = new CountCardCondition(new(){WhichBoardToSearch = PlayerType.PLAYER, Filter = new(){CurrentAttack = new(CountType.AT_MAX, 1)}}, new(CountType.AT_LEAST, 1))
    },
    new()
    {
        Id = "49",
        Families = ["MOTIVACION"],
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
        Type = CardType.Spell,
        Effects = [
            new(TriggerType.SpellPlayed, [new AlterPlayerBasedOnCardStats(PlayerType.PLAYER, new(), AffectedStats.HEALTH, PlayerType.PLAYER, 1)], new DurationByExecutions(1), null)
        ]
    },
    new()
    {
        Id = "51",
        Families = ["LEYENDA", "MOTIVACION"],
        Type = CardType.Unit,
        BaseAttack = 3,
        BaseHealth = 3,
        Effects = [
            new(TriggerType.UnitPlayed, [new AlterUnitStatsEffect(1, 1, new(){WhichBoardToSearch = PlayerType.PLAYER, Filter = new()})], new DurationByExecutions(1), null),
        ],
        ConditionToPlay = new CountCardCondition(new(){WhichBoardToSearch = PlayerType.PLAYER, Filter = new(){CurrentFamilies = ["GYM"]}}, new(CountType.AT_LEAST, 2))
    },
    new()
    {
        Id = "52",
        Type = CardType.Spell,
        Effects = [
            new(TriggerType.SpellPlayed, [new DrawCardEffect(3, null)], new DurationByExecutions(1), new PlayerHealthCondition(true, new(CountType.AT_LEAST, 10))),
            new(TriggerType.SpellPlayed, [new AlterPlayerHealthEffect(5, false)], new DurationByExecutions(1), new PlayerHealthCondition(true, new(CountType.AT_MAX_UNDER, 10))),
        ],
    },

    

new()
{
    Id = "53",
    Type = CardType.Spell,
    Families = ["HIELO"],
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
    Id = "54",
    Type = CardType.Unit,
    Families = ["BRUJO"],
    BaseAttack = 1,
    BaseHealth = 3,
    Effects = [
        new(
            TriggerType.SpellPlayed,
            [
                new AlterUnitStatsEffect(0, -1, new()
                {
                    WhichBoardToSearch = PlayerType.RIVAL,
                    MaxLength = 1,
                    Filter = new()
                })
            ],
            new Always(),
            new PlayerCardCondition(true, new(){CurrentFamilies = ["HIELO"]})
        )
    ]
},

new()
{
    Id = "55",
    Type = CardType.Spell,
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
    Type = CardType.Unit,
    Families = ["HIELO"],
    BaseAttack = 0,
    BaseHealth = 3,
    Effects = [
        new (TriggerType.UnitDamageChanged, [new AlterMySelf(1, 0, false)], new Always(), 
        new MultiEffectCondition([
        new PlayerCardCondition(false, null),
        new NumericEventCondition(new(CountType.AT_MAX_UNDER, 0))
        ], false)
        )
    ]
},

new()
{
    Id = "57",
    Type = CardType.Spell,
    Families = ["HIELO"],
    Effects = [
        new(
            TriggerType.SpellPlayed,
            [
                new SetUnitToCeroEffect(1, true),
                new SetUnitToCeroEffect(1, false),
            ],
            new DurationByExecutions(1),
            new IHaveBeenPlayedCondition()
        )
    ],
    ConditionToPlay = new CountCardCondition(new(){WhichBoardToSearch = PlayerType.PLAYER, Filter = new()}, new(CountType.AT_LEAST, 1))
},

new()
{
    Id = "58",
    Type = CardType.Unit,
    Families = ["BRUJO"],
    BaseAttack = 1,
    BaseHealth = 2,
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
    Type = CardType.Spell,
    Families = ["HIELO"],
    Effects = [
        new(
            TriggerType.SpellPlayed,
            [
                new DrawCardsBasedOnFilter(1, new(){WhichBoardToSearch = PlayerType.RIVAL, Filter = new(){ CurrentAttack = new(CountType.AT_MAX, 0)}}, false, null),
                new KillCards(new () {CurrentAttack = new(CountType.AT_MAX, 0)}, PlayerType.RIVAL, 4),
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
    Type = CardType.Unit,
    Families = ["HIELO"],
    BaseAttack = 2,
    BaseHealth = 1,
    Effects = [
        new(
            TriggerType.UnitPlayed,
            [
                new AlterUnitStatsEffect(0, -1, new(){WhichBoardToSearch = PlayerType.RIVAL, Filter = new()}), 
                ],
            new DurationByExecutions(1),
            new IHaveBeenPlayedCondition()
        ),
        new(
            TriggerType.UnitDeath,
            [
                new AlterPlayerHealthEffect(3, false)
            ],
            new DurationByExecutions(1),
            new IHaveBeenPlayedCondition()
        )
    ]
},

new()
{
    Id = "61",
    Type = CardType.Spell,
    Families = ["HIELO"],
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
                    "CARD_61_GLOBAL_EFFECT"
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
    Type = CardType.Unit,
    Families = ["HIELO", "LEYENDA", "DRAGON"],
    BaseAttack = 5,
    BaseHealth = 6,
    Effects = [
        new(TriggerType.TurnEnd, [
            new PlayCardEffect("65", false, false)
        ], new Always(), null)
    ],
    ConditionToPlay = new MultiEffectCondition([
        new CountPlayedCardsCondition(
        new() { CurrentFamilies = ["HIELO"] },
        PlayerType.PLAYER,
        new(CountType.AT_LEAST, 8)
    ),
    new CountCardCondition(new(){WhichBoardToSearch = PlayerType.PLAYER, Filter = new()}, new(CountType.EXACTLY, 0))
    ], false)
},

new()
{
    Id = "63",
    Type = CardType.Unit,
    Families = ["HIELO", "BRUJO"],
    BaseAttack = 2,
    BaseHealth = 4,
    Effects = [
        new(
            TriggerType.UnitDamageChanged,
            [
                new AlterMySelf(0, -2, true)
            ],
            new Always(),
            new PlayerCardCondition(false, new(){ CurrentAttack = new(CountType.EXACTLY, 0) })
        )
    ]
},

new()
{
    Id = "64",
    Type = CardType.Spell,
    Families = ["HIELO"],
    Effects = [
        new(
            TriggerType.SpellPlayed,
            [
                new AlterUnitStatsEffect(0, -2, new()
                {
                    WhichBoardToSearch = PlayerType.RIVAL,
                    Filter = new()
                }),
                new AlterUnitStatsEffect(-3, 0, new()
                {
                    WhichBoardToSearch = PlayerType.RIVAL,
                    Filter = new(){ CurrentAttack = new(CountType.EXACTLY, 0) }
                })
            ],
            new DurationByExecutions(1),
            new IHaveBeenPlayedCondition()
        )
    ],
    ConditionToPlay = new CountPlayedCardsCondition(new(){CurrentFamilies = ["HIELO"], CardType = CardType.Spell}, PlayerType.PLAYER, new(CountType.AT_LEAST, 5))
},
new()
{
    Id = "65",
    Type = CardType.Unit,
    Families = ["LEYENDA", "DRAGON"],
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
    Id = "70",
    Families = ["CAPA"],
    Type = CardType.Unit,
    BaseHealth = 1,
    BaseAttack = 1,
    Effects = [
        new(TriggerType.UnitPlayed, [
            new AppendCardToDeck(1, "71", false, 10),
            new AppendCardToDeck(1, "72", false, 10),
        ], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
    ]
},
new()
{
    Id = "71",
    Families = ["PAQUETE"],
    Type = CardType.Spell,
    Effects = [
        new(TriggerType.SpellPlayed, [new AlterPlayerHealthEffect(2, false)], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
    ]
},
new()
{
    Id = "72",
    Families = ["CAPA"],
    Type = CardType.Unit,
    BaseHealth = 2,
    BaseAttack = 2,
    Effects = [
        new(TriggerType.UnitPlayed, [
            new AppendCardToDeck(1, "73", false, 10),
            new AppendCardToDeck(1, "74", false, 10),
        ], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
    ]
},
new()
{
    Id = "73",
    Families = ["PAQUETE"],
    Type = CardType.Spell,
    Effects = [
        new(TriggerType.SpellPlayed, [new DrawCardEffect()], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
    ]
},
new()
{
    Id = "74",
    Families = ["CAPA"],
    Type = CardType.Unit,
    BaseHealth = 3,
    BaseAttack = 3,
    Effects = [
        new(TriggerType.UnitPlayed, [
            new AppendCardToDeck(1, "75", false, 10),
            new AppendCardToDeck(1, "76", false, 10),
        ], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
    ]
},
new()
{
    Id = "75",
    Families = ["PAQUETE"],
    Type = CardType.Spell,
    Effects = [
        new(TriggerType.SpellPlayed, [new AlterUnitStatsEffect(1, 0, new(){WhichBoardToSearch = PlayerType.PLAYER, Filter = new()})], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
    ]
},
new()
{
    Id = "76",
    Families = ["CAPA"],
    Type = CardType.Unit,
    BaseHealth = 4,
    BaseAttack = 4,
    Effects = [
        new(TriggerType.UnitPlayed, [
            new AppendCardToDeck(1, "77", false, 10),
            new AppendCardToDeck(1, "78", false, 10),
        ], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
    ]
},
new()
{
    Id = "77",
    Families = ["PAQUETE"],
    Type = CardType.Spell,
    Effects = [
        new(TriggerType.SpellPlayed, [new AlterUnitStatsEffect(0, 1, new(){WhichBoardToSearch = PlayerType.PLAYER, Filter = new()})], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
    ]
},
new()
{
    Id = "78",
    Families = ["CAPA"],
    Type = CardType.Unit,
    BaseHealth = 5,
    BaseAttack = 5,
    Effects = [
        new(TriggerType.UnitPlayed, [
            new AppendCardToDeck(1, "79", false, 10),
            new AppendCardToDeck(1, "80", false, 10),
        ], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
    ]
},
new()
{
    Id = "79",
    Families = ["PAQUETE"],
    Type = CardType.Spell,
    Effects = [
        new(TriggerType.SpellPlayed, [new AlterPlayerHealthEffect(4, false)], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
    ]
},
new()
{
    Id = "80",
    Families = ["CAPA"],
    Type = CardType.Unit,
    BaseHealth = 6,
    BaseAttack = 6,
    Effects = [
        new(TriggerType.UnitPlayed, [
            new AppendCardToDeck(1, "81", false, 10),
            new AppendCardToDeck(1, "82", false, 10),
        ], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
    ]
},
new()
{
    Id = "81",
    Families = ["PAQUETE"],
    Type = CardType.Spell,
    Effects = [
        new(TriggerType.SpellPlayed, [
            new AppendGlobalEffect(
                new(TriggerType.UnitPlayed, [new AlterMySelf(2, 2, true)], new DurationByExecutions(2), new PlayerCardCondition(true, null)),
                "CARD_81_GLOBAL_EFFECT"
            )
            ], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
    ]
},
new()
{
    Id = "82",
    Families = ["CAPA"],
    Type = CardType.Unit,
    BaseHealth = 7,
    BaseAttack = 7,
    Effects = [
        new(TriggerType.UnitPlayed, [
            new AppendCardToDeck(1, "83", false, 10),
            new AppendCardToDeck(1, "84", false, 10),
        ], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
    ]
},
new()
{
    Id = "83",
    Families = ["PAQUETE"],
    Type = CardType.Spell,
    Effects = [
        new(TriggerType.SpellPlayed, [new DrawCardEffect(3, null)], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
    ]
},
new()
{
    Id = "84",
    Families = ["MACHINA"],
    Type = CardType.Unit,
    BaseAttack = 0,
    BaseHealth = 25,
    Effects = [
        new(TriggerType.TurnEnd, [new DecideTurnEffect()], new Always(), null)
    ]
},
new()
{
    Id = "85",
    Families = ["NODO"],
    Type = CardType.Unit,
    BaseAttack = 0,
    BaseHealth = 6,
    Effects = [
        new(TriggerType.SpellPlayed, [new RetriggerSpellEffect()], new Always(), new PlayerCardCondition(true, new(){CurrentFamilies = ["PAQUETE"]}))
    ]
},
new()
{
    Id = "86",
    Families = ["NODO"],
    Type = CardType.Unit,
    BaseAttack = 1,
    BaseHealth = 3,
    Effects = [
        new(TriggerType.TurnEnd, [new DrawCardEffect(1, new() {CurrentFamilies = ["PAQUETE"]})], new Always(), null)
    ]
},
new()
{
    Id = "87",
    Type = CardType.Unit,
    BaseAttack = 2,
    BaseHealth = 4,
    Effects = [
        new(TriggerType.UnitPlayed, [new DrawCardEffect(1, new(){CurrentFamilies = ["CAPA"]})], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
    ]
},
new()
{
    Id = "88",
    BaseAttack = 1,
    BaseHealth = 3,
    Type = CardType.Unit,
    Families = ["SEGURIDAD"],
    Effects = [
        new(TriggerType.SpellPlayed, [new CreateCardInHand(1, "89", true)], new Always(), new PlayerCardCondition(true, new(){CurrentFamilies = ["PAQUETE"]}))
    ],
    PlayEffectTriggerTimes = 1,
    PlayEffects = [
        new DamagePlayerBasedOnCards(true, -1, PlayerType.RIVAL, new() {WhichHandToSearch = PlayerType.RIVAL, Filter = new(){CurrentFamilies = ["VIRUS"]}})
    ]
},
new()
{
    Id = "89",
    Type = CardType.Spell,
    Families = ["VIRUS"],
    Effects = [
        new(TriggerType.SpellPlayed, [new AlterPlayerHealthEffect(-1, false)], new DurationByExecutions(1), null)
    ]
},
new()
{
    Id = "90",
    Families = ["PAQUETE"],
    Type = CardType.Spell,
    Effects = [
        new(TriggerType.SpellPlayed, [new ForcePlayCardInHandEffect(PlayerType.RIVAL, new(){CurrentFamilies = ["VIRUS"]}, int.MaxValue)], new DurationByExecutions(1), null)
    ]
},
new()
{
    Id = "91",
    Type = CardType.Spell,
    Effects = [
        new(TriggerType.SpellPlayed, [
            new AlterUnitStatsEffect(0, -3, new(){WhichBoardToSearch = PlayerType.RIVAL, Filter = new()}),   
        ], new DurationByExecutions(1), null)
    ],
},
new()
{
    Id = "92",
    Type = CardType.Unit,
    BaseAttack = 1,
    BaseHealth = 1,
    Families = ["VIRUS"],
    Effects = [
        new(
            TriggerType.UnitDeath,
            [
                new CreateCardInHand(1, "89", true)
            ],
            new Always(),
            new IHaveBeenPlayedCondition()
        ),
        new(
            TriggerType.UnitPlayed,
            [
                new ForcePlayCardInHandEffect(PlayerType.RIVAL, new(){CurrentFamilies = ["VIRUS"]}, 1)
            ],
            new DurationByExecutions(1),
            new IHaveBeenPlayedCondition()
        )
    ]
},
new()
{
    Id = "93",
    Families = ["SEGURIDAD"],
    Type = CardType.Unit,
    BaseAttack = 2,
    BaseHealth = 3,
    Effects = [
        new(
            TriggerType.SpellPlayed,
            [
                new AlterMySelf(1, 1, false)
            ],
            new Always(),
            new PlayerCardCondition(false, new(){CurrentFamilies = ["VIRUS"]})
        ),
        new(
            TriggerType.UnitPlayed,
            [
                new AlterMySelf(1, 1, false)
            ],
            new Always(),
            new PlayerCardCondition(false, new(){CurrentFamilies = ["VIRUS"]})
        )
    ]
},
new()
{
    Id = "94",
    Families = ["SEGURIDAD"],
    Type = CardType.Unit,
    BaseAttack = 2,
    BaseHealth = 4,
    PlayEffectTriggerTimes = 1,
    PlayEffects = [
        new AlterPlayerHealthBasedOnMyStats(AffectedStats.HEALTH, 1, false),
        new KillMySelf(),
    ]
},
new()
{
    Id = "95",
    Type = CardType.Spell,
    Effects = [
        new(TriggerType.SpellPlayed,
        [
            new AlterUnitStatsEffect(1, 0, new()
            {
                WhichBoardToSearch = PlayerType.PLAYER,
                WhichDeckToSearch = PlayerType.PLAYER,
                Filter = new()
                {
                    CurrentFamilies = ["SEGURIDAD"]
                }
            }),
            new DrawCardEffect(1, new(){CurrentFamilies = ["CAPA"]})
            
        ],new DurationByExecutions(1), null)
    ]
},
new()
{
    Id = "96",
    Families = ["MONEDA_DEL_CAOS"],
    Type = CardType.Spell,
    Effects = [
        new(TriggerType.SpellPlayed,[
            new PlayCardEffect("97", false, false)
        ], new DurationByExecutions(1), new RandomCondition(50))
    ]
},
new()
{
    Id = "97",
    Type = CardType.Unit,
    BaseAttack = 3,
    BaseHealth = 4   
},
new()
{
    Id = "98",
    Type = CardType.Spell,
    Families = ["MONEDA_DEL_CAOS"],
    Effects = [
        new(TriggerType.SpellPlayed,[
            new AlterPlayerHealthEffect(2, false),
            new AlterPlayerHealthEffect(-2, true)
        ], new DurationByExecutions(1), new RandomCondition(50))
    ]
},
new()
{
    Id = "99",
    Type = CardType.Spell,
    Families = ["MONEDA_DEL_CAOS"],
    Effects = [
        new(TriggerType.SpellPlayed,[
            new AlterUnitStatsEffect(-1, -1, new() { WhichBoardToSearch = PlayerType.RIVAL, Filter = new()})
        ], new DurationByExecutions(1), new RandomCondition(50))
    ]
},
new()
{
    Id = "100",
    Type = CardType.Spell,
    Families = ["MONEDA_DEL_CAOS"],
    Effects = [
        new(TriggerType.SpellPlayed,[
            new AlterUnitStatsEffect(1, 1, new() { WhichBoardToSearch = PlayerType.PLAYER, WhichDeckToSearch = PlayerType.PLAYER, Filter = new()})
        ], new DurationByExecutions(1), new RandomCondition(50))
    ]
},
new()
{
    Id = "101",
    Type = CardType.Spell,
    Families = ["MONEDA_DEL_CAOS"],
    Effects = [
        new(TriggerType.SpellPlayed,[
            new AlterPlayerHealthEffect(4, false)
        ], new DurationByExecutions(1), new RandomCondition(50))
    ]
},
new()
{
    Id = "102",
    Type = CardType.Spell,
    Families = ["MONEDA_DEL_CAOS"],
    Effects = [
        new(TriggerType.SpellPlayed,[
            new AlterPlayerHealthEffect(-4, true)
        ], new DurationByExecutions(1), new RandomCondition(50))
    ]
},
new()
{
    Id = "103",
    Type = CardType.Spell,
    Families = ["MONEDA_DEL_CAOS", "CAOS_VERDADERO"],
    Effects = [
        new(TriggerType.SpellPlayed,[
            new UseRandomSpellFromEnemyDeck()
        ], new DurationByExecutions(1), null)
    ]
},


new()
{
    Id = "104",
    Type = CardType.Unit,
    BaseAttack = 1,
    BaseHealth = 1,
    Effects = [
        new(TriggerType.UnitDeath, [new CreateRandomCoin(2)], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
    ]   
},
new()
{
    Id = "105",
    Type = CardType.Unit,
    BaseAttack = 0,
    BaseHealth = 3,
    Effects = [
        new(TriggerType.SpellPlayed, [new RetriggerSpellEffect()], new Always(), new PlayerCardCondition(true, new() { CurrentFamilies = ["MONEDA_DEL_CAOS"]}))
    ]  
},
new()
{
    Id = "106",
    Type = CardType.Spell,
    Effects = [
        new(TriggerType.SpellPlayed, [new CreateRandomCoin(6)], new DurationByExecutions(1), null)
    ]
},
new()
{
    Id = "107",
    Type = CardType.Spell,
    Effects = [
        new(TriggerType.SpellPlayed, [
            new KillCards(new(), PlayerType.PLAYER, 1),
            new AlterUnitStatsEffect(-4, 0, new() {Filter = new(), WhichBoardToSearch = PlayerType.RIVAL, MaxLength = 1}),
            new CreateRandomCoin(2)
        ], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
    ],
    ConditionToPlay = new CountCardCondition(new() { Filter = new(), WhichBoardToSearch = PlayerType.PLAYER}, new(CountType.AT_LEAST, 1))
},
new()
{
    Id = "108",
    Type = CardType.Unit,
    BaseAttack = 2,
    BaseHealth = 2,
    PlayEffectTriggerTimes = 2,
    PlayEffects = [
        new AlterUnitStatsEffect(0, -1, new() {Filter = new(), WhichBoardToSearch = PlayerType.RIVAL}),
        new AlterMySelf(0, -1, false),
    ]
},
new()
{
    Id = "109",
    Type = CardType.Unit,
    BaseAttack = 2,
    BaseHealth = 1,
    PlayEffectTriggerTimes = 1,
    PlayEffects = [
        new ForcePlayCardInHandEffect(PlayerType.PLAYER, new() { CurrentFamilies = ["MONEDA_DEL_CAOS"]}, 1)
    ]
},
new()
{
    Id = "110",
    Type = CardType.Unit,
    BaseAttack = 0,
    BaseHealth = 4,
    Effects = [
        new(TriggerType.SpellPlayed, [
            new CreateRandomCoin(2)
        ], new Always(), new PlayerCardCondition(true, new() { CurrentFamilies = ["MONEDA_DEL_CAOS"]}))
    ]
},
new()
{
    Id = "111",
    Type = CardType.Spell,
    Effects = [
        new(TriggerType.SpellPlayed, [
            new AppendGlobalEffect(
                new(TriggerType.TurnEnd, [
                    new ForcePlayCardInHandEffect(PlayerType.PLAYER, new() { CurrentFamilies = ["MONEDA_DEL_CAOS"]}, 1)
                ], new DurationByExecutions(4), new TurnCounterCondition(1)),
                "CARD_111_GLOBAL_EFFECT"
            )
        ], new DurationByExecutions(1), null)
    ],
    ConditionToPlay = new CountPlayedCardsCondition(new() { CurrentFamilies = ["MONEDA_DEL_CAOS"]}, PlayerType.PLAYER, new(CountType.AT_LEAST, 4))
},
new()
{
    Id = "112",
    Type = CardType.Unit,
    BaseAttack = 1,
    BaseHealth = 1,
    Effects = [
        new(TriggerType.SpellPlayed, [new AlterMySelf(1, 1, false)], new Always(), new PlayerCardCondition(true, new() { CurrentFamilies = ["MONEDA_DEL_CAOS"]}))
    ]
},
new()
{
    Id = "113",
    Type = CardType.Spell,
    Effects = [
        new(TriggerType.SpellPlayed, [
            new PlayCardEffect("103", false, true),
            new PlayCardEffect("103", false, true),
            new PlayCardEffect("103", false, true),
        ], new DurationByExecutions(1), null)
    ],
    ConditionToPlay = new CountPlayedCardsCondition(new() { CurrentFamilies = ["MONEDA_DEL_CAOS"]}, PlayerType.PLAYER, new(CountType.AT_LEAST, 10))
},
new()
{
    Id = "114",
    Type = CardType.Unit,
    BaseAttack = 2,
    BaseHealth = 1,
    Effects = [
        new(TriggerType.UnitPlayed, [
            new AppendGlobalEffect(
                new(TriggerType.SpellPlayed,
                [
                    new CreateRandomCoin(2)
                ], new DurationByExecutions(3), new PlayerCardCondition(true, new(){CurrentFamilies = ["MONEDA_DEL_CAOS"]})),
                "CARD_114_GLOBAL_EFFECT"
            )
        ], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
    ]
},

// ===== Mazo cromático (115-123) =====
new()
{
    Id = "115",
    Type = CardType.Unit,
    BaseAttack = 1,
    BaseHealth = 1,
    Families = ["CROMATICO"],
    Effects = [
        new(TriggerType.UnitPlayed, [
            new RotateColorAndBranchEffect(
                [ new AlterUnitStatsEffect(-1, 0, new() { Filter = new(), WhichBoardToSearch = PlayerType.RIVAL }) ],
                [ new AlterUnitStatsEffect(1, 0, new() { Filter = new(), WhichBoardToSearch = PlayerType.PLAYER }) ],
                [ new DrawCardEffect(1, null), new AppendCardToDeck(1, "115", false, 0) ]
            )
        ], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
    ]
},
new()
{
    Id = "116",
    Type = CardType.Spell,
    Effects = [
        new(TriggerType.SpellPlayed, [
            new RotateColorAndBranchEffect(
                [ new AlterPlayerHealthEffect(-3, true) ],
                [ new AlterPlayerHealthEffect(3, false) ],
                [ new AlterUnitStatsEffect(0, 2, new() { Filter = new() { CardType = CardType.Unit }, WhichDeckToSearch = PlayerType.PLAYER, MaxLength = 2 }) ]
            )
        ], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
    ]
},
new()
{
    Id = "117",
    Type = CardType.Unit,
    BaseAttack = 0,
    BaseHealth = 3,
    Families = ["CROMATICO"],
    Effects = [
        new(TriggerType.UnitPlayed, [
            new BranchOnColorInclusiveEffect(
                [ new AlterUnitStatsEffect(1, 0, new() { Filter = new(), WhichBoardToSearch = PlayerType.RIVAL }) ],
                [ new AlterUnitStatsEffect(-1, 0, new() { Filter = new(), WhichBoardToSearch = PlayerType.PLAYER }) ],
                [ new DrawCardPerBoardUnitEffect(PlayerType.PLAYER, true) ]
            )
        ], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
    ]
},
new()
{
    Id = "118",
    Type = CardType.Spell,
    Effects = [
        new(TriggerType.SpellPlayed, [
            new AppendGlobalEffect(
                new(TriggerType.TurnEnd, [ new RotateColorEffect() ], new Always(), null),
                "CARD_118_GLOBAL_EFFECT"
            )
        ], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
    ]
},
new()
{
    Id = "119",
    Type = CardType.Unit,
    BaseAttack = 6,
    BaseHealth = 1,
    Families = ["CROMATICO"],
    ConditionToPlay = new PlayerHasColorCondition(ChromaticColor.Rojo)
},
new()
{
    Id = "120",
    Type = CardType.Unit,
    BaseAttack = 0,
    BaseHealth = 6,
    Families = ["CROMATICO"],
    ConditionToPlay = new PlayerHasColorCondition(ChromaticColor.Verde),
    Effects = [
        new(TriggerType.ColorChanged, [ new AlterPlayerHealthEffect(4, false) ], new Always(), new ColorChangedToCondition(ChromaticColor.Verde))
    ]
},
new()
{
    Id = "121",
    Type = CardType.Unit,
    BaseAttack = 1,
    BaseHealth = 2,
    Families = ["CROMATICO"],
    ConditionToPlay = new PlayerHasColorCondition(ChromaticColor.Azul),
    PlayEffectTriggerTimes = 3,
    PlayEffects = [ new DrawCardEffect() ]
},
new()
{
    Id = "122",
    Type = CardType.Spell,
    Effects = [
        new(TriggerType.SpellPlayed, [ new MixColorEffect() ], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
    ],
    ConditionToPlay = new CountCardCondition(new() { Filter = new(), WhichDeckToSearch = PlayerType.PLAYER }, new(CountType.AT_MAX, 20))
},
new()
{
    Id = "123",
    Type = CardType.Spell,
    Effects = [
        new(TriggerType.SpellPlayed, [
            new PlayCardEffect("115", false, false),
            new PlayCardEffect("115", false, false),
            new PlayCardEffect("117", false, false),
            new PlayCardEffect("117", false, false),
        ], new DurationByExecutions(1), new IHaveBeenPlayedCondition())
    ],
    ConditionToPlay = new CountCardCondition(new() { Filter = new(), WhichDeckToSearch = PlayerType.PLAYER }, new(CountType.AT_MAX, 10))
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
            new DeckDto(
                11,
                "Misterioso adicto",
                "Prueba tu suerte. Deja la victoria en manos del azar, y juega monedas que pueden o no ganarte la partida. Perfecto para olvidarte de las estrategias."
            ),
            new()
            {
                {"96", 0},
                {"97", 0},
                {"98", 0},
                {"99", 0},
                {"100", 0},
                {"101", 0},
                {"102", 0},
                {"103", 0},
                {"104", 4},
                {"105", 2},
                {"106", 3},
                {"107", 3},
                {"108", 3},
                {"109", 4},
                {"110", 2},
                {"111", 1},
                {"112", 4},
                {"113", 1},
                {"114", 3},
            }
        },
        {
            
            new DeckDto(
                10,
                "Tecnomago",
                "Escala el poder de tus cartas poco a poco mientras utilizas virus y copias de seguridad para mantenerte vivo en la partida. Perfecto para jugadores experimentados."
            ),
            new()
            {
                //32
                {"70", 2},
                {"71", 0},
                {"72", 0},
                {"73", 0},
                {"74", 0},
                {"75", 0},
                {"76", 0},
                {"77", 0},
                {"78", 0},
                {"79", 0},
                {"80", 0},
                {"81", 0},
                {"82", 0},
                {"83", 0},
                {"84", 0},
                {"85", 3},
                {"86", 2},
                {"87", 4},
                {"88", 3},
                {"90", 2},
                {"91", 3},
                {"92", 2},
                {"93", 3},
                {"94", 3},
                {"95", 3},
            }
        },
        {
            new DeckDto(
                9,
                "Archimago del invierno",
                "Niega el ataque de las unidades enemigas, controla el terreno de batalla y no dejes que tu rival te abrume. Perfecto para gente con ansiedad.") ,
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
                {"64", 3},
                {"65", 0},
            }
        },
        {
            new DeckDto(7,
            "Don Bola de Fuego Jr",
            "Tu fuerza reside en los hechizos. Acaba con las unidades del enemigo sin darles opción a atacar, y ve drenando la vida del rival poco a poco. Perfecto para quienes les gusta controlar la partida."),
            new()
            {
                //32
                {"27", 4},
                {"28", 3},
                {"29", 2},
                {"30", 3},
                {"31", 2},
                {"32", 2},
                {"33", 2},
                {"34", 2},
                {"35", 2},
                {"36", 3},
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
            "Infla de ratas el mazo rival, potencia las tuyas propias y hazle caer en la desesperación. Perfecto para quien quiera pasárselo bien."),
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
                "Combina los efectos de tus unidades para crear criaturas peligrosas. Encuentra a Don Quijote y destapa la locura. Perfecto para un estilo de bola de nieve."
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
            new DeckDto(8, "Mago cachas", "Entrena con tus cartas y ponlas fuertes. No le des tiempo a tu rival para responder y la victoria será tuya. Perfecto para quienes quieren tener una victoria rápida."),
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
        },
        {
            new DeckDto(12, "El Alquimista Cromático", "Domina el ciclo de los colores: rojo golpea, verde sana y azul roba cartas. Rota o mezcla los colores en el momento justo para desatar su máximo potencial."),
            new()
            {
                //29
                {"115", 5},
                {"116", 3},
                {"117", 5},
                {"118", 2},
                {"119", 4},
                {"120", 3},
                {"121", 3},
                {"122", 2},
                {"123", 2},
            }
        }


    };

}