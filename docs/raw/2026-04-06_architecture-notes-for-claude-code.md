---
id: architecture-notes-for-claude-code
date: 2026-04-06
time: "17:39:33"
source_type: architecture
knowledge_domain: technical
source_file: CLAUDE.md
title: Notes for Claude Code
tags: []
---

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
