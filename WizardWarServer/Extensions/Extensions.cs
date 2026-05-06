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
}