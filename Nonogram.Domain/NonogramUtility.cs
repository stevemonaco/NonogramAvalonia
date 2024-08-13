namespace Nonogram.Domain;
public static class NonogramUtility
{
    public static List<CellState> ParseCellString(string cellString) =>
        cellString.Select(x => _lookup[x]).ToList();

    private static readonly Dictionary<char, CellState> _lookup =
        new Dictionary<char, CellState>() { ['-'] = CellState.Undetermined, ['x'] = CellState.Empty, ['o'] = CellState.Filled };

    public static List<string> ToCellStrings(NonogramPuzzle puzzle)
    {
        var cellStrings = new List<string>();
        for (int row = 0; row < puzzle.Rows; row++)
        {
            var cellString = puzzle.GetRow(row).Select(x => StateToChar(x.State));
            cellStrings.Add(string.Join("", cellString));
        }

        return cellStrings;

        char StateToChar(CellState state) => state switch
        {
            CellState.Undetermined => '-',
            CellState.Filled => 'o',
            CellState.Empty => 'x',
            _ => throw new InvalidOperationException()
        };
    }
}
