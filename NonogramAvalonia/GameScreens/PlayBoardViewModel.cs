using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Nonogram.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NonogramAvalonia.ViewModels;
public partial class PlayBoardViewModel : BoardViewModel
{
    [ObservableProperty] private bool _isSolved;
    [ObservableProperty] private TimeSpan? _timeElapsed;

    public IEnumerable<string> SolutionRowConstraints =>
        GenerateConstraintStrings(Board.SolutionRowConstraints, " ");

    public IEnumerable<string> SolutionColumnConstraints =>
        GenerateConstraintStrings(Board.SolutionColumnConstraints, "\n");

    public IEnumerable<string> RowConstraints =>
        GenerateConstraintStrings(Board.PlayerRowConstraints, " ");

    public IEnumerable<string> ColumnConstraints =>
        GenerateConstraintStrings(Board.PlayerColumnConstraints, "\n");

    public PlayBoardViewModel(NonogramBoard board) : base(board)
    {
    }

    public override bool TryApplyCellTransition(NonogramCell cell)
    {
        if (IsSolved || !base.TryApplyCellTransition(cell))
            return false;

        if (Board.CheckWinState())
            PuzzleSolved();

        return true;
    }

    public override bool TryStartCellTransition(NonogramCell cell, bool secondary)
    {
        if (IsSolved)
            return false;

        return base.TryStartCellTransition(cell, secondary);
    }

    private void PuzzleSolved()
    {
        IsSolved = true;

        foreach (var cell in BoardCells.Where(x => x.CellState != CellState.Filled))
            cell.CellState = CellState.Undetermined;

        Messenger.Send(new GameWinMessage());
    }

    private IEnumerable<string> GenerateConstraintStrings(IEnumerable<LineConstraint> lineConstraints, string separator)
    {
        foreach (var constraints in lineConstraints)
            yield return string.Join(separator, constraints.Items.Select(x => x.ToString("d")));
    }
}
