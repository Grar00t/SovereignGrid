# Architecture

## Style

Modular Monolith

## UI

WPF

MVVM

## Projects

### SovereignGrid.App

Desktop application shell.

Responsibilities:

- Startup
- Navigation
- Theme management
- Localization

### SovereignGrid.Core

Business domain.

Responsibilities:

- Workbook model
- Asset model
- Validation rules
- Policy evaluation

### SovereignGrid.Storage

Persistence layer.

Responsibilities:

- File storage
- Backup
- Recovery
- Data integrity

### SovereignGrid.Plugins

Extension system.

Responsibilities:

- Plugin loading
- Plugin isolation
- Plugin contracts

### SovereignGrid.Tests

Automated tests.

## Network Policy

Deny by default.

## Telemetry Policy

Disabled.

## Cloud Policy

Optional.
Never required.

## Supported Platforms

Windows x64

## Localization

Primary:
English

Secondary:
Arabic

RTL:
Supported

