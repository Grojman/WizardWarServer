
using System.Text.Json.Serialization;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(IHaveBeenPlayedCondition), nameof(IHaveBeenPlayedCondition))]

//TODO: ADD DERIVED TYPES HERE
public abstract class EffectCondition : ICloneable<EffectCondition>
{
    public abstract bool Check(Guid playerId, Guid cardId, GameState state, GameEvent? ev);
    public abstract EffectCondition Clone();
}