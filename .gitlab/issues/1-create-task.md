# US-1: Create Task - Project Manager can create new tasks

**Project:** ACMS (ID: 1)
**Issue Type:** User Story
**Labels:** `user-story`, `task-management`, `backend`, `frontend`, `api`
**Weight:** 8

## User Story

As a project manager, I want to create a new task with a title, description, assignee, and due date so that team members know what work needs to be done.

## Acceptance Criteria

- Task must have a title (required), description (optional), assignee, and due date
- On save, the task appears immediately in the task list
- A confirmation message is shown after successful creation

## Technical Tasks

### Backend

- [ ] Create Task entity/model with properties: Id, Title, Description, AssigneeId, DueDate, Status, CreatedAt, UpdatedAt
- [ ] Create TaskDto for input/output
- [ ] Implement ICreateTaskAppService with CreateAsync method
- [ ] Add validation: Title required, DueDate must be future date, AssigneeId must exist
- [ ] Add database migration for Tasks table
- [ ] Create Task repository/extensions for EF Core

### API

- [ ] Add POST `/api/app/tasks` endpoint
- [ ] Configure authorization: Require 'Tasks.Create' permission
- [ ] Add API controller with proper error handling
- [ ] Add request/response logging

### Frontend

- [ ] Create TaskCreateModal component/page
- [ ] Build form with fields: Title (required), Description (textarea), Assignee (dropdown), Due Date (date picker)
- [ ] Implement form validation
- [ ] Add success/error message notifications
- [ ] Refresh task list after successful creation

### Testing

- [ ] Unit tests for TaskAppService creation logic
- [ ] Integration tests for POST endpoint
- [ ] E2E tests for task creation UI flow

## Definition of Ready

- [ ] Business value is clear
- [ ] Acceptance criteria are defined
- [ ] Dependencies are identified
- [ ] Technical design is reviewed
- [ ] UI/UX mockups are available (if needed)

## Definition of Done

- [ ] All acceptance criteria are met
- [ ] Code is reviewed and approved
- [ ] Unit tests pass with >80% coverage
- [ ] Integration tests pass
- [ ] E2E tests pass
- [ ] Documentation is updated
- [ ] Code is merged to main branch
- [ ] Feature works in production environment
