namespace Nonogram.Domain;

public static class NonogramUtility
{
    private static readonly Dictionary<char, CellState> _lookup = 
        new Dictionary<char, CellState>() { ['-'] = CellState.Undetermined, ['x'] = CellState.Empty, ['o'] = CellState.Filled };

    public static IList<NonogramCell> CellsFromString(string cellString) =>
        cellString.Select(x => new NonogramCell(_lookup[x])).ToList();
}
