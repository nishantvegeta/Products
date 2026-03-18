# US-2: Read Tasks - View task list and details

**Project:** ACMS (ID: 1)
**Issue Type:** User Story
**Labels:** `user-story`, `task-management`, `backend`, `frontend`, `api`
**Weight:** 6

## User Story

As a project manager, I want to view a list of all tasks and open any task to see its full details so that I can monitor project progress at a glance.

## Acceptance Criteria

- Task list shows all tasks with summary info (title, assignee, due date, status)
- List supports pagination if there are many tasks
- Clicking a task opens detailed view with all fields
- Task details show complete information including description and history
- Optional: Filter/search by assignee, status, or text

## Technical Tasks

### Backend

- [ ] Implement GetListAsync with pagination support
- [ ] Implement GetAsync(id) for task details
- [ ] Add filtering capabilities (by assignee, status, date range)
- [ ] Create TaskDto for list view (summary fields)
- [ ] Create TaskDetailDto for detail view (all fields)
- [ ] Add eager loading for related entities (assignee)

### API

- [ ] Add GET `/api/app/tasks` endpoint (with pagination and filters)
- [ ] Add GET `/api/app/tasks/{id}` endpoint
- [ ] Configure proper caching headers for list endpoint

### Frontend

- [ ] Create Tasks page/list component with table/grid view
- [ ] Display columns: Title, Assignee (avatar+name), Due Date, Status badge
- [ ] Implement pagination controls
- [ ] Add click-to-navigate to task detail view
- [ ] Create TaskDetail page with all fields
- [ ] Add "Back to list" navigation
- [ ] Optional: Add filter/sidebar panel

### Testing

- [ ] Unit tests for GetListAsync with pagination
- [ ] Unit tests for GetAsync with valid/invalid IDs
- [ ] Integration tests for GET endpoints
- [ ] E2E tests for task list and detail views

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
