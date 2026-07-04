public class NumberFilter
{
    public NumberFilter(CountType type, int amount)
    {
        Type = type;
        Amount = amount;
    }

    public CountType Type { get; set; }
    public int Amount { get; set; }

    public bool Compare(int value)
    {
        return Type.Evaluate(value, Amount);
    }
}