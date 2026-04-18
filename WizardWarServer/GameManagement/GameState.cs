public class GameState
{
    const int INITIAL_HAND = 3;
    public List<EffectInstance> GlobalEffects{ get; } = new();
    public GameActionResult GameActionResult { get; set; } = new();
    public PlayerState Player1 { get; set; }
    public PlayerState Player2 { get; set; }
    public int CurrentTurn { get; set; }

    public void Initialize(
        PlayerConnection c1,
        PlayerConnection c2
    )
    {
        var deck1 = CardManager.GetById(c1.SelectedDeckId);
        var deck2 = CardManager.GetById(c2.SelectedDeckId);

        var dD1 = CardManager.GetDefinitionsByDeck(c1.SelectedDeckId);
        var dD2 = CardManager.GetDefinitionsByDeck(c2.SelectedDeckId);

        Player1 = new PlayerState()
        {
            Id = c1.Guid,
            Name = c1.Name,
            Connection = c1,
            Deck = new Deck(deck1.name, dD1, c1.Guid)
        };
        Player2 = new PlayerState()
        {
            Id = c2.Guid,
            Name = c2.Name,
            Connection = c2,
            Deck = new Deck(deck2.name, dD2, c2.Guid)
        };

        GameActionResult.Events.Enqueue(
            new GameEvent.PlayerHealthChanged()
            {
                PlayerSource = Player1,
                PlayerId = Player1.Id,
                Source = Player1,
                Amount = Player1.Health
            }
        );

        GameActionResult.Events.Enqueue(
            new GameEvent.PlayerHealthChanged()
            {
                PlayerSource = Player2,
                PlayerId = Player2.Id,
                Source = Player2,
                Amount = Player2.Health
            }
        );

        CurrentTurn = 1;

        for (int i = 0; i < INITIAL_HAND; i++)
        {
            DrawCard(c1);
            DrawCard(c2);
        }
    }

    public void ApplyAction(
        PlayerConnection player,
        PlayerAction action)
    {
        switch (action)
        {
            case PlayerAction.DrawCardAction:
                DrawCard(player);
                break;

            case PlayerAction.PlayCardAction a:
                PlayCard(player, a.CardIndex, a.BoardIndex);
                break;

            case PlayerAction.AttackAction a:
                Attack(player,
                       a.AttackerIndex,
                       a.TargetType,
                       a.TargetIndex);
                break;
            default:
                Console.WriteLine($"Illegal action inside game!! : {action}");
                break;
        }

        NextTurn();
    }

    public PlayerState GetState(Guid id) => Player1.Id == id ? Player1 : Player2;
    public PlayerState GetRival(Guid id) => Player1.Id == id ? Player2 : Player1;

    void NextTurn()
    {
        CurrentTurn =
            CurrentTurn == 1 ? 2 : 1;

        if (CurrentTurn == 1)
        {
            ApplyEffect(TriggerType.TurnEnd, null);
        }
        CleanExpiredEffects();
    }

    public void DrawCard(PlayerConnection p)
    {
        var player = GetState(p.Guid);
        
        var card = player.Deck.Draw();

        if(card is null)
        {
            GameActionResult.GameEnded = true;
            GameActionResult.Winner = GetRival(player.Id).Id;
            GameActionResult.Events.Enqueue(new GameEvent.DeckOutOfCards()
            {
                PlayerSource = player,
                Source = player
            });
        } else
        {
            player.Hand.Add(card);
            var gevent = new GameEvent.CardDrawnEvent()
            {
                PlayerSource = player,
                Source = player,
                CardInstance = card,
                PlayerId = player.Id
            };
            GameActionResult.Events.Enqueue(gevent);
            ApplyEffect(TriggerType.DrawCard, gevent);
        }
    }

    void PlayCard(PlayerConnection p, int handIndex, int boardIndex)
    {
        var player = GetState(p.Guid);
        if (player.Hand.Count <= handIndex)
        {
            Console.WriteLine($"WTF: Hand size smaller than index");
        } else
        {
            var card = player.GetFromHand(handIndex);

            if (card.Definition.Type == CardType.Unit)
            {
                if (player.Board[boardIndex] is not null)
                {
                    Console.WriteLine($"WTF: setting boardIndex when that place is occupied");
                } 
                player.Board[boardIndex] = card;
                var gevent = new GameEvent.UnitPlayed()
                {
                    PlayerSource = player,
                    Source = player,
                    BoardPosition = boardIndex,
                    Unit = card
                };
                GameActionResult.Events.Enqueue(gevent);
                ApplyEffect(TriggerType.UnitPlayed, gevent);
            } else
            {
                var gevent = new GameEvent.SpellPlayed()
                {
                    PlayerSource = player,
                    Source = player,
                    Spell = card
                };
                GameActionResult.Events.Enqueue(gevent);
                ApplyEffect(card, TriggerType.SpellPlayed, gevent);
                ApplyEffect(TriggerType.SpellPlayed, gevent);   
            }
        }
    }

    void Attack(
        PlayerConnection p,
        int attacker,
        TargetType targetType,
        int target)
    {
        var player = GetState(p.Guid);
        var card = player.Board[attacker];
        if (card is null)
        {
            Console.WriteLine($"WTF. Attacking with unexisting card");
        } else
        {
            var gevent = new GameEvent.CardAttacked()
            {
                PlayerSource = player,
                Source = card,
                Attacker = card,
                TargetIndex = target,
                TargetType = targetType
            };
            GameActionResult.Events.Enqueue(gevent);
            switch (targetType)
            {
                case TargetType.PLAYER:
                    AlterPlayerHealth(card, player, -card.CurrentAttack);
                    break;
                case TargetType.RIVAL:
                    AlterPlayerHealth(card, GetRival(player.Id), -card.CurrentAttack);
                    break;
                case TargetType.ENEMY_BOARD:
                    var cardTarget = GetRival(player.Id).Board[target];
                    if(cardTarget is null)
                    {
                        Console.WriteLine($"WTF. Attacking a card that doesnt exists on rival board");
                    } else
                    {
                        AlterUnitHealth(card, cardTarget, -card.CurrentAttack);
                    }
                    break;
                case TargetType.OWN_BOARD:
                    var cardTarget2 = player.Board[target];
                    if(cardTarget2 is null)
                    {
                        Console.WriteLine($"WTF. Attacking a card that doesnt exists on own board");
                    } else
                    {
                        AlterUnitHealth(card, cardTarget2, -card.CurrentAttack);
                    }
                    break;
                default:
                    Console.WriteLine($"WTF. Unexisting TargetType value");
                    break;
            }
        }
    }


    public void ApplyEffect(CardInstance card, TriggerType type, GameEvent? ev)
    {
        foreach(EffectInstance ei in card.Effects)
        {
            if(ei.Trigger == type)
            {
                ei.TryExecute(this, ev);
            }
        }   
    }

    void ApplyEffect(TriggerType type, GameEvent? ev)
    {
        foreach(EffectInstance e in GlobalEffects)
        {
            if(e.Trigger == type)
            {
                e.TryExecute(this, ev);
            }
        }

        foreach(CardInstance? e in Player1.Board)
        {
            if (e is not null) ApplyEffect(e, type, ev);    
        }
        

        foreach(CardInstance? e in Player2.Board)
        {
            if (e is not null) ApplyEffect(e, type, ev);    
        }
    }

    void CleanExpiredEffects()
    {
        GlobalEffects.RemoveAll(n => n.Expired);
        foreach(CardInstance? e in Player1.Board) e?.Effects.RemoveAll(n => n.Expired);
        foreach(CardInstance? e in Player2.Board) e?.Effects.RemoveAll(n => n.Expired);
    }


    public void AlterUnitHealth(IdentificableObject source, CardInstance Unit, int Amount)
    {
        Unit.CurrentHealth += Amount;
        var gevent = new GameEvent.UnitHealthChanged()
        {
            PlayerSource = GetState(Unit.PlayerGuid),
            Card = Unit,
            Source = source,
            Amount = Amount
        };

        GameActionResult.Events.Enqueue(gevent);
        ApplyEffect(TriggerType.UnitHealthChanged, gevent);

        if (Unit.CurrentHealth <= 0)
        {
            KillUnit(source, Unit);
        }
    }

    public void AlterUnitDamage(IdentificableObject source, CardInstance Unit, int Amount)
    {
        Unit.CurrentAttack += Amount;
        var gevent = new GameEvent.UnitDamageChanged()
        {
            PlayerSource = GetState(Unit.PlayerGuid),
            Card = Unit,
            Source = source,
            Amount = Amount
        };

        GameActionResult.Events.Enqueue(gevent);
        ApplyEffect(TriggerType.UnitDamageChanged, gevent);
    }

    public void AlterPlayerHealth(IdentificableObject source, PlayerState player, int Amount)
    {
        player.Health += Amount;
        var gevent = new GameEvent.PlayerHealthChanged()
        {
            PlayerSource = player,
            Source = source,
            PlayerId = player.Id,
            Amount = Amount
        };

        if (player.Health <= 0)
        {
            GameActionResult.Winner = GetRival(player.Id).Id;
            GameActionResult.GameEnded = true;
        }

        GameActionResult.Events.Enqueue(gevent);
        ApplyEffect(TriggerType.PlayerHealthChanged, gevent);
    }

    public void KillUnit(IdentificableObject source, CardInstance Unit)
    {
        var position = RemoveFromBoard(Unit);
        var gevent = new GameEvent.UnitDeath()
        {
            PlayerSource = GetState(Unit.PlayerGuid),
            Source = source,
            Unit = Unit,
            BoardPosition = position
        };

        foreach(EffectInstance e in Unit.Effects) if (e.Trigger == TriggerType.UnitDeath) e.TryExecute(this, gevent);

        GameActionResult.Events.Enqueue(gevent);
        ApplyEffect(TriggerType.UnitDeath, gevent);
    }

    int RemoveFromBoard(CardInstance unit)
    {
        var player = Player1.Id == unit.PlayerGuid ? Player1 : Player2;

        for(int i = 0; i < player.Board.Length; i++)
        {
            if (player.Board[i]?.Id == unit.Id)
            {
                player.Board[i] = null;
                return i;
            }
        }

        return -1;
    }
}