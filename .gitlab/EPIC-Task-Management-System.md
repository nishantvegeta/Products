# Epic: Task Management System

**Project:** ACMS (ID: 1)
**Epic ID:** EPIC-001
**Status:** Ready for Planning
**Total Story Points:** 26
**Priority:** P0 (Critical)

## Overview

Implement a complete task management module for project managers to create, read, update, and delete tasks, assign work to team members, and track project progress.

## Business Value

Project managers need a simple but effective way to manage work items, distribute tasks to team members, and monitor progress. This is a core feature for project coordination and team productivity.

## User Stories

### US-1: Create Task (8 points)
As a project manager, I want to create a new task with a title, description, assignee, and due date so that team members know what work needs to be done.

**Key Deliverables:**
- Task entity and database table
- Create API endpoint
- Task creation UI with form validation
- Unit and integration tests

### US-2: Read Tasks (6 points)
As a project manager, I want to view a list of all tasks and open any task to see its full details so that I can monitor project progress at a glance.

**Key Deliverables:**
- Task list endpoint with pagination
- Task detail endpoint
- Task list and detail pages/views
- Optional filtering and search

### US-3: Update Task (7 points)
As a project manager, I want to edit an existing task to update its status, reassign it, or change the due date so that the task reflects current priorities.

**Key Deliverables:**
- Status field with workflow states
- Edit history tracking
- Update endpoints (PUT/PATCH)
- Edit UI with permissions
- Concurrency handling

### US-4: Delete Task (5 points)
As a project manager, I want to delete a task that is no longer relevant so that the project board stays clean and accurate.

**Key Deliverables:**
- Soft delete implementation
- Delete endpoint
- Delete UI with confirmation
- Permission checks

## Non-Functional Requirements

- **Security:** Role-based access control (only project managers can perform all CRUD; assignees may only view)
- **Performance:** Task list loads in <2 seconds with 100+ tasks
- **Availability:** 99.9% uptime
- **Audit:** All create/update/delete operations logged with user and timestamp
- **Responsive:** UI works on desktop and tablet

## Technical Considerations

### Database Schema

```sql
CREATE TABLE Tasks (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Title NVARCHAR(200) NOT NULL,
    Description NVARCHAR(MAX) NULL,
    AssigneeId UNIQUEIDENTIFIER NOT NULL,
    DueDate DATETIME2 NOT NULL,
    Status INT NOT NULL DEFAULT 0,
    IsDeleted BIT NOT NULL DEFAULT 0,
    CreatorId UNIQUEIDENTIFIER NOT NULL,
    CreationTime DATETIME2 NOT NULL,
    LastModificationTime DATETIME2 NULL,
    RowVersion ROWVERSION
);

-- Optional: Edit history table
CREATE TABLE TaskEditHistory (
    Id BIGINT IDENTITY PRIMARY KEY,
    TaskId UNIQUEIDENTIFIER NOT NULL,
    EditedBy NVARCHAR(100) NOT NULL,
    EditTime DATETIME2 NOT NULL,
    PropertyName NVARCHAR(100) NOT NULL,
    OldValue NVARCHAR(MAX) NULL,
    NewValue NVARCHAR(MAX) NULL,
    FOREIGN KEY (TaskId) REFERENCES Tasks(Id) ON DELETE CASCADE
);
```

### Enums

```csharp
public enum TaskStatus
{
    ToDo = 0,
    InProgress = 1,
    Done = 2,
    Blocked = 3,
    Cancelled = 4
}
```

### Permissions

- `Tasks.Create` - Create new tasks
- `Tasks.Read` - View tasks (all users can see their assigned tasks)
- `Tasks.Update` - Edit tasks (assignee or manager)
- `Tasks.Delete` - Delete tasks (managers only)

## Dependencies

- [ ] User and role management system (for assignee dropdown)
- [ ] Identity/permissions infrastructure
- [ ] Database connection and migrations setup
- [ ] UI component library (if not already present)

## Risks and Mitigations

| Risk | Impact | Probability | Mitigation |
|------|--------|-------------|------------|
| Permissions complexity | High | Medium | Implement role hierarchy early; test thoroughly |
| Concurrency conflicts | Medium | Medium | Use RowVersion and handle 409 responses |
| Performance with large datasets | Medium | Low | Implement pagination and indexing |
| User adoption | High | Low | Simple, intuitive UI; proper training |

## Success Metrics

- Task creation takes <3 seconds
- >95% successful task creation rate
- <1% error rate on API endpoints
- Positive user feedback from project managers

## Timeline

**Estimated Delivery:** 2 sprints

- **Sprint 1 (2 weeks):** US-1 (Create) + partial US-2 (Read list)
- **Sprint 2 (2 weeks):** Remaining US-2 (Read details) + US-3 (Update) + US-4 (Delete)

## Out of Scope (Future)

- Task comments/discussions
- Task attachments/files
- Task dependencies/blockers
- Task templates
- Task notifications/email alerts
- Task reports/analytics
- Agile boards (Kanban/Scrum views)
- Bulk operations
- Task importing/exporting

## Notes

- Start with soft delete (IsDeleted flag) for safety
- Consider implementing audit logging using ABP's built-in audit system
- Reuse ABP's standard permission management
- Follow ABP's application service patterns
