using Sudoku.Core.Puzzles;

namespace Sudoku.Tests;

public class PuzzleGeneratorTests
{
    [Theory]
    [InlineData(Difficulty.Easy)]
    [InlineData(Difficulty.Medium)]
    [InlineData(Difficulty.Hard)]
    [InlineData(Difficulty.Evil)]
    public void Generated_puzzle_has_unique_solution_and_expected_clue_count(Difficulty difficulty)
    {
        var puzzle = new PuzzleGenerator(new Random(42)).Generate(difficulty);

        var givenCount = puzzle.Givens.Count(digit => digit != 0);
        var (min, max) = difficulty.GivenRange();

        Assert.InRange(givenCount, min, max);
        Assert.Equal(1, SudokuSolver.CountSolutions(puzzle.Givens));
        Assert.Equal(puzzle.Solution, SudokuSolver.Solve(puzzle.Givens));
    }

    [Fact]
    public void Solution_respects_sudoku_rules()
    {
        var solution = new PuzzleGenerator(new Random(7)).Generate(Difficulty.Easy).Solution;

        for (var unit = 0; unit < 9; unit++)
        {
            var row = Enumerable.Range(0, 9).Select(i => solution[unit * 9 + i]);
            var column = Enumerable.Range(0, 9).Select(i => solution[i * 9 + unit]);
            var box = Enumerable.Range(0, 9)
                .Select(i => solution[(unit / 3 * 3 + i / 3) * 9 + unit % 3 * 3 + i % 3]);

            Assert.Equal(Enumerable.Range(1, 9), row.Order());
            Assert.Equal(Enumerable.Range(1, 9), column.Order());
            Assert.Equal(Enumerable.Range(1, 9), box.Order());
        }
    }

    [Fact]
    public void Solver_rejects_grid_with_duplicate_digits()
    {
        var grid = new int[81];
        grid[0] = 5;
        grid[1] = 5;

        Assert.Equal(0, SudokuSolver.CountSolutions(grid));
    }
}
