using Nonogram.Domain.Solver;

namespace Nonogram.Domain;
public static class NonogramUtility
{
    public static List<CellState> ParseCellString(string cellString) =>
        cellString.Select(x => _lookup[x]).ToList();

    private static readonly Dictionary<char, CellState> _lookup =
        new Dictionary<char, CellState>() { ['-'] = CellState.Undetermined, ['x'] = CellState.Empty, ['o'] = CellState.Filled };

    /// <summary>
    /// Returns a list of strings where each string represents a row's cell states
    /// </summary>
    public static List<string> PuzzleToCellStrings(NonogramPuzzle puzzle)
    {
        return Enumerable.Range(0, puzzle.Rows)
            .Select(row => RowToCellString(puzzle, row))
            .ToList();
    }

    /// <summary>
    /// Returns a string that represents a row's cell states
    /// </summary>
    public static string RowToCellString(NonogramPuzzle puzzle, int row)
    {
        var cellString = puzzle.GetRow(row).Select(x => StateToChar(x.State));
        return string.Join("", cellString);
    }

    /// <summary>
    /// Returns a string that represents a columns's cell states
    /// </summary>
    public static string ColumnToCellString(NonogramPuzzle puzzle, int col)
    {
        var cellString = puzzle.GetColumn(col).Select(x => StateToChar(x.State));
        return string.Join("", cellString);
    }

    private static char StateToChar(CellState state) => state switch
    {
        CellState.Undetermined => '-',
        CellState.Filled => 'o',
        CellState.Empty => 'x',
        _ => throw new InvalidOperationException()
    };

    /// <summary>
    /// Updates the puzzle's state using the current solver's state
    /// </summary>
    /// <param name="puzzle"></param>
    /// <param name="solver"></param>
    public static void UpdateStateFromSolver(NonogramPuzzle puzzle, NonogramSolver solver)
    {
        for (int row = 0; row < puzzle.Rows; row++)
        {
            for (int col = 0; col < puzzle.Columns; col++)
            {
                puzzle.Cells[row, col].State = solver.Cells[row, col].State;
            }
        }
    }
}
