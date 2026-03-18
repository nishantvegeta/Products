# Technical Plan
## Customer Address Management

---

**Version:** 1.0
**Date:** 2025-03-18
**Status:** Draft
**API Spec Source:** customer-address-api-spec.md
**Project Profile:** Auto-detected

---

## Project Context

|| |
|---|---|
| **Project** | Acme.Products
| **Language** | C#
| **Framework** | ABP Framework 9.3
| **Architecture** | DDD Layered
| **Response Pattern** | ResponseDto<T>
| **Error Handling** | Custom helpers (this.Ok / this.BadRequest)
| **DTO Validation** | DataAnnotations ([Required], [StringLength])
| **Permission Prefix** | AcmeCRMPermissions

---

## Architecture Layers

```
Acme.Products Solution
├── Domain                → Entity definition, domain rules
├── Domain.Shared        → Enums, constants
├── Application.Contracts→ DTOs, interface, permissions
├── Application          → AppService implementation
├── EntityFrameworkCore  → DbContext, migrations
└── HttpApi              → HTTP routes (auto-generated)
```

---

## API Endpoints Being Implemented

| Method | Path                      | Permission        | Story |
|---|---|---|
| POST | /api/app/addresses        | Address.Create    | US-001 |
| GET  | /api/app/addresses        | Address.Default   | US-002 |
| GET  | /api/app/addresses/{id}   | Address.Default   | US-003 |
| PUT  | /api/app/addresses/{id}   | Address.Edit      | US-004 |
| DELETE| /api/app/addresses/{id}  | Address.Delete    | US-005 |

---

## Task Summary

| Task | Title | Endpoint | Layer | Est |
|---|---|---|---|---|
| SHARED-001 | Create Address entity | All | Domain | 1h |
| SHARED-002 | Add Address to DbContext | All | Infrastructure | 1h |
| SHARED-003 | Create permission constants | All | App.Contracts | 30m |
| SHARED-004 | Create DTOs | All | App.Contracts | 1h |
| SHARED-005 | Define IAddressAppService | All | App.Contracts | 30m |
| T-001 | Implement CreateAsync | POST | Application | 2h |
| T-002 | Implement GetAsync | GET /{id} | Application | 1h |
| T-003 | Implement GetListAsync | GET list | Application | 2h |
| T-004 | Implement UpdateAsync | PUT | Application | 1h |
| T-005 | Implement DeleteAsync | DELETE | Application | 1h |
| T-006 | Unit tests | All | Tests | 3h |

**Total Estimate:** 14h

---

## Shared Foundation Tasks

These tasks must be completed before any endpoint-specific tasks.

### SHARED-001 — Create Address Domain Entity

**Layer:** Domain
**Required by:** All endpoint tasks

**Technical Description:**
The Address domain entity is the core data model for this feature. It must represent all fields defined in the API spec's response shape and enforce domain-level invariants like address validation rules.

**Approach:**
1. Create `Address.cs` in Domain layer using `FullAuditedAggregateRoot` base class
2. Add properties:
   - `string street`
   - `string city`
   - `string state`
   - `string postalCode`
   - `string country`
   - `bool default`
   - `bool isActive` (default: true)
   - `DateTime createdAt` (auto-set by constructor)
3. Implement validation logic in constructor for:
   - Street length (max 128)
   - Email uniqueness across tenants
4. Add to EntityFrameworkCore DbSet

**Affected Components:**
- Domain: `Entities/Addresses/Address.cs`
- EntityFrameworkCore: `DbContext.cs` DbSet registration

**Dependencies:** None

---

### SHARED-002 — Add Address to DbContext and Generate Migration

**Layer:** Infrastructure
**Required by:** All endpoint tasks

**Technical Description:**
Register Address entity in DbContext with proper configuration for unique constraints and column types.

**Approach:**
1. Add `DbSet<Address>` to `ProductsEntityFrameworkCoreDbContext.cs`
2. Configure unique index on email through Fluent API
3. Generate migration using `dotnet ef migrations add CreateAddressTable`

**Affected Components:**
- EntityFrameworkCore: `DbContext.cs`
- EntityFrameworkCore: `Migrations/` (new migration file)

**Dependencies:** SHARED-001

---

### SHARED-003 — Create Address Permission Constants

