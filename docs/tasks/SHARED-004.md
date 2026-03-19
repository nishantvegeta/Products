# SHARED-004 — Create Address DTOs

**Layer:** Application.Contracts
**Required by:** T-001, T-002, T-003, T-004, T-006
**Estimate:** 1.5 hours

---

## Linked User Story

| ID | Title | File |
|---|---|---|
| US-001 | Create Address | [../user-stories/US-001.md](../user-stories/US-001.md) |

---

## Technical Description

DTOs define the exact request and response shapes specified in the API spec. Each endpoint's request body and response shape maps to a specific DTO class. These DTOs will be used by the AppService methods and must include validation attributes that match the spec's validation rules.

---

## Approach

Following the existing DTO pattern (e.g., `CreateUpdateCustomerDto`, `CustomerDto`, `CreateOrderDto`), create the following DTOs in `src/Products.Application.Contracts/Addresses/`:

1. **`CreateAddressDto`** — for POST create endpoint
   - `customerId` (Guid) — Required
   - `name` (string) — Required, StringLength(100)
   - `addressLine1` (string) — Required, StringLength(200)
   - `addressLine2` (string) — Optional, StringLength(200)
   - `city` (string) — Required, StringLength(100)
   - `state` (string) — Required, StringLength(100)
   - `country` (string) — Required, StringLength(100)
   - `postalCode` (string) — Required, StringLength(20)
   - `phone` (string) — Required, StringLength(20)
   - `isDefaultBilling` (bool) — Optional, default false
   - `isDefaultShipping` (bool) — Optional, default false

2. **`UpdateAddressDto`** — for PUT update endpoint (all fields optional to support partial updates)
   - Same fields as CreateAddressDto but all marked Optional (no [Required] attributes)
   - This allows clients to send only the fields they want to update

3. **`AddressDto`** — response shape for GET endpoints and order creation response
   - `id` (Guid)
   - `customerId` (Guid)
   - `name` (string)
   - `addressLine1` (string)
   - `addressLine2` (string, nullable)
   - `city` (string)
   - `state` (string)
   - `country` (string)
   - `postalCode` (string)
   - `phone` (string)
   - `isDefaultBilling` (bool)
   - `isDefaultShipping` (bool)
   - `isActive` (bool)
   - `creationTime` (DateTime) — maps to createdAt

4. **`GetAddressesInput`** — for GET list query parameters
   - `filter` (string, optional) — text search
   - `isActive` (bool?, optional) — filter by active status, null = all
   - `maxResultCount` (int, optional, default 10, max 100)
   - `skipCount` (int, optional, default 0)
   - `sorting` (string, optional, default "name asc")

Use `System.ComponentModel.DataAnnotations` attributes: `[Required]`, `StringLength`, `EmailAddress` (if email field exists). Follow the same namespace convention: `Products.Addresses` or `Products.Addresses.Dtos`.

---

## Affected Components

- **App.Contracts:** `src/Products.Application.Contracts/Addresses/CreateAddressDto.cs`
- **App.Contracts:** `src/Products.Application.Contracts/Addresses/UpdateAddressDto.cs`
- **App.Contracts:** `src/Products.Application.Contracts/Addresses/AddressDto.cs`
- **App.Contracts:** `src/Products.Application.Contracts/Addresses/GetAddressesInput.cs`

---

## Dependencies

| Task | Title | File |
|---|---|---|
| — | No dependencies | — |

---

## Acceptance Criteria

- [ ] All four DTO classes exist and compile
- [ ] Validation attributes match spec constraints (Required, StringLength)
- [ ] Property names match spec field names exactly (addressLine1, isDefaultBilling, etc.)
- [ ] GetAddressesInput includes all query parameters from spec
- [ ] DTOs follow existing coding style (using statements, namespace, property order)
- [ ] Response DTO includes all fields from spec response shape
- [ ] Update DTO fields are all optional (no [Required])

---

*Part of technical plan: customer-address-management-technical-plan-2026-03-19.md*
