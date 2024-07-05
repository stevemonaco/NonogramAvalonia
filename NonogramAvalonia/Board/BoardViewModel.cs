using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.ComponentModel;
using Nonogram.Domain;

namespace NonogramAvalonia.ViewModels;

public enum CellTransition { None, ToUndetermined, ToEmpty, ToFilled }
public partial class BoardViewModel : ObservableRecipient
{
    private readonly NonogramBoard _board;
    private CellTransition _transition;

    [ObservableProperty] private int _gridRows;
    [ObservableProperty] private int _gridColumns;

    [ObservableProperty] private ObservableCollection<NonogramCell> _boardCells;
    [ObservableProperty] private string _puzzleName = "";
    [ObservableProperty] private bool _isSolved;

    public IEnumerable<string> SolutionRowConstraints
    {
        get
        {
            foreach (var constraints in _board.SolutionRowConstraints)
                yield return string.Join(" ", constraints.Items.Select(x => x.ToString("d")));
        }
    }

    public IEnumerable<string> SolutionColumnConstraints
    {
        get
        {
            foreach (var constraints in _board.SolutionColumnConstraints)
                yield return string.Join("\n", constraints.Items.Select(x => x.ToString("d")));
        }
    }

    public IEnumerable<string> RowConstraints
    {
        get
        {
            foreach (var constraints in _board.PlayerRowConstraints)
                yield return string.Join(" ", constraints.Items.Select(x => x.ToString("d")));
        }
    }

    public IEnumerable<string> ColumnConstraints
    {
        get
        {
            foreach (var constraints in _board.PlayerColumnConstraints)
                yield return string.Join(" ", constraints.Items.Select(x => x.ToString("d")));
        }
    }

    public BoardViewModel(NonogramBoard board)
    {
        _board = board;
        _boardCells = new(_board.Board);

        GridRows = _board.Rows;
        GridColumns = _board.Columns;

        OnPropertyChanged(nameof(SolutionRowConstraints));
        OnPropertyChanged(nameof(SolutionColumnConstraints));
    }

    public bool TryApplyCellTransition(NonogramCell cell)
    {
        if (cell.Locked || IsSolved)
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

        if (_board.CheckWinState())
            PuzzleSolved();

        return true;
    }

    public bool TryStartCellTransition(NonogramCell cell, bool secondary)
    {
        if (cell.Locked || IsSolved)
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

    private void PuzzleSolved()
    {
        IsSolved = true;

        foreach (var cell in BoardCells.Where(x => x.CellState != CellState.Filled))
            cell.CellState = CellState.Undetermined;

        Messenger.Send(new GameWinMessage());
    }
}
