using System.Windows;
using Sudoku.Core.Scores;

namespace Sudoku.App.Features.Stats;

public partial class StatsWindow : Window
{
    public StatsWindow(OverallStats stats)
    {
        DataContext = new StatsViewModel(stats);
        InitializeComponent();
    }
}
