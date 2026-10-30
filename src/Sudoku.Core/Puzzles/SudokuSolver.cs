namespace Sudoku.Core.Puzzles;

public static class SudokuSolver
{
    private const int Size = 9;
    private const int CellCount = Size * Size;
    private const int AllDigits = 0b1_1111_1111;

    /// Counts solutions of the grid, stopping early once the limit is reached.
    public static int CountSolutions(int[] grid, int limit = 2, Random? shuffleWith = null)
    {
        var state = SolverState.From(grid);
        if (state is null)
            return 0;

        var found = 0;
        state.Search(ref found, limit, shuffleWith, null);
        return found;
    }

    /// Fills the grid with the first solution found, or returns null if there is none.
    public static int[]? Solve(int[] grid, Random? shuffleWith = null)
    {
        var state = SolverState.From(grid);
        if (state is null)
            return null;

        var found = 0;
        int[]? solution = null;
        state.Search(ref found, 1, shuffleWith, solved => solution = solved);
        return solution;
    }

    private sealed class SolverState
    {
        private readonly int[] cells;
        private readonly int[] rowMasks = new int[Size];
        private readonly int[] columnMasks = new int[Size];
        private readonly int[] boxMasks = new int[Size];

        private SolverState(int[] cells) => this.cells = cells;

        public static SolverState? From(int[] grid)
        {
            var state = new SolverState((int[])grid.Clone());
            for (var index = 0; index < CellCount; index++)
            {
                var digit = state.cells[index];
                if (digit == 0)
                    continue;

                var bit = 1 << (digit - 1);
                if ((state.UsedMask(index) & bit) != 0)
                    return null;

                state.Place(index, bit);
            }
            return state;
        }

        public void Search(ref int found, int limit, Random? shuffleWith, Action<int[]>? onSolved)
        {
            var (index, candidates) = PickMostConstrainedCell();
            if (index < 0)
            {
                found++;
                onSolved?.Invoke((int[])cells.Clone());
                return;
            }

            foreach (var bit in Digits(candidates, shuffleWith))
            {
                Place(index, bit);
                cells[index] = BitToDigit(bit);
                Search(ref found, limit, shuffleWith, onSolved);
                Remove(index, bit);
                cells[index] = 0;

                if (found >= limit)
                    return;
            }
        }

        private (int Index, int Candidates) PickMostConstrainedCell()
        {
            var bestIndex = -1;
            var bestCandidates = 0;
            var bestCount = int.MaxValue;

            for (var index = 0; index < CellCount; index++)
            {
                if (cells[index] != 0)
                    continue;

                var candidates = AllDigits & ~UsedMask(index);
                var count = int.PopCount(candidates);
                if (count >= bestCount)
                    continue;

                bestIndex = index;
                bestCandidates = candidates;
                bestCount = count;
                if (count <= 1)
                    break;
            }
            return (bestIndex, bestCandidates);
        }

        private int UsedMask(int index) =>
            rowMasks[index / Size] | columnMasks[index % Size] | boxMasks[BoxOf(index)];

        private void Place(int index, int bit)
        {
            rowMasks[index / Size] |= bit;
            columnMasks[index % Size] |= bit;
            boxMasks[BoxOf(index)] |= bit;
        }

        private void Remove(int index, int bit)
        {
            rowMasks[index / Size] &= ~bit;
            columnMasks[index % Size] &= ~bit;
            boxMasks[BoxOf(index)] &= ~bit;
        }

        private static int BoxOf(int index) => index / 27 * 3 + index % Size / 3;

        private static int BitToDigit(int bit) => int.TrailingZeroCount(bit) + 1;

        private static int[] Digits(int candidates, Random? shuffleWith)
        {
            var bits = new List<int>(Size);
            for (var mask = candidates; mask != 0; mask &= mask - 1)
                bits.Add(mask & -mask);

            var ordered = bits.ToArray();
            shuffleWith?.Shuffle(ordered);
            return ordered;
        }
    }
}
