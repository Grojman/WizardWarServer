public class GameState
{
    const int INITIAL_HAND = 3;
    public List<EffectInstance> GlobalEffects{ get; } = new();
    public GameActionResult GameActionResult { get; set; }
    public PlayerState Player1 { get; set; }
    public PlayerState Player2 { get; set; }
    public int CurrentTurn { get; set; }

    public int TurnCounter { get; set; }

    public GameState()
    {
        GameActionResult = new() { State = this };
    }

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
            Deck = new Deck(deck1.name, dD1, c1.Guid, deck1.id)
        };
        Player2 = new PlayerState()
        {
            Id = c2.Guid,
            Name = c2.Name,
            Connection = c2,
            Deck = new Deck(deck2.name, dD2, c2.Guid, deck2.id)
        };

        GameActionResult.AddEvent(
            new GameEvent.PlayerHealthChanged()
            {
                PlayerSource = Player1,
                PlayerId = Player1.Id,
                Source = Player1,
                Amount = Player1.Health
            }
        );

        GameActionResult.AddEvent(
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

    public void AddCard(PlayerState target, PlayerState playerSource, CardInstance cardToAdd, IdentificableObject Source)
    {
        var gevent = new GameEvent.AddedCardToDeck()
        {
            Source = Source,
            PlayerSource = playerSource,
            TargetedPlayer = target,
            Card = cardToAdd
        };

        target.Deck.AddCard(cardToAdd);

        GameActionResult.AddEvent(gevent);

        ApplyEffect(TriggerType.CardAddedToDeck, gevent);
    }


    public void AlterDeck(PlayerState target, IdentificableObject Source, PlayerState playerSource, IEnumerable<CardInstance> affectedCards)
    {
        var gevent = new GameEvent.DeckModifiedStats()
        {
            Source = Source,
            PlayerSource = playerSource,
            TargetedPlayer = target,
            AffectedCards = affectedCards
        };

        GameActionResult.AddEvent(gevent);

        ApplyEffect(TriggerType.DeckModified, gevent);
    }

    public void ApplyEffect(PlayerConnection player, int cardIndex)
    {
        var state = GetState(player.Guid);

        if (cardIndex != -1 && cardIndex >= state.Board.Length + 1)
        {
            Console.WriteLine("WTF. Index is not valid for an effect to play");
        } else if(state.Board[cardIndex]?.SpecialEffect is null)
        {
            Console.WriteLine("WTF. The card either doesnt exist or it has no play effect");
        } else
        {
            var card = cardIndex == state.Board.Length ? state.LastSpellPlayed : state.Board[cardIndex];
            card?.SpecialEffect?.Execute(state.Id, card, this, null);

            var gevent  = new GameEvent.CardEventPlayed()
            {
                Source = card,
                Card = card,
                PlayerSource = state,
            };

            GameActionResult.AddEvent(gevent);
            ApplyEffect(TriggerType.CardEffectPlayed, gevent);
        }
    }

    public void ApplyAction(
        PlayerConnection player,
        PlayerAction action)
    {
        switch (action)
        {
            case PlayerAction.CardEffectActivated a:
                ApplyEffect(player, a.CardIndex);
                return;
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

            TurnCounter++;

            if (TurnCounter % 2 == 0)
            {
                DrawCard(Player1.Connection);
                DrawCard(Player2.Connection);
            }
        }
        CleanExpiredEffects();
    }

    public void DrawCard(PlayerConnection p, CardFilter? filter = null)
    {
        var player = GetState(p.Guid);
        
        var card = filter is null ? player.Deck.Draw() : player.Deck.Draw(filter);

        if(card is null)
        {
            GameActionResult.GameEnded = true;
            GameActionResult.Winner = GetRival(player.Id).Id;
            GameActionResult.AddEvent(new GameEvent.DeckOutOfCards()
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
                Card = card,
                PlayerId = player.Id
            };
            GameActionResult.AddEvent(gevent);
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
            player.PlayedCards.Add(card);

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
                    Card = card
                };
                GameActionResult.AddEvent(gevent);
                ApplyEffect(TriggerType.UnitPlayed, gevent);
            } else
            {
                player.LastSpellPlayed = card;
                var gevent = new GameEvent.SpellPlayed()
                {
                    PlayerSource = player,
                    Source = player,
                    Card = card
                };
                GameActionResult.AddEvent(gevent);
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
                TargetType = targetType,
                Deffender = null
            };
            switch (targetType)
            {
                case TargetType.PLAYER:
                    GameActionResult.AddEvent(gevent);
                    ApplyEffect(TriggerType.CardAttacked, gevent);
                    AlterPlayerHealth(card, player, -card.CurrentAttack, false);

                    break;
                case TargetType.RIVAL:
                    GameActionResult.AddEvent(gevent);
                    ApplyEffect(TriggerType.CardAttacked, gevent);
                    AlterPlayerHealth(card, GetRival(player.Id), -card.CurrentAttack, false);
                    break;
                case TargetType.ENEMY_BOARD:
                    var cardTarget = GetRival(player.Id).Board[target];
                    gevent.Deffender = cardTarget;
                    GameActionResult.AddEvent(gevent);
                    ApplyEffect(TriggerType.CardAttacked, gevent);
                    if(cardTarget is null)
                    {
                        Console.WriteLine($"WTF. Attacking a card that doesnt exists on rival board");
                    } else
                    {
                        AlterUnitHealth(card, cardTarget, -card.CurrentAttack, false, false);
                        AlterUnitHealth(cardTarget, card, -cardTarget.CurrentAttack, false, false);
                        CheckKill(card, cardTarget);
                        CheckKill(cardTarget, card);
                    }
                    break;
                case TargetType.OWN_BOARD:
                    var cardTarget2 = player.Board[target];
                    gevent.Deffender = cardTarget2;
                    GameActionResult.AddEvent(gevent);
                    ApplyEffect(TriggerType.CardAttacked, gevent);
                    if(cardTarget2 is null)
                    {
                        Console.WriteLine($"WTF. Attacking a card that doesnt exists on own board");
                    } else
                    {
                        AlterUnitHealth(card, cardTarget2, -card.CurrentAttack, false, false);
                        AlterUnitHealth(cardTarget2, card, -cardTarget2.CurrentAttack, false, false);
                        CheckKill(card, cardTarget2);
                        CheckKill(cardTarget2, card);
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


    public void AlterUnitHealth(IdentificableObject source, CardInstance Unit, int Amount, bool checkKill = true, bool enqueueToUsers = true)
    {
        Unit.CurrentHealth += Amount;
        var gevent = new GameEvent.UnitHealthChanged()
        {
            PlayerSource = GetState(Unit.PlayerGuid),
            Card = Unit,
            Source = source,
            Amount = Amount
        };

        if (enqueueToUsers) GameActionResult.AddEvent(gevent);
        ApplyEffect(TriggerType.UnitHealthChanged, gevent);

        if (checkKill) CheckKill(source, Unit);
    }

    void CheckKill(IdentificableObject source, CardInstance Unit)
    {
        if (Unit.CurrentHealth <= 0)
        {
            KillUnit(source, Unit);
        }
    }

    public void AlterUnitDamage(IdentificableObject source, CardInstance Unit, int Amount)
    {
        if (Amount == 0) return;
        Unit.CurrentAttack += Amount;
        var gevent = new GameEvent.UnitDamageChanged()
        {
            PlayerSource = GetState(Unit.PlayerGuid),
            Card = Unit,
            Source = source,
            Amount = Amount
        };

        GameActionResult.AddEvent(gevent);
        ApplyEffect(TriggerType.UnitDamageChanged, gevent);
    }

    public void AlterPlayerHealth(IdentificableObject source, PlayerState player, int Amount, bool enqueueToUsers = true)
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

        if (enqueueToUsers) GameActionResult.AddEvent(gevent);
        ApplyEffect(TriggerType.PlayerHealthChanged, gevent);
    }

    public void KillUnit(IdentificableObject source, CardInstance Unit)
    {
        if (Unit.DeathChecked) return;
        Unit.DeathChecked = true;
        var position = RemoveFromBoard(Unit);
        var gevent = new GameEvent.UnitDeath()
        {
            PlayerSource = GetState(Unit.PlayerGuid),
            Source = source,
            Card = Unit,
            BoardPosition = position
        };

        foreach(EffectInstance e in Unit.Effects) if (e.Trigger == TriggerType.UnitDeath) e.TryExecute(this, gevent);

        GameActionResult.AddEvent(gevent);
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