using CommunityToolkit.Mvvm.ComponentModel;
using Nonogram.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NonogramAvalonia.ViewModels;

/// <summary>
/// Represents an interactable Nonogram puzzle
/// Solution constraints are the provided hints required to be satisfied to solve
/// Player constraints are recalculated based upon the current CellState of the board
/// </summary>
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
        Cells = new(CreateDefaultCells(RowCount, ColumnCount));

        RebuildSolutionConstraints();
        RebuildPlayerConstraints();
    }

    public NonogramViewModel(List<List<int>> rowConstraints, List<List<int>> columnConstraints)
    {
        RowCount = rowConstraints.Count;
        ColumnCount = columnConstraints.Count;
        Cells = new(CreateDefaultCells(RowCount, ColumnCount));

        SolutionRowConstraints = new(rowConstraints.Select(x => new LineConstraints(x)));
        SolutionColumnConstraints = new(columnConstraints.Select(x => new LineConstraints(x)));

        PlayerRowConstraints = new(rowConstraints.Select(x => new LineConstraints([0])));
        PlayerColumnConstraints = new(columnConstraints.Select(x => new LineConstraints([0])));
    }

    /// <summary>
    /// Resizes the board with the specified dimensions. Preserves CellState and constraints when possible.
    /// </summary>
    /// <param name="rows"></param>
    /// <param name="columns"></param>
    public void Resize(int rows, int columns)
    {
        var newCells = new ObservableCollection<CellViewModel>(CreateDefaultCells(rows, columns));
        var minRows = int.Min(rows, RowCount);
        var minColumns = int.Min(columns, ColumnCount);

        for (int row = 0; row < minRows; row++)
        {
            for (int column = 0; column < minColumns; column++)
            {
                newCells[row * columns + column].CellState = Cells[row * ColumnCount + column].CellState;
            }
        }

        if (rows < RowCount)
        {
            SolutionRowConstraints = new(SolutionRowConstraints.Take(rows));
            SolutionColumnConstraints = new(SolutionColumnConstraints.Take(rows));
        }
        else
        {
            for (int i = 0; i < rows - RowCount; i++)
            {
                SolutionRowConstraints.Add(new([0]));
            }

            for (int i = 0; i < columns - ColumnCount; i++)
            {
                SolutionColumnConstraints.Add(new([0]));
            }
        }

        RowCount = rows;
        ColumnCount = columns;

        Cells = newCells;
        RebuildPlayerConstraints();
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
            .Select(x => LineConstraints.FromCells(GetRow(x)));

        var columnConstraints = Enumerable.Range(0, ColumnCount)
            .Select(x => LineConstraints.FromCells(GetColumn(x)));

        PlayerRowConstraints = new(rowConstraints);
        PlayerColumnConstraints = new(columnConstraints);
    }

    /// <summary>
    /// Rebuilds the Solution Row and Column constraints based on the current Cells state
    /// </summary>
    public void RebuildSolutionConstraints()
    {
        var rowConstraints = Enumerable.Range(0, RowCount)
            .Select(x => LineConstraints.FromCells(GetRow(x)));

        var columnConstraints = Enumerable.Range(0, ColumnCount)
            .Select(x => LineConstraints.FromCells(GetColumn(x)));

        SolutionRowConstraints = new(rowConstraints);
        SolutionColumnConstraints = new(columnConstraints);
    }

    /// <summary>
    /// Updates Player Constraints for a given (row, column)
    /// </summary>
    public void UpdatePlayerConstraints(int row, int column)
    {
        PlayerRowConstraints[row] = LineConstraints.FromCells(GetRow(row));
        PlayerColumnConstraints[column] = LineConstraints.FromCells(GetColumn(column));
    }

    /// <summary>
    /// Updates Solution Constraints for a given (row, column)
    /// </summary>
    public void UpdateSolutionConstraints(int row, int column)
    {
        SolutionRowConstraints[row] = LineConstraints.FromCellStates(GetRow(row).Select(x => x.CellState));
        SolutionColumnConstraints[column] = LineConstraints.FromCellStates(GetColumn(column).Select(x => x.CellState));
    }

    private IEnumerable<CellViewModel> CreateDefaultCells(int rows, int columns)
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

    /// <summary>
    /// Sets the CellState for a given Cell location. Does not update Player Constraints.
    /// </summary>
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
