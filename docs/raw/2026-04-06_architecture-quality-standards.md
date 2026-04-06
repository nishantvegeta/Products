---
id: architecture-quality-standards
date: 2026-04-06
time: "17:39:33"
source_type: architecture
knowledge_domain: technical
source_file: CLAUDE.md
title: Quality Standards
tags: []
---

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
