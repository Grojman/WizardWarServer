
using System.Text.Json.Serialization;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(DrawCardEffect), nameof(DrawCardEffect))]

//TODO: ADD DERIVED TYPES HERE
public interface IEffect
{
    void Execute(
        Guid playerId,
        CardInstance cardId,
        GameState state,
        GameEvent? ev);
}