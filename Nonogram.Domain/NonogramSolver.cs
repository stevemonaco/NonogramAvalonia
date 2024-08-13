using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nonogram.Domain;
public class NonogramSolver
{
    internal List<LabelMap> RowLabelMaps { get; } = [];
    internal List<LabelMap> ColumnLabelMaps { get; } = [];
    internal List<LabelMap> ReverseRowLabelMaps { get; } = [];
    internal List<LabelMap> ReverseColumnLabelMaps { get; } = [];

    private NonogramPuzzle _puzzle;

    private int Rows => _puzzle.Rows;
    private int Columns => _puzzle.Columns;
    private Cell[,] Cells => _puzzle.Cells;

    public NonogramSolver(NonogramPuzzle puzzle)
    {
        _puzzle = puzzle;
        CreateMaps();
    }

    private void CreateMaps()
    {
        for (int row = 0; row < Rows; row++)
        {
            var labels = SolverUtility.CreateLabels(_puzzle.RowConstraints[row]);
            var mapping = SolverUtility.CreateMappings(labels);
            var reverseMapping = SolverUtility.CreateMappings(labels.AsEnumerable().Reverse().ToList());

            RowLabelMaps.Add(mapping);
            ReverseRowLabelMaps.Add(reverseMapping);

            var firstCell = Cells[row, 0];
            var lastCell = Cells[row, Columns - 1];

            firstCell.RowLabelProspects = _puzzle.RowConstraints[row].Sum() == 0 ? [-1, -1] : [-1, 2];
            lastCell.RowLabelProspects = [labels.Max(), labels.Min()];

            for (int col = 1; col < Columns - 1; col++)
            {
                var previousCell = Cells[row, col - 1];
                var cell = Cells[row, col];

                SolverUtility.UnionRowCell(cell, previousCell, mapping);
            }

            if (Columns == 1)
            {
                var state = _puzzle.RowConstraints[row][0] == 0 ? CellState.Empty : CellState.Filled;
                Cells[row, 0].State = state;
            }
        }

        for (int col = 0; col < Columns; col++)
        {
            var labels = SolverUtility.CreateLabels(_puzzle.ColumnConstraints[col]);
            var mapping = SolverUtility.CreateMappings(labels);
            var reverseMapping = SolverUtility.CreateMappings(labels.AsEnumerable().Reverse().ToList());

            ColumnLabelMaps.Add(mapping);
            ReverseColumnLabelMaps.Add(reverseMapping);

            var firstCell = Cells[0, col];
            var lastCell = Cells[Rows - 1, col];

            firstCell.ColumnLabelProspects = _puzzle.ColumnConstraints[col].Sum() == 0 ? [-1, -1] : [-1, 2];
            lastCell.ColumnLabelProspects = [labels.Max(), labels.Min()];

            for (int row = 1; row < Rows - 1; row++)
            {
                var previousCell = Cells[row - 1, col];
                var cell = Cells[row, col];

                SolverUtility.UnionColumnCell(cell, previousCell, mapping);
            }

            if (Rows == 1)
            {
                var state = _puzzle.ColumnConstraints[col][0] == 0 ? CellState.Empty : CellState.Filled;
                Cells[0, col].State = state;
            }
        }
    }

    public bool SolvePuzzle()
    {
        int count = -1;

        while (count != 0)
        {
            count = 0;

            for (int row = 0; row < Rows; row++)
                count += SolveRow(row);

            for (int col = 0; col < Columns; col++)
                count += SolveColumn(col);
        }

        return IsSolved();

        bool IsSolved()
        {
            return Cells.Cast<Cell>().All(x => x.State != CellState.Undetermined);
        }
    }

    public int SolveRow(int row) => SolveRowForward(row) + SolveRowReverse(row);

    public int SolveRowForward(int row)
    {
        int count = 0;

        for (int col = 1; col < Columns; col++)
        {
            count += SolveRowCell(Cells[row, col], Cells[row, col - 1], RowLabelMaps[row]);
        }

        return count;
    }

    public int SolveRowReverse(int row)
    {
        int count = 0;
        for (int col = Columns - 2; col >= 0; col--)
        {
            count += SolveRowCell(Cells[row, col], Cells[row, col + 1], ReverseRowLabelMaps[row]);
        }

        return count;
    }

    public int SolveColumn(int col) => SolveColumnForward(col) + SolveColumnReverse(col);

    public int SolveColumnForward(int col)
    {
        int count = 0;
        for (int row = 1; row < Rows; row++)
        {
            count += SolveColumnCell(Cells[row, col], Cells[row - 1, col], ColumnLabelMaps[col]);
        }

        return count;
    }

    public int SolveColumnReverse(int col)
    {
        int count = 0;
        for (int row = Rows - 2; row >= 0; row--)
        {
            count += SolveColumnCell(Cells[row, col], Cells[row + 1, col], ReverseColumnLabelMaps[col]);
        }

        return count;
    }

    private int SolveRowCell(Cell cell, Cell neighborCell, LabelMap map)
    {
        var prospects = map.GetNextProspectiveLabels(neighborCell.RowLabelProspects);
        return SolveCell(cell, cell.RowLabelProspects, prospects);
    }

    private int SolveColumnCell(Cell cell, Cell neighborCell, LabelMap map)
    {
        var prospects = map.GetNextProspectiveLabels(neighborCell.ColumnLabelProspects);
        return SolveCell(cell, cell.ColumnLabelProspects, prospects);
    }

    /// <summary>
    /// Solves a cell by reducing its prospects
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="cellProspects"></param>
    /// <param name="adjacentProspects"></param>
    /// <returns>Number of prospects ruled out</returns>
    private int SolveCell(Cell cell, HashSet<int> cellProspects, HashSet<int> adjacentProspects)
    {
        var count = cellProspects.Count;
        if (count == 1) // Cell is already fully constrained
            return 0;

        cellProspects.IntersectWith(adjacentProspects);

        var change = count - cellProspects.Count;
        if (change != 0)
            cell.CalculateState();

        return change;
    }

    public void PrintRowProspects(int row) => PrintLineProspects(_puzzle.GetRow(row), true);
    public void PrintColumnProspects(int column) => PrintLineProspects(_puzzle.GetColumn(column), false);

    public void PrintLineProspects(IEnumerable<Cell> cells, bool isRow)
    {
        int i = 0;

        foreach (var cell in cells)
        {
            var prospects = isRow ? cell.RowLabelProspects : cell.ColumnLabelProspects;
            Console.WriteLine($"[{i}]: {string.Join(',', prospects)}");

            i++;
        }
    }
}
