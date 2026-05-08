
using System.Threading.Tasks;

public class BotConnection : PlayerConnection
{
    bool Resend { get; set; } = false;
    public readonly string[] BOT_NAMES = ["Cartman"];
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

    public BotConnection() : base(null)
    {
        SelectedDeckId = CardManager.Decks.GetRandom().id;
        Name = BOT_NAMES.GetRandom();
    }

    async void DecideNextAction(GameStateDto state)
    {
        if (!state.IsMyTurn || Game is null) return;
        Thread.Sleep(new Random().Next(1000, 3000));
        List<Func<GameStateDto, Task>> options = new();

        if(CanDraw(state)) options.Add(DrawCardAsync);
        if(CanPlayCard(state)) options.Add(PlayCard);
        if(CanAttack(state)) options.Add(Attack);

        await options.GetRandom()(state);
    }


    async Task DrawCardAsync(GameStateDto state)
    {
        await Game.HandleAction(this, new PlayerAction.DrawCardAction());
    }

    bool CanDraw(GameStateDto state) => state.Me.HandData.Count() < 5;

    async Task Attack(GameStateDto state)
    {
        if (Game is null) return;
        var rivalPositions = state.Rival.Board.Select((a, b) => new {a, b}).Where(n => n.a is not null).Select(n => n.b);
        var playerPositions = state.Me.Board.Select((a, b) => new {a, b}).Where(n => n.a is not null).Select(n => n.b);
        TargetType[] options = !rivalPositions.Any() ? [TargetType.RIVAL] : [TargetType.RIVAL, TargetType.ENEMY_BOARD];
        await Game.HandleAction(this, new PlayerAction.AttackAction()
        {
            TargetType = options.GetRandom(),
            TargetIndex = rivalPositions.GetRandom(),
            AttackerIndex = playerPositions.GetRandom()
        });
    }

    bool CanAttack(GameStateDto state) => state.Me.Board.Any(n => n is not null && n.attack > 0);

    async Task PlayCard(GameStateDto state)
    {
        if (Game is null) return;

        var isPlace = state.Me.Board.Any(n => n is null);
        var boardIndexes = state.Me.Board.Select((a,b) => new {a, b}).Where(n => n.a is null).Select(n => n.b);
        var cardIndexes = state.Me.HandData.Select((a, b) => new{a, b}).Where(n => n.a.canPlay && (n.a.type == "Spell" || isPlace)).Select(n => n.b);

        await Game.HandleAction(this, new PlayerAction.PlayCardAction()
        {
            BoardIndex = boardIndexes.GetRandom(),
            CardIndex = cardIndexes.GetRandom()
        });
    }

    bool CanPlayCard(GameStateDto state) => state.Me.HandData.Any(n => n.canPlay && (n.type == "Spell" || state.Me.Board.Any(n => n is null)));


    async Task SendMessage()
    {
        if (Game is null) return;

        Thread.Sleep(new Random().Next(250, 750));
        await Game.HandleAction(this, new PlayerAction.TextMessage()
        {
            Message = TEXT_RESPONSES.GetRandom()
        });
    }

    public override Task Send(string type, object obj)
    {
        if (Game is null) Task.FromResult(0);

        Task.Run(async() =>
        {
             switch(type)
            {
                case "game_state":
                DecideNextAction(obj as GameStateDto);
                break;
                case "text_message":
                var type = obj.GetType();
                var messageProperty = type.GetProperty("message");
                var playerProperty = type.GetProperty("player");

                var playerValue = (Guid)playerProperty.GetValue(obj, null);
                
                if(playerValue != Guid)
                    {
                var messageValue = (string)messageProperty.GetValue(obj, null);

                Resend = messageValue == ":04:"; // Qué poner ?
                        
                    }
                if (playerValue != Guid || Resend)
                {

                    SendMessage();

                }
                break;
            }
        });
       
        return Task.FromResult(0);
    }
}