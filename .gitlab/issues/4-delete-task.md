# US-4: Delete Task - Remove irrelevant tasks

**Project:** ACMS (ID: 1)
**Issue Type:** User Story
**Labels:** `user-story`, `task-management`, `backend`, `frontend`, `api`
**Weight:** 5

## User Story

As a project manager, I want to delete a task that is no longer relevant so that the project board stays clean and accurate.

## Acceptance Criteria

- Delete option available from task detail view and list
- Confirmation dialog before deletion (prevent accidental clicks)
- After deletion, task is removed from list immediately
- Deletion is permanent (or soft delete with archive option)
- Appropriate error message if deletion fails

## Technical Tasks

### Backend

- [ ] Implement soft delete by default (IsDeleted flag)
- [ ] Add DeleteAsync(id) method in TaskAppService
- [ ] Check permissions: Only managers or task creator can delete
- [ ] Add cascade handling for related records (if any)

### API

- [ ] Add DELETE `/api/app/tasks/{id}` endpoint
- [ ] Return 204 No Content on success
- [ ] Return 404 if task not found
- [ ] Return 403 if user lacks permission

### Frontend

- [ ] Add Delete button on task detail page
- [ ] Add delete action in task list context menu
- [ ] Show confirmation dialog: "Are you sure you want to delete this task?"
- [ ] Remove deleted task from UI immediately
- [ ] Show success toast: "Task deleted successfully"
- [ ] Refresh task list after deletion

### Testing

- [ ] Unit tests for DeleteAsync with valid/invalid IDs
- [ ] Authorization tests (permission denied scenarios)
- [ ] Integration tests for DELETE endpoint
- [ ] E2E tests for delete flow with confirmation

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
