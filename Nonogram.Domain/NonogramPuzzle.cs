namespace Nonogram.Domain;
public class NonogramPuzzle
{
    /// <summary>
    /// Row constraints required to be satisfied for the solution
    /// </summary>
    public List<LineConstraints> RowConstraints { get; }

    /// <summary>
    /// Column constraints required to be satisfied for the solution
    /// </summary>
    public List<LineConstraints> ColumnConstraints { get; }

    public int Rows => RowConstraints.Count;
    public int Columns => ColumnConstraints.Count;

    /// <summary>
    /// Cells in [row, column] order
    /// </summary>
    public Cell[,] Cells { get; set; } = null!;

    public NonogramPuzzle(IEnumerable<LineConstraints> rowConstraints, IEnumerable<LineConstraints> columnConstraints)
    {
        RowConstraints = rowConstraints.ToList();
        ColumnConstraints = columnConstraints.ToList();

        CreateCells();
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