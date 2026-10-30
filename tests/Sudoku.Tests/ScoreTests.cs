using Sudoku.Core.Puzzles;
using Sudoku.Core.Scores;

namespace Sudoku.Tests;

public sealed class ScoreTests : IDisposable
{
    private readonly string databasePath = Path.Combine(Path.GetTempPath(), $"sudoku-{Guid.NewGuid():N}.db");

    public void Dispose() => File.Delete(databasePath);

    [Fact]
    public void Par_time_earns_base_score_and_checks_cost_points()
    {
        var parTime = Difficulty.Medium.ParTime();

        Assert.Equal(2000, ScoreCalculator.Calculate(Difficulty.Medium, parTime, 0));
        Assert.Equal(1900, ScoreCalculator.Calculate(Difficulty.Medium, parTime, 2));
    }

    [Fact]
    public void Score_is_capped_for_very_fast_and_very_slow_games()
    {
        Assert.Equal(1500, ScoreCalculator.Calculate(Difficulty.Easy, TimeSpan.Zero, 0));
        Assert.Equal(250, ScoreCalculator.Calculate(Difficulty.Easy, TimeSpan.FromHours(5), 0));
    }

    [Fact]
    public void Empty_database_has_no_averages()
    {
        var stats = new ScoreRepository(databasePath).LoadStats();

        Assert.Equal(0, stats.GamesSolved);
        Assert.Null(stats.AverageScore);
        Assert.Equal(4, stats.ByDifficulty.Count);
    }

    [Fact]
    public void Stats_average_across_saved_games()
    {
        var repository = new ScoreRepository(databasePath);
        repository.Save(new GameResult(Difficulty.Easy, TimeSpan.FromSeconds(100), 0, 1000, DateTime.UtcNow));
        repository.Save(new GameResult(Difficulty.Easy, TimeSpan.FromSeconds(200), 1, 2000, DateTime.UtcNow));
        repository.Save(new GameResult(Difficulty.Evil, TimeSpan.FromSeconds(600), 0, 3000, DateTime.UtcNow));

        var stats = repository.LoadStats();
        var easy = stats.ByDifficulty.Single(item => item.Difficulty == Difficulty.Easy);

        Assert.Equal(3, stats.GamesSolved);
        Assert.Equal(2000, stats.AverageScore);
        Assert.Equal(2, easy.GamesSolved);
        Assert.Equal(TimeSpan.FromSeconds(100), easy.BestTime);
        Assert.Equal(TimeSpan.FromSeconds(150), easy.AverageTime);
        Assert.Equal(1500, easy.AverageScore);
    }
}
