using NonogramAvalonia.ViewModels;
using Nonogram.Domain;
using System.Linq;
using Nonogram.Domain.Solver;

namespace NonogramAvalonia.Services;
public class SolverService
{
    /// <summary>
    /// Tries to solve a Nonogram using only its constraints and not the current Cell state
    /// </summary>
    /// <param name="nonogram">Nonogram to be solved</param>
    /// <param name="fillState">Fills <paramref name="nonogram"/> with the solved Cell state, even if only partially solved</param>
    /// <returns>True if the nonogram was fully solved</returns>
    public bool TrySolve(NonogramViewModel nonogram, bool fillState)
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
        var rowConstraints = nonogram.SolutionRowConstraints.Select(x => new LineConstraints(x));
        var columnConstraints = nonogram.SolutionColumnConstraints.Select(x => new LineConstraints(x));

        return new NonogramPuzzle(rowConstraints, columnConstraints);
    }
}