**Layer:** Application.Contracts
**Required by:** T-001 through T-005

**Technical Description:**
Define permission constants for Address resource following ABP convention.

**Approach:**
1. Create `AddressPermissions.cs` in Application.Contracts
2. Define constants:
   ```csharp
   public static class AddressPermissions
   {
       public const string Default = "AcmeCRMPermissions.Address.Default";
       public const string Create = "AcmeCRMPermissions.Address.Create";
       public const string Edit = "AcmeCRMPermissions.Address.Edit";
       public const string Delete = "AcmeCRMPermissions.Address.Delete";
   }
   ```
3. Register in `PermissionDefinitionProvider`:
   ```csharp
   protected override void Load(IServiceProvider provider)
   {
       // Existing permissions registration
       AddPermissionGroup("Address", provider.GetRequiredService<IAuthorizationService>());
   }
   ```

**Affected Components:**
- Application.Contracts: `AddressPermissions.cs`
- Application.Contracts: `PermissionDefinitionProvider.cs`

**Dependencies:** None

---

### SHARED-004 — Create DTOs

**Layer:** Application.Contracts
**Required by:** T-001 through T-004

**Technical Description:**
Define DTOs matching the API spec's request/response shapes with validation attributes.

**Approach:**
1. Create request DTOs:
   - `CreateAddressDto` (required fields)
   - `UpdateAddressDto` (partial updates)
2. Create response DTO:
   - `AddressDto` (matches API spec shape)
3. Create filter DTO:
   - `GetAddressesInput` (filter, isActive, pagination)
4. Annotate with DataAnnotations:
   - [Required] for mandatory fields
   - [StringLength(128)] for street
   - [EmailAddress] for email

**Affected Components:**
- Application.Contracts: `Dtos/CreateAddressDto.cs`
- Application.Contracts: `Dtos/UpdateAddressDto.cs`
- Application.Contracts: `Dtos/AddressDto.cs`
- Application.Contracts: `Dtos/GetAddressesInput.cs`

**Dependencies:** None

---

### SHARED-005 — Define IAddressAppService Interface

**Layer:** Application.Contracts
**Required by:** T-001 through T-005

**Technical Description:**
Define the public contract for Address endpoints.

**Approach:**
1. Create `IAddressAppService.cs`
2. Define methods following ABP convention:
   ```csharp
   [AbpAuthorize(AddressPermissions.Create)]
   public Task<ResponseDto<AddressDto>> CreateAsync(CreateAddressDto input);

   [AbpAuthorize(AddressPermissions.Default)]
   public Task<ResponseDto<AddressDto>> GetAsync(Guid id);

   [AbpAuthorize(AddressPermissions.Default)]
   public Task<PagedAndSortedResultDto<AddressDto>> GetListAsync(GetAddressesInput input);

   [AbpAuthorize(AddressPermissions.Edit)]
   public Task<ResponseDto<AddressDto>> UpdateAsync(Guid id, UpdateAddressDto input);

   [AbpAuthorize(AddressPermissions.Delete)]
   public Task DeleteAsync(Guid id);
   ```

**Affected Components:**
- Application.Contracts: `IAddressAppService.cs`

**Dependencies:** SHARED-003, SHARED-004

---

## Endpoint Implementation Tasks

---

### T-001 — Implement CreateAsync (POST /api/app/addresses)

**Layer:** Application
**Implements:** POST /api/app/addresses
**Permission:** Address.Create

**Technical Description:**
Handle address creation with validation and duplication check.

**Approach:**
1. Receive `CreateAddressDto`
2. Validate street length and format
3. Check for duplicate email across all addresses
4. Create entity with default `isActive=true`
5. Save via repository using UoW pattern
6. Map to `AddressDto` using ObjectMapper
7. Return `ResponseDto<AddressDto>`

**Affected Components:**
- Application: `AddressAppService.cs`

**Dependencies:** SHARED-001, SHARED-002, SHARED-003, SHARED-004, SHARED-005

---

### T-002 — Implement GetAsync (GET /api/app/addresses/{id})

**Layer:** Application
**Implements:** GET /api/app/addresses/{id}
**Permission:** Address.Default

**Technical Description:**
Retrieve specific address by ID.

**Approach:**
1. Find address by ID using repository
2. Return `NotFound` if not exists
3. Map to `AddressDto`
4. Return `ResponseDto<AddressDto>`

**Affected Components:**
- Application: `AddressAppService.cs`

**Dependencies:** SHARED-001, SHARED-004, SHARED-005

---

### T-003 — Implement GetListAsync (GET /api/app/addresses)

**Layer:** Application
**Implements:** GET /api/app/addresses
**Permission:** Address.Default

**Technical Description:**
List addresses with filtering and pagination.

**Approach:**
1. Build query using `GetAddressesInput`
2. Apply default `isActive=true` filter
3. Implement text search on street/city
4. Apply sorting from input or default to street asc
5. Paginate results using `SkipCount` and `MaxResultCount`
6. Map to `AddressDto` list
7. Return `PagedAndSortedResultDto<AddressDto>`

**Affected Components:**
- Application: `AddressAppService.cs`

**Dependencies:** SHARED-001, SHARED-004, SHARED-005

---

### T-004 — Implement UpdateAsync (PUT /api/app/addresses/{id})

**Layer:** Application
**Implements:** PUT /api/app/addresses/{id}
**Permission:** Address.Edit

**Technical Description:**
Handle partial updates with re-validation.

**Approach:**
1. Find address by ID
2. Return `NotFound` if not exists
3. Update only provided fields
4. Re-validate changed fields:
   - Email uniqueness if changed
   - Street length if modified
5. Save changes via UoW
6. Return updated `AddressDto`

**Affected Components:**
- Application: `AddressAppService.cs`

**Dependencies:** SHARED-001, SHARED-004, SHARED-005

---

### T-005 — Implement DeleteAsync (DELETE /api/app/addresses/{id})

**Layer:** Application
**Implements:** DELETE /api/app/addresses/{id}
**Permission:** Address.Delete

**Technical Description:**
Deactivate address by setting `isActive=false`.

**Approach:**
1. Find address by ID
2. Return `NotFound` if not exists
3. Set `isActive=false`
4. Save changes via repository
5. Return success response

**Affected Components:**
- Application: `AddressAppService.cs`

**Dependencies:** SHARED-001, SHARED-004, SHARED-005

---

### T-006 — Write Unit Tests

**Layer:** Tests
**Implements:** Test coverage for all endpoints

**Technical Description:**
Verify endpoint behavior matches spec requirements.

**Approach:**
1. Create `AddressAppServiceTests.cs`
2. Cover scenarios:
   - Valid creation with unique email
   - Duplicate email rejection
   - Partial update validation
   - Different permission checks
   - Pagination boundaries
3. Use xUnit and Moq for mocking dependencies

**Affected Components:**
- Tests: `AddressAppServiceTests.cs`

**Dependencies:** T-001 through T-005

---

## Dependency Graph

```
SHARED-001 (Domain entity)
  └── SHARED-002 (DbContext + migration)

SHARED-003 (Permissions)
SHARED-004 (DTOs)

SHARED-003 + SHARED-004
  └── SHARED-005 (Interface)
        ├── T-001 (CreateAsync)
        ├── T-002 (GetAsync)
        ├── T-003 (GetListAsync)
        ├── T-004 (UpdateAsync)
        └── T-005 (DeleteAsync)
              └── T-006 (Tests)
```

---

## Recommended Implementation Order

| Order | Task | Reason |
|---|---|---|
| 1 | SHARED-001 | No dependencies — domain first |
| 2 | SHARED-002 | Needs domain entity |
| 3 | SHARED-003 | No dependencies — parallel with 1 |
| 4 | SHARED-004 | No dependencies — parallel with 1 |
| 5 | SHARED-005 | Needs permissions and DTOs |
| 6 | T-002, T-003 | Simple reads — good starting points |
| 7 | T-001 | Create — needs validation logic |
| 8 | T-004 | Update — similar to create |
| 9 | T-005 | Delete — simplest write operation |
| 10 | T-006 | All implementation must be done first |

---

## Open Technical Questions

| # | Question | From Spec | Blocks |
|---|---|---|---|
| 1 | How should cross-tenant email uniqueness be implemented? | Business rules | Implementation approach |

---

*Generated by api-spec-to-technical-plan skill*
*API Spec source: customer-address-api-spec.md*
*Project profile auto-detected from codebase*
*Feed this file into abp-developer or gitlab-issue-lifecycle*``` | |
|---|---|
| **Project** | Acme.Products
| **Language** | C#
| **Framework** | ABP Framework 9.3
| **Architecture** | DDD Layered
| **Response Pattern** | ResponseDto<T>
| **Error Handling** | Custom helpers (this.Ok / this.BadRequest)
| **DTO Validation** | DataAnnotations ([Required], [StringLength])
| **Permission Prefix** | AcmeCRMPermissions

---

## Architecture Layers

```
Acme.Products Solution
├── Domain                → Entity definition, domain rules
├── Domain.Shared        → Enums, constants
├── Application.Contracts→ DTOs, interface, permissions
├── Application          → AppService implementation
├── EntityFrameworkCore  → DbContext, migrations
└── HttpApi              → HTTP routes (auto-generated)
```

---

## API Endpoints Being Implemented

| Method | Path                      | Permission        | Story |
|---|---|---|
| POST | /api/app/addresses        | Address.Create    | US-001 |
| GET  | /api/app/addresses        | Address.Default   | US-002 |
| GET  | /api/app/addresses/{id}   | Address.Default   | US-003 |
| PUT  | /api/app/addresses/{id}   | Address.Edit      | US-004 |
| DELETE| /api/app/addresses/{id}  | Address.Delete    | US-005 |

---

## Task Summary

| Task | Title | Endpoint | Layer | Est |
|---|---|---|---|---|
| SHARED-001 | Create Address entity | All | Domain | 1h |
| SHARED-002 | Add Address to DbContext | All | Infrastructure | 1h |
| SHARED-003 | Create permission constants | All | App.Contracts | 30m |
| SHARED-004 | Create DTOs | All | App.Contracts | 1h |
| SHARED-005 | Define IAddressAppService | All | App.Contracts | 30m |
| T-001 | Implement CreateAsync | POST | Application | 2h |
| T-002 | Implement GetAsync | GET /{id} | Application | 1h |
| T-003 | Implement GetListAsync | GET list | Application | 2h |
| T-004 | Implement UpdateAsync | PUT | Application | 1h |
| T-005 | Implement DeleteAsync | DELETE | Application | 1h |
| T-006 | Unit tests | All | Tests | 3h |

**Total Estimate:** 14h

---

## Shared Foundation Tasks

These tasks must be completed before any endpoint-specific tasks.

### SHARED-001 — Create Address Domain Entity

**Layer:** Domain
**Required by:** All endpoint tasks

**Technical Description:**
The Address domain entity is the core data model for this feature. It must represent all fields defined in the API spec's response shape and enforce domain-level invariants like address validation rules.

**Approach:**
1. Create `Address.cs` in Domain layer using `FullAuditedAggregateRoot` base class
2. Add properties:
   - `string street`
   - `string city`
   - `string state`
   - `string postalCode`
   - `string country`
   - `bool default`
   - `bool isActive` (default: true)
   - `DateTime createdAt` (auto-set by constructor)
3. Implement validation logic in constructor for:
   - Street length (max 128)
   - Email uniqueness across tenants
4. Add to EntityFrameworkCore DbSet

**Affected Components:**
- Domain: `Entities/Addresses/Address.cs`
- EntityFrameworkCore: `DbContext.cs` DbSet registration

**Dependencies:** None

---

### SHARED-002 — Add Address to DbContext and Generate Migration

**Layer:** Infrastructure
**Required by:** All endpoint tasks

**Technical Description:**
Register Address entity in DbContext with proper configuration for unique constraints and column types.

**Approach:**
1. Add `DbSet<Address>` to `ProductsEntityFrameworkCoreDbContext.cs`
2. Configure unique index on email through Fluent API
3. Generate migration using `dotnet ef migrations add CreateAddressTable`

**Affected Components:**
- EntityFrameworkCore: `DbContext.cs`
- EntityFrameworkCore: `Migrations/` (new migration file)

**Dependencies:** SHARED-001

---

### SHARED-003 — Create Address Permission Constants

**Layer:** Application.Contracts
**Required by:** T-001 through T-005

**Technical Description:**
Define permission constants for Address resource following ABP convention.

**Approach:**
1. Create `AddressPermissions.cs` in Application.Contracts
2. Define constants:
   ```csharp
   public static class AddressPermissions
   {
       public const string Default = "AcmeCRMPermissions.Address.Default";
       public const string Create = "AcmeCRMPermissions.Address.Create";
       public const string Edit = "AcmeCRMPermissions.Address.Edit";
       public const string Delete = "AcmeCRMPermissions.Address.Delete";
   }
   ```
3. Register in `PermissionDefinitionProvider`:
   ```csharp
   protected override void Load(IServiceProvider provider)
   {
       // Existing permissions registration
       AddPermissionGroup("Address", provider.GetRequiredService<IAuthorizationService>());
   }
   ```

**Affected Components:**
- Application.Contracts: `AddressPermissions.cs`
- Application.Contracts: `PermissionDefinitionProvider.cs`

**Dependencies:** None

---

### SHARED-004 — Create DTOs

**Layer:** Application.Contracts
**Required by:** T-001 through T-004

**Technical Description:**
Define DTOs matching the API spec's request/response shapes with validation attributes.

**Approach:**
1. Create request DTOs:
   - `CreateAddressDto` (required fields)
   - `UpdateAddressDto` (partial updates)
2. Create response DTO:
   - `AddressDto` (matches API spec shape)
3. Create filter DTO:
   - `GetAddressesInput` (filter, isActive, pagination)
4. Annotate with DataAnnotations:
   - [Required] for mandatory fields
   - [StringLength(128)] for street
   - [EmailAddress] for email

**Affected Components:**
- Application.Contracts: `Dtos/CreateAddressDto.cs`
- Application.Contracts: `Dtos/UpdateAddressDto.cs`
- Application.Contracts: `Dtos/AddressDto.cs`
- Application.Contracts: `Dtos/GetAddressesInput.cs`

**Dependencies:** None

---

### SHARED-005 — Define IAddressAppService Interface

**Layer:** Application.Contracts
**Required by:** T-001 through T-005

**Technical Description:**
Define the public contract for Address endpoints.

**Approach:**
1. Create `IAddressAppService.cs`
2. Define methods following ABP convention:
   ```csharp
   [AbpAuthorize(AddressPermissions.Create)]
   public Task<ResponseDto<AddressDto>> CreateAsync(CreateAddressDto input);

   [AbpAuthorize(AddressPermissions.Default)]
   public Task<ResponseDto<AddressDto>> GetAsync(Guid id);

   [AbpAuthorize(AddressPermissions.Default)]
   public Task<PagedAndSortedResultDto<AddressDto>> GetListAsync(GetAddressesInput input);

   [AbpAuthorize(AddressPermissions.Edit)]
   public Task<ResponseDto<AddressDto>> UpdateAsync(Guid id, UpdateAddressDto input);

   [AbpAuthorize(AddressPermissions.Delete)]
   public Task DeleteAsync(Guid id);
   ```

**Affected Components:**
- Application.Contracts: `IAddressAppService.cs`

**Dependencies:** SHARED-003, SHARED-004

---

## Endpoint Implementation Tasks

---

### T-001 — Implement CreateAsync (POST /api/app/addresses)

**Layer:** Application
**Implements:** POST /api/app/addresses
**Permission:** Address.Create

**Technical Description:**
Handle address creation with validation and duplication check.

**Approach:**
1. Receive `CreateAddressDto`
2. Validate street length and format
3. Check for duplicate email across all addresses
4. Create entity with default `isActive=true`
5. Save via repository using UoW pattern
6. Map to `AddressDto` using ObjectMapper
7. Return `ResponseDto<AddressDto>`

**Affected Components:**
- Application: `AddressAppService.cs`

**Dependencies:** SHARED-001, SHARED-002, SHARED-003, SHARED-004, SHARED-005

---

### T-002 — Implement GetAsync (GET /api/app/addresses/{id})

**Layer:** Application
**Implements:** GET /api/app/addresses/{id}
**Permission:** Address.Default

**Technical Description:**
Retrieve specific address by ID.

**Approach:**
1. Find address by ID using repository
2. Return `NotFound` if not exists
3. Map to `AddressDto`
4. Return `ResponseDto<AddressDto>`

**Affected Components:**
- Application: `AddressAppService.cs`

**Dependencies:** SHARED-001, SHARED-004, SHARED-005

---

### T-003 — Implement GetListAsync (GET /api/app/addresses)

**Layer:** Application
**Implements:** GET /api/app/addresses
**Permission:** Address.Default

**Technical Description:**
List addresses with filtering and pagination.

**Approach:**
1. Build query using `GetAddressesInput`
2. Apply default `isActive=true` filter
3. Implement text search on street/city
4. Apply sorting from input or default to street asc
5. Paginate results using `SkipCount` and `MaxResultCount`
6. Map to `AddressDto` list
7. Return `PagedAndSortedResultDto<AddressDto>`

**Affected Components:**
- Application: `AddressAppService.cs`

**Dependencies:** SHARED-001, SHARED-004, SHARED-005

---

### T-004 — Implement UpdateAsync (PUT /api/app/addresses/{id})

**Layer:** Application
**Implements:** PUT /api/app/addresses/{id}
**Permission:** Address.Edit

**Technical Description:**
Handle partial updates with re-validation.

**Approach:**
1. Find address by ID
2. Return `NotFound` if not exists
3. Update only provided fields
4. Re-validate changed fields:
   - Email uniqueness if changed
   - Street length if modified
5. Save changes via UoW
6. Return updated `AddressDto`

**Affected Components:**
- Application: `AddressAppService.cs`

**Dependencies:** SHARED-001, SHARED-004, SHARED-005

---

### T-005 — Implement DeleteAsync (DELETE /api/app/addresses/{id})

**Layer:** Application
**Implements:** DELETE /api/app/addresses/{id}
**Permission:** Address.Delete

**Technical Description:**
Deactivate address by setting `isActive=false`.

**Approach:**
1. Find address by ID
2. Return `NotFound` if not exists
3. Set `isActive=false`
4. Save changes via repository
5. Return success response

**Affected Components:**
- Application: `AddressAppService.cs`

**Dependencies:** SHARED-001, SHARED-004, SHARED-005

---

### T-006 — Write Unit Tests

**Layer:** Tests
**Implements:** Test coverage for all endpoints

**Technical Description:**
Verify endpoint behavior matches spec requirements.

**Approach:**
1. Create `AddressAppServiceTests.cs`
2. Cover scenarios:
   - Valid creation with unique email
   - Duplicate email rejection
   - Partial update validation
   - Different permission checks
   - Pagination boundaries
3. Use xUnit and Moq for mocking dependencies

**Affected Components:**
- Tests: `AddressAppServiceTests.cs`

**Dependencies:** T-001 through T-005

---

## Dependency Graph

```
SHARED-001 (Domain entity)
  └── SHARED-002 (DbContext + migration)

