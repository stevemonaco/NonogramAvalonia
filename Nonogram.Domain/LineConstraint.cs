namespace Nonogram.Domain;

public class LineConstraint : List<int>
{
    public LineConstraint() { }

    public LineConstraint(IEnumerable<int> values) : base(values) { }

    public new LineConstraint Reverse()
    {
        return new LineConstraint(this);
    }
}
