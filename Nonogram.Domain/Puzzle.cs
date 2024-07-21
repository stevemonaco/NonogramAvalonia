namespace Nonogram.Domain;
public class Puzzle
{
    public List<LineConstraint> RowConstraints { get; }
    public List<LineConstraint> ColumnConstraints { get; }

    internal List<LabelMap> RowLabelMaps { get; }
    internal List<LabelMap> ColumnLabelMaps { get; }
    internal List<LabelMap> ReverseRowLabelMaps { get; }
    internal List<LabelMap> ReverseColumnLabelMaps { get; }

    public int Rows => RowConstraints.Count;
    public int Columns => ColumnConstraints.Count;

    /// <summary>
    /// Cells in [row, column] order
    /// </summary>
    public Cell[,] Cells { get; set; } = null!;

    public Puzzle(IEnumerable<LineConstraint> rowConstraints, IEnumerable<LineConstraint> columnConstraints)
    {
        RowConstraints = rowConstraints.ToList();
        ColumnConstraints = columnConstraints.ToList();

        RowLabelMaps = new List<LabelMap>(RowConstraints.Count);
        ColumnLabelMaps = new List<LabelMap>(ColumnConstraints.Count);
        ReverseRowLabelMaps = new List<LabelMap>(RowConstraints.Count);
        ReverseColumnLabelMaps = new List<LabelMap>(ColumnConstraints.Count);

        BuildCells();
    }

    private void BuildCells()
    {
        CreateCells();

        for (int row = 0; row < Rows; row++)
        {
            var labels = Solver.CreateLabels(RowConstraints[row]);
            var mapping = Solver.CreateMappings(labels);
            var reverseMapping = Solver.CreateMappings(labels.AsEnumerable().Reverse().ToList());

            RowLabelMaps.Add(mapping);
            ReverseRowLabelMaps.Add(reverseMapping);

            var firstCell = Cells[row, 0];
            var lastCell = Cells[row, Columns - 1];

            firstCell.RowLabelProspects = RowConstraints[row].Sum() == 0 ? [-1, -1] : [-1, 2];
            lastCell.RowLabelProspects = [labels.Max(), labels.Min()];

            for (int col = 1; col < Columns - 1; col++)
            {
                var previousCell = Cells[row, col - 1];
                var cell = Cells[row, col];

                UnionRowCell(cell, previousCell, mapping);
            }

            if (Columns == 1)
            {
                var state = RowConstraints[row][0] == 0 ? CellState.Empty : CellState.Filled;
                Cells[row, 0].State = state;
            }

            //PrintRowProspects(row);
        }

        for (int col = 0; col < Columns; col++)
        {
            var labels = Solver.CreateLabels(ColumnConstraints[col]);
            var mapping = Solver.CreateMappings(labels);
            var reverseMapping = Solver.CreateMappings(labels.AsEnumerable().Reverse().ToList());

            ColumnLabelMaps.Add(mapping);
            ReverseColumnLabelMaps.Add(reverseMapping);

            var firstCell = Cells[0, col];
            var lastCell = Cells[Rows - 1, col];

            firstCell.ColumnLabelProspects = ColumnConstraints[col].Sum() == 0 ? [-1, -1] : [-1, 2];
            lastCell.ColumnLabelProspects = [labels.Max(), labels.Min()];

            for (int row = 1; row < Rows - 1; row++)
            {
                var previousCell = Cells[row - 1, col];
                var cell = Cells[row, col];

                UnionColumnCell(cell, previousCell, mapping);
            }

            if (Rows == 1)
            {
                var state = ColumnConstraints[col][0] == 0 ? CellState.Empty : CellState.Filled;
                Cells[0, col].State = state;
            }

            //PrintColumnProspects(col);
        }
    }

    private void UnionRowCell(Cell cell, Cell neighborCell, LabelMap map) =>
        UnionCell(cell, neighborCell, map, true);

    private void UnionColumnCell(Cell cell, Cell neighborCell, LabelMap map) =>
        UnionCell(cell, neighborCell, map, false);

