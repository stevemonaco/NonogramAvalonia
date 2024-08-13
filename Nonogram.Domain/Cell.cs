namespace Nonogram.Domain;
public class Cell
{
    private CellState _state;
    public CellState State
    {
        get => _state;
        set
        {
            _state = value;
            FilterProspects();
        }
    }

    /// <summary>
    /// Label values that this cell's row constraint may be
    /// </summary>
    public HashSet<int> RowLabelProspects { get; set; } = [];

    /// <summary>
    /// Label values that this cell's column constraint may be
    /// </summary>
    public HashSet<int> ColumnLabelProspects { get; set; } = [];

    public int Row { get; }
    public int Column { get; }

    public Cell(int row, int column)
    {
        Row = row;
        Column = column;
        _state = CellState.Undetermined;
    }

    public void CalculateState()
    {
        if (State != CellState.Undetermined)
            return;

        if (RowLabelProspects.All(x => x > 0))
        {
            State = CellState.Filled;
        }
        else if (RowLabelProspects.All(x => x < 1))
        {
            State = CellState.Empty;
        }
        else if (ColumnLabelProspects.All(x => x > 0))
        {
            State = CellState.Filled;
        }
        else if (ColumnLabelProspects.All(x => x < 1))
        {
            State = CellState.Empty;
        }

        FilterProspects();
    }

    private void FilterProspects()
    {
        if (State == CellState.Filled)
        {
            RowLabelProspects.RemoveWhere(x => x < 0);
            ColumnLabelProspects.RemoveWhere(x => x < 0);
        }
        else if (State == CellState.Empty)
        {
            RowLabelProspects.RemoveWhere(x => x > 0);
            ColumnLabelProspects.RemoveWhere(x => x > 0);
        }
    }
}