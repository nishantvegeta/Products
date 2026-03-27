# Claude Code Configuration - ACMS Project

This file contains project-specific instructions and conventions for Claude Code when working on the ACMS (Administrative Case Management System) project.

## Project Overview

- **Project Name:** ACMS (Administrative Case Management System)
- **GitLab Project:** `root/biometric` (ID: 4)
- **Primary Module:** Task Management
- **Architecture:** ABP Framework with Domain-Driven Design
- **Repository:** https://gitlab.local:8080/root/biometric

## Technology Stack

### Backend
- **Framework:** ABP Framework v7.x+
- **Language:** C# / .NET 8.0
- **ORM:** Entity Framework Core
- **Database:** PostgreSQL (development), SQL Server (production)
- **Authentication:** OpenIddict / JWT Bearer tokens
- **API:** RESTful with ASP.NET Core Web API

### Frontend
- **Framework:** To be determined (Blazor, Angular, or React)
- **UI Components:** ABP LeptonX theme
- **State Management:** To be determined

### DevOps
- **CI/CD:** GitLab CI/CD
- **Containerization:** Docker & Docker Compose
- **Orchestration:** Kubernetes (production)
- **Monitoring:** (To be determined)

## Project Structure

```
src/
├── Acme.Products.Domain/           # Domain layer (entities, value objects, domain services)
├── Acme.Products.Application/      # Application layer (Dtos, AppServices, CQRS)
├── Acme.Products.EntityFrameworkCore/ # EF Core configurations, DbContext, migrations
├── Acme.Products.HttpApi/          # API controllers, routing, filters
├── Acme.Products.HttpApi.Client/   # TypeScript client proxies
└── Acme.Products.Web/              # Web UI project (Blazor/MVC/SPA)

tests/
├── Acme.Products.Domain.UnitTests/
├── Acme.Products.Application.UnitTests/
├── Acme.Products.Application.IntegrationTests/
└── Acme.Products.Web.IntegrationTests/
```

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

## Test Standards

### Unit Tests
- Use xUnit or NUnit
- Arrange-Act-Assert (AAA) pattern
- Mock dependencies using Moq
- Test coverage target: >80%
- One test class per production class
- Test method names: `MethodName_StateUnderTest_ExpectedBehavior`

### Integration Tests
- Use test database (SQLite in-memory for EF Core)
- Test API endpoints with `WebApplicationFactory`
- Include real database operations
- Seed test data in fixtures
- Clean up after tests

### E2E Tests
- Playwright or Selenium for UI tests
- Test complete user workflows
- Use test users with specific roles
- Run in isolated test environment

## Git Workflow

### Branch Strategy
- `main` - production-ready code
- `develop` - integration branch (optional, based on team preference)
- Feature branches: `feature/issue-XX-description`
- Bugfix branches: `bugfix/issue-XX-description`
- Hotfix branches: `hotfix/description`

