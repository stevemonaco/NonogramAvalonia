namespace Nonogram.Domain;
internal static class Solver
{
    public static List<int> CreateLabels(LineConstraint constraint)
    {
        if (constraint.Count == 0)
            throw new ArgumentOutOfRangeException();

        int label = 1;
        List<int> labels = [-label, -label];
        label++;

        if (constraint.Sum() == 0) // All negative => all empty
            return labels;

        foreach (var segment in constraint)
        {
            for (int i = 0; i < segment; i++)
            {
                labels.Add(label);
                label++;
            }

            labels.Add(-label);
            labels.Add(-label);
            label++;
        }

        return labels;
    }

    public static LabelMap CreateMappings(IList<int> labels)
    {
        var afters = new LabelMap();

        for (int i = 0; i < labels.Count - 1; i++)
        {
            if (!afters.ContainsKey(labels[i]))
            {
                afters[labels[i]] = new List<int>();
            }
            afters[labels[i]].Add(labels[i + 1]);
        }

        return afters;
    }
}
