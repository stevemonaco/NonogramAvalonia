using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NonogramAvalonia.ViewModels;

public partial class NonogramViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<CellViewModel> _cells = [];
    [ObservableProperty] private int _columnCount;
    [ObservableProperty] private int _rowCount;

    [ObservableProperty] private ObservableCollection<LineConstraints> _solutionRowConstraints = [];
    [ObservableProperty] private ObservableCollection<LineConstraints> _solutionColumnConstraints = [];

    [ObservableProperty] private ObservableCollection<LineConstraints> _playerRowConstraints = [];
    [ObservableProperty] private ObservableCollection<LineConstraints> _playerColumnConstraints = [];

    [ObservableProperty] private string? _name;

    public NonogramViewModel(int rows, int columns)
    {
        RowCount = rows;
        ColumnCount = columns;
        Cells = new(CreateCells(RowCount, ColumnCount));

        RebuildSolutionConstraints();
        RebuildPlayerConstraints();
    }

    public NonogramViewModel(List<List<int>> rowConstraints, List<List<int>> columnConstraints)
    {
        RowCount = rowConstraints.Count;
        ColumnCount = columnConstraints.Count;
        Cells = new(CreateCells(RowCount, ColumnCount));

        SolutionRowConstraints = new(rowConstraints.Select(x => new LineConstraints(x)));
        SolutionColumnConstraints = new(columnConstraints.Select(x => new LineConstraints(x)));

        PlayerRowConstraints = new(rowConstraints.Select(x => new LineConstraints([0])));
        PlayerColumnConstraints = new(columnConstraints.Select(x => new LineConstraints([0])));
    }

    public bool CheckWinState()
    {
        if (!AreConstraintsEqual(PlayerRowConstraints, SolutionRowConstraints))
            return false;

        if (!AreConstraintsEqual(PlayerColumnConstraints, SolutionColumnConstraints))
            return false;

        return true;

        bool AreConstraintsEqual(IList<LineConstraints> a, IList<LineConstraints> b)
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

    /// <summary>
    /// Rebuilds the Player Row and Column constraints based on the current Cells state
    /// </summary>
    public void RebuildPlayerConstraints()
    {
        var rowConstraints = Enumerable.Range(0, RowCount)
            .Select(x => BuildLineConstraints(GetRow(x)));

        var columnConstraints = Enumerable.Range(0, ColumnCount)
            .Select(x => BuildLineConstraints(GetColumn(x)));

        PlayerRowConstraints = new(rowConstraints);
        PlayerColumnConstraints = new(columnConstraints);
    }

    /// <summary>
    /// Rebuilds the Solution Row and Column constraints based on the current Cells state
    /// </summary>
    public void RebuildSolutionConstraints()
    {
        var rowConstraints = Enumerable.Range(0, RowCount)
            .Select(x => BuildLineConstraints(GetRow(x)));

        var columnConstraints = Enumerable.Range(0, ColumnCount)
            .Select(x => BuildLineConstraints(GetColumn(x)));

        SolutionRowConstraints = new(rowConstraints);
        SolutionColumnConstraints = new(columnConstraints);
    }

    /// <summary>
    /// Updates Player Constraints for a given (row, column)
    /// </summary>
    public void UpdatePlayerConstraints(int row, int column)
    {
        PlayerRowConstraints[row] = BuildLineConstraints(GetRow(row));
        PlayerColumnConstraints[column] = BuildLineConstraints(GetColumn(column));
    }

    /// <summary>
    /// Updates Solution Constraints for a given (row, column)
    /// </summary>
    public void UpdateSolutionConstraints(int row, int column)
    {
        SolutionRowConstraints[row] = BuildLineConstraints(GetRow(row));
        SolutionColumnConstraints[column] = BuildLineConstraints(GetColumn(column));
    }

    private LineConstraints BuildLineConstraints(IEnumerable<CellViewModel> cells)
    {
        var constraints = new LineConstraints();
        int run = 0;
        foreach (var cell in cells)
        {
            if (cell.CellState == CellState.Filled)
            {
                run++;
            }
            else if (run > 0)
            {
                constraints.Add(run);
                run = 0;
            }
        }

        if (run > 0)
            constraints.Add(run);

        if (constraints.Count == 0)
            constraints.Add(0);

        return constraints;
    }

    private IEnumerable<CellViewModel> CreateCells(int rows, int columns)
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                yield return new CellViewModel(CellState.Undetermined, row, col);
            }
        }
    }

    public CellState GetState(int row, int column)
    {
        if (row < RowCount && column < ColumnCount && row >= 0 && column >= 0)
            return Cells[row * ColumnCount + column].CellState;
        else
            throw new IndexOutOfRangeException($"Nonogram cell ({row}, {column}) is out of range");
    }

    public void SetState(int row, int column, CellState cs)
    {
        if (row < RowCount && column < ColumnCount && row >= 0 && column >= 0)
            Cells[row * ColumnCount + column].CellState = cs;
        else
            throw new IndexOutOfRangeException($"Nonogram cell ({row}, {column}) is out of range");
    }

    private IEnumerable<CellViewModel> GetRow(int row)
    {
        if (row >= RowCount || row < 0)
            throw new IndexOutOfRangeException();
        
        for (int column = 0; column < ColumnCount; column++)
        {
            yield return Cells[row * ColumnCount + column];
        }
    }

    private IEnumerable<CellViewModel> GetColumn(int column)
    {
        if (column >= ColumnCount || column < 0)
            throw new IndexOutOfRangeException();

        for (int row = 0; row < RowCount; row++)
        {
            yield return Cells[row * ColumnCount + column];
        }
    }
}
