# SHARED-005 — Define IAddressAppService Interface

**Layer:** Application.Contracts
**Required by:** T-001, T-002, T-003, T-004, T-005, T-006
**Estimate:** 30 minutes

---

## Linked User Story

| ID | Title | File |
|---|---|---|
| US-001 | Create Address | [../user-stories/US-001.md](../user-stories/US-001.md) |

---

## Technical Description

The interface defines the public contract for all address-related endpoints specified in the API spec. It must include a method for every address endpoint (POST, GET list, GET by id, PUT, DELETE) plus any additional actions. The interface will be implemented by the AddressAppService in the Application layer.

---

## Approach

Create a new interface `IAddressAppService` in `src/Products.Application.Contracts/Addresses/` that extends `IApplicationService` (from Volo.Abp.Application.Services). Add the following method signatures, following the same pattern as `ICustomerAppService`:

- `Task<ResponseDataDto<object>> CreateAsync(CreateAddressDto input)`
- `Task<ResponseDataDto<object>> GetAsync([Required] Guid id)`
- `Task<ResponseDataDto<object>> GetListAsync(GetAddressesInput input)`
- `Task<ResponseDataDto<object>> UpdateAsync([Required] Guid id, UpdateAddressDto input)`
- `Task<ResponseDataDto<object>> DeleteAsync([Required] Guid id)`

Note the return type: `ResponseDataDto<object>` matches the existing pattern in the project (see CustomerAppService). If the project later evolves to use `ResponseDataDto<T>` with a generic type, adjust accordingly. Use `Task` for async methods and include `[Required]` attribute on path parameters as seen in existing interfaces.

Place the interface in the same namespace pattern as other contracts: under `Products.Addresses` or `Products.Addresses.Services`.

---

## Affected Components

- **App.Contracts:** `src/Products.Application.Contracts/Addresses/IAddressAppService.cs` — new interface definition

---

## Dependencies

| Task | Title | File |
|---|---|---|
| SHARED-003 | Create permission constants | [../tasks/SHARED-003.md](./SHARED-003.md) |
| SHARED-004 | Create DTOs | [../tasks/SHARED-004.md](./SHARED-004.md) |

---

## Acceptance Criteria

- [ ] Interface exists and compiles
- [ ] Interface extends IApplicationService
- [ ] All five methods are declared with correct signatures
- [ ] Method names follow convention (CreateAsync, GetAsync, GetListAsync, UpdateAsync, DeleteAsync)
- [ ] Return type is `Task<ResponseDataDto<object>>` to match project pattern
- [ ] Parameters and DTOs match those created in SHARED-004
- [ ] Namespace and folder structure matches convention

---

*Part of technical plan: customer-address-management-technical-plan-2026-03-19.md*
