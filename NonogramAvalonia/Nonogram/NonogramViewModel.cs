using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NonogramAvalonia.ViewModels;

public partial class NonogramViewModel : ObservableObject
{
    public int Columns { get => Cells.GetLength(1); }
    public int Rows { get => Cells.GetLength(0); }

    public List<LineConstraints> SolutionRowConstraints { get; private set; } = [];
    public List<LineConstraints> SolutionColumnConstraints { get; private set; } = [];

    public List<LineConstraints> PlayerRowConstraints { get; private set; } = [];
    public List<LineConstraints> PlayerColumnConstraints { get; private set; } = [];

    /// <summary>
    /// Board cells in [row, column] order
    /// </summary>
    public CellViewModel[,] Cells { get; private set; }

    [ObservableProperty] private string? _name;

    public NonogramViewModel(int rows, int columns)
    {
        Cells = new CellViewModel[rows, columns];
        ResetCellStates();
    }

    public NonogramViewModel(List<List<int>> rowConstraints, List<List<int>> columnConstraints)
    {
        Cells = new CellViewModel[rowConstraints.Count, columnConstraints.Count];
        SolutionRowConstraints = rowConstraints.Select(x => new LineConstraints(x)).ToList();
        SolutionColumnConstraints = columnConstraints.Select(x => new LineConstraints(x)).ToList();

        ResetCellStates();
    }

    public bool CheckWinState()
    {
        if (!AreConstraintsEqual(PlayerRowConstraints, SolutionRowConstraints))
            return false;

        if (!AreConstraintsEqual(PlayerColumnConstraints, SolutionColumnConstraints))
            return false;

        return true;

        bool AreConstraintsEqual(List<LineConstraints> a, List<LineConstraints> b)
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
        for (int row = 0; row < Rows; row++)
        {
            LineConstraints constraint = new LineConstraints();
            int run = 0;
            for (int col = 0; col < Columns; col++)
            {
                if (Cells[row, col].CellState == CellState.Filled)
                    run++;
                else if (run > 0)
                {
                    constraint.Add(run);
                    run = 0;
                }
            }
            if (run > 0)
                constraint.Add(run);

            if (constraint.Count == 0)
                constraint.Add(0);

            PlayerRowConstraints.Add(constraint);
        }

        // Build Column Constraints
        for (int col = 0; col < Columns; col++)
        {
            LineConstraints constraint = new LineConstraints();
            int run = 0;
            for (int row = 0; row < Rows; row++)
            {
                if (Cells[row, col].CellState == CellState.Filled)
                    run++;
                else if (run > 0)
                {
                    constraint.Add(run);
                    run = 0;
                }
            }
            if (run > 0)
                constraint.Add(run);

            if (constraint.Count == 0)
                constraint.Add(0);

            PlayerColumnConstraints.Add(constraint);
        }
    }

    public void ResetCellStates()
    {
        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                Cells[row, col] = new CellViewModel(CellState.Undetermined, row, col);
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

    public IEnumerable<CellViewModel> GetRow(int row)
    {
        if (row >= Cells.GetLength(0) || row < 0)
            throw new IndexOutOfRangeException();
        
        for (int col = 0; col < Cells.GetLength(1); col++)
        {
            yield return Cells[row, col];
        }
    }

    public IEnumerable<CellViewModel> GetColumn(int column)
    {
        if (column >= Cells.GetLength(1) || column < 0)
            throw new IndexOutOfRangeException();

        for (int row = 0; row < Cells.GetLength(0); row++)
        {
            yield return Cells[row, column];
        }
    }

    public IEnumerable<CellViewModel> Board
    {
        get
        {
            for (int row = 0; row < Rows; row++)
                for (int col = 0; col < Columns; col++)
                    yield return Cells[row, col];
        }
    }
}
