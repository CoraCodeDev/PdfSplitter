# PdfSplitter - Agent Guide

## Project Overview

PdfSplitter is a .NET MAUI desktop application that allows users to load PDF files, select specific pages, and save them as new PDF files.

## Coding Standards

When working on code, follow the standards in:
- `coding-standards/csharp.md` — for C#/.NET

## Git Workflow

- **Never commit directly to `main` or `master`**
- Always work on feature/fix branches
- Branch naming: `feature/<feature-name>` or `fix/<fix-name>`
- **One feature per branch** - Each logical change should have its own branch
- **Don't start new work** until the previous branch's PR has been reviewed and merged
- **Never force push** - create new commits instead

## Commit Messages

Use **Semantic Commit Format**:
```
<type>(<scope>): <description>

[optional body]
[optional footer]
```

**Types:**
- `feat` - New feature
- `fix` - Bug fix
- `docs` - Documentation
- `style` - Formatting
- `refactor` - Code restructuring
- `test` - Tests
- `chore` - Maintenance

**Examples:**
```
feat(pdf): add page selection functionality
fix(save): resolve save file dialog issue
docs(readme): update getting started guide
```

## Pull Requests

- Create PR after completing work
- **Request review** - do not review your own PRs
- **Always verify CI/build jobs pass** before marking as ready or requesting review
- **Add a comment** summarizing the review, changes made, and build status
- **Do NOT merge** - wait for user confirmation
- Tests must pass before requesting review

## General Principles

- **One task at a time** - don't work on multiple things simultaneously
- If given multiple tasks, ask for priority order
- Complete one thing before starting another
- **Do not downgrade or update SDK versions** - SDK version changes and package version upgrades require explicit approval from the user before implementation

## Tech Stack

- **Framework:** .NET MAUI (multi-platform UI)
- **Language:** C# 12
- **PDF Library:** PdfPig (for reading PDF content)
- **PDF Creation:** QuestPDF (for creating new PDFs)

## Project Structure

```
PdfSplitter/
├── PdfSplitter/                    # Main .NET MAUI application
│   ├── App.xaml(.cs)               # Application entry point
│   ├── MauiProgram.cs              # MAUI program configuration
│   ├── Controls/                   # Custom UI controls
│   │   ├── PdfPageContent          # Individual page content view
│   │   ├── PdfPages                 # Pages list control
│   │   ├── PdfSaveControl           # Save dialog control
│   │   └── PdfSelectedPages         # Selected pages list
│   ├── Models/                      # Data models
│   │   ├── BindableModelBase        # MVVM base class
│   │   ├── PdfDocumentModel         # PDF document model
│   │   └── PdfPageItem              # Individual page model
│   ├── Services/                    # Business logic services
│   │   ├── PdfService               # PDF loading and page extraction
│   │   ├── SavePdfService           # PDF saving functionality
│   │   └── Interfaces/              # Service interfaces
│   ├── ViewModels/                  # MVVM view models
│   │   ├── AboutViewModel
│   │   ├── AppShellViewModel
│   │   ├── MainPageViewModel
│   │   └── PdfViewPageViewModel
│   ├── Views/                       # XAML views
│   │   ├── About.xaml
│   │   ├── AppShell.xaml
│   │   ├── MainPage.xaml
│   │   └── PdfViewPage.xaml
│   ├── Platforms/                   # Platform-specific code
│   │   └── Windows/                 # Windows-specific implementation
│   ├── Resources/                   # Assets and resources
│   │   ├── Fonts/                   # App fonts
│   │   ├── Images/                  # Images
│   │   ├── Styles/                  # XAML styles
│   │   └── Splash/                  # Splash screen
│   └── Properties/                  # Build properties
└── PdfSplitter.sln                  # Solution file
```

### Projects

| Project | Purpose |
|---------|---------|
| **PdfSplitter** | Main .NET MAUI application - runs on Windows, iOS, Android |

## Prerequisites

- .NET 10 SDK
- Visual Studio 2022+ or VS Code with .NET MAUI extension

## Getting Started

### Build
```bash
dotnet build
```

### Run Locally
```bash
dotnet run --project PdfSplitter
```

Or use your IDE's run configuration to launch the application.

### Run on Windows
```bash
dotnet run --project PdfSplitter -f net10.0-windows
```

## Architecture

The application follows the MVVM (Model-View-ViewModel) pattern:

- **Models:** Data structures representing PDF documents and pages
- **ViewModels:** Business logic and state management
- **Views:** XAML UI definitions
- **Services:** Reusable business logic (PDF loading, saving)

## Key Features

- Load PDF files from local storage
- Display PDF pages as thumbnails
- Select/deselect individual pages
- Preview selected pages
- Save selected pages as new PDF file
- Multi-platform support (Windows, iOS, Android)
