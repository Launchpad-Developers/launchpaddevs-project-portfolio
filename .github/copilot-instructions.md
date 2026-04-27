# Copilot Instructions

## Repository Purpose

This is a **case study portfolio** for Launchpad Developers Inc — a collection of documented client projects, not a single deployable application. Most subdirectories contain only documentation (README, screenshots, code snippets). The exception is `workout-tracker/source/`, which contains full source code as a Visual Studio solution (`ASWorkoutTracker.sln`).

## Repository Structure

Each project folder follows this layout:
```
project-name/
├── README.md          # Case study narrative (required)
├── screenshots/       # Annotated UI screenshots
├── snippets/
│   └── snippets.md    # Curated code samples (optional)
└── source/            # Full source only in workout-tracker
```

## README Convention

All project READMEs follow the same section structure — preserve this when adding or editing entries:

1. **Leadership & Project Overview** — client, scope, platform
2. **My Role** — specific contributions and ownership
3. **Leadership Principles in Action** — product influence, decisions made
4. **Key Capabilities** — feature list with emoji bullets
5. **Tech Stack** — specific libraries and services used
6. **Screenshots** — 3–6 annotated images
7. **Notes** — licensing, app store links, proprietary status

Use emoji-prefixed section headers (e.g., `## 🚀 Key Capabilities`) consistent with existing entries.

## Tech Stack Patterns

The projects span two eras of .NET mobile development:

- **Xamarin.Forms** — older projects (CUFI Mobile, Facility Fit suite, EM3 Admin, JPI Express, Leslie's Pools)
- **.NET MAUI** — newer projects (Workout Tracker, Spiritual Gifts Test, Into His Marvelous Light)
- **Xamarin.Native (iOS + Android)** — Participant Mobile, where platform performance required it

Common dependencies across all mobile projects:
- SQLite for offline-first data storage
- Azure AD / Azure B2C for enterprise authentication
- REST APIs for all backend communication (no GraphQL)
- MS App Center for CI/CD and crash reporting

## Architecture Conventions (from snippets)

### MVVM
All projects use MVVM. ViewModels implement `INotifyPropertyChanged`. Base classes handle boilerplate:

```csharp
// BaseViewModel pattern — used across all projects
public class BaseViewModel : INotifyPropertyChanged
{
    protected void OnPropertyChanged([CallerMemberName] string name = null) { ... }
}

// Generic list ViewModel
public class BaseListViewModel<T> : BaseViewModel
{
    public ObservableCollection<T> Items { get; set; }
}
```

### Type-Safe Enums
Prefer `TypeSafeEnum<T>` over standard C# enums for domain values that need comparison or display logic (pattern from EM3 Admin):

```csharp
public class TypeSafeEnum<T> where T : TypeSafeEnum<T>
{
    public string Value { get; }
    // ...
}
public class NameableTypeSafeEnum<T> : TypeSafeEnum<T>
{
    public string Name { get; }
}
```

### Settings / Preferences
Use a static `StandardSettings` class wrapping `Preferences` API for cross-page state, rather than passing state through navigation parameters.

### Dependency Injection (.NET MAUI)
Bootstrap services in `MauiProgram.cs` using the standard `builder.Services` pattern. Register ViewModels and platform services there.

### Offline-First
All enterprise apps queue writes locally in SQLite and sync on reconnect. ViewModels backed by SQLite use a primary key field (`[PrimaryKey, AutoIncrement]`).

## Snippet Style Guide

When adding entries to `snippets/snippets.md`, use this format:

```markdown
### FileName.cs
Brief one-line description of what pattern this illustrates.

```csharp
// code here
```
```

Snippets should demonstrate **clarity over cleverness** — practical, readable patterns for small-team velocity without over-abstraction.

## Workout Tracker (Source Available)

The only project with full source code. Built with .NET MAUI. Open with Visual Studio:
```
workout-tracker/source/ASWorkoutTracker.sln
```
Platform projects: `ASWorkoutTracker.iOS` and `ASWorkoutTracker.Android`.
