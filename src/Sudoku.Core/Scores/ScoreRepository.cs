using Microsoft.Data.Sqlite;
using Sudoku.Core.Puzzles;

namespace Sudoku.Core.Scores;

public sealed class ScoreRepository
{
    private readonly string connectionString;

    public ScoreRepository(string databasePath)
    {
        var directory = Path.GetDirectoryName(databasePath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        connectionString = new SqliteConnectionStringBuilder { DataSource = databasePath, Pooling = false }.ToString();
        EnsureSchema();
    }

    public void Save(GameResult result)
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO game_results (difficulty, duration_seconds, checks_used, score, played_at)
            VALUES ($difficulty, $duration, $checks, $score, $playedAt)
            """;
        command.Parameters.AddWithValue("$difficulty", (int)result.Difficulty);
        command.Parameters.AddWithValue("$duration", result.Duration.TotalSeconds);
        command.Parameters.AddWithValue("$checks", result.ChecksUsed);
        command.Parameters.AddWithValue("$score", result.Score);
        command.Parameters.AddWithValue("$playedAt", result.PlayedAt.ToString("O"));
        command.ExecuteNonQuery();
    }

    public OverallStats LoadStats()
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT difficulty, COUNT(*), MIN(duration_seconds), AVG(duration_seconds), AVG(score)
            FROM game_results
            GROUP BY difficulty
            """;

        var solvedByDifficulty = new Dictionary<Difficulty, DifficultyStats>();
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                var difficulty = (Difficulty)reader.GetInt32(0);
                solvedByDifficulty[difficulty] = new DifficultyStats(
                    difficulty,
                    reader.GetInt32(1),
                    TimeSpan.FromSeconds(reader.GetDouble(2)),
                    TimeSpan.FromSeconds(reader.GetDouble(3)),
                    reader.GetDouble(4));
            }
        }

        var perDifficulty = Enum.GetValues<Difficulty>()
            .Select(difficulty => solvedByDifficulty.GetValueOrDefault(difficulty)
                ?? new DifficultyStats(difficulty, 0, null, null, null))
            .ToList();

        var totalGames = perDifficulty.Sum(stats => stats.GamesSolved);
        var averageScore = totalGames == 0
            ? (double?)null
            : perDifficulty.Sum(stats => (stats.AverageScore ?? 0) * stats.GamesSolved) / totalGames;

        return new OverallStats(totalGames, averageScore, perDifficulty);
    }

    private void EnsureSchema()
    {
        using var connection = Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS game_results (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                difficulty INTEGER NOT NULL,
                duration_seconds REAL NOT NULL,
                checks_used INTEGER NOT NULL,
                score INTEGER NOT NULL,
                played_at TEXT NOT NULL
            )
            """;
        command.ExecuteNonQuery();
    }

    private SqliteConnection Open()
    {
        var connection = new SqliteConnection(connectionString);
        connection.Open();
        return connection;
    }
}
