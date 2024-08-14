using Nonogram.Domain;
using Nonogram.Domain.Solver;

namespace Nonogram.UnitTests.Solver;
public partial class PuzzleFullySolvedTests
{
    [Theory]
    [MemberData(nameof(SolvePuzzleFullyCases))]
    public void SolvePuzzles_FullySolved(NonogramPuzzle puzzle, List<string> expected)
    {
        var solver = new NonogramSolver(puzzle);
        var isSolved = solver.SolvePuzzle();
        NonogramUtility.UpdateStateFromSolver(puzzle, solver);
        var actual = NonogramUtility.PuzzleToCellStrings(puzzle);

        Assert.True(isSolved);
        Assert.Equal(expected, actual);
    }
}
