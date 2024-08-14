namespace Nonogram.Domain.Solver;

/// <summary>
/// Solves a Nonogram
/// </summary>
/// <remarks>
/// Implementation adapted from the label-based approach created by https://www.twanvl.nl/blog/haskell/Nonograms in Haskell
/// However, this solver doesn't implement the guessing mechanism
/// </remarks>
public class NonogramSolver
{
    /// <summary>
    /// Cells in [row, column] order
    /// </summary>
    public SolverCell[,] Cells { get; }

    public int Rows { get; }
    public int Columns { get; }

    internal List<LabelMap> RowLabelMaps { get; } = [];
    internal List<LabelMap> ColumnLabelMaps { get; } = [];
    internal List<LabelMap> ReverseRowLabelMaps { get; } = [];
    internal List<LabelMap> ReverseColumnLabelMaps { get; } = [];

    private NonogramPuzzle _puzzle;

    public NonogramSolver(NonogramPuzzle puzzle)
    {
        _puzzle = puzzle;
        Rows = puzzle.Rows;
        Columns = puzzle.Columns;

        Cells = new SolverCell[Rows, Columns];

        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
                Cells[row, col] = new SolverCell(row, col, _puzzle.Cells[row, col].State);
        }

        CreateMaps();
    }

    private void CreateMaps()
    {
        for (int row = 0; row < Rows; row++)
        {
            var labels = SolverUtility.CreateLabels(_puzzle.RowConstraints[row]);
            var mapping = SolverUtility.CreateMappings(labels);
            var reverseMapping = SolverUtility.CreateMappings(labels.AsEnumerable().Reverse().ToList());

            RowLabelMaps.Add(mapping);
            ReverseRowLabelMaps.Add(reverseMapping);

            var firstCell = Cells[row, 0];
            var lastCell = Cells[row, Columns - 1];

            firstCell.RowLabelProspects = _puzzle.RowConstraints[row].Sum() == 0 ? [-1, -1] : [-1, 2];
            lastCell.RowLabelProspects = [labels.Max(), labels.Min()];

            for (int col = 1; col < Columns - 1; col++)
            {
                var previousCell = Cells[row, col - 1];
                var cell = Cells[row, col];

                SolverUtility.UnionRowCell(cell, previousCell, mapping);
            }

            if (Columns == 1)
            {
                var state = _puzzle.RowConstraints[row][0] == 0 ? CellState.Empty : CellState.Filled;
                Cells[row, 0].State = state;
            }
        }

        for (int col = 0; col < Columns; col++)
        {
            var labels = SolverUtility.CreateLabels(_puzzle.ColumnConstraints[col]);
            var mapping = SolverUtility.CreateMappings(labels);
            var reverseMapping = SolverUtility.CreateMappings(labels.AsEnumerable().Reverse().ToList());

            ColumnLabelMaps.Add(mapping);
            ReverseColumnLabelMaps.Add(reverseMapping);

            var firstCell = Cells[0, col];
            var lastCell = Cells[Rows - 1, col];

            firstCell.ColumnLabelProspects = _puzzle.ColumnConstraints[col].Sum() == 0 ? [-1, -1] : [-1, 2];
            lastCell.ColumnLabelProspects = [labels.Max(), labels.Min()];

            for (int row = 1; row < Rows - 1; row++)
            {
                var previousCell = Cells[row - 1, col];
                var cell = Cells[row, col];

                SolverUtility.UnionColumnCell(cell, previousCell, mapping);
            }

            if (Rows == 1)
            {
                var state = _puzzle.ColumnConstraints[col][0] == 0 ? CellState.Empty : CellState.Filled;
                Cells[0, col].State = state;
            }
        }
    }

    /// <summary>
    /// Solves the puzzle to the extent possible
    /// </summary>
    /// <returns>True if fully solved, false if partially solved</returns>
    public bool SolvePuzzle()
    {
        int count = -1;

        while (count != 0)
        {
            count = 0;

            for (int row = 0; row < Rows; row++)
                count += SolveRow(row);

            for (int col = 0; col < Columns; col++)
                count += SolveColumn(col);
        }

        return IsSolved();

        bool IsSolved()
        {
            return Cells.Cast<Cell>().All(x => x.State != CellState.Undetermined);
        }
    }

    /// <summary>
    /// Solves the specified row to the extent possible
    /// </summary>
    /// <param name="row">Row to solve</param>
    /// <returns>The total count of prospects ruled out</returns>
    public int SolveRow(int row) => SolveRowForward(row) + SolveRowReverse(row);

    private int SolveRowForward(int row)
    {
        int count = 0;

        for (int col = 1; col < Columns; col++)
        {
            count += SolveRowCell(Cells[row, col], Cells[row, col - 1], RowLabelMaps[row]);
        }

        return count;
    }

    private int SolveRowReverse(int row)
    {
        int count = 0;
        for (int col = Columns - 2; col >= 0; col--)
        {
            count += SolveRowCell(Cells[row, col], Cells[row, col + 1], ReverseRowLabelMaps[row]);
        }

        return count;
    }

    /// <summary>
    /// Solves the specified column to the extent possible
    /// </summary>
    /// <param name="col">Column to solve</param>
    /// <returns>The total count of prospects ruled out</returns>
    public int SolveColumn(int col) => SolveColumnForward(col) + SolveColumnReverse(col);

    private int SolveColumnForward(int col)
    {
        int count = 0;
        for (int row = 1; row < Rows; row++)
        {
            count += SolveColumnCell(Cells[row, col], Cells[row - 1, col], ColumnLabelMaps[col]);
        }

        return count;
    }

    private int SolveColumnReverse(int col)
    {
        int count = 0;
        for (int row = Rows - 2; row >= 0; row--)
        {
            count += SolveColumnCell(Cells[row, col], Cells[row + 1, col], ReverseColumnLabelMaps[col]);
        }

        return count;
    }

    private int SolveRowCell(SolverCell cell, SolverCell neighborCell, LabelMap map)
    {
        var prospects = map.GetNextProspectiveLabels(neighborCell.RowLabelProspects);
        return SolveCell(cell, cell.RowLabelProspects, prospects);
    }

    private int SolveColumnCell(SolverCell cell, SolverCell neighborCell, LabelMap map)
    {
        var prospects = map.GetNextProspectiveLabels(neighborCell.ColumnLabelProspects);
        return SolveCell(cell, cell.ColumnLabelProspects, prospects);
    }

    /// <summary>
    /// Solves a cell by reducing its prospects
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="cellProspects"></param>
    /// <param name="adjacentProspects"></param>
    /// <returns>Number of prospects ruled out</returns>
    private int SolveCell(SolverCell cell, HashSet<int> cellProspects, HashSet<int> adjacentProspects)
    {
        var count = cellProspects.Count;
        if (count == 1) // Cell is already fully constrained
            return 0;

        cellProspects.IntersectWith(adjacentProspects);

        var change = count - cellProspects.Count;
        if (change != 0)
            cell.CalculateState();

        return change;
    }

    public IEnumerable<SolverCell> GetRow(int row)
    {
        for (int column = 0; column < Columns; column++)
            yield return Cells[row, column];
    }

    public IEnumerable<SolverCell> GetColumn(int column)
    {
        for (int row = 0; row < Rows; row++)
            yield return Cells[row, column];
    }

    public void PrintRowProspects(int row) => PrintLineProspects(GetRow(row), true);
    public void PrintColumnProspects(int column) => PrintLineProspects(GetColumn(column), false);

    public void PrintLineProspects(IEnumerable<SolverCell> cells, bool isRow)
    {
        int i = 0;

        foreach (var cell in cells)
        {
            var prospects = isRow ? cell.RowLabelProspects : cell.ColumnLabelProspects;
            Console.WriteLine($"[{i}]: {string.Join(',', prospects)}");

            i++;
        }
    }
}
