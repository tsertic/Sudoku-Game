using Sudoku.App.Features.Game;
using Sudoku.Core.Scores;

namespace Sudoku.App.Features.Stats;

public sealed class StatsViewModel
{
    public StatsViewModel(OverallStats stats)
    {
        Rows = stats.ByDifficulty.Select(StatsRow.From).ToList();
        Summary = stats.AverageScore is { } average
            ? $"{stats.GamesSolved} solved  ·  {average:N0} average score"
            : "Nothing solved yet. Go finish a puzzle!";
    }

    public string Summary { get; }
    public IReadOnlyList<StatsRow> Rows { get; }
}

public sealed record StatsRow(string Level, string Solved, string BestTime, string AverageTime, string AverageScore)
{
    private const string Empty = "-";

    public static StatsRow From(DifficultyStats stats) => new(
        stats.Difficulty.ToString(),
        stats.GamesSolved.ToString(),
        FormatTime(stats.BestTime),
        FormatTime(stats.AverageTime),
        stats.AverageScore is { } score ? $"{score:N0}" : Empty);

    private static string FormatTime(TimeSpan? time) =>
        time is { } value ? GameClock.Format(value) : Empty;
}
