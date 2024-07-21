using Nonogram.Domain;

namespace Nonogram.UnitTests;
public partial class PuzzleSolverTests
{
    [Theory]
    [MemberData(nameof(SolvePuzzleFullyCases))]
    public void SolvePuzzles_FullySolved(Puzzle puzzle, List<string> solution)
    {
        var isSolved = puzzle.SolvePuzzle();

        var expected = solution;
        var actual = NonogramUtility.ToCellStrings(puzzle);

        Assert.True(isSolved);
        Assert.Equal(expected, actual);
    }
}
