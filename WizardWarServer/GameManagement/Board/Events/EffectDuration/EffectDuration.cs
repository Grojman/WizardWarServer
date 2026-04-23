using System.Text.Json.Serialization;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]

[JsonDerivedType(typeof(DurationByExecutions), nameof(DurationByExecutions))]
[JsonDerivedType(typeof(Always), nameof(Always))]
//TODO: ADD DERIVED TYPES HERE
public abstract class EffectDuration : ICloneable<EffectDuration>
{
    public abstract void NotifyExecution();

    public abstract bool IsExpired();

    public abstract EffectDuration Clone();
}