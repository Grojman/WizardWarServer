
using System.Text.Json.Serialization;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(IHaveBeenPlayedCondition), nameof(IHaveBeenPlayedCondition))]
[JsonDerivedType(typeof(PlayerCardCondition), nameof(PlayerCardCondition))]
[JsonDerivedType(typeof(IDefendedCondition), nameof(IDefendedCondition))]
[JsonDerivedType(typeof(CountPlayedCardsCondition), nameof(CountPlayedCardsCondition))]
[JsonDerivedType(typeof(FilterPlayerCardCondition), nameof(FilterPlayerCardCondition))]
[JsonDerivedType(typeof(MultiEffectCondition), nameof(MultiEffectCondition))]
[JsonDerivedType(typeof(CountCardCondition), nameof(CountCardCondition))]
[JsonDerivedType(typeof(PlayerHealthCondition), nameof(PlayerHealthCondition))]
[JsonDerivedType(typeof(IAttackedCondition), nameof(IAttackedCondition))]
[JsonDerivedType(typeof(TurnCounterCondition), nameof(TurnCounterCondition))]
[JsonDerivedType(typeof(NumericEventCondition), nameof(NumericEventCondition))]

//TODO: ADD DERIVED TYPES HERE
public abstract class EffectCondition : ICloneable<EffectCondition>
{
    public abstract bool Check(Guid playerId, Guid rivalId, CardInstance sourceCard, GameState state, GameEvent? ev);
    public abstract EffectCondition Clone();
}