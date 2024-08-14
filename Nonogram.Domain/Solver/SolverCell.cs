using System.Diagnostics;

namespace Nonogram.Domain.Solver;

[DebuggerDisplay("({Row}, {Column}) {State}")]
public class SolverCell : Cell
{
    private CellState _state;
    public override CellState State
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

    public SolverCell(int row, int column, CellState state = CellState.Undetermined) : base(row, column, state)
    {
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
