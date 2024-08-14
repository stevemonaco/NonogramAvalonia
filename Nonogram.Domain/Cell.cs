namespace Nonogram.Domain;
public class Cell
{
    public virtual CellState State { get; set; }

    public int Row { get; }
    public int Column { get; }

    public Cell(int row, int column, CellState state = CellState.Undetermined)
    {
        Row = row;
        Column = column;
        State = state;
    }
}