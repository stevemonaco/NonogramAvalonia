using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Nonogram.Domain;

namespace NonogramAvalonia.ViewModels;
public enum CellTransition { None, ToUndetermined, ToEmpty, ToFilled }

public partial class BoardViewModel : ObservableRecipient
{
    public NonogramBoard Board { get; }
    [ObservableProperty] private int _gridRows;
    [ObservableProperty] private int _gridColumns;

    [ObservableProperty] private ObservableCollection<NonogramCell> _boardCells;
    [ObservableProperty] private string? _puzzleName;

    protected CellTransition _transition;

    public BoardViewModel(NonogramBoard board)
    {
        Board = board;
        _boardCells = new(Board.Board);
        
        _puzzleName = board.Name;
        GridRows = Board.Rows;
        GridColumns = Board.Columns;
    }

    public virtual bool TryApplyCellTransition(NonogramCell cell)
    {
        if (cell.Locked)
            return false;

        if (_transition == CellTransition.None)
            return false;

        cell.CellState = _transition switch
        {
            CellTransition.ToUndetermined => CellState.Undetermined,
            CellTransition.ToEmpty => CellState.Empty,
            CellTransition.ToFilled => CellState.Filled,
            _ => throw new InvalidOperationException($"{nameof(TryApplyCellTransition)} attempted to apply invalid transition {_transition}")
        };

        return true;
    }

    public virtual bool TryStartCellTransition(NonogramCell cell, bool secondary)
    {
        if (cell.Locked)
            return false;

        _transition = (cell.CellState, secondary) switch
        {
            (CellState.Undetermined, false) => CellTransition.ToFilled,
            (CellState.Undetermined, true) => CellTransition.ToEmpty,
            (CellState.Empty, false) => CellTransition.ToFilled,
            (CellState.Empty, true) => CellTransition.ToUndetermined,
            (CellState.Filled, false) => CellTransition.ToUndetermined,
            (CellState.Filled, true) => CellTransition.ToEmpty,
            _ => CellTransition.None
        };

        return true;
    }

    public void EndCellTransition()
    {
        _transition = CellTransition.None;
    }
}
