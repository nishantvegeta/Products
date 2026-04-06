---
id: architecture-database-conventions
date: 2026-04-06
time: "17:39:33"
source_type: architecture
knowledge_domain: technical
source_file: CLAUDE.md
title: Database Conventions
tags: []
---

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
