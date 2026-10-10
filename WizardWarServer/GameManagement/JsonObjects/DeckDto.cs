public enum DeckDifficulty
{
    Easy = 1,
    Medium = 2,
    Hard = 3
}

public record DeckDto(int id, string name, string description, DeckDifficulty difficulty);
