namespace Nonogram.Domain;

/// <summary>
/// Used to map a Cell's labels to prospective following Cell labels
/// </summary>
internal class LabelMap : Dictionary<int, List<int>>
{
    public HashSet<int> GetNextProspectiveLabels(IEnumerable<int> currentProspects)
    {
        return currentProspects.SelectMany(x => this[x]).ToHashSet();
    }
}
