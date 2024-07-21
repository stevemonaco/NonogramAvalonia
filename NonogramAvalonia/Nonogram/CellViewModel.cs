using CommunityToolkit.Mvvm.ComponentModel;

namespace NonogramAvalonia.ViewModels;

public enum CellState { Undetermined, Empty, Filled };

public partial class CellViewModel : ObservableObject
{
    [ObservableProperty] private CellState _cellState;
    [ObservableProperty] private int _row;
    [ObservableProperty] private int _column;
    [ObservableProperty] private bool _locked;

    public CellViewModel() : this(CellState.Undetermined)
    {
    }

    public CellViewModel(CellState state)
    {
        CellState = state;
    }

    public CellViewModel(CellState state, int row, int column)
    {
        CellState = state;
        Row = row;
        Column = column;
    }
}
