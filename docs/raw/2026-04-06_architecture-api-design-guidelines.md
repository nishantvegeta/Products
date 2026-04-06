---
id: architecture-api-design-guidelines
date: 2026-04-06
time: "17:39:33"
source_type: architecture
knowledge_domain: technical
source_file: CLAUDE.md
title: API Design Guidelines
tags: []
---

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
