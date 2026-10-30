using Sudoku.App.Shared;
using Sudoku.Core.Puzzles;

namespace Sudoku.App.Features.Game;

public sealed class LevelOption(Difficulty level) : ObservableObject
{
    private bool isSelected;

    public Difficulty Level { get; } = level;
    public string Name => Level.ToString();

    public bool IsSelected
    {
        get => isSelected;
        set => SetField(ref isSelected, value);
    }
}
