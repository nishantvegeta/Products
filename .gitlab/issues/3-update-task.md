# US-3: Update Task - Edit task status, assignee, and due date

**Project:** ACMS (ID: 1)
**Issue Type:** User Story
**Labels:** `user-story`, `task-management`, `backend`, `frontend`, `api`
**Weight:** 7

## User Story

As a project manager, I want to edit an existing task to update its status, reassign it, or change the due date so that the task reflects current priorities.

## Acceptance Criteria

- Task can be edited from detail view or inline in list
- Status can be changed (e.g., To Do, In Progress, Done)
- Assignee can be reassigned to another team member
- Due date can be modified
- Changes are saved and reflected immediately
- Edit history is tracked (who changed what and when)

## Technical Tasks

### Backend

- [ ] Add Status enum (ToDo, InProgress, Done, Blocked, Cancelled)
- [ ] Add EditHistory tracking (audit fields or separate table)
- [ ] Implement UpdateAsync(id, input) in TaskAppService
- [ ] Add optimistic concurrency control with RowVersion
- [ ] Add permission check: Users can only edit tasks assigned to them or if they have manager role

### API

- [ ] Add PUT `/api/app/tasks/{id}` endpoint
- [ ] Add PATCH `/api/app/tasks/{id}` for partial updates
- [ ] Return updated task in response
- [ ] Handle concurrency conflicts with 409 response

### Frontend

- [ ] Add Edit button on task detail page
- [ ] Create TaskEditModal with form pre-filled with current values
- [ ] Enable inline editing in task list (optional)
- [ ] Show status as dropdown with color coding
- [ ] Show confirmation dialog before unsaved changes
- [ ] Display success/error notifications

### Testing

- [ ] Unit tests for UpdateAsync with valid/invalid data
- [ ] Concurrency conflict tests
- [ ] Authorization tests (user can/cannot edit)
- [ ] Integration tests for PUT/PATCH endpoints
- [ ] E2E tests for edit flow

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
