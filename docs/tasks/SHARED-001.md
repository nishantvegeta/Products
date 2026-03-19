# SHARED-001 — Create Address Domain Entity

**Layer:** Domain
**Required by:** All address-related tasks
**Estimate:** 1 hour

---

## Linked User Story

| ID | Title | File |
|---|---|---|
| US-001 | Create Address | [../user-stories/US-001.md](../user-stories/US-001.md) |

---

## Technical Description

The Address domain entity is the core data model for the customer address management feature. It must represent all fields defined in the API specification's response shape and enforce domain-level invariants such as uniqueness of address names per customer and management of default billing/shipping flags. This entity will be the foundation that all other implementation tasks depend on.

---

## Approach

Create a new `Address` class in `src/Products.Domain/Entities/Addresses/` following the same pattern as existing entities (e.g., `Customer`, `Order`). Use `FullAuditedAggregateRoot<Guid>` as the base class to inherit ABP's standard auditing fields (CreationTime, CreatorId, LastModificationTime, LastModifierId, IsActive). Add all properties that appear in the API spec response shape:

- `Id` (Guid) — inherited from base
- `CustomerId` (Guid) — foreign key to Customer
- `Name` (string, max 100)
- `AddressLine1` (string, max 200)
- `AddressLine2` (string, nullable, max 200)
- `City` (string, max 100)
- `State` (string, max 100)
- `Country` (string, max 100)
- `PostalCode` (string, max 20)
- `Phone` (string, max 20)
- `IsDefaultBilling` (bool, default false)
- `IsDefaultShipping` (bool, default false)
- `IsActive` (bool, default true) — from base or explicit

Follow the same namespace pattern: `Products.Entities.Addresses`. Organize entities in folders by type (Customers, Orders, Addresses). Ensure the entity includes a navigation property for `Customer` to establish the relationship.

---

## Affected Components

- **Domain:** `src/Products.Domain/Entities/Addresses/Address.cs` — new entity class with all fields from spec, configured with proper string length constraints via ABP conventions (will be applied in DbContext configuration)
- **Domain:** `src/Products.Domain/Entities/Addresses/` — create folder if it doesn't exist

---

## Dependencies

| Task | Title | File |
|---|---|---|
| — | No dependencies | — |

---

## Acceptance Criteria

- [ ] Address entity class exists and compiles
- [ ] Entity inherits from `FullAuditedAggregateRoot<Guid>`
- [ ] All required properties from spec are present with correct types
- [ ] Navigation property to Customer is defined
- [ ] Entity is in the correct namespace and folder structure
- [ ] Entity follows the same coding conventions as existing entities (regions, commenting, property order)

---

*Part of technical plan: customer-address-management-technical-plan-2026-03-19.md*
