# High-Level Technical Task Plan
## Customer Address Management

**Version:** 1.0
**Date:** 2026-03-23
**Status:** Draft
**Source Stories:** US-001, US-002, US-003, US-004, US-005
**Feature Folder:** docs/feat/customer-address-management/

---

## Overview

This plan breaks down the technical implementation tasks for the Customer Address Management feature across five user stories. The work covers domain modeling (Address entity), application services, API endpoints, frontend pages and components, permission definitions, and comprehensive testing. The tasks are organized by architectural layer and include effort estimates totaling approximately 33.5 hours.

The main implementation areas are:
- Address entity and its relationship to Customer
- Address CRUD operations with default billing/shipping logic
- Integration of saved addresses into order creation
- Administrative access to all customer addresses for support
- Order processor read access to addresses
- Frontend pages for address management, checkout, admin, and order processing
- Permission model covering Customer, Admin, and OrderProcessor roles
- Unit and integration test coverage

Dependencies flow from data model → repositories → services → APIs → frontend.

---

## Stories Covered

| Story ID | Title | Actor |
|----------|-------|-------|
| US-001 | Manage Personal Address Book | Customer |
| US-002 | Set Default Billing and Shipping Addresses | Customer |
| US-003 | Select Saved Address During Order Creation | Customer |
| US-004 | Administrator Manages Customer Addresses for Support | System Administrator |
| US-005 | View Customer Addresses During Order Processing | Order Processor |

---

## Task List

### US-001 — Manage Personal Address Book

#### Backend

| Task ID | Title | Layer | Estimate | Notes |
|---------|-------|-------|----------|-------|
| T-001 | Define Address Entity with Customer Relationship | Data / Entity | 2h | Includes Id, CustomerId, address fields, IsDefaultBilling, IsDefaultShipping, ABP base classes, soft-delete, auditing |
| T-002 | Update Customer Entity to Include Addresses Collection | Data / Entity | 1h | Add ICollection<Address> Addresses, initialize collection |
| T-003 | Create Address DbContext Configuration and Migration | Data / Entity | 1.5h | DbSet, relationship config, initial migration |
| T-004 | Define Address DTOs for CRUD Operations | Application Service | 1h | CreateAddressDto, UpdateAddressDto, AddressDto with validation |
| T-005 | Implement Address Repository Interface | Repository | 1h | IAddressRepository with custom methods for defaults |
| T-006 | Implement Address Repository with EF Core | Repository | 1.5h | Repository implementation, transactional default handling |
| T-007 | Implement Address AppService with Business Logic | Application Service | 2h | Full CRUD + default exclusivity enforcement |
| T-008 | Create Address API Controller with Endpoints | API / Controller | 1.5h | RESTful endpoints, permission attributes, ABP response wrapper |
| T-024 | Handle Default Address Constraint in Deletion Workflow | Application Service | 1h | Prevent deletion of default or auto-clear; business rule BR-009 |

#### Frontend

| Task ID | Title | Layer | Estimate | Notes |
|---------|-------|-------|----------|-------|
| T-018 | Create Frontend Page for Customer Address Management | Frontend — Page / View | 2h | Route /account/addresses, list view, navigation |
| T-019 | Build Address Form Component with Validation | Frontend — Component | 2h | Create/edit form with required fields and default checkboxes |
| T-021 | Implement Frontend State Management for Address Data | Frontend — State / Integration | 2h | Store management for addresses, API integration |

#### Supporting

| Task ID | Title | Layer | Estimate | Notes |
|---------|-------|-------|----------|-------|
| T-009 | Define Permissions for Customer Address Management | Permissions | 0.5h | Constants: Create, Update, Delete, View |
| T-025 | Write Unit Tests for Address Domain and Service Logic | Testing | 3h | Domain, repository, service, validation, 80%+ coverage |
| T-026 | Write Integration Tests for API Endpoints | Testing | 3h | End-to-end API tests with multiple roles and scenarios |

---

### US-002 — Set Default Billing and Shipping Addresses

