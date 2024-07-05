using CommunityToolkit.Mvvm.ComponentModel;

namespace Nonogram.Domain;

public enum CellState { Undetermined, Empty, Filled };

public partial class NonogramCell : ObservableObject
{
    [ObservableProperty] private CellState _cellState;
    [ObservableProperty] private int _row;
    [ObservableProperty] private int _column;
    [ObservableProperty] private bool _locked;

    public NonogramCell() : this(CellState.Undetermined)
    {
    }

    public NonogramCell(CellState state)
    {
        CellState = state;
    }

    public NonogramCell(CellState state, int row, int column)
    {
        CellState = state;
        Row = row;
        Column = column;
    }
}
