using NonogramAvalonia.ViewModels;
using Nonogram.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainCellState = Nonogram.Domain.CellState;
using ViewModelCellState = NonogramAvalonia.ViewModels.CellState;

namespace NonogramAvalonia.Services;
public class SolverService
{
    public bool SolveBoard(NonogramViewModel nonogram, bool fillState)
    {
        var puzzle = ToPuzzle(nonogram);
        var isSolved = puzzle.SolvePuzzle();

        if (fillState)
        {
            for (int col = 0; col < nonogram.Columns; col++)
            {
                for (int row = 0; row < nonogram.Rows; row++)
                {
                    var state = puzzle.Cells[row, col].State;
                    if (state != DomainCellState.Undetermined)
                    {
                        nonogram.Cells[row, col].CellState = ToCellState(state);
                    }
                }
            }
        }

        return isSolved;
    }

    private Puzzle ToPuzzle(NonogramViewModel nonogram)
    {
        var rowConstraints = nonogram.SolutionRowConstraints.Select(x => new LineConstraint(x));
        var columnConstraints = nonogram.SolutionColumnConstraints.Select(x => new LineConstraint(x));

        return new Puzzle(rowConstraints, columnConstraints);
    }

    private ViewModelCellState ToCellState(DomainCellState state) => state switch
    {
        DomainCellState.Undetermined => ViewModelCellState.Undetermined,
        DomainCellState.Empty => ViewModelCellState.Empty,
        DomainCellState.Filled => ViewModelCellState.Filled,
        _ => throw new NotSupportedException()
    };
}
