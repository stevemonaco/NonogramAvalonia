using Nonogram.Domain.Solver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nonogram.Domain;
public class NonogramGenerator
{
    public int RowCount { get; }
    public int ColumnCount { get; }

    Random _random;

    CellState[,] _states;

    public NonogramGenerator(int rows, int columns, int? seed = null)
    {
        _random = seed is null ? new Random() : new Random(seed.Value);
        RowCount = rows;
        ColumnCount = columns;

        _states = new CellState[RowCount, ColumnCount];
    }

    public NonogramPuzzle CreateRandom()
    {
        double startingThreshold = _random.NextDouble() * 0.5d;
        double iterativeThreshold = 0.95;

        FillCells(startingThreshold);
        var puzzle = CreatePuzzle();

        while (!TrySolve(puzzle))
        {
            FillCells(iterativeThreshold);
            puzzle = CreatePuzzle();
        }

        return puzzle;

        void FillCells(double transitionThreshold)
        {
            for (int row = 0; row < RowCount; row++)
            {
                for (int col = 0; col < ColumnCount; col++)
                {
                    if (_random.NextDouble() > transitionThreshold)
                        _states[row, col] = CellState.Filled;
                }
            }
        }
    }

    private bool TrySolve(NonogramPuzzle puzzle)
    {
        var solver = new NonogramSolver(puzzle);

        return solver.SolvePuzzle();
    }

    private NonogramPuzzle CreatePuzzle()
    {
        var rowConstraints = Enumerable.Range(0, RowCount)
            .Select(x => NonogramUtility.GetRow(_states, x))
            .Select(LineConstraints.FromCellStates);

        var columnConstraints = Enumerable.Range(0, ColumnCount)
            .Select(x => NonogramUtility.GetColumn(_states, x))
            .Select(LineConstraints.FromCellStates);

        return new NonogramPuzzle(rowConstraints, columnConstraints);
    }
}
