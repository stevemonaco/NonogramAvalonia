using System;
using System.Collections.Generic;
using System.Linq;
using Nonogram.Domain;

namespace NonogramAvalonia.ViewModels;

public class LineConstraints : List<int>, IEquatable<LineConstraints>
{
    public LineConstraints() { }

    public LineConstraints(IEnumerable<int> constraintValues)
    {
        AddRange(constraintValues);
    }

    public static LineConstraints FromCells(IEnumerable<CellViewModel> cellStates) =>
        FromCellStates(cellStates.Select(x => x.CellState));

    public static LineConstraints FromCellStates(IEnumerable<CellState> cellStates)
    {
        var constraints = new LineConstraints();
        int run = 0;
        foreach (var state in cellStates)
        {
            if (state == CellState.Filled)
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

    public bool Equals(LineConstraints? other)
    {
        if (other is null)
            return false;

        if (Count != other.Count)
            return false;

        return this.SequenceEqual(other);
    }

    public new LineConstraints Reverse()
    {
        return new LineConstraints(this);
    }
}
