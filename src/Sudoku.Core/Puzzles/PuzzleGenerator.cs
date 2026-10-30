namespace Sudoku.Core.Puzzles;

public sealed class PuzzleGenerator(Random? random = null)
{
    private const int CellCount = 81;
    private readonly Random random = random ?? new Random();

    public Puzzle Generate(Difficulty difficulty)
    {
        var solution = SudokuSolver.Solve(new int[CellCount], random)
            ?? throw new InvalidOperationException("An empty grid always has a solution.");

        var (min, max) = difficulty.GivenRange();
        var targetGivens = random.Next(min, max + 1);
        var givens = (int[])solution.Clone();
        var remaining = CellCount;

        foreach (var index in Enumerable.Range(0, CellCount).OrderBy(_ => random.Next()))
        {
            if (remaining <= targetGivens)
                break;

            var digit = givens[index];
            givens[index] = 0;

            if (SudokuSolver.CountSolutions(givens) != 1)
                givens[index] = digit;
            else
                remaining--;
        }

        return new Puzzle(difficulty, givens, solution);
    }
}
