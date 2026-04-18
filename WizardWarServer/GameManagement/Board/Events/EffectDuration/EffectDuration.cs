using System.Text.Json.Serialization;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]

[JsonDerivedType(typeof(DurationByExecutions), nameof(DurationByExecutions))]
//TODO: ADD DERIVED TYPES HERE
public abstract class EffectDuration : ICloneable<EffectDuration>
{
    public abstract void NotifyExecution();

    public abstract bool IsExpired();

    public abstract EffectDuration Clone();
}