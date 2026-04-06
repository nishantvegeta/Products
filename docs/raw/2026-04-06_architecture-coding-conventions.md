---
id: architecture-coding-conventions
date: 2026-04-06
time: "17:39:33"
source_type: architecture
knowledge_domain: technical
source_file: CLAUDE.md
title: Coding Conventions
tags: []
---

## Coding Conventions

### Naming Conventions
- **Entities:** PascalCase, singular (e.g., `Task`, `Project`, `User`)
- **DTOs:** Suffix with `Dto` (e.g., `TaskDto`, `CreateTaskDto`)
- **AppServices:** Suffix with `AppService` (e.g., `TaskAppService`)
- **Interfaces:** Prefix with `I` (e.g., `ITaskAppService`)
- **Methods:** PascalCase
- **Parameters/Local Variables:** camelCase
- **Private Fields:** _camelCase with underscore prefix
- **Constants:** PascalCase or UPPER_SNAKE_CASE depending on context

### File Organization
- One class per file
- File name matches the primary class name
- Related classes grouped in same folder
- Extension methods in static `Extensions` folder/class

### Async Patterns
- All I/O operations should be async
- Method names ending with `Async` (but exclude from interface)
- Use `CancellationToken` for long-running operations
- Return `Task<T>` or `Task` (not `void` except for event handlers)

### ABP Framework Conventions
- Always use ABP's base classes (`ApplicationService`, `CrudAppService`, `DomainService`)
- Prefer ABP's built-in features (soft delete, auditing, multi-tenancy)
- Use `IObjectMapper` (AutoMapper) for entity-DTO mapping
- Repository pattern via `IRepository<TEntity, TKey>`
- Localization strings in `Resources` folder
- Permission definitions in `Permissions` class
- Feature management via `IFeatureChecker`
