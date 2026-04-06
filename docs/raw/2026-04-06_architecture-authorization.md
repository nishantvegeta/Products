---
id: architecture-authorization
date: 2026-04-06
time: "17:39:33"
source_type: architecture
knowledge_domain: technical
source_file: CLAUDE.md
title: Authorization
tags: []
---

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
