### API Specification

# Customer Address Management API

**Version:** 1.0
**Date:** 2025-03-18
**Status:** Draft
**Project:** ACMS

## Overview
This API exposes endpoints for managing customer addresses in the ACMS system. It enables customers to create, view, update, and delete addresses while enforcing business rules like address validation and single minimum address requirement.

**Base URL:** /api/app
**Auth:** Bearer JWT - Required for all endpoints except public list endpoints

## Endpoint Mapping
| Method | Path                  | Description                          | Auth Required | Story Link |
|--------|-----------------------|--------------------------------------|---------------|------------|
| POST   | /api/app/addresses    | Create new customer address          | Yes           | [US-001](#us-001) |
| GET    | /api/app/addresses    | Get all addresses (paginated)        | Yes           | [US-002](#us-002) |
| GET    | /api/app/addresses/{id}| Get specific address by ID           | Yes           | [US-003](#us-003) |
| PUT    | /api/app/addresses/{id}| Update existing address              | Yes           | [US-004](#us-004) |
| DELETE | /api/app/addresses/{id}| Deactivate address (soft delete)     | Yes           | [US-005](#us-005) |

## Data Structures
### Request/Response Wrapper
```json
{
  "result": { /* data object */ },
  "success": true,
  "error": null,
  "target": "/api/app/addresses"
}
```

### Address DTO
```json
{
  "id": "{guid}",
  "type": "billing|shipping|delivery",
  "street": "string",
  "city": "string",
  "state": "string",
  "postalCode": "string",
  "country": "string",
  "default": boolean,
  "isActive": boolean,
  "createdAt": "datetime"
}
```