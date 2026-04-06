---
id: architecture-development-environment
date: 2026-04-06
time: "17:39:33"
source_type: architecture
knowledge_domain: technical
source_file: CLAUDE.md
title: Development Environment
tags: []
---

## Development Environment

### Prerequisites
- .NET 8.0 SDK
- IDE: Visual Studio 2022+ or JetBrains Rider or VS Code
- Git (with credentials for GitLab)
- Docker & Docker Compose
- PostgreSQL (if not using Docker)

### Local Setup
1. Clone repository: `git clone <repo-url>`
2. Copy `.env.example` to `.env` and configure
3. Restore NuGet packages: `dotnet restore`
4. Apply migrations: `dotnet ef database update`
5. Run application: `dotnet run --project src/Acme.Products.Web`
6. Open browser: `https://localhost:44321`

### Docker Setup (Recommended)
```bash
cd .docker
docker-compose -f docker-compose.Development.yml up
```
