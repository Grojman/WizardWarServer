// TODO: AHORA QUE SE TIENE EN CUENTA QUE PUEDEN HABER MÁS DE UN JUGADOR, TIENE QUE HABER UNA MANERA PARA LOS HECHIZOS DE TENER UN TARGET O ALGO POR EL ESTILO
// IDEA: EN EL CLIENTE CONSTANTEMENTE HAY UNA FLECHITA APUNTANDO A TU TARGET, LA CUAL PUEDES CAMBIAR SI QUIERES ANTES DE HACER CUALQUIER JUGADA


public class GameState
{
    const int INITIAL_HAND = 3;
    public GameActionResult GameActionResult { get; set; }
    public List<PlayerState> Players { get; set; }
    List<PlayerState> DeadPlayers { get; set; } = new();
    List<PlayerState> AlivePlayers { get; set; } = new();

    public int CurrentPlayerIndex { get; set; } = 0;

    public int TurnCounter { get; set; }

    public GameState()
    {
        GameActionResult = new() { State = this };
    }

    public void Initialize(
        IEnumerable<PlayerConnection> connections
    )
    {
        var players = new List<PlayerState>();
        foreach(var con in connections)
        {
            var deck = CardManager.GetById(con.SelectedDeckId);

            var definitions = CardManager.GetDefinitionsByDeck(con.SelectedDeckId);

            var player = new PlayerState()
            {
                Id = con.Guid,
                Name = con.Name,
                Connection = con,
                Deck = null
            };

            player.Deck = new Deck(deck.name, definitions, player, deck.id);

            players.Add(player);
        }

        for(int i = 0; i < players.Count - 1; i++)
        {
            players[i].PlayerTarget = players[i + 1];
        }

        players[^1].PlayerTarget = players[0];

        Players = players;
        AlivePlayers = [.. Players];
        Players.ElementAt(0).IsMyTurn = true;

        for (int i = 0; i < INITIAL_HAND; i++)
        {
            DrawCard(); 
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
        }  else
        {
            var card = cardIndex == state.Board.Length ? state.LastSpellPlayed : state.Board[cardIndex];
            
            foreach(var e in card?.SpecialEffects ?? []) e.Execute(state.Id, state.PlayerTarget.Id, card, this, null);

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

    public void ChangeTarget(PlayerConnection player, Guid target)
    {
        var state = GetState(player.Guid);

        var newTarget = Players.First(n => n.Id == target);
        state.PlayerTarget = newTarget;

        GameActionResult.AddEvent(new GameEvent.TargetPlayerChanged()
        {
            Source = state,
            PlayerSource = state,
            NewTarget = target
        });
    }

    public void ApplyAction(
        PlayerConnection player,
        PlayerAction action)
    {
        switch (action)
        {
            case PlayerAction.ChangeTarget a:
                ChangeTarget(player, a.NewTarget);
                return;
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
                        a.PlayerTarget,
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

    public PlayerState GetState(Guid id) => Players.First(n => n.Id == id);
    public IEnumerable<PlayerState> GetRivals(Guid id) => Players.Where(n => n.Id != id);

    void NextTurn()
    {
        Players.ElementAt(CurrentPlayerIndex).IsMyTurn = false;
        CurrentPlayerIndex = CurrentPlayerIndex >= Players.Count() - 1 ? 0 : CurrentPlayerIndex + 1;
        Players.ElementAt(CurrentPlayerIndex).IsMyTurn = true;

        if (CurrentPlayerIndex == 0)
        {
            ApplyEffect(TriggerType.TurnEnd, null);

            TurnCounter++;

            if (TurnCounter % 2 == 0)
            {
                DrawCard();
            }
        }
        CleanExpiredEffects();
    }

    public void DrawCard(CardFilter? filter = null)
    {
        foreach(var p in Players) DrawCard(p.Connection, filter);
    }

    public void KillPlayer(PlayerState state)
    {
        //¿Qué pasa si se elimina el jugador justo cuando es su turno?
        DeadPlayers.Add(state);
        AlivePlayers.Remove(state);

        var gevent = new GameEvent.PlayerDeath()
        {
            PlayerSource = state,
            Source = state
        };

        GameActionResult.AddEvent(gevent);


        if (AlivePlayers.Count == 1)
        {
            GameActionResult.GameEnded = true;
            GameActionResult.Winner = AlivePlayers[0].Id;
        }
    }

    public void DrawCard(PlayerConnection p, CardFilter? filter = null)
    {
        var player = GetState(p.Guid);
        
        var card = filter is null ? player.Deck.Draw() : player.Deck.Draw(filter);

        if(card is null)
        {
            KillPlayer(player);
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
        Guid targetedPlayerId,
        int attacker,
        TargetType targetType,
        int target)
    {
        var player = GetState(p.Guid);
        var targetedPlayer = GetState(targetedPlayerId);
        var card = player.Board[attacker];
        if (card is null)
        {
            Console.WriteLine($"WTF. Attacking with unexisting card");
        } else
        {
            var gevent = new GameEvent.CardAttacked()
            {
                PlayerSource = player,
                PlayerTarget = targetedPlayer,
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
                    AlterPlayerHealth(card, targetedPlayer, -card.CurrentAttack, false);

                    break;
                case TargetType.BOARD:
                    var cardTarget = targetedPlayer.Board[target];
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
        foreach(var player in Players)
        {
            foreach(EffectInstance e in player.GlobalEffects)
            {
                if(e.Trigger == type)
                {
                    e.TryExecute(this, ev);
                }
            }
        }

        foreach(var player in Players)
        {
            foreach(CardInstance? e in player.Board)
            {
                if (e is not null) ApplyEffect(e, type, ev);    
            }
        }
    }

    void CleanExpiredEffects()
    {
        foreach(var player in Players)
        {
            player.GlobalEffects.RemoveAll(n => n.Expired);
            foreach(CardInstance? e in player.Board) e?.Effects.RemoveAll(n => n.Expired);
        }
    }

    public void AlterUnitHealth(IdentificableObject source, CardInstance Unit, int Amount, bool checkKill = true, bool enqueueToUsers = true)
    {
        Unit.CurrentHealth += Amount;
        var gevent = new GameEvent.UnitHealthChanged()
        {
            PlayerSource = Unit.Player,
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
            PlayerSource = Unit.Player,
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
            KillPlayer(player);
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
            PlayerSource = Unit.Player,
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
        var player = unit.Player;

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