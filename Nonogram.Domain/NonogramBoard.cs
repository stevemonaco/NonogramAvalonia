namespace Nonogram.Domain;

public class NonogramBoard
{
    public int Columns { get => Cells.GetLength(0); }
    public int Rows { get => Cells.GetLength(1); }

    public List<LineConstraint> SolutionRowConstraints { get; private set; } = [];
    public List<LineConstraint> SolutionColumnConstraints { get; private set; } = [];

    public List<LineConstraint> PlayerRowConstraints { get; private set; } = [];
    public List<LineConstraint> PlayerColumnConstraints { get; private set; } = [];

    public NonogramCell[,] Cells { get; private set; }

    public string? Name { get; set; }

    public NonogramBoard(int rows, int columns)
    {
        Cells = new NonogramCell[rows, columns];
        ResetCellStates();
    }

    public NonogramBoard(List<List<int>> rowConstraints, List<List<int>> columnConstraints)
    {
        Cells = new NonogramCell[rowConstraints.Count, columnConstraints.Count];
        SolutionRowConstraints = rowConstraints.Select(x => new LineConstraint(x)).ToList();
        SolutionColumnConstraints = columnConstraints.Select(x => new LineConstraint(x)).ToList();

        ResetCellStates();
    }

    public bool CheckWinState()
    {
        if (!AreConstraintsEqual(PlayerRowConstraints, SolutionRowConstraints))
            return false;

        if (!AreConstraintsEqual(PlayerColumnConstraints, SolutionColumnConstraints))
            return false;

        return true;

        bool AreConstraintsEqual(List<LineConstraint> a, List<LineConstraint> b)
        {
            if (a.Count != b.Count)
                return false;

            for (int i = 0; i < a.Count; i++)
            {
                if (!a[i].Equals(b[i]))
                    return false;
            }

            return true;
        }
    }

    public void BuildConstraints()
    {
        PlayerRowConstraints.Clear();
        PlayerColumnConstraints.Clear();

        // Build Row Constraints
        for (int y = 0; y < Rows; y++)
        {
            LineConstraint constraint = new LineConstraint();
            int run = 0;
            for (int x = 0; x < Columns; x++)
            {
                if (Cells[x, y].CellState == CellState.Filled)
                    run++;
                else if (run > 0)
                {
                    constraint.Add(run);
                    run = 0;
                }
            }
            if (run > 0)
                constraint.Add(run);

            if (constraint.Items.Count == 0)
                constraint.Add(0);

            PlayerRowConstraints.Add(constraint);
        }

        // Build Column Constraints
        for (int x = 0; x < Columns; x++)
        {
            LineConstraint constraint = new LineConstraint();
            int run = 0;
            for (int y = 0; y < Rows; y++)
            {
                if (Cells[x, y].CellState == CellState.Filled)
                    run++;
                else if (run > 0)
                {
                    constraint.Add(run);
                    run = 0;
                }
            }
            if (run > 0)
                constraint.Add(run);

            if (constraint.Items.Count == 0)
                constraint.Add(0);

            PlayerColumnConstraints.Add(constraint);
        }
    }

    public void ResetCellStates()
    {
        for (int row = 0; row < Rows; row++)
        {
            for (int column = 0; column < Columns; column++)
            {
                Cells[row, column] = new NonogramCell(CellState.Undetermined, row, column);
            }
        }
    }

    public CellState GetState(int row, int column)
    {
        if (row < Columns && column < Rows && row >= 0 && column >= 0)
            return Cells[row, column].CellState;
        else
            throw new IndexOutOfRangeException();
    }

    public void SetState(int row, int column, CellState cs)
    {
        if (row < Columns && column < Rows && row >= 0 && column >= 0)
            Cells[row, column].CellState = cs;
        else
            throw new IndexOutOfRangeException();
    }

    public IEnumerable<NonogramCell> GetRowCells(int row)
    {
        if (row >= Cells.GetLength(1) || row < 0)
            throw new IndexOutOfRangeException();
        
        for (int x = 0; x < Cells.GetLength(0); x++)
        {
            yield return Cells[x, row];
        }
    }

    public IEnumerable<NonogramCell> GetColumnCells(int column)
    {
        if (column >= Cells.GetLength(0) || column < 0)
            throw new IndexOutOfRangeException();

        for (int y = 0; y < Cells.GetLength(1); y++)
        {
            yield return Cells[column, y];
        }
    }

    public IEnumerable<NonogramCell> Board
    {
        get
        {
            for (int y = 0; y < Rows; y++)
                for (int x = 0; x < Columns; x++)
                    yield return Cells[x, y];
        }
    }
}
