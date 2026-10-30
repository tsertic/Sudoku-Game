using Sudoku.Core.Puzzles;

namespace Sudoku.Core.Scores;

public sealed record GameResult(
    Difficulty Difficulty,
    TimeSpan Duration,
    int ChecksUsed,
    int Score,
    DateTime PlayedAt);
