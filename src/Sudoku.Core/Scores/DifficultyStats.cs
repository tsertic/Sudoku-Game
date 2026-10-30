using Sudoku.Core.Puzzles;

namespace Sudoku.Core.Scores;

public sealed record DifficultyStats(
    Difficulty Difficulty,
    int GamesSolved,
    TimeSpan? BestTime,
    TimeSpan? AverageTime,
    double? AverageScore);

public sealed record OverallStats(int GamesSolved, double? AverageScore, IReadOnlyList<DifficultyStats> ByDifficulty);
