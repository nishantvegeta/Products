---
id: architecture-git-workflow
date: 2026-04-06
time: "17:39:33"
source_type: architecture
knowledge_domain: technical
source_file: CLAUDE.md
title: Git Workflow
tags: []
---

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
