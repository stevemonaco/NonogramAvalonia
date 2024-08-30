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
        Cells = new(CreateCellStates());
    }

    public NonogramViewModel(List<List<int>> rowConstraints, List<List<int>> columnConstraints)
    {
        RowCount = rowConstraints.Count;
        ColumnCount = columnConstraints.Count;
        Cells = new(CreateCellStates());

        SolutionRowConstraints = new(rowConstraints.Select(x => new LineConstraints(x)));
        SolutionColumnConstraints = new(columnConstraints.Select(x => new LineConstraints(x)));
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
        PlayerRowConstraints.Clear();
        PlayerColumnConstraints.Clear();

        for (int row = 0; row < RowCount; row++)
        {
            var constraint = BuildLineConstraint(GetRow(row));
            PlayerRowConstraints.Add(constraint);
        }

        for (int col = 0; col < ColumnCount; col++)
        {
            var constraint = BuildLineConstraint(GetColumn(col));
            PlayerColumnConstraints.Add(constraint);
        }

        LineConstraints BuildLineConstraint(IEnumerable<CellViewModel> cells)
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
    }

    private IEnumerable<CellViewModel> CreateCellStates()
    {
        for (int row = 0; row < RowCount; row++)
        {
            for (int col = 0; col < ColumnCount; col++)
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
            throw new IndexOutOfRangeException();
    }

    public void SetState(int row, int column, CellState cs)
    {
        if (row < RowCount && column < ColumnCount && row >= 0 && column >= 0)
            Cells[row * ColumnCount + column].CellState = cs;
        else
            throw new IndexOutOfRangeException();
    }

    public CellViewModel GetCell(int row, int column)
    {
        if (row >= RowCount || row < 0 || column >= ColumnCount || column < 0)
            throw new IndexOutOfRangeException($"Nonogram cell ({row}, {column}) is out of range");

        return Cells[row * ColumnCount + column];
    }

    public void SetCell(CellViewModel cell, int row, int column)
    {
        if (row >= RowCount || row < 0 || column >= ColumnCount || column < 0)
            throw new IndexOutOfRangeException($"Nonogram cell ({row}, {column}) is out of range");

        Cells[row * ColumnCount + column] = cell;
    }

    public IEnumerable<CellViewModel> GetRow(int row)
    {
        if (row >= RowCount || row < 0)
            throw new IndexOutOfRangeException();
        
        for (int col = 0; col < ColumnCount; col++)
        {
            yield return GetCell(row, col);
        }
    }

    public IEnumerable<CellViewModel> GetColumn(int column)
    {
        if (column >= ColumnCount || column < 0)
            throw new IndexOutOfRangeException();

        for (int row = 0; row < RowCount; row++)
        {
            yield return GetCell(row, column);
        }
    }
}
