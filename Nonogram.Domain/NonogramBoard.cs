using System.Xml.Linq;

namespace Nonogram.Domain;

public class NonogramBoard
{
    public int Columns { get => Cells.GetLength(0); }
    public int Rows { get => Cells.GetLength(1); }

    public List<LineConstraint> SolutionRowConstraints { get; private set; }
    public List<LineConstraint> SolutionColumnConstraints { get; private set; }

    public List<LineConstraint> PlayerRowConstraints { get; private set; } = new();
    public List<LineConstraint> PlayerColumnConstraints { get; private set; } = new();

    public NonogramCell[,] Cells { get; private set; }

    public bool IsGameActive { get; private set; } = false;

    public NonogramBoard(List<List<int>> rowConstraints, List<List<int>> columnConstraints)
    {
        Cells = new NonogramCell[rowConstraints.Count, columnConstraints.Count];
        SolutionRowConstraints = rowConstraints.Select(x => new LineConstraint(x)).ToList();
        SolutionColumnConstraints = columnConstraints.Select(x => new LineConstraint(x)).ToList();

        ResetMatrixStates();

        IsGameActive = true;
    }

    //public bool LoadFromXml(string fileName)
    //{
    //    var root = XElement.Load(fileName);

    //    var rowConstraints = root.Element("RowConstraints");
    //    foreach (var el in rowConstraints.Descendants())
    //    {
    //        var vals = el.Value.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries).Where(x => int.TryParse(x, out _));
    //        var constraints = new LineConstraint(vals.Select(x => int.Parse(x)));
    //        SolutionRowConstraints.Add(constraints);
    //    }

    //    var columnConstraints = root.Element("ColumnConstraints");
    //    foreach (var el in columnConstraints.Descendants())
    //    {
    //        var vals = el.Value.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries).Where(x => int.TryParse(x, out _));
    //        var constraints = new LineConstraint(vals.Select(x => int.Parse(x)));
    //        SolutionColumnConstraints.Add(constraints);
    //    }

    //    int.TryParse(rowConstraints.Attribute("count").Value, out var rowConstraintsCount);
    //    int.TryParse(columnConstraints.Attribute("count").Value, out var columnConstraintsCount);

    //    InitializeBoard(columnConstraintsCount, rowConstraintsCount);
    //    IsGameActive = true;

    //    return true;
    //}

    //public void InitializeBoard(int x, int y)
    //{
    //    Cells = new NonogramCell[x, y];
    //    ResetMatrixStates();
    //}

    public bool CheckWinState()
    {
        BuildConstraints();

        for (int i = 0; i < PlayerRowConstraints.Count; i++)
        {
            if (!SolutionRowConstraints[i].Equals(PlayerRowConstraints[i]))
                return false;
        }

        for (int i = 0; i < PlayerColumnConstraints.Count; i++)
        {
            if (!SolutionColumnConstraints[i].Equals(PlayerColumnConstraints[i]))
                return false;
        }

        IsGameActive = false;
        return true;
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

    public void ResetMatrixStates()
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
        if (IsGameActive)
        {
            if (row < Columns && column < Rows && row >= 0 && column >= 0)
                Cells[row, column].CellState = cs;
            else
                throw new IndexOutOfRangeException();
        }
        else
            throw new InvalidOperationException();
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
