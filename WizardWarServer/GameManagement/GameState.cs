using Serilog;
public class GameState
{
    public const int INITIAL_HAND = 3;
    public const int MAX_HAND = 10;
    public GameActionResult GameActionResult { get; set; }
    public List<PlayerState> Players { get; set; } = new();
    List<PlayerState> DeadPlayers { get; set; } = new();
    List<PlayerState> AlivePlayers { get; set; } = new();

    public PlayerState CurrentPlayer { get; private set; }

    public int TurnCounter { get; set; } = 1;

    DateTime Start;
    DateTime End;

    public int TotalInSeconds { get => (int)(End - Start).TotalSeconds; }

    public GameState()
    {
        GameActionResult = new() { State = this };
    }

    public void Initialize(
        IEnumerable<PlayerConnection> connections,
        Guid? forcedStarterId = null
    )
    {
        Start = DateTime.Now;
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

        var starter = forcedStarterId.HasValue
            ? players.First(p => p.Id == forcedStarterId.Value)
            : players[Random.Shared.Next(players.Count)];

        starter.IsMyTurn = true;
        CurrentPlayer = starter;

        for (int i = 0; i < INITIAL_HAND; i++)
        {
            DrawCard(null); 
        }
    }

    public void AddCard(PlayerState target, PlayerState playerSource, CardInstance cardToAdd, IdentificableObject Source, int topDeckDepth = 0)
    {
        var gevent = new GameEvent.AddedCardToDeck()
        {
            Source = Source,
            PlayerSource = playerSource,
            TargetedPlayer = target,
            Card = cardToAdd
        };

        target.Deck!.AddCard(cardToAdd, topDeckDepth);

        GameActionResult.AddEvent(gevent);

        ApplyEffect(TriggerType.CardAddedToDeck, gevent);
    }

    public void ClearState()
    {
        AlivePlayers.Clear();
        DeadPlayers.Clear();
        Players.ForEach(n => n.PlayerTarget = null);
        Players.Clear();
    }


    public void AlterDeck(PlayerState target, IdentificableObject Source, PlayerState playerSource, IEnumerable<CardInstance> affectedCards)
    {
        var gevent = new GameEvent.DeckModifiedStats()
        {
            TargetedPlayer = target,
            Source = Source,
            PlayerSource = playerSource,
            AffectedCards = affectedCards
        };

        GameActionResult.AddEvent(gevent);

        ApplyEffect(TriggerType.DeckModified, gevent);
    }

    public void ApplyEffect(PlayerConnection player, int cardIndex)
    {
        var state = GetState(player.Guid);

        if (cardIndex != -1 && (cardIndex < 0 || cardIndex > state.Board.Length))
        {
            Log.Warning("Invalid card index for an effect to play: {CardIndex}", cardIndex);
            return;
        }  else
        {
            var card = cardIndex == state.Board.Length ? state.LastSpellPlayed : state.Board[cardIndex];

            if(card is null || card.MaxSpecialEffectTimes <= 0)
            {
                return;
            }

            var activeCard = card;
            var gevent  = new GameEvent.CardEventPlayed()
            {
                Source = activeCard,
                Card = activeCard,
                PlayerSource = state,
            };

            GameActionResult.AddEvent(gevent);
            foreach(var e in activeCard.SpecialEffects ?? []) e.Execute(state.Id, state.PlayerTarget!.Id, activeCard, this, null);

            activeCard.MaxSpecialEffectTimes--;

            ApplyEffect(TriggerType.CardEffectPlayed, gevent);
        }
    }

    public void ChangeTarget(PlayerConnection player, Guid target)
    {
        var state = GetState(player.Guid);

        var newTarget = Players.First(n => n.Id == target);

        if (state.PlayerTarget!.Id != target)
        {
            state.PlayerTarget = newTarget;

            GameActionResult.AddEvent(new GameEvent.TargetPlayerChanged()
            {
                Source = state,
                PlayerSource = state,
                NewTarget = target
            });
        }
        
    }

    public void ApplyAction(
        PlayerConnection player,
        PlayerAction action)
    {
        try
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
                DrawCard(player, null);
                break;

            case PlayerAction.PlayCardAction a:
                PlayCard(player, a.CardId, a.BoardIndex);
                break;

            case PlayerAction.AttackAction a:
                Attack(player,
                        a.PlayerTarget,
                       a.AttackerIndex,
                       a.TargetType,
                       a.TargetIndex);
                break;
            default:
                Log.Warning("Illegal action inside game from player {PlayerId}: {Action}", player?.Guid, action);
                return;
            }
        }
        catch(Exception ex)
        {
            try
            {
                Log.Error(ex, "Error while applying action from player {PlayerId}", player?.Guid);
            }
            catch {}
            return;
        }

        NextTurn();
    }

    public PlayerState GetState(Guid id) => Players.First(n => n.Id == id);
    public IEnumerable<PlayerState> GetRivals(Guid id) => Players.Where(n => n.Id != id);

    void NextTurn()
    {
        var idx = AlivePlayers.IndexOf(CurrentPlayer);

        CurrentPlayer.IsMyTurn = false;

        idx = (idx + 1) % AlivePlayers.Count;

        CurrentPlayer = AlivePlayers[idx];
        CurrentPlayer.IsMyTurn = true;

        if (idx == 0)
        {
            ApplyEffect(TriggerType.TurnEnd, null);

            TurnCounter++;

            if (TurnCounter % 2 == 0)
            {
                DrawCard(null);
            }
        }
        CleanExpiredEffects();
    }

    public void DrawCard(IdentificableObject? source, CardFilter? filter = null)
    {
        for (int i = 0; i < AlivePlayers.Count && !GameActionResult.GameEnded;)
        {
            var player = AlivePlayers[i];
            DrawCard(player.Connection, source, filter);

            // Si el jugador sigue ocupando esta posición, avanzamos.
            if (i < AlivePlayers.Count && ReferenceEquals(AlivePlayers[i], player))
                i++;
        }
    }

    public void KillPlayer(PlayerState state, bool forceChangeTurn = false)
    {
        state.IsMyTurn = false;
        DeadPlayers.Add(state);
        AlivePlayers.Remove(state);

        var gevent = new GameEvent.PlayerDeath()
        {
            PlayerSource = state,
            Source = state
        };

        GameActionResult.AddEvent(gevent);

        foreach (var p in AlivePlayers)
        {
            if (p.PlayerTarget != state) continue;

            var next = state.PlayerTarget;
            while (next is not null && next != p && !AlivePlayers.Contains(next))
            {
                next = next.PlayerTarget;
            }

            if (next is null || next == p) continue;

            p.PlayerTarget = next;

            GameActionResult.AddEvent(new GameEvent.TargetPlayerChanged()
            {
                Source = p,
                PlayerSource = p,
                NewTarget = next.Id
            });
        }

        if(forceChangeTurn && state.IsMyTurn)
        {
            NextTurn();
        }


        if (AlivePlayers.Count == 1)
        {
            GameActionResult.GameEnded = true;
            End = DateTime.Now;
            GameActionResult.Winner = AlivePlayers[0].Id;
        }
    }

    public void DrawCard(PlayerState player, CardInstance card,  IdentificableObject? source, bool fromDeck = false)
    {
        if (player.Hand.Count >= MAX_HAND) return;

        player.Hand.Add(card);
        var gevent = new GameEvent.CardDrawnEvent()
        {
            PlayerSource = player,
            Source = source ?? player,
            Card = card,
            PlayerId = player.Id,
            FromDeck = fromDeck
        };
        GameActionResult.AddEvent(gevent);
        ApplyEffect(TriggerType.DrawCard, gevent);
    }
    public void DrawCard(PlayerConnection p, IdentificableObject? source, CardFilter? filter = null)
    {
        var player = GetState(p.Guid);

        if (player.Hand.Count >= MAX_HAND) return;

        var card = filter is null ? player.Deck!.Draw() : player.Deck!.Draw(filter);

        if(card is null && player.Deck.Count == 0)
        {
            KillPlayer(player);
        } else if (card is not null)
        {
            DrawCard(player, card, source, true);
        }
    }

    public void PlayCard(PlayerState player, CardInstance card, int boardIndex)
    {
        player.PlayedCards.Add(card);

        if (card.Definition.Type == CardType.Unit)
        {
            if (player.Board[boardIndex] is not null)
            {
                Log.Warning("Setting boardIndex {BoardIndex} when that place is already occupied", boardIndex);
            } 
            player.Board[boardIndex] = card;
            var gevent = new GameEvent.UnitPlayed()
            {
                PlayerSource = player,
                Source = player,
                BoardPosition = boardIndex,
                Card = card,
                // Captured now, before the card's own on-play effects (which
                // can alter its stats) run below.
                PlacedAttack = card.CurrentAttack,
                PlacedHealth = card.CurrentHealth
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

    public void PlayCard(PlayerConnection p, Guid cardId, int boardIndex)
    {
        var player = GetState(p.Guid);

        if (boardIndex != -1 && (boardIndex < 0 || boardIndex >= player.Board.Length))
        {
            Log.Warning("Player {PlayerId} sent a board index out of range: {BoardIndex}", p.Guid, boardIndex);
            return;
        }

        var handCard = player.Hand.FirstOrDefault(c => c.Id == cardId);
        if (handCard is null)
        {
            Log.Warning("Player {PlayerId} tried to play a card not in their hand: {CardId}", p.Guid, cardId);
            return;
        }

        if (handCard.CanPlay is not null && !handCard.CanPlay.Check(player.Id, player.PlayerTarget!.Id, handCard, this, null))
        {
            Log.Warning("Player {PlayerId} tried to play card {CardId} without meeting its play condition", p.Guid, cardId);
            return;
        }

        var card = player.GetFromHand(cardId)!;

        PlayCard(player, card, boardIndex);
    }

    public void Attack(
        PlayerConnection p,
        Guid targetedPlayerId,
        int attacker,
        TargetType targetType,
        int target)
    {
        var player = GetState(p.Guid);
        var targetedPlayer = GetState(targetedPlayerId);

        if (attacker < 0 || attacker >= player.Board.Length)
        {
            Log.Warning("Player {PlayerId} attacked with an invalid board index: {AttackerIndex}", p.Guid, attacker);
            return;
        }

        var card = player.Board[attacker];
        if (card is null)
        {
            Log.Warning("Player {PlayerId} attacked with a nonexistent card at index {AttackerIndex}", p.Guid, attacker);
        } else
        {
            ChangeTarget(player.Connection, targetedPlayerId);            

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
                    AlterPlayerHealth(card, targetedPlayer, -card.CurrentAttack, false);
                    ApplyEffect(TriggerType.CardAttacked, gevent);
                    break;
                case TargetType.BOARD:
                    if (target < 0 || target >= targetedPlayer.Board.Length)
                    {
                        Log.Warning("Player {PlayerId} targeted a board index that doesn't exist on the rival board: {TargetIndex}", p.Guid, target);
                        return;
                    }

                    var cardTarget = targetedPlayer.Board[target];
                    gevent.Deffender = cardTarget;
                    GameActionResult.AddEvent(gevent);
                    if(cardTarget is null)
                    {
                        Log.Warning("Player {PlayerId} attacked a card that doesn't exist on the rival board at index {TargetIndex}", p.Guid, target);
                    } else
                    {
                        int damage = -card.CurrentAttack;
                        int targetDamage = -cardTarget.CurrentAttack;
                        AlterUnitHealth(card, cardTarget, damage, false, false);
                        AlterUnitHealth(cardTarget, card, targetDamage, false, false);
                        CheckKill(card, cardTarget);
                        CheckKill(cardTarget, card);
                    }
                    ApplyEffect(TriggerType.CardAttacked, gevent);
                    break;
                default:
                    Log.Warning("Player {PlayerId} sent an unknown TargetType value: {TargetType}", p.Guid, targetType);
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
            if (player.Health <= 0) return;
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
        if (checkKill) CheckKill(source, Unit);
        ApplyEffect(TriggerType.UnitHealthChanged, gevent);
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
        if (Amount == 0 || (Amount < 0 && Unit.CurrentAttack == 0)) return;
        Amount = Unit.CurrentAttack + Amount < 0 ? -Unit.CurrentAttack : Amount;
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

    public void SetPlayerColor(IdentificableObject source, PlayerState player, ChromaticColor? oldColor, ChromaticColor newColor)
    {
        var gevent = new GameEvent.PlayerColorChanged()
        {
            PlayerSource = player,
            Source = source,
            PlayerId = player.Id,
            OldColor = oldColor,
            NewColor = newColor
        };

        GameActionResult.AddEvent(gevent);
        ApplyEffect(TriggerType.ColorChanged, gevent);
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