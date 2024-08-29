using NonogramAvalonia.ViewModels;
using Nonogram.Domain;
using System;
using System.Linq;
using Nonogram.Domain.Solver;
using DomainCellState = Nonogram.Domain.CellState;
using ViewModelCellState = NonogramAvalonia.ViewModels.CellState;

namespace NonogramAvalonia.Services;
public class SolverService
{
    public bool SolveBoard(NonogramViewModel nonogram, bool fillState)
    {
        var puzzle = ToPuzzle(nonogram);
        var solver = new NonogramSolver(puzzle);
        var isSolved = solver.SolvePuzzle();

        if (fillState)
        {
            for (int col = 0; col < nonogram.ColumnCount; col++)
            {
                for (int row = 0; row < nonogram.RowCount; row++)
                {
                    var state = solver.Cells[row, col].State;
                    if (state != DomainCellState.Undetermined)
                    {
                        nonogram.SetState(row, col, ToCellState(state));
                    }
                }
            }
        }

        return isSolved;
    }

    private NonogramPuzzle ToPuzzle(NonogramViewModel nonogram)
    {
        var rowConstraints = nonogram.SolutionRowConstraints.Select(x => new LineConstraint(x));
        var columnConstraints = nonogram.SolutionColumnConstraints.Select(x => new LineConstraint(x));

        return new NonogramPuzzle(rowConstraints, columnConstraints);
    }

    private ViewModelCellState ToCellState(DomainCellState state) => state switch
    {
        DomainCellState.Undetermined => ViewModelCellState.Undetermined,
        DomainCellState.Empty => ViewModelCellState.Empty,
        DomainCellState.Filled => ViewModelCellState.Filled,
        _ => throw new NotSupportedException()
    };
}
