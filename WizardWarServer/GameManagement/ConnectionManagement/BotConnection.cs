using System.Net.WebSockets;

public class BotConnection : PlayerConnection
{
    bool Resend { get; set; } = false;
    public readonly string[] BOT_NAMES = ["Cartman", "Morgan Freeman", "Julio", "Segismundo", "Godofredo", "Gertrudis", "La rivers", "La marers"];
    public readonly string[] TEXT_RESPONSES = [
"¡Respeta mi autoridad, maldita sea! Esa carta no debería ni existir.",
"Oh, claro, porque TÚ siempre robas justo lo que necesitas… qué casualidad.",
"Paso turno… pero que sepas que estoy planeando algo MUY gordo.",
"¿En serio? ¿Otra contra? Eres literalmente la peor persona del mundo.",
"No es trampa si soy yo quien gana, ¿vale?",
"Mira, voy a bajar esta carta y tú te vas a callar un rato.",
"¡¿QUÉ?! ¡Eso no hace lo que dices que hace! Enséñame las reglas ahora mismo.",
"Oh nooo… mi criatura favorita… eres un monstruo.",
"Relájate, tengo todo bajo control… *todo bajo control*.",
"Vale, esto es una estrategia avanzada que probablemente no entiendas.",
"Voy a atacar con TODO. Sí, TODO. Llora.",
"No puedes hacer eso. Bueno, sí puedes, pero no deberías.",
"Esto es injusto. Exijo una repetición de la partida.",
"Tranquilo, estoy dejando que creas que ganas.",
"Si gano esta partida, tienes que reconocer que soy el mejor jugador de la historia.",
"¿Sabes qué? Esta baraja está rota. Totalmente rota. No es culpa mía.",
"Oh, genial, otra vez sin tierras. Juego perfectamente balanceado, sí señor.",
"Voy a hacer una jugada maestra… espera… espera… ya verás.",
"No, no, no, eso no estaba en el plan. Rebobina.",
"Esto es exactamente lo que quería que pasara. Exactamente.",
"Si pierdo es porque el juego está mal diseñado.",
"Si gano es porque soy un genio táctico.",
"No puedes contra mí, soy básicamente un prodigio de las cartas.",
"Voy a robar… y si no es lo que necesito, esta partida no cuenta.",
"Oh, ¿te estás poniendo nervioso? Bien. Así me gusta.",
":1:",
":2:",
":3:",
":4:",
":5:",
":6:",
":7:",
":8:",
":01:",
":02:",
":03:",
":04:",
":05:",
":06:",
":07:",
":08:",
"Esa jugada ha sido ilegal en al menos tres dimensiones distintas.",
"Te dejo vivir este turno… porque soy buena persona.",
"¿Ves? Todo forma parte de mi plan maestro desde el principio.",
"Vale, eso sí que no me lo esperaba… pero sigo siendo mejor que tú.",
"Cállate y acepta tu derrota, tío."
];

    public BotConnection() : base(CreateDummyWebSocket())
    {
        SelectedDeckId = CardManager.Decks.GetRandom().id;
        Name = BOT_NAMES.GetRandom();
    }

    private static WebSocket CreateDummyWebSocket()
    {
        return new DummyWebSocket();
    }

    private sealed class DummyWebSocket : WebSocket
    {
        public override WebSocketCloseStatus? CloseStatus => null;
        public override string? CloseStatusDescription => null;
        public override WebSocketState State => WebSocketState.Open;
        public override string? SubProtocol => null;

