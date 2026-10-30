using Sudoku.App.Shared;
using Sudoku.Core.Puzzles;
using Sudoku.Core.Scores;

namespace Sudoku.App.Features.Game;

public sealed class GameViewModel : ObservableObject
{
    private readonly ScoreRepository scores;
    private readonly PuzzleGenerator generator = new();
    private readonly GameClock clock = new();

    private Puzzle? puzzle;
    private CellViewModel? selectedCell;
    private Difficulty difficulty = Difficulty.Easy;
    private string timerText = "00:00";
    private string statusText = string.Empty;
    private string averageText = string.Empty;
    private bool isBusy;
    private bool isPlaying;
    private int checksUsed;

    public GameViewModel(ScoreRepository scores, Action showScores)
    {
        this.scores = scores;

        Levels = Enum.GetValues<Difficulty>().Select(level => new LevelOption(level)).ToList();
        Levels[0].IsSelected = true;
        Cells = Enumerable.Range(0, 81).Select(index => new CellViewModel(index)).ToList();
        clock.Ticked += () => TimerText = GameClock.Format(clock.Elapsed);

        SelectCellCommand = new RelayCommand(cell => Select((CellViewModel)cell!));
        SelectDifficultyCommand = new RelayCommand(
            level => _ = StartNewGameAsync((Difficulty)level!), _ => !isBusy);
        NewGameCommand = new RelayCommand(_ => _ = StartNewGameAsync(difficulty), _ => !isBusy);
        ClearCommand = new RelayCommand(_ => ClearEntries(), _ => isPlaying);
        CheckCommand = new RelayCommand(_ => CheckProgress(), _ => isPlaying);
        ShowSolutionCommand = new RelayCommand(_ => RevealSolution(), _ => isPlaying);
        ShowScoresCommand = new RelayCommand(_ => showScores());

        RefreshAverage();
    }

    public IReadOnlyList<CellViewModel> Cells { get; }
    public IReadOnlyList<LevelOption> Levels { get; }

    public RelayCommand SelectCellCommand { get; }
    public RelayCommand SelectDifficultyCommand { get; }
    public RelayCommand NewGameCommand { get; }
    public RelayCommand ClearCommand { get; }
    public RelayCommand CheckCommand { get; }
    public RelayCommand ShowSolutionCommand { get; }
    public RelayCommand ShowScoresCommand { get; }

    public Difficulty Difficulty
    {
        get => difficulty;
        private set
        {
            SetField(ref difficulty, value);
            foreach (var option in Levels)
                option.IsSelected = option.Level == value;
        }
    }

    public string TimerText
    {
        get => timerText;
        private set => SetField(ref timerText, value);
    }

    public string StatusText
    {
        get => statusText;
        private set => SetField(ref statusText, value);
    }

    public string AverageText
    {
        get => averageText;
        private set => SetField(ref averageText, value);
    }

    public async Task StartNewGameAsync(Difficulty level)
    {
        isBusy = true;
        isPlaying = false;
        clock.Stop();
        Difficulty = level;
        StatusText = "Shuffling a fresh puzzle...";

        var generated = await Task.Run(() => generator.Generate(level));
        puzzle = generated;

        foreach (var cell in Cells)
            cell.Load(generated.Givens[cell.Index]);

        checksUsed = 0;
        isPlaying = true;
        isBusy = false;
        StatusText = string.Empty;
        TimerText = "00:00";
        Select(Cells.First(cell => !cell.IsGiven));
        clock.Restart();
    }

    public void EnterDigit(int digit)
    {
        if (!isPlaying || selectedCell is null || selectedCell.IsGiven)
            return;

        selectedCell.Value = digit;
        StatusText = string.Empty;
        if (IsSolved())
            FinishGame();
    }

    public void MoveSelection(int rowStep, int columnStep)
    {
        var current = selectedCell ?? Cells[0];
        var row = Math.Clamp(current.Row + rowStep, 0, 8);
        var column = Math.Clamp(current.Column + columnStep, 0, 8);
        Select(Cells[row * 9 + column]);
    }

    private void Select(CellViewModel cell)
    {
        if (selectedCell is not null)
            selectedCell.IsSelected = false;

        selectedCell = cell;
        cell.IsSelected = true;
    }

    private void ClearEntries()
    {
        foreach (var cell in Cells.Where(cell => !cell.IsGiven))
            cell.Value = 0;
        StatusText = string.Empty;
    }

    private void CheckProgress()
    {
        checksUsed++;
        var allCorrect = Cells
            .Where(cell => cell.Value != 0)
            .All(cell => cell.Value == puzzle!.Solution[cell.Index]);

        StatusText = allCorrect
            ? "So far so good. Keep going!"
            : "Something doesn't add up. Take another look.";
    }

    private void RevealSolution()
    {
        foreach (var cell in Cells)
            cell.Value = puzzle!.Solution[cell.Index];

        EndGame();
        StatusText = "Solution revealed. This one doesn't count towards your score.";
    }

    private bool IsSolved() => Cells.All(cell => cell.Value == puzzle!.Solution[cell.Index]);

    private void FinishGame()
    {
        EndGame();
        var duration = clock.Elapsed;
        var score = ScoreCalculator.Calculate(difficulty, duration, checksUsed);
        scores.Save(new GameResult(difficulty, duration, checksUsed, score, DateTime.UtcNow));

        StatusText = $"Solved in {GameClock.Format(duration)}. You scored {score:N0} points!";
        RefreshAverage();
    }

    private void EndGame()
    {
        isPlaying = false;
        clock.Stop();
        TimerText = GameClock.Format(clock.Elapsed);
    }

    private void RefreshAverage()
    {
        var stats = scores.LoadStats();
        AverageText = stats.AverageScore is { } average
            ? $"Average score {average:N0} over {stats.GamesSolved} solved"
            : "No solved puzzles yet";
    }
}
