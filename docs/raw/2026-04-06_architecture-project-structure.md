---
id: architecture-project-structure
date: 2026-04-06
time: "17:39:33"
source_type: architecture
knowledge_domain: technical
source_file: CLAUDE.md
title: Project Structure
tags: []
---

## Project Structure

```
src/
├── Acme.Products.Domain/           # Domain layer (entities, value objects, domain services)
├── Acme.Products.Application/      # Application layer (Dtos, AppServices, CQRS)
├── Acme.Products.EntityFrameworkCore/ # EF Core configurations, DbContext, migrations
├── Acme.Products.HttpApi/          # API controllers, routing, filters
├── Acme.Products.HttpApi.Client/   # TypeScript client proxies
└── Acme.Products.Web/              # Web UI project (Blazor/MVC/SPA)

tests/
├── Acme.Products.Domain.UnitTests/
├── Acme.Products.Application.UnitTests/
├── Acme.Products.Application.IntegrationTests/
└── Acme.Products.Web.IntegrationTests/
```
