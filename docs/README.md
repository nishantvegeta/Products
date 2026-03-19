# Technical Documentation Index
## Customer Address Management Feature

**Generated:** 2026-03-19
**Technical Plan:** `customer-address-management-technical-plan-2026-03-19.md`
**User Stories:** 6
**Tasks:** 12
**Total Estimate:** 19.5 hours

---

## Overview

This feature adds customer address management capabilities to the Products application. Customers can create, read, update, delete, and list their saved addresses, and select addresses during order creation. The implementation follows ABP Framework DDD patterns.

---

## User Stories & Task Breakdown

### US-001 — Create Address
> As a customer, I want to create and save multiple addresses so that I can reuse them for future orders without re-entering information.

| Task | Title | Layer | Estimate | File |
|---|---|---|---|---|
| SHARED-001 | Create Address domain entity | Domain | 1h | [tasks/SHARED-001.md](tasks/SHARED-001.md) |
| SHARED-002 | Add Address to DbContext + migration | Infrastructure | 1h | [tasks/SHARED-002.md](tasks/SHARED-002.md) |
| SHARED-003 | Create Address permission constants | App.Contracts | 30m | [tasks/SHARED-003.md](tasks/SHARED-003.md) |
| SHARED-004 | Create Address DTOs | App.Contracts | 1.5h | [tasks/SHARED-004.md](tasks/SHARED-004.md) |
| SHARED-005 | Define IAddressAppService interface | App.Contracts | 30m | [tasks/SHARED-005.md](tasks/SHARED-005.md) |
| T-001 | Implement CreateAsync | Application | 2h | [tasks/T-001.md](tasks/T-001.md) |

**Story Estimate:** 6.5h

---

### US-002 — Update Address
> As a customer, I want to update my saved addresses so that I can keep my information current and correct.

| Task | Title | Layer | Estimate | File |
|---|---|---|---|---|
| T-004 | Implement UpdateAsync | Application | 1.5h | [tasks/T-004.md](tasks/T-004.md) |

**Story Estimate:** 1.5h

---

### US-003 — Delete Address
> As a customer, I want to delete my saved addresses so that I can remove outdated or incorrect addresses.

| Task | Title | Layer | Estimate | File |
|---|---|---|---|---|
| T-005 | Implement DeleteAsync | Application | 1h | [tasks/T-005.md](tasks/T-005.md) |

**Story Estimate:** 1h

---

### US-004 — List Addresses
> As a customer, I want to list all my saved addresses so that I can see which addresses I have stored.

| Task | Title | Layer | Estimate | File |
|---|---|---|---|---|
| T-003 | Implement GetListAsync | Application | 2.5h | [tasks/T-003.md](tasks/T-003.md) |

**Story Estimate:** 2.5h

---

### US-005 — View Address Details
> As a customer, I want to view the full details of a specific saved address so that I can verify it before selecting it for an order.

| Task | Title | Layer | Estimate | File |
|---|---|---|---|---|
| T-002 | Implement GetAsync | Application | 1h | [tasks/T-002.md](tasks/T-002.md) |

**Story Estimate:** 1h

---

### US-006 — Select Saved Addresses During Order Creation
> As a customer, I want to select from my saved billing and shipping addresses when creating an order so that I can quickly checkout without re-entering address details, and the system will use my default addresses if I don't make a selection.

| Task | Title | Layer | Estimate | File |
|---|---|---|---|---|
| T-006 | Enhance Order creation with address selection | Application | 3h | [tasks/T-006.md](tasks/T-006.md) |

**Story Estimate:** 3h

---

## All Tasks (Chronological Order)

| Task | Title | Story | Layer | Estimate | Status |
|---|---|---|---|---|
| SHARED-001 | Create Address domain entity | US-001 | Domain | 1h | Open |
| SHARED-002 | Add DbSet + migration | US-001 | Infrastructure | 1h | Open |
| SHARED-003 | Create permission constants | US-001 | App.Contracts | 30m | Open |
| SHARED-004 | Create DTOs | US-001 | App.Contracts | 1.5h | Open |
| SHARED-005 | Define interface | US-001 | App.Contracts | 30m | Open |
| T-001 | Implement CreateAsync | US-001 | Application | 2h | Open |
| T-002 | Implement GetAsync | US-005 | Application | 1h | Open |
| T-003 | Implement GetListAsync | US-004 | Application | 2.5h | Open |
| T-004 | Implement UpdateAsync | US-002 | Application | 1.5h | Open |
| T-005 | Implement DeleteAsync | US-003 | Application | 1h | Open |
| T-006 | Enhance Order creation with address selection | US-006 | Application | 3h | Open |
| T-007 | Write unit tests for all endpoints | US-001 | Tests | 4h | Open |

**Total Estimate:** 19.5h

---

## Implementation Order Recommendation

1. **SHARED-001** — Domain entity first
2. **SHARED-002** — DbContext + migration
3. **SHARED-003** — Permissions
4. **SHARED-004** — DTOs
5. **SHARED-005** — Interface
6. **T-002** — GetAsync (simple read)
7. **T-003** — GetListAsync (list with filters)
8. **T-001** — CreateAsync (core write)
9. **T-004** — UpdateAsync (partial updates)
10. **T-005** — DeleteAsync (soft delete with checks)
11. **T-006** — Order enhancement (most complex)
12. **T-007** — Tests (after implementation stable)

---

## Project Auto-Detected Context

| Attribute | Value |
|---|---|
| **Project** | Products |
| **Language** | C# |
| **Framework** | ABP Framework 9.x |
| **Target** | .NET 9.0 |
| **Architecture** | DDD Layered |
| **Root Namespace** | Products |
| **Response Pattern** | `ResponseDataDto<object>` |
| **Error Handling** | `UserFriendlyException` |
| **DTO Validation** | DataAnnotations |
| **Entity Base** | `FullAuditedAggregateRoot<Guid>` |
| **Repository** | `IRepository<T, Guid>` |
| **ORM** | Entity Framework Core |
| **Database** | PostgreSQL / SQL Server |
| **Logging** | `ILogger<T>` |
| **AutoMapper** | Yes |
| **Permissions** | `Products.Permissions.*` |

---

## Dependencies Graph

```
SHARED-001 ──┐
             ├─> SHARED-002
             │
             ├─> SHARED-004 ──┐
             │                ├─> SHARED-005 ──┐
SHARED-003 ──┘                │              ├─> T-001
                              │              ├─> T-002
                              │              ├─> T-003
                              │              ├─> T-004
                              │              ├─> T-005
                              │              └─> T-006
                              │
                              └─> T-007 (after all T-001..006)
```

---

## Open Questions

See the [Technical Plan](customer-address-management-technical-plan-2026-03-19.md#open-technical-questions) for unresolved decisions.

---

*Generated by spec-to-technical skill*
*API Spec: docs/customer-management-api-spec-2026-03-19.md*
