using Nonogram.Domain;

namespace Nonogram.UnitTests;
public partial class PuzzleSolverTests
{
    [Theory]
    [MemberData(nameof(SolvePuzzleFullyCases))]
    public void SolvePuzzles_FullySolved(Puzzle puzzle, List<string> expected)
    {
        var isSolved = puzzle.SolvePuzzle();
        var actual = NonogramUtility.ToCellStrings(puzzle);

        Assert.True(isSolved);
        Assert.Equal(expected, actual);
    }
}
