using Sudoku.Core.Puzzles;

namespace Sudoku.Core.Scores;

public static class ScoreCalculator
{
    private const int CheckPenalty = 50;
    private const double FastestMultiplier = 1.5;
    private const double SlowestMultiplier = 0.25;

    public static int Calculate(Difficulty difficulty, TimeSpan duration, int checksUsed)
    {
        var overPar = duration / difficulty.ParTime();
        var multiplier = Math.Clamp(FastestMultiplier - 0.5 * overPar, SlowestMultiplier, FastestMultiplier);
        var score = (int)Math.Round(difficulty.BaseScore() * multiplier) - checksUsed * CheckPenalty;
        return Math.Max(score, 0);
    }
}
