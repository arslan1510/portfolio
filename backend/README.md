# Backend - F# Web API

This folder contains the F# backend application built with Oxpecker.

## Structure

```
backend/
├── Models.fs           # Data models and types
├── DataLoader.fs       # JSON data loading utilities
├── GitHubService.fs    # GitHub API integration
├── TerminalCommands.fs # Terminal command implementations
├── TerminalView.fs     # HTML view generation
├── Handlers.fs         # HTTP request handlers
├── Program.fs          # Application entry point
├── project.fsproj      # F# project configuration
├── data/               # JSON data files
│   ├── about.json      # About information
│   ├── contact.json    # Contact details
│   ├── help.json       # Help text
│   └── skills.json     # Technical skills
├── Dockerfile          # Container configuration
└── railway.json        # Railway deployment config
```

## Architecture

### Core Components

#### Models.fs

- Defines all data types and models
- Project, GitHubRepo, ContactData, etc.
- Provides type safety throughout the application

#### DataLoader.fs

- Loads JSON data from files
- Caches data for performance
- Provides typed access to configuration

#### GitHubService.fs

- Integrates with GitHub API
- Fetches public repositories
- Handles API errors gracefully

#### TerminalCommands.fs

- Implements all terminal commands
- Handles command parsing and execution
- Returns structured results

#### TerminalView.fs

- Server-side HTML generation using Oxpecker ViewEngine
- Responsive terminal interface
- HTMX integration for interactivity

#### Handlers.fs

- HTTP request handlers
- Routes requests to appropriate functions
- Manages HTMX responses

#### Program.fs

- Application configuration
- Middleware setup
- Server startup

### Technology Stack

- **F#** - Primary language
- **Oxpecker** - Web framework
- **Oxpecker.ViewEngine** - HTML DSL
- **Oxpecker.Htmx** - HTMX integration
- **ASP.NET Core** - Web server

## Development

### Prerequisites

- .NET 9.0 SDK
- F# development tools

### Running

```bash
dotnet run
```

### Building

```bash
dotnet build
```

### Testing

```bash
dotnet test
```

### Deployment

The application is configured for Railway deployment with Docker.

## Configuration

### Environment Variables

- `PORT` - HTTP port (default: 8080)
- `ASPNETCORE_ENVIRONMENT` - Environment (Development/Production)

### Data Files

All content is stored in JSON files in the `data/` directory:

## API Endpoints

- `GET /` - Main terminal interface
- `POST /api/terminal/command` - Process terminal commands
- `GET /api/github` - GitHub API proxy
