using Nonogram.Domain;

namespace Nonogram.UnitTests.Solver;
public partial class PuzzleFullySolvedTests
{
    public static TheoryData<NonogramPuzzle, List<string>> SolvePuzzleFullyCases => new()
    {
        { TestData.SmallStarPuzzle, TestData.SmallStarSolution },
        { TestData.SmallHeartPuzzle, TestData.SmallHeartSolution },
        { TestData.SingleLinePuzzle, TestData.SingleLineSolution },
    };
}

file class TestData
{
    /// <summary>
    /// 5x5 puzzle in the shape of a small star
    /// Contains an empty row, lines fully constrained in 1d, and lines requiring 2d constraints to solve
    /// </summary>
    public static NonogramPuzzle SmallStarPuzzle => new NonogramPuzzle(_smallStarRowConstraints, _smallStarColumnConstraints);
    public static LineConstraints[] _smallStarRowConstraints = [[2, 2], [2, 2], [0], [1, 1], [1, 1, 1]];
    public static LineConstraints[] _smallStarColumnConstraints = [[2, 2], [2], [1], [2], [2, 2]];
    public static List<string> SmallStarSolution =>
    [
        "ooxoo",
        "ooxoo",
        "xxxxx",
        "oxxxo",
        "oxoxo"
    ];

    /// <summary>
    /// 5x5 puzzle in the shape of a small heart
    /// Contains some entirely filled rows, lines fully constrained in 1d, and lines requiring 2d constraints to solve
    /// </summary>
    public static NonogramPuzzle SmallHeartPuzzle => new NonogramPuzzle(_smallHeartRowConstraints, _smallHeartColumnConstraints);
    public static LineConstraints[] _smallHeartRowConstraints = [[2, 2], [5], [5], [3], [1]];
    public static LineConstraints[] _smallHeartColumnConstraints = [[3], [4], [4], [4], [3]];
    public static List<string> SmallHeartSolution =>
    [
        "ooxoo",
        "ooooo",
        "ooooo",
        "xooox",
        "xxoxx"
    ];

    public static NonogramPuzzle SingleLinePuzzle => new NonogramPuzzle(_singleLineRowConstraints, _singleLineColumnConstraints);
    public static LineConstraints[] _singleLineRowConstraints = [[4, 3]];
    public static LineConstraints[] _singleLineColumnConstraints = [[0], [1], [1], [1], [1], [0], [0], [1], [1], [1]];
    public static List<string> SingleLineSolution =>
    [
        "xooooxxooo"
    ];
}