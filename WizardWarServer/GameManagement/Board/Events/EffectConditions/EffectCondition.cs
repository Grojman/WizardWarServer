
using System.Text.Json.Serialization;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(IHaveBeenPlayedCondition), nameof(IHaveBeenPlayedCondition))]
[JsonDerivedType(typeof(IHaveDiedCondition), nameof(IHaveDiedCondition))]
[JsonDerivedType(typeof(PlayerCardCondition), nameof(PlayerCardCondition))]
[JsonDerivedType(typeof(EvenTurnCondition), nameof(EvenTurnCondition))]
[JsonDerivedType(typeof(CountPlayedCardsCondition), nameof(CountPlayedCardsCondition))]
[JsonDerivedType(typeof(FilterPlayerCardCondition), nameof(FilterPlayerCardCondition))]

//TODO: ADD DERIVED TYPES HERE
public abstract class EffectCondition : ICloneable<EffectCondition>
{
    public abstract bool Check(Guid playerId, CardInstance sourceCard, GameState state, GameEvent? ev);
    public abstract EffectCondition Clone();
}