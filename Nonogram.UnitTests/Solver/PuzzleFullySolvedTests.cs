using Nonogram.Domain;

namespace Nonogram.UnitTests.Solver;
public partial class PuzzleFullySolvedTests
{
    [Theory]
    [MemberData(nameof(SolvePuzzleFullyCases))]
    public void SolvePuzzles_FullySolved(NonogramPuzzle puzzle, List<string> expected)
    {
        var solver = new NonogramSolver(puzzle);
        var isSolved = solver.SolvePuzzle();
        var actual = NonogramUtility.ToCellStrings(puzzle);

        Assert.True(isSolved);
        Assert.Equal(expected, actual);
    }
}
