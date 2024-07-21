using Nonogram.Domain;

namespace Nonogram.UnitTests;
public partial class PuzzleSolverTests
{
    public static TheoryData<Puzzle, List<string>> SolvePuzzleFullyCases => new()
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
    public static Puzzle SmallStarPuzzle => new Puzzle(_smallStarRowConstraints, _smallStarColumnConstraints);
    public static LineConstraint[] _smallStarRowConstraints = [[2, 2], [2, 2], [0], [1, 1], [1, 1, 1]];
    public static LineConstraint[] _smallStarColumnConstraints = [[2, 2], [2], [1], [2], [2, 2]];
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
    public static Puzzle SmallHeartPuzzle => new Puzzle(_smallHeartRowConstraints, _smallHeartColumnConstraints);
    public static LineConstraint[] _smallHeartRowConstraints = [[2, 2], [5], [5], [3], [1]];
    public static LineConstraint[] _smallHeartColumnConstraints = [[3], [4], [4], [4], [3]];
    public static List<string> SmallHeartSolution =>
    [
        "ooxoo",
        "ooooo",
        "ooooo",
        "xooox",
        "xxoxx"
    ];

    public static Puzzle SingleLinePuzzle => new Puzzle(_singleLineRowConstraints, _singleLineColumnConstraints);
    public static LineConstraint[] _singleLineRowConstraints = [[4, 3]];
    public static LineConstraint[] _singleLineColumnConstraints = [[0], [1], [1], [1], [1], [0], [0], [1], [1], [1]];
    public static List<string> SingleLineSolution =>
    [
        "xooooxxooo"
    ];
}