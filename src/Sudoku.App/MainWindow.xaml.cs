using System.Windows;
using System.Windows.Input;
using Sudoku.App.Features.Game;

namespace Sudoku.App;

public partial class MainWindow : Window
{
    private readonly GameViewModel game;

    public MainWindow(GameViewModel game)
    {
        this.game = game;
        DataContext = game;
        InitializeComponent();
    }

    private void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (TryReadDigit(e.Key) is { } digit)
            game.EnterDigit(digit);
        else if (TryReadDirection(e.Key) is var (rowStep, columnStep))
            game.MoveSelection(rowStep, columnStep);
        else
            return;

        e.Handled = true;
    }

    private static int? TryReadDigit(Key key) => key switch
    {
        >= Key.D0 and <= Key.D9 => key - Key.D0,
        >= Key.NumPad0 and <= Key.NumPad9 => key - Key.NumPad0,
        Key.Back or Key.Delete => 0,
        _ => null
    };

    private static (int Row, int Column)? TryReadDirection(Key key) => key switch
    {
        Key.Up => (-1, 0),
        Key.Down => (1, 0),
        Key.Left => (0, -1),
        Key.Right => (0, 1),
        _ => null
    };
}
