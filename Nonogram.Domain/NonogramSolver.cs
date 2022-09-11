namespace Nonogram.Domain;

public enum SolveStrategy { Set, AlignmentPack };

public class NonogramSolver
{
    public bool TrySolveLine(IList<LineConstraint> constraints, IList<NonogramCell> cells)
    {
        //var regions = ChunkRegions(cells);

        return true;
    }

    public bool TrySolveLine(IList<LineConstraint> constraints, IList<NonogramCell> cells, SolveStrategy strategy)
    {
        if (strategy == SolveStrategy.AlignmentPack)
        {

        }

        return false;
    }

    private IEnumerable<IList<(int Index, int Length)>> InternalAlignmentPack(LineConstraint constraints, IList<(int Index, int Length)> chunks)
    {
        var alignment = new List<(int, int)>();

        var index = 0;
        var chunkIndex = 0;

        int i = 0;

        while (i < constraints.Items.Count)
        {
            var chunk = chunks[chunkIndex];
            var length = constraints.Items[i];

            if (index + length <= chunk.Index + chunk.Length)
            {
                alignment.Add((index, length));
                index += length + 1;
                i++;
            }
            else
            {
                chunkIndex++;
                if (chunkIndex == constraints.Items.Count)
                    yield break;
            }
        }

        yield return alignment;

        index--;
        chunkIndex = chunks.Count - 1;

        for (i = constraints.Items.Count - 1; i >= 0; i--)
        {
            var previousAlignment = alignment;
            alignment = new List<(int, int)>();

            var chunk = chunks[chunkIndex];
            var length = constraints.Items[i];

            if (index + length <= chunk.Index + chunk.Length)
            {
                alignment.Add((index, length));
                index += length + 1;
                i++;
            }
        }

        //foreach (var length in constraints.Items)
        //{
        //    var chunk = chunks[chunkIndex];
        //    if (index + length <= chunks[chunkIndex].Length)
        //}

        //while (!finished)
        //{
        //    foreach()
        //}
    }

    public static IEnumerable<(int, int)> ChunkIntervals(IList<NonogramCell> cells)
    {
        int startIndex = 0;
        int cellIndex = 0;

        foreach (var cell in cells)
        {
            if (cell.CellState == CellState.Empty)
            {
                if (startIndex != cellIndex)
                {
                    yield return (startIndex, cellIndex - startIndex);
                }
                startIndex = cellIndex + 1;
            }
            cellIndex++;
        }

        if (startIndex != cellIndex)
            yield return (startIndex, cellIndex - startIndex);
    }

    public static IEnumerable<SortedSet<int>> LabelCells(IList<int> constraints, IList<NonogramCell> cells)
    {
        //var count = cells.Count;
        //var freeCells = cells.Count - (constraints.Sum() + constraints.Count - 1);

        //var labels = Enumerable.Range(1, count).Select(x => -x);

        //labels.Aggregate()

        //for (int i = 0; i < cells.Count; i++)
        //{

        //}

        yield break;
    }
}