SHARED-003 (Permissions)
SHARED-004 (DTOs)

SHARED-003 + SHARED-004
  └── SHARED-005 (Interface)
        ├── T-001 (CreateAsync)
        ├── T-002 (GetAsync)
        ├── T-003 (GetListAsync)
        ├── T-004 (UpdateAsync)
        └── T-005 (DeleteAsync)
              └── T-006 (Tests)
```

---

## Recommended Implementation Order

| Order | Task | Reason |
|---|---|---|
| 1 | SHARED-001 | No dependencies — domain first |
| 2 | SHARED-002 | Needs domain entity |
| 3 | SHARED-003 | No dependencies — parallel with 1 |
| 4 | SHARED-004 | No dependencies — parallel with 1 |
| 5 | SHARED-005 | Needs permissions and DTOs |
| 6 | T-002, T-003 | Simple reads — good starting points |
| 7 | T-001 | Create — needs validation logic |
| 8 | T-004 | Update — similar to create |
| 9 | T-005 | Delete — simplest write operation |
| 10 | T-006 | All implementation must be done first |

---

## Open Technical Questions

| # | Question | From Spec | Blocks |
|---|---|---|---|
| 1 | How should cross-tenant email uniqueness be implemented? | Business rules | Implementation approach |

---

*Generated by api-spec-to-technical-plan skill*
*API Spec source: customer-address-api-spec.md*
*Project profile auto-detected from codebase*
*Feed this file into abp-developer or gitlab-issue-lifecycle*```