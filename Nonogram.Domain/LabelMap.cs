namespace Nonogram.Domain;

/// <summary>
/// Used to map a Cell's labels to the prospective (possible) labels of the following Cell
/// </summary>
internal class LabelMap : Dictionary<int, List<int>>
{
    public HashSet<int> GetNextProspectiveLabels(IEnumerable<int> currentProspects)
    {
        return currentProspects.SelectMany(x => this[x]).ToHashSet();
    }
}
