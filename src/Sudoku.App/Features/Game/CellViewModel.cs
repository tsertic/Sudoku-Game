using Sudoku.App.Shared;

namespace Sudoku.App.Features.Game;

public sealed class CellViewModel : ObservableObject
{
    private int value;
    private bool isSelected;

    public CellViewModel(int index)
    {
        Index = index;
        Row = index / 9;
        Column = index % 9;
    }

    public int Index { get; }
    public int Row { get; }
    public int Column { get; }

    public bool IsGiven { get; private set; }
    public string Text => value == 0 ? string.Empty : value.ToString();

    public int Value
    {
        get => value;
        set
        {
            SetField(ref this.value, value);
            Raise(nameof(Text));
        }
    }

    public bool IsSelected
    {
        get => isSelected;
        set => SetField(ref isSelected, value);
    }

    public void Load(int digit)
    {
        IsGiven = digit != 0;
        Raise(nameof(IsGiven));
        Value = digit;
    }
}
