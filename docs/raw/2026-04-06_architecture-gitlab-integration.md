---
id: architecture-gitlab-integration
date: 2026-04-06
time: "17:39:33"
source_type: architecture
knowledge_domain: technical
source_file: CLAUDE.md
title: GitLab Integration
tags: []
---

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