    private void UnionCell(Cell cell, Cell neighborCell, LabelMap map, bool isRow)
    {
        var neighborLabels = map.GetNextProspectiveLabels(isRow ? neighborCell.RowLabelProspects : neighborCell.ColumnLabelProspects);
        var cellLabels = isRow ? cell.RowLabelProspects : cell.ColumnLabelProspects;

        cellLabels.UnionWith(neighborLabels);
    }

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

    public int SolveRow(int row) => SolveRowForward(row) + SolveRowReverse(row);

    public int SolveRowForward(int row)
    {
        int count = 0;

        for (int col = 1; col < Columns; col++)
        {
            count += SolveRowCell(Cells[row, col], Cells[row, col - 1], RowLabelMaps[row]);
        }

        return count;
    }

    public int SolveRowReverse(int row)
    {
        int count = 0;
        for (int col = Columns - 2; col >= 0; col--)
        {
            count += SolveRowCell(Cells[row, col], Cells[row, col + 1], ReverseRowLabelMaps[row]);
        }

        return count;
    }

    public int SolveColumn(int col) => SolveColumnForward(col) + SolveColumnReverse(col);

    public int SolveColumnForward(int col)
    {
        int count = 0;
        for (int row = 1; row < Rows; row++)
        {
            count += SolveColumnCell(Cells[row, col], Cells[row - 1, col], ColumnLabelMaps[col]);
        }

        return count;
    }

    public int SolveColumnReverse(int col)
    {
        int count = 0;
        for (int row = Rows - 2; row >= 0; row--)
        {
            count += SolveColumnCell(Cells[row, col], Cells[row + 1, col], ReverseColumnLabelMaps[col]);
        }

        return count;
    }

    private int SolveRowCell(Cell cell, Cell neighborCell, LabelMap map)
    {
        var prospects = map.GetNextProspectiveLabels(neighborCell.RowLabelProspects);
        return SolveCell(cell, cell.RowLabelProspects, prospects);
    }

    private int SolveColumnCell(Cell cell, Cell neighborCell, LabelMap map)
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
    private int SolveCell(Cell cell, HashSet<int> cellProspects, HashSet<int> adjacentProspects)
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

    private void CreateCells()
    {
        Cells = new Cell[Rows, Columns];

        for (int row = 0; row < Rows; row++)
            for (int col = 0; col < Columns; col++)
                Cells[row, col] = new Cell(row, col);
    }

    public IEnumerable<Cell> GetRow(int row)
    {
        for (int column = 0; column < Columns; column++)
            yield return Cells[row, column];
    }

    public IEnumerable<Cell> GetColumn(int column)
    {
        for (int row = 0; row < Rows; row++)
            yield return Cells[row, column];
    }

    public void PrintRowProspects(int row) => PrintLineProspects(GetRow(row), true);
    public void PrintColumnProspects(int column) => PrintLineProspects(GetColumn(column), false);

    public void PrintLineProspects(IEnumerable<Cell> cells, bool isRow)
    {
        int i = 0;

        foreach (var cell in cells)
        {
            var prospects = isRow ? cell.RowLabelProspects : cell.ColumnLabelProspects;
            Console.WriteLine($"[{i}]: {string.Join(',', prospects)}");

            i++;
        }
    }

    public void PrintPuzzle()
    {
        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0 ; col < Columns ; col++)
            {
                var cell = Cells[row, col];

                var display = cell.State switch
                {
                    CellState.Undetermined => ".",
                    CellState.Filled => "■",
                    CellState.Empty => "X",
                    _ => throw new InvalidOperationException()
                };

                Console.Write(display);
            }
            Console.WriteLine();
        }
    }

    public double CalculateFractionSolved()
    {
        var cellCount = Rows * Columns;
        var solvedCount = Cells.Cast<Cell>().Count(x => x.State != CellState.Undetermined);

        return (double)solvedCount / cellCount;
    }
}