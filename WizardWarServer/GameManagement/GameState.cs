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

    public IReadOnlyList<PlayerState> AlivePlayersView => AlivePlayers;
    public bool IsAlive(PlayerState player) => AlivePlayers.Contains(player);

    // Games with more than two players don't let anyone pick a target: each
    // round one alive player is "hunted" (random on the first round, then
    // rotating in seat order). The hunted player targets every other alive
    // player and all of them target the hunted one (see RecomputeTargets).
    // A 1v1 keeps both players targeting each other.
    public bool UsesHuntedTargeting => Players.Count > 2;
    public PlayerState? HuntedPlayer { get; private set; }

    public int TurnCounter { get; set; } = 1;

    // Set for the duration of a global (PlayerState.GlobalEffects) EffectInstance's
    // execution by EffectInstance.TryExecute/ForceExecute. Every mutation method
    // below that takes an explicit `source` defers to it via ResolveSource, so a
    // recurring aura's side effects (unit stats, player health, ...) are attributed
    // back to the aura itself rather than to whatever card originally granted it
    // (which may be long dead) — that's what lets the client animate the resulting
    // particle from the global effect's own badge (see PlayerComponent).
    public EffectInstance? CurrentGlobalEffectSource { get; set; }

    IdentificableObject ResolveSource(IdentificableObject source) => CurrentGlobalEffectSource ?? source;

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

        Players = players;
        AlivePlayers = [.. Players];

        if (UsesHuntedTargeting)
        {
            HuntedPlayer = players[Random.Shared.Next(players.Count)];
        }
        RecomputeTargets(emitEvents: false);

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
        Players.ForEach(n => n.Targets.Clear());
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

        if (cardIndex < 0 || cardIndex > state.Board.Length)
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
            EffectFanOut.Execute(activeCard.SpecialEffects ?? [], state, [.. state.Targets.Players], activeCard, this, null);

            activeCard.MaxSpecialEffectTimes--;

            ApplyEffect(TriggerType.CardEffectPlayed, gevent);
        }
    }

    // Targets are decided by the game itself now (see RecomputeTargets): a 1v1
    // can only ever target the other player, and bigger games follow the
    // hunted-player rotation. Kept so older clients sending it don't error out.
    public void ChangeTarget(PlayerConnection player, Guid target)
    {
        Log.Debug("Ignoring manual target change from player {PlayerId}: targets are assigned by the game", player.Guid);
    }

    // Recomputes every player's TargetGroup from the current alive players and
    // hunted player, emitting a TargetPlayerChanged for each group that changed.
    void RecomputeTargets(bool emitEvents)
    {
        foreach (var p in Players)
        {
            IEnumerable<PlayerState> targets;

            if (Players.Count == 1)
                targets = [p];
            else if (!UsesHuntedTargeting)
                targets = Players.Where(o => o != p);
            else if (!AlivePlayers.Contains(p) || HuntedPlayer is null)
                targets = [];
            else if (p == HuntedPlayer)
                targets = AliveInSeatOrderAfter(p);
            else
                targets = [HuntedPlayer];

            if (p.Targets.Set(targets) && emitEvents)
            {
                GameActionResult.AddEvent(new GameEvent.TargetPlayerChanged()
                {
                    Source = p,
                    PlayerSource = p,
                    NewTarget = p.Targets.Primary?.Id ?? Guid.Empty,
                    NewTargets = [.. p.Targets.Players.Select(t => t.Id)]
                });
            }
        }
    }

    // Every other alive player, starting from the one seated right after `player`.
    IEnumerable<PlayerState> AliveInSeatOrderAfter(PlayerState player)
    {
        int start = Players.IndexOf(player);
        for (int step = 1; step < Players.Count; step++)
        {
            var candidate = Players[(start + step) % Players.Count];
            if (AlivePlayers.Contains(candidate)) yield return candidate;
        }
    }

    // A card's play condition is met if it holds against any of its owner's targets.
    public bool CanPlay(CardInstance card)
    {
        if (card.CanPlay is null) return true;
        return card.Player.Targets.Players.Any(t => card.CanPlay.Check(card.Player.Id, t.Id, card, this, null));
    }

    // Returns false (and changes nothing) when the action isn't allowed right
    // now, e.g. it isn't that player's turn.
    public bool ApplyAction(
        PlayerConnection player,
        PlayerAction action)
    {
        var actor = Players.FirstOrDefault(n => n.Id == player.Guid);
        if (actor is null || GameActionResult.GameEnded) return false;

        if (action is not PlayerAction.ChangeTarget && actor != CurrentPlayer)
        {
            Log.Warning("Player {PlayerId} sent {Action} outside of their turn", player.Guid, action.GetType().Name);
            return false;
        }

        try
        {
            switch (action)
            {
            case PlayerAction.ChangeTarget a:
                ChangeTarget(player, a.NewTarget);
                return true;
            case PlayerAction.CardEffectActivated a:
                ApplyEffect(player, a.CardIndex);
                return true;
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
                return false;
            }
        }
        catch(Exception ex)
        {
            try
            {
                Log.Error(ex, "Error while applying action from player {PlayerId}", player?.Guid);
            }
            catch {}
            return true;
        }

        NextTurn();
        return true;
    }

    public PlayerState GetState(Guid id) => Players.First(n => n.Id == id);
    public IEnumerable<PlayerState> GetRivals(Guid id) => Players.Where(n => n.Id != id);

    // Hands the turn to the next alive player in seat order. Works even if the
    // current player has just died (deck out, surrender, disconnect...), which
    // used to send the turn back to the first player and fire a bogus round end.
    void NextTurn()
    {
        if (GameActionResult.GameEnded || AlivePlayers.Count == 0) return;

        var previous = CurrentPlayer;
        previous.IsMyTurn = false;

        int start = Players.IndexOf(previous);
        bool roundEnded = false;
        PlayerState next = previous;
        for (int step = 1; step <= Players.Count; step++)
        {
            if (start + step >= Players.Count) roundEnded = true;

            var candidate = Players[(start + step) % Players.Count];
            if (AlivePlayers.Contains(candidate))
            {
                next = candidate;
                break;
            }
        }

        CurrentPlayer = next;
        CurrentPlayer.IsMyTurn = true;

        if (roundEnded)
        {
            ApplyEffect(TriggerType.TurnEnd, null);

            TurnCounter++;

            if (UsesHuntedTargeting && HuntedPlayer is not null && !GameActionResult.GameEnded)
            {
                HuntedPlayer = AliveInSeatOrderAfter(HuntedPlayer).FirstOrDefault() ?? HuntedPlayer;
                RecomputeTargets(emitEvents: true);
            }

            if (TurnCounter % 2 == 0)
            {
                DrawCard(null);
            }
        }
        CleanExpiredEffects();

        // A round-end effect may have killed whoever was about to play.
        if (!GameActionResult.GameEnded && !AlivePlayers.Contains(CurrentPlayer))
        {
            NextTurn();
        }
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

    public void Surrender(PlayerState player)
    {
        if (GameActionResult.GameEnded) return;

        KillPlayer(player, DefeatReason.Surrender, forceChangeTurn: true);
    }

    // forceChangeTurn: pass the turn on if it was this player's (surrender,
    // disconnect). Deaths in the middle of an action don't need it: the action
    // itself ends with NextTurn.
    public void KillPlayer(PlayerState state, DefeatReason reason, bool forceChangeTurn = false)
    {
        if (!AlivePlayers.Contains(state)) return;

        var hadTurn = state.IsMyTurn;

        state.IsMyTurn = false;
        state.DefeatReason = reason;
        DeadPlayers.Add(state);
        AlivePlayers.Remove(state);

        var gevent = new GameEvent.PlayerDeath()
        {
            PlayerSource = state,
            Source = state
        };

        GameActionResult.AddEvent(gevent);

        if (AlivePlayers.Count <= 1)
        {
            GameActionResult.GameEnded = true;
            End = DateTime.Now;
            GameActionResult.Winner = AlivePlayers.FirstOrDefault()?.Id;
            return;
        }

        if (HuntedPlayer == state)
        {
            HuntedPlayer = AliveInSeatOrderAfter(state).FirstOrDefault();
        }
        RecomputeTargets(emitEvents: true);

        if (forceChangeTurn && hadTurn)
        {
            NextTurn();
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
            KillPlayer(player, DefeatReason.OutOfCards);
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

        if (!CanPlay(handCard))
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
        var targetedPlayer = Players.FirstOrDefault(n => n.Id == targetedPlayerId);

        if (targetedPlayer is null || targetedPlayer == player || !AlivePlayers.Contains(targetedPlayer))
        {
            Log.Warning("Player {PlayerId} attacked an invalid player: {TargetId}", p.Guid, targetedPlayerId);
            return;
        }

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
            var gevent = new GameEvent.CardAttacked()
            {
                PlayerSource = player,
                PlayerTarget = targetedPlayer,
                Source = card,
                Attacker = card,
                TargetIndex = target,
                TargetType = targetType,
                Deffender = null,
                AttackerDamage = card.CurrentAttack,
                DefenderDamage = 0
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
                    gevent.DefenderDamage = cardTarget?.CurrentAttack ?? 0;
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
        if (card.DeathChecked) return;

        foreach(EffectInstance ei in card.Effects)
        {
            if(ei.Trigger == type)
            {
                ei.TryExecute(this, ev);
            }
        }   
    }

    // Dead players' auras and units no longer react to anything. This used to
    // `return` on the first dead player found, which in games with more than
    // two players silently disabled every effect of everyone listed after them.
    void ApplyEffect(TriggerType type, GameEvent? ev)
    {
        foreach(var player in Players.ToList())
        {
            if (!AlivePlayers.Contains(player)) continue;
            for (int i = 0; i < player.GlobalEffects.Count; i++)
            {
                EffectInstance e = player.GlobalEffects[i];
                if (e.Trigger == type)
                {
                    e.TryExecute(this, ev);
                }
            }
        }

        foreach(var player in Players.ToList())
        {
            if (!AlivePlayers.Contains(player)) continue;
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
            // Only prune dynamically-added auras (AppendGlobalEffect). A unit's own
            // Effects are part of its card definition and must stay in the list even
            // after they expire (TryExecute already refuses to re-fire them) — effects
            // like RetriggerUnitPlayedEffect/RetriggerSpellEffect rely on ForceExecute
            // being able to run an already-expired EffectInstance, which is impossible
            // once it's been removed here.
            player.GlobalEffects.RemoveAll(n => n.Expired);
        }
    }

    public void AlterUnitHealth(IdentificableObject source, CardInstance Unit, int Amount, bool checkKill = true, bool enqueueToUsers = true)
    {
        if(Unit.CurrentHealth <= 0) return;
        source = ResolveSource(source);
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
        source = ResolveSource(source);
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
        source = ResolveSource(source);
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
            KillPlayer(player, DefeatReason.NoHealth);
        }

        if (enqueueToUsers) GameActionResult.AddEvent(gevent);
        ApplyEffect(TriggerType.PlayerHealthChanged, gevent);
    }

    public void AddGlobalEffect(IdentificableObject source, PlayerState player, EffectInstance effect)
    {
        source = ResolveSource(source);
        effect.IsGlobal = true;
        player.GlobalEffects.Add(effect);

        var gevent = new GameEvent.GlobalEffectAdded()
        {
            PlayerSource = player,
            Source = source,
            PlayerId = player.Id,
            Effect = effect
        };

        ApplyEffect(TriggerType.GlobalEffectAdded, gevent);
    }

    public void KillUnit(IdentificableObject source, CardInstance Unit)
    {
        source = ResolveSource(source);
        if (Unit.DeathChecked) return;
        Unit.DeathChecked = true;
        Unit.Player.DeadCards.Add(Unit);
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