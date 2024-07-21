using System;
using System.Collections.Generic;
using System.Linq;

namespace NonogramAvalonia.ViewModels;

public class LineConstraints : List<int>, IEquatable<LineConstraints>
{
    public LineConstraints() { }

    public LineConstraints(IEnumerable<int> constraintValues)
    {
        AddRange(constraintValues);
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