### Commit Messages
Follow [Conventional Commits](https://www.conventionalcommits.org/):

```
type(scope): description

feat(task-management): add create task API endpoint
fix(application): handle null assignee in task creation
test(integration): add task creation E2E tests
refactor(domain): extract task status enum
docs(api): update task API documentation
chore(dependencies): update AutoMapper to 12.0
```

**Types:** feat, fix, test, refactor, docs, chore, style, ci, build

### Pull Request Guidelines
- Target branch: `main`
- One PR per feature/bugfix
- Include issue reference in description: `Closes #1`
- PR title: concise summary (<50 chars)
- PR description: explain what, why, and how
- All tests must pass
- Code review required (at least 1 approver)
- Squash merge commits

### Issue References
- Always reference issue numbers: `Fixes #1`, `Closes #2`, `Relates to #3`
- Use GitHub Flavored Markdown in PR/issue descriptions
- Link to relevant documentation or external resources

## GitLab Integration

### Issues
- Use GitLab issues for tracking work
- Each issue should have:
  - Clear user story format
  - Acceptance criteria
  - Technical task breakdown (Backend, API, Frontend, Testing)
  - Labels: `user-story`, `backend`, `frontend`, `api`, `testing`
  - Weight estimation for planning
  - Definition of Ready/DoD checklists

### Labels
- `user-story` - Business requirement
- `task-management` - Task Management module
- `backend` - Server-side implementation
- `frontend` - User interface work
- `api` - API endpoint development
- `testing` - Test automation
- `bug` - Bug fixes
- `enhancement` - Feature enhancements
- `documentation` - Docs updates

### Milestones
- Group related issues into sprints
- Sprint naming: `Sprint 1`, `Sprint 2`, etc.
- Use for release planning and burndown tracking

### Merge Request Templates
- Follow ABP's standard MR template
- Include linked issue numbers
- Checklist for reviews
- Test verification notes

## API Design Guidelines

### RESTful Principles
- Use nouns for resources (not verbs)
- Plural resource names: `/api/app/tasks`
- Use proper HTTP verbs: GET, POST, PUT, PATCH, DELETE
-versioning via URL path: `/api/app/v1/tasks` or header-based

### Response Format
```json
{
  "result": { /* data */ },
  "target": "/api/app/tasks",
  "success": true,
  "error": null,
  "unAuthorizedRequest": false
}
```

ABP's default response wrapper

### Error Handling
- Use proper HTTP status codes
- Return validation errors with 400 Bad Request
- Include error details in response body
- Log exceptions server-side
- Don't expose stack traces in production

### Pagination
- Use `Skip` and `MaxResultCount` parameters
- Return `PagedResultDto<T>` with total count
- Default page size: 10-50 items
- Max page size: 100

### Filtering & Sorting
- Provide `Filter` and `Sorting` parameters
- Use `Name` property for sorting fields
- Support multiple sort criteria

## Database Conventions

### Migrations
- One migration per feature/batch of changes
- Migration names: `AddTaskTable`, `UpdateTaskStatus`
- Auto-generate migrations via `dotnet ef migrations add`
- Review generated migration code (avoid destructive operations)
- Test migrations on copy of production data

### Entity Design
- Base entity: `AggregateRoot` or `Entity`
- Include `Id` (GUID or long)
- Concurrency token: `RowVersion` (byte[])
- Soft delete: `IsDeleted` flag (ABP built-in)
- Auditing: `CreationTime`, `CreatorId`, `LastModificationTime`, `LastModifierId`
- Use value objects for complex attributes

## Authorization

### Permissions
- Define permissions in `Permissions` class
- Permission names: `{Module}.{Action}` (e.g., `TaskManagement.Create`)
- Permission groups: `Pages`, `Features`, `Data`
- Multi-tenant awareness (if applicable)

### Role-Based Access
- Create roles: `Admin`, `Manager`, `User`, `Guest`
- Assign permissions to roles
- Users inherit permissions from roles
- Test authorization at unit and integration levels

## Documentation

### Code Documentation
- XML comments for public APIs (methods, classes, properties)
- Summary description, parameter docs, return value docs
- Use `<remarks>` for complex logic
- Keep documentation up-to-date with code changes

### API Documentation
- Swagger/OpenAPI auto-generated from controllers
- Add description attributes for clarity
- Document request/response examples
- Mark required vs optional parameters

### README Files
- Each project/solution needs README.md
- Include: purpose, setup instructions, dependencies, build/run commands
- Link to external docs (ABP, GitLab, etc.)

## Quality Standards

### Code Reviews
- Check for: correctness, performance, security, maintainability
- Verify test coverage
- Ensure compliance with conventions
- Look for code smells and anti-patterns

### Security
- Validate all user inputs
- Use parameterized queries (EF Core handles this)
- Implement rate limiting on APIs
- Store secrets in environment variables or secret managers
- Never commit sensitive data (connection strings, keys)

### Performance
- Use async/await for I/O operations
- Implement caching where appropriate (ABP's ICacheManager)
- Avoid N+1 queries (use `.Include()` or explicit joins)
- Profile slow queries with tools
- Consider indexing for frequent search columns

## Development Environment

### Prerequisites
- .NET 8.0 SDK
- IDE: Visual Studio 2022+ or JetBrains Rider or VS Code
- Git (with credentials for GitLab)
- Docker & Docker Compose
- PostgreSQL (if not using Docker)

### Local Setup
1. Clone repository: `git clone <repo-url>`
2. Copy `.env.example` to `.env` and configure
3. Restore NuGet packages: `dotnet restore`
4. Apply migrations: `dotnet ef database update`
5. Run application: `dotnet run --project src/Acme.Products.Web`
6. Open browser: `https://localhost:44321`

### Docker Setup (Recommended)
```bash
cd .docker
docker-compose -f docker-compose.Development.yml up
```

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

## Getting Help

- ABP Documentation: https://docs.abp.io
- GitLab Project: http://gitlab.local:8080/root/acms
- Team Conventions: See `docs/` folder
- Architecture Decisions: See `docs/adr/` folder

## Notes for Claude Code

When working on this project:
1. Follow the ABP Framework patterns and conventions
2. Use dependency injection (constructor injection)
3. Prefer interfaces over concrete implementations
4. Keep domain logic in the Domain layer, not in Application
5. Write tests for all new code (unit & integration)
6. Update documentation when changing public APIs
7. Reference GitLab issues in commits and PRs
8. Keep commits focused and atomic
9. Use meaningful branch names descriptive of the feature
10. Always run tests before committing

### Code Navigation and Analysis

Prefer LSP (Language Server Protocol) operations over Grep/Read for code navigation — it's faster, more precise, and avoids reading entire files:

- **`workspaceSymbol`** - Find where something is defined across the codebase
- **`findReferences`** - See all usages of a symbol across the codebase
- **`goToDefinition` / `goToImplementation`** - Jump directly to source code
- **`hover`** - Get type info and documentation without reading the file

Use **Grep** only when LSP isn't available or for text/pattern searches (comments, strings, configuration).

**After writing or editing code**, always check LSP diagnostics and fix any errors before proceeding.

### When generating code:
- Generate complete, compilable code following ABP patterns
- Include proper XML documentation
- Add unit tests for new Application/Domain services
- Use the correct project structure and namespaces
- Follow the naming conventions strictly

### When refactoring:
- Ensure existing tests still pass
- Don't change public APIs without discussion
- Update affected documentation
- Consider backwards compatibility

### When fixing bugs:
- Write a failing test first (if no test exists)
- Fix the bug with minimal changes
- Verify the fix doesn't break other functionality
- Add regression test if appropriate

### When implementing features:
- Start from the Domain model (entities, value objects)
- Build Application layer (AppServices, DTOs)
- Then implement Infrastructure (DbContext, repositories)
- Finally, create API controllers
- Keep layers clean and properly separated

---

**Last Updated:** 2026-03-18
**Maintained By:** ACMS Development Team