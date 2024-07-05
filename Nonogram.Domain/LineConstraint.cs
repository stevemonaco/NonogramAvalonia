namespace Nonogram.Domain;

public class LineConstraint : IEquatable<LineConstraint>
{
    public List<int> Items { get; private set; } = new List<int>();

    public LineConstraint() { }

    public LineConstraint(IEnumerable<int> constraintValues)
    {
        foreach(var value in constraintValues)
            Items.Add(value);
    }

    public void Add(int constraint)
    {
        Items.Add(constraint);
    }

    public bool Equals(LineConstraint? other)
    {
        if (other is null)
            return false;

        if (Items.Count != other.Items.Count)
            return false;

        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i] != other.Items[i])
                return false;
        }

        return true;
    }
}
