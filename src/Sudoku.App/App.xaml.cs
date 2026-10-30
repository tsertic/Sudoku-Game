using System.IO;
using System.Windows;
using Sudoku.App.Features.Game;
using Sudoku.App.Features.Stats;
using Sudoku.Core.Puzzles;
using Sudoku.Core.Scores;

namespace Sudoku.App;

public partial class App : Application
{
    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var databasePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Sudoku", "scores.db");
        var scores = new ScoreRepository(databasePath);

        MainWindow window = null!;
        var game = new GameViewModel(scores, () => new StatsWindow(scores.LoadStats()) { Owner = window }.ShowDialog());
        window = new MainWindow(game);
        MainWindow = window;
        window.Show();

        await game.StartNewGameAsync(Difficulty.Easy);
    }
}
