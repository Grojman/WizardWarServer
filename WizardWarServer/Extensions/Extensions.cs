public static class Extensions
{
    public static int FindFirstNullPosition(this object?[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if(array[i] is null) return i;
        }
        return -1;
    }
    public static int FindFirstNullPosition(this IEnumerable<object?> array)
    {
        for (int i = 0; i < array.Count(); i++)
        {
            if(array.ElementAt(i) is null) return i;
        }
        return -1;
    }
    public static T GetRandom<T>(this T[] array)
    {
        return array[new Random().Next(array.Length)];
    }

    public static T GetRandom<T>(this ICollection<T> array)
    {
        return array.ElementAt(new Random().Next(array.Count));
    }

    public static T GetRandom<T>(this IEnumerable<T> array)
    {
        return array.ElementAt(new Random().Next(array.Count()));
    }

    public static bool Evaluate(this CountType type, int count, int Amount)
    {
        return type switch
        {
            CountType.AT_LEAST => count >= Amount,
            CountType.AT_MAX => count <= Amount,
            CountType.EXACTLY => count == Amount,
            CountType.AT_LEAST_OVER => count > Amount,
            CountType.AT_MAX_UNDER => count < Amount,
            _ => false
        };
    }


    public static bool CanDraw(this PlayerStateDto state, int options) => options == 0 || state.HandData.Length < 5;
    public static bool CanDraw(this PlayerState state, int options) => options == 0 || state.Hand.Count < 5;


    public static bool CanAttack(this PlayerStateDto state) => state.Board.Any(n => n is not null && n.attack > 0);
    public static bool CanAttack(this PlayerState state) => state.Board.Any(n => n is not null && n.CurrentAttack > 0);


    public static bool CanPlayCard(this PlayerStateDto state) => state.HandData.Any(n => n.canPlay && (n.type == "Spell" || state.Board.Any(n => n is null)));
    public static bool CanPlayCard(this PlayerState state, GameState g) => state.Hand.Any(n => n.CanPlay.Check(state.Id, state.PlayerTarget.Id, n, g, null) && (n.Definition.Type == CardType.Spell || state.Board.Any(n => n is null)));

}