namespace Sudoku.Core.Puzzles;

public sealed record Puzzle(Difficulty Difficulty, int[] Givens, int[] Solution);
