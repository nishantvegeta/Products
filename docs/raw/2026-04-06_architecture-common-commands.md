---
id: architecture-common-commands
date: 2026-04-06
time: "17:39:33"
source_type: architecture
knowledge_domain: technical
source_file: CLAUDE.md
title: Common Commands
tags: []
---

## Common Commands

### .NET CLI
```bash
dotnet restore              # Restore NuGet packages
dotnet build                # Build solution
dotnet test                 # Run all tests
dotnet run --project <proj> # Run specific project
dotnet ef migrations add <name>  # Add EF Core migration
dotnet ef database update   # Apply migrations
```

### Git
```bash
git checkout -b feature/issue-XX-description  # Create feature branch
git add .                                          # Stage changes
git commit -m "feat: description"                 # Commit
git push -u origin feature/issue-XX-description  # Push
```

### Testing
```bash
dotnet test --logger "console;verbosity=detailed"  # Run tests with output
dotnet test --filter "FullyQualifiedName~Task"     # Filter tests
```
