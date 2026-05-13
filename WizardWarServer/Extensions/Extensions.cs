public static class Extensions
{
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
}