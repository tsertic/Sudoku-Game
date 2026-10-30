# AGENTS.md

## Welcome

This project is open to contributions of any kind — code, design, documentation, ideas, bug reports. If you want to make it better, you're welcome here.

---

## Getting Started

1. Fork the repository
2. Clone your fork locally
3. Install the .NET 8 SDK (or newer) on Windows
4. Create a feature branch from `dev`: `git checkout -b feat/your-feature-name`
5. Make your changes
6. Run `dotnet build` and `dotnet test`, then launch with `dotnet run --project src/Sudoku.App`
7. Submit a pull request to `dev`

---

## Project Layout

```
src/Sudoku.Core   puzzle generation, solver, scoring and SQLite storage (no UI references)
src/Sudoku.App    WPF app, organised by feature (Game, Stats) plus Shared helpers
tests/Sudoku.Tests  xUnit tests for the engine and scores
```

Keep game rules and persistence in `Sudoku.Core` so they stay testable. The WPF project should only deal with presentation and view models.

---

## Code Style

- **Readable code over clever code.** If you have to re-read it twice, simplify it.
- **No decorative comments.** No section headers, no ascii dividers. Brief function docs only when the name isn't self-explanatory.
- **Small, focused types.** No god classes. Feature-based folders in the app project.
- **English everywhere.** Code, comments, docs, UI strings — all English.
- **Design with intention.** If you're touching UI, make it look good. Use the tokens in `Shared/Theme.xaml` instead of ad-hoc colors.
- Add a test when you touch generation, solving or scoring logic.

---

## Branch Naming

```
feat/short-description    → new features
fix/short-description     → bug fixes
refactor/short-description → code improvements
docs/short-description    → documentation updates
```

---

## Commit Messages

We use conventional commits:

```
feat: add notes mode for pencil marks
fix: correct score for puzzles solved under par
refactor: simplify solver candidate picking
docs: update scoring section
chore: upgrade dependencies
```

Keep them lowercase, concise, and meaningful.

---

## Pull Request Guidelines

- One logical change per PR
- Describe **what** you changed and **why**
- Include screenshots for UI changes
- Make sure the app builds, runs and the tests pass
- Keep PRs small — easier to review, faster to merge

---

## Reporting Issues

Found a bug or have an idea? Open an issue with:

- Clear title
- Steps to reproduce (for bugs)
- Expected vs actual behavior
- Screenshots if relevant

---

## Philosophy

- Ship working software in small increments
- Readability and maintainability over premature optimization
- Every contribution matters, no matter how small
