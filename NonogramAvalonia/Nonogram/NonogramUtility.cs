using System;
using System.Collections.Generic;
using System.Linq;

namespace NonogramAvalonia.ViewModels;

public static class NonogramUtility
{
    private static readonly Dictionary<char, CellState> _lookup = 
        new Dictionary<char, CellState>() { ['-'] = CellState.Undetermined, ['x'] = CellState.Empty, ['o'] = CellState.Filled };

    public static IList<CellViewModel> ParseCellString(string cellString) =>
        cellString.Select(x => new CellViewModel(_lookup[x])).ToList();

    public static List<string> ToCellStrings(NonogramViewModel puzzle)
    {
        var cellStrings = new List<string>();
        for (int row = 0; row < puzzle.RowCount; row++)
        {
            var cellString = puzzle.GetRow(row).Select(x => StateToChar(x.CellState));
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