        public override void Abort() { }
        public override Task CloseAsync(WebSocketCloseStatus closeStatus, string? statusDescription, CancellationToken cancellationToken) => Task.CompletedTask;
        public override Task CloseOutputAsync(WebSocketCloseStatus closeStatus, string? statusDescription, CancellationToken cancellationToken) => Task.CompletedTask;
        public override void Dispose() { }
        public override Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken) => Task.FromResult(new WebSocketReceiveResult(0, WebSocketMessageType.Close, true, null, null));
        public override Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken) => Task.CompletedTask;
        public override ValueTask<ValueWebSocketReceiveResult> ReceiveAsync(Memory<byte> buffer, CancellationToken cancellationToken) => ValueTask.FromResult(new ValueWebSocketReceiveResult(0, WebSocketMessageType.Close, true));
        public override ValueTask SendAsync(ReadOnlyMemory<byte> buffer, WebSocketMessageType messageType, bool endOfMessage, CancellationToken cancellationToken) => ValueTask.CompletedTask;
    }

    async Task DecideNextAction(GameStateDto? state)
    {
        if (state is null || !state.Me.IsMyTurn || Game is null) return;
        Thread.Sleep(new Random().Next(1000, 3000));
        List<Func<GameStateDto, Task>> options = new();

        if(state.Me.CanPlayCard()) options.Add(PlayCard);
        if(state.Me.CanAttack()) options.Add(Attack);
        if(state.Me.CanDraw(options.Count)) options.Add(DrawCardAsync);

        await options.GetRandom()(state);
    }


    async Task DrawCardAsync(GameStateDto state)
    {
        await Game!.HandleAction(this, new PlayerAction.DrawCardAction());
    }

    async Task Attack(GameStateDto state)
    {
        if (Game is null) return;
        var rivalPositions = state.Rivals.First(n => n.Id == state.Me.TargetPlayer).Board.Select((a, b) => new {a, b}).Where(n => n.a is not null).Select(n => n.b);
        var playerPositions = state.Me.Board.Select((a, b) => new {a, b}).Where(n => n.a is not null).Select(n => n.b);
        TargetType[] options = !rivalPositions.Any() ? [TargetType.PLAYER] : [TargetType.PLAYER, TargetType.BOARD];
        await Game.HandleAction(this, new PlayerAction.AttackAction()
        {
            PlayerTarget = state.Me.TargetPlayer,
            TargetType = options.GetRandom(),
            TargetIndex = !rivalPositions.Any() ? 0 : rivalPositions.GetRandom(),
            AttackerIndex = !playerPositions.Any() ? 0 : playerPositions.GetRandom()
        });
    }


    async Task PlayCard(GameStateDto state)
    {
        if (Game is null) return;

        var isPlace = state.Me.Board.Any(n => n is null);
        var boardIndexes = state.Me.Board.Select((a,b) => new {a, b}).Where(n => n.a is null).Select(n => n.b);
        var cardIndexes = state.Me.HandData.Select((a, b) => new{a, b}).Where(n => n.a.canPlay && (n.a.type == "Spell" || isPlace)).Select(n => n.b);

        await Game.HandleAction(this, new PlayerAction.PlayCardAction()
        {
            BoardIndex = boardIndexes.Count() == 0 ? -1 : boardIndexes.GetRandom(),
            CardIndex = cardIndexes.GetRandom()
        });
    }



    async Task SendMessage()
    {
        if (Game?.HasEnded ?? true) return;

        Thread.Sleep(new Random().Next(250, 750));
        await Game.HandleAction(this, new PlayerAction.TextMessage()
        {
            Message = TEXT_RESPONSES.GetRandom()
        });
    }

    public override Task Send(string type, object obj)
    {
        if (Game?.HasEnded ?? true) return Task.CompletedTask;

        _ = Task.Run(async () =>
        {
            switch (type)
            {
                case "game_state":
                    await DecideNextAction(obj as GameStateDto);
                    break;
                case "text_message":
                    var typeInfo = obj.GetType();
                    var messageProperty = typeInfo.GetProperty("message");
                    var playerProperty = typeInfo.GetProperty("player");

                    var playerValue = playerProperty?.GetValue(obj, null) as Guid?;

                    if (playerValue is Guid otherPlayer && otherPlayer != Guid)
                    {
                        var messageValue = messageProperty?.GetValue(obj, null) as string;
                        Resend = messageValue == ":04:";

                        if (playerValue != Guid || Resend)
                        {
                            await SendMessage();
                        }
                    }
                    break;
            }
        });

        return Task.CompletedTask;
    }

    public override string ToString()
    {
        return $"[BOT] Player: {Name} Id: {Guid} In-game: {Game is not null}";
    }
}