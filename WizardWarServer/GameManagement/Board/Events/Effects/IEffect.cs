
using System.Text.Json.Serialization;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(DrawCardEffect), nameof(DrawCardEffect))]
[JsonDerivedType(typeof(AlterPlayerHealthEffect), nameof(AlterPlayerHealthEffect))]
[JsonDerivedType(typeof(AlterUnitStatsEffect), nameof(AlterUnitStatsEffect))]
[JsonDerivedType(typeof(AppendCardToDeck), nameof(AppendCardToDeck))]
[JsonDerivedType(typeof(DamagePlayerBasedOnCards), nameof(DamagePlayerBasedOnCards))]
[JsonDerivedType(typeof(GrowStatsBasedOnCardPlayed), nameof(GrowStatsBasedOnCardPlayed))]

//TODO: ADD DERIVED TYPES HERE
public interface IEffect : ICloneable<IEffect>
{
    void Execute(
        Guid playerId,
        CardInstance cardId,
        GameState state,
        GameEvent? ev);
}