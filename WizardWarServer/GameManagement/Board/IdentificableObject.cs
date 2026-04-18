public abstract class IdentificableObject
{
    public Guid Id { get; set; } = Guid.NewGuid();
}