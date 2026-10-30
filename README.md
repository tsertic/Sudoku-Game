# Sudoku

A desktop Sudoku game modelled on the classic [Web Sudoku](https://www.websudoku.com/): one clean board, four difficulty levels, no row or column highlighting, and a score history that keeps track of how you're improving.

## Features

- Four levels: Easy, Medium, Hard, Evil
- Every puzzle is freshly generated and has exactly one solution
- Keyboard-first play: digits to fill, Backspace/Delete to erase, arrow keys to move
- "How am I doing?" check, "Show solution" and "Clear", just like the original
- Timer and score per solved puzzle, stored locally in SQLite
- Scores window with solved count, best time, average time and average score per level

## Scoring

Finishing at a level's par time earns its base score (Easy 1000, Medium 2000, Hard 3000, Evil 4000). Faster solves earn up to 1.5x, slower ones decay towards 0.25x, and every "How am I doing?" check costs 50 points. Revealing the solution doesn't record a result.

## Tech Stack

- C# / .NET 8, WPF
- SQLite via `Microsoft.Data.Sqlite`
- xUnit for the engine and score tests

## Setup

Requires Windows and the .NET 8 SDK or newer.

```bash
git clone <your-fork-url>
cd sudoku-websudoku-clone
dotnet run --project src/Sudoku.App
```

Run the tests with:

```bash
dotnet test
```

Scores live in `%LocalAppData%\Sudoku\scores.db`. Delete the file to reset your history.

## Contributing

Pull requests, issues, and ideas are all welcome. Whether it's fixing a typo,
improving performance, adding a feature, or rethinking the architecture —
if you see something that could be better, open a PR or start a discussion.

Check out [AGENTS.md](./AGENTS.md) for contribution guidelines.
