using System.Diagnostics;
using System.Windows.Threading;

namespace Sudoku.App.Features.Game;

public sealed class GameClock
{
    private readonly Stopwatch stopwatch = new();
    private readonly DispatcherTimer ticker = new() { Interval = TimeSpan.FromSeconds(1) };

    public GameClock() => ticker.Tick += (_, _) => Ticked?.Invoke();

    public event Action? Ticked;

    public TimeSpan Elapsed => stopwatch.Elapsed;

    public void Restart()
    {
        stopwatch.Restart();
        ticker.Start();
    }

    public void Stop()
    {
        stopwatch.Stop();
        ticker.Stop();
    }

    public static string Format(TimeSpan time) =>
        time.TotalHours >= 1 ? time.ToString(@"h\:mm\:ss") : time.ToString(@"mm\:ss");
}
