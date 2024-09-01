using NonogramAvalonia.ViewModels;
using Nonogram.Domain;
using System;
using System.Linq;
using Nonogram.Domain.Solver;

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
                    if (state != CellState.Undetermined)
                    {
                        nonogram.SetState(row, col, state);
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
}