#### Backend

| Task ID | Title | Layer | Estimate | Notes |
|---------|-------|-------|----------|-------|
| T-005 | Implement Address Repository Interface | Repository | (shared with US-001) | Methods: SetDefaultBillingAsync, SetDefaultShippingAsync |
| T-007 | Implement Address AppService with Business Logic | Application Service | (shared with US-001) | Default clearing logic already covered |
| T-024 | Handle Default Address Constraint in Deletion Workflow | Application Service | (shared above) | Deletion of default address handling |

#### Frontend

| Task ID | Title | Layer | Estimate | Notes |
|---------|-------|-------|----------|-------|
| T-019 | Build Address Form Component with Validation | Frontend — Component | (shared) | Includes default checkboxes in form |
| T-021 | Implement Frontend State Management for Address Data | Frontend — State / Integration | (shared) | Actions for setting defaults |

#### Supporting

| Task ID | Title | Layer | Estimate | Notes |
|---------|-------|-------|----------|-------|
| T-009 | Define Permissions for Customer Address Management | Permissions | (shared) | |

---

### US-003 — Select Saved Address During Order Creation

#### Backend

| Task ID | Title | Layer | Estimate | Notes |
|---------|-------|-------|----------|-------|
| T-010 | Update Order Entity to Reference Saved Addresses | Data / Entity | 1.5h | Add nullable BillingAddressId, ShippingAddressId, navigation properties |
| T-011 | Extend Order DTOs to Include Address Information | Application Service | 1h | OrderDto includes address fields or IDs, handle one-time vs saved |
| T-012 | Modify Order Creation Logic to Accept Address Selection | Application Service | 2h | Accept address selection, validation, optional save-new, default fallback |

#### Frontend

| Task ID | Title | Layer | Estimate | Notes |
|---------|-------|-------|----------|-------|
| T-020 | Integrate Address Selection into Order Checkout Page | Frontend — Component | 2.5h | Checkout UI with saved address picker and new address entry |
| T-021 | Implement Frontend State Management for Address Data | Frontend — State / Integration | (shared) | Load addresses for selection in checkout |

#### Supporting

| Task ID | Title | Layer | Estimate | Notes |
|---------|-------|-------|----------|-------|
| N/A | Permissions | — | — | Reuse existing order permissions for order creation |

---

### US-004 — Administrator Manages Customer Addresses for Support

#### Backend

| Task ID | Title | Layer | Estimate | Notes |
|---------|-------|-------|----------|-------|
| T-012 | Modify Order Creation Logic to Accept Address Selection | Application Service | (shared) | Not directly related |
| T-013 | Implement Admin Methods in AddressAppService | Application Service | 1.5h | GetAddressesByCustomerAsync, SearchAddressesAsync, admin delete with audit |
| T-014 | Add Admin Endpoints to Address API Controller | API / Controller | 1h | GET /api/admin/addresses/customer/{id}, search, delete with reason |
| T-015 | Define and Assign Admin Permissions | Permissions | 0.5h | Admin-specific constants and role assignments |
| T-027 | Seed Permissions and Roles for Address Management | Config / Migration | 0.5h | Ensure admin role gets the admin permissions |

#### Frontend

| Task ID | Title | Layer | Estimate | Notes |
|---------|-------|-------|----------|-------|
| T-022 | Build Admin Interface for Customer Address Support | Frontend — Page / View | 2h | Admin page with customer search and address management |

#### Supporting

| Task ID | Title | Layer | Estimate | Notes |
|---------|-------|-------|----------|-------|
| N/A | Audit Logging | — | — | Assumed built-in or reused from T-013 |

---

### US-005 — View Customer Addresses During Order Processing

#### Backend

| Task ID | Title | Layer | Estimate | Notes |
|---------|-------|-------|----------|-------|
| T-011 | Extend Order DTOs to Include Address Information | Application Service | (shared with US-003) | Ensure full address data in order queries |
| T-016 | Enhance Order Read Model to Include Address Details | Application Service | 1h | Populate address details in order queries for order processors |
| T-017 | Grant Order Processor Read Access to Customer Addresses | Permissions | 0.5h | Define view permission and assign to OrderProcessor role |

