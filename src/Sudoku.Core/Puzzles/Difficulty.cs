namespace Sudoku.Core.Puzzles;

public enum Difficulty
{
    Easy,
    Medium,
    Hard,
    Evil
}

public static class DifficultyExtensions
{
    public static (int Min, int Max) GivenRange(this Difficulty difficulty) => difficulty switch
    {
        Difficulty.Easy => (36, 40),
        Difficulty.Medium => (31, 35),
        Difficulty.Hard => (27, 30),
        Difficulty.Evil => (23, 26),
        _ => throw new ArgumentOutOfRangeException(nameof(difficulty))
    };

    public static TimeSpan ParTime(this Difficulty difficulty) => difficulty switch
    {
        Difficulty.Easy => TimeSpan.FromMinutes(6),
        Difficulty.Medium => TimeSpan.FromMinutes(12),
        Difficulty.Hard => TimeSpan.FromMinutes(20),
        Difficulty.Evil => TimeSpan.FromMinutes(30),
        _ => throw new ArgumentOutOfRangeException(nameof(difficulty))
    };

    public static int BaseScore(this Difficulty difficulty) => ((int)difficulty + 1) * 1000;
}
