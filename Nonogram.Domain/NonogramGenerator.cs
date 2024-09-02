using Nonogram.Domain.Solver;
using System.Diagnostics;

namespace Nonogram.Domain;
public class NonogramGenerator
{
    public int RowCount { get; }
    public int ColumnCount { get; }

    Random _random;

    CellState[,] _states;

    public NonogramGenerator(int rows, int columns)
    {
        RowCount = rows;
        ColumnCount = columns;

        _states = new CellState[RowCount, ColumnCount];
    }

    public NonogramPuzzle CreateRandom(int? seed = null)
    {
        Random _random = seed is null ? new Random() : new Random(seed.Value);
        double startingThreshold = 0.7d;
        double iterativeThreshold = 0.9d;
        int generations = 1;

        FillCells(startingThreshold);
        var puzzle = CreatePuzzle();

        while (!TrySolve(puzzle))
        {
            FillCells(iterativeThreshold);
            puzzle = CreatePuzzle();
            generations++;
        }

        Debug.WriteLine($"Seed: {seed}, Generations: {generations}");

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
