# GitLab Issues - ACMS Project (ID: 1)

This directory contains GitLab issues for the Task Management module of the ACMS project.

## Project Information

- **Project Name:** ACMS
- **Project ID:** 1
- **Module:** Task Management
- **Total User Stories:** 4
- **Total Weight:** 26

## Issue Structure

Each issue file follows the standard GitLab markdown format and includes:

- User story with acceptance criteria
- Breakdown into backend, API, frontend, and testing tasks
- Labels for categorization
- Weight estimation
- Definition of Ready (DoR)
- Definition of Done (DoD)

## Issues Overview

| Issue | Title | Weight | Labels |
|-------|-------|--------|--------|
| `1-create-task.md` | Create Task | 8 | user-story, task-management, backend, frontend, api |
| `2-read-tasks.md` | Read Tasks | 6 | user-story, task-management, backend, frontend, api |
| `3-update-task.md` | Update Task | 7 | user-story, task-management, backend, frontend, api |
| `4-delete-task.md` | Delete Task | 5 | user-story, task-management, backend, frontend, api |

## How to Import to GitLab

### Option 1: Manual Import (One by One)

1. Go to your GitLab project: `https://gitlab.com/acms/acms/-/issues/new`
2. Copy the content from each markdown file
3. Paste into the issue description field
4. Set labels, weight, and other metadata as specified
5. Click "Create issue"

### Option 2: API Import (Bulk)

Use the GitLab API to create issues programmatically:

```bash
# Set your GitLab instance and authentication token
GITLAB_URL="https://gitlab.com"
PROJECT_ID="1"
PRIVATE_TOKEN="your_access_token"

# Import all issues
for file in issues/*.md; do
  title=$(head -n 1 "$file" | sed 's/^# //')
  body=$(tail -n +2 "$file")

  curl --request POST --header "PRIVATE-TOKEN: $PRIVATE_TOKEN" \
    --header "Content-Type: application/json" \
    --data "{\"title\":\"$title\",\"description\":\"$body\",\"labels\":\"user-story,task-management\"}" \
    "$GITLAB_URL/api/v4/projects/$PROJECT_ID/issues"
done
```

### Option 3: CSV Import

1. Create a CSV file with columns: `title,description,labels,weight`
2. Use GitLab's import feature: Project → Issues → Import issues

## Epic and Milestone Planning

### Recommended Epic
Create an epic called **"Task Management System"** and add all 4 user stories to it.

### Suggested Milestones
- **Sprint 1:** Create Task (8) + Read Tasks (6) = 14 points
- **Sprint 2:** Update Task (7) + Delete Task (5) = 12 points

### Dependencies
- Read tasks depends on Create task (CRUD sequence)
- Update and Delete depend on Read (need to fetch first)
- All depend on: authentication/authorization setup

## Labels Usage

| Label | Purpose |
|-------|---------|
| `user-story` | High-level business requirement |
| `task-management` | Related to task operations |
| `backend` | Server-side implementation |
| `frontend` | User interface implementation |
| `api` | API endpoint work |
| `testing` | Test automation |

## Technical Stack (Assumed)

Based on the ACMS project structure:

- **Backend:** ABP Framework (C#/.NET)
- **API:** RESTful with OpenAPI/Swagger
- **Frontend:** Likely Blazor/MVC/Razor Pages (ABP standard)
- **Database:** Entity Framework Core
- **Authentication:** OpenID Connect / JWT

## Contact

For questions or clarifications, contact the project team or refer to:
- [ABP Documentation](https://docs.abp.io)
- Project repository README
