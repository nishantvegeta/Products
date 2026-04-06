---
id: architecture-test-standards
date: 2026-04-06
time: "17:39:33"
source_type: architecture
knowledge_domain: technical
source_file: CLAUDE.md
title: Test Standards
tags: []
---

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