#### Frontend

| Task ID | Title | Layer | Estimate | Notes |
|---------|-------|-------|----------|-------|
| T-023 | Enhance Order Management View for Order Processors | Frontend — Page / View | 1.5h | Order detail page displays addresses clearly, with saved/one-time distinction |

---

## Task Dependencies

| Task | Depends On | Reason |
|------|------------|--------|
| T-003 | T-001, T-002 | Migration needs entity definitions |
| T-005 | T-001 | Interface depends on entity |
| T-006 | T-005 | Repository implementation depends on interface |
| T-007 | T-004, T-005, T-006 | Service needs DTOs and repository |
| T-008 | T-007 | Controller depends on service |
| T-010 | T-001 | Order needs Address entity |
| T-011 | T-010 | DTOs depend on order entity changes |
| T-012 | T-011, T-007 | Order creation service needs address DTOs and address service |
| T-013 | T-006 | Admin methods use repository |
| T-014 | T-013 | Admin controller depends on admin service methods |
| T-016 | T-010, T-011, T-012 | Order read model needs address integration |
| T-018 | T-008 | Frontend page needs backend API available |
| T-019 | T-018 | Form component used on page |
| T-020 | T-008, T-018 | Checkout component needs address API |
| T-021 | T-008, T-018, T-020 | State connects components to APIs |
| T-022 | T-014 | Admin page uses admin endpoints |
| T-023 | T-016 | Order processor view uses enhanced order read model |
| T-024 | T-007 | Deletion logic inside AppService |
| T-027 | T-009, T-015 | Seeding requires permission definitions |
| T-025 | T-007, T-013 | Unit tests target service classes |
| T-026 | T-008, T-014, T-016 | Integration tests cover API endpoints |

---

## Effort Summary

| Layer | Tasks | Hours |
|-------|-------|-------|
| Data / Entity | T-001, T-002, T-003, T-010 | 6h |
| Repository | T-005, T-006 | 2.5h |
| Application Service | T-004, T-007, T-011, T-012, T-013, T-016, T-024 | 11.5h |
| API / Controller | T-008, T-014 | 2.5h |
| Permissions | T-009, T-015, T-017 | 1.5h |
| Frontend — Page / View | T-018, T-022, T-023 | 5.5h |
| Frontend — Component | T-019, T-020 | 4.5h |
| Frontend — State / Integration | T-021 | 2h |
| Config / Migration | T-027 | 0.5h |
| Testing | T-025, T-026 | 6h |
| **Total** | **27 tasks** | **33.5h** |

---

## Out of Scope for This Plan

- Address verification against external postal services
- International address formatting variations beyond basic fields
- Bulk address import/export
- Advanced address autocomplete or search
- UI/UX polish beyond functional requirements (styling, branding)
- Performance optimization beyond standard patterns
- Multi-tenancy considerations beyond ABP defaults

---

## Open Questions

| # | Question | Blocks |
|---|----------|--------|
| 1 | When deleting an address that is currently set as default, should the system prevent deletion until a new default is chosen, or automatically clear the default flag? | T-024 (but can be decided within task) |
| 2 | Should orders store a historical snapshot of address details, or should they always reference the current saved address? | T-010, T-011, T-012, T-016 (affects data model design) |
| 3 | Should there be a limit on the number of addresses a customer can save? | None (can be decided later without affecting current tasks) |
| 4 | Are there any specific address type labels needed beyond billing/shipping defaults? | None (out of scope) |

---

## Notes

- Tasks T-025 and T-026 cover all five stories' testing needs comprehensively.
- Permission seeding T-27 ensures roles for Customer, Admin, and OrderProcessor are properly configured.
- State management T-021 is shared across all customer-facing address features.

---

*Generated by user-stories-to-tasks skill*
*Source: docs/feat/customer-address-management/US-*.md*
