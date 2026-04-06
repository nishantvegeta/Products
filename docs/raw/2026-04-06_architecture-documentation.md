---
id: architecture-documentation
date: 2026-04-06
time: "17:39:33"
source_type: architecture
knowledge_domain: technical
source_file: CLAUDE.md
title: Documentation
tags: []
---

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
