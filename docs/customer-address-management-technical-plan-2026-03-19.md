# Technical Plan
## Customer Address Management

---

**Version:** 1.0
**Date:** 2026-03-19
**Status:** Draft
**API Spec Source:** customer-management-api-spec-2026-03-19.md
**Project Profile:** Auto-detected

---

## Project Context

| | |
|---|---|
| **Project** | Products |
| **Language** | C# |
| **Framework** | ABP Framework 9.x |
| **Target** | .NET 9.0 |
| **Architecture** | DDD Layered |
| **Root Namespace** | Products |
| **Response Pattern** | `ResponseDataDto<object>` wrapper with `Success`, `Message`, `Data`, `Code` |
| **Error Handling** | `UserFriendlyException` for validation, caught and wrapped in ResponseDataDto |
| **DTO Validation** | DataAnnotations (`[Required]`, `[StringLength]`) |
| **Permission Prefix** | `Products` (but new Address permissions will use `AddressManagement` group) |
| **Existing Entities** | `Customer`, `Order`, `Product`, `Category` |
| **Existing DbSets** | `Users`, `Roles`, `Categories`, `Products` |

---

## Architecture Layers

```
Products Solution
├── Products.Domain                      → Entity definition, domain rules
│   └── Entities/
│       ├── Customers/
│       ├── Orders/
│       └── Addresses/                   ← new
├── Products.Domain.Shared               → Enums, constants
├── Products.Application.Contracts       → DTOs, interfaces, permissions
│   ├── Addresses/                       ← new
│   ├── Permissions/
│   └── Orders/
├── Products.Application                → AppService implementations
│   ├── Addresses/                       ← new
│   └── Orders/
├── Products.EntityFrameworkCore           → DbContext, migrations, repositories
│   └── EntityFrameworkCore/
│       └── ProductsDbContext.cs
└── Products.HttpApi                     → HTTP routes (auto-generated)
```

---

## API Endpoints Being Implemented

| Method | Path | Permission | User Story |
|---|---|---|---|
| POST | /api/app/addresses | AddressManagement.Create | US-001 |
| GET | /api/app/addresses | AddressManagement.Default | US-004 |
| GET | /api/app/addresses/{id} | AddressManagement.Default | US-005 |
| PUT | /api/app/addresses/{id} | AddressManagement.Edit | US-002 |
| DELETE | /api/app/addresses/{id} | AddressManagement.Delete | US-003 |
| POST | /api/app/orders | Order.Create | US-006 (enhancement) |

---

## Task Summary

| Task | Title | Endpoint(s) | Layer | Est |
|---|---|---|---|
| SHARED-001 | Create Address domain entity | All | Domain | 1h |
| SHARED-002 | Add Address to DbContext + migration | All | Infrastructure | 1h |
| SHARED-003 | Create Address permission constants | All | App.Contracts | 30m |
| SHARED-004 | Create Address DTOs (CreateAddressDto, UpdateAddressDto, AddressDto, GetAddressesInput) | All | App.Contracts | 1.5h |
| SHARED-005 | Define IAddressAppService interface | All | App.Contracts | 30m |
| T-001 | Implement CreateAsync | POST /addresses | Application | 2h |
| T-002 | Implement GetAsync | GET /addresses/{id} | Application | 1h |
| T-003 | Implement GetListAsync | GET /addresses | Application | 2.5h |
| T-004 | Implement UpdateAsync | PUT /addresses/{id} | Application | 1.5h |
| T-005 | Implement DeleteAsync | DELETE /addresses/{id} | Application | 1h |
| T-006 | Enhance Order creation with address selection | POST /orders | Application | 3h |
| T-007 | Write unit tests for all endpoints | All | Tests | 4h |

**Total Estimate:** 19 hours

---

## Shared Foundation Tasks

These tasks must be completed before any endpoint-specific tasks.

---

### SHARED-001 — Create Address Domain Entity

**Layer:** Domain
**Required by:** All address-related tasks

**Technical Description:**
The Address domain entity is the core data model for customer address management. It must represent all fields defined in the API spec's response shape and enforce domain-level invariants such as uniqueness of address names per customer (will be enforced at repository level) and default address management. This is the foundation all other tasks depend on.

**Approach:**
Create a new `Address` class in `src/Products.Domain/Entities/Addresses/` following the same pattern as existing entities (Customer, Order). Use `FullAuditedAggregateRoot<Guid>` as the base class. Add all properties that appear in the API spec response shape:

- `CustomerId` (Guid)
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
- `IsActive` (bool, default true) — from ABP base or explicit
- `CreationTime` (DateTime) — from base

Include navigation property `public virtual Customer Customer { get; set; }`. Follow namespace pattern: `Products.Entities.Addresses`.

**Affected Components:**
- Domain: `src/Products.Domain/Entities/Addresses/Address.cs` — new entity class

**Dependencies:** None

---

### SHARED-002 — Add Address to DbContext and Generate Migration

**Layer:** Infrastructure
**Required by:** All address-related tasks

**Technical Description:**
Register the Address entity in `ProductsDbContext` and generate a migration to create the table with constraints, foreign key to Customer, and the unique index on `(CustomerId, Name)`.

**Approach:**
In `src/Products.EntityFrameworkCore/EntityFrameworkCore/ProductsDbContext.cs`:
- Add `public DbSet<Address> Addresses { get; set; }` in the DbSets region.
- In `OnModelCreating`, add:
  ```csharp
  builder.Entity<Address>(b =>
  {
      b.ToTable("Addresses");
      b.ConfigureByConvention();
      b.Property(x => x.Name).IsRequired().HasMaxLength(100);
      b.Property(x => x.AddressLine1).IsRequired().HasMaxLength(200);
      b.Property(x => x.AddressLine2).HasMaxLength(200);
      b.Property(x => x.City).IsRequired().HasMaxLength(100);
      b.Property(x => x.State).IsRequired().HasMaxLength(100);
      b.Property(x => x.Country).IsRequired().HasMaxLength(100);
      b.Property(x => x.PostalCode).IsRequired().HasMaxLength(20);
      b.Property(x => x.Phone).IsRequired().HasMaxLength(20);
      b.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Cascade);
      b.HasIndex(x => new { x.CustomerId, x.Name }).IsUnique();
  });
  ```
Generate a new migration using `dotnet ef migrations add AddAddressTable` and review the generated code.

**Affected Components:**
- Infrastructure: `ProductsDbContext.cs`
- Infrastructure: `Migrations/xxxx_AddAddressTable.cs`

**Dependencies:** SHARED-001

---

### SHARED-003 — Create Address Permission Constants

**Layer:** Application.Contracts
**Required by:** T-001, T-002, T-003, T-004, T-005

**Technical Description:**
Define the permission constants for the Address management feature and register them in the permission definition provider.

**Approach:**
1. Create `src/Products.Application.Contracts/Permissions/AddressPermissions.cs`:
   ```csharp
   namespace Products.Permissions
   {
       public static class AddressPermissions
       {
           public const string GroupName = "AddressManagement";
           public const string Default = GroupName + ".Default";
           public const string Create = GroupName + ".Create";
           public const string Edit = GroupName + ".Edit";
           public const string Delete = GroupName + ".Delete";
       }
   }
   ```
2. Update `ProductsPermissionDefinitionProvider`:
   ```csharp
   var addressGroup = context.AddGroup(AddressPermissions.GroupName);
   addressGroup.AddPermission(AddressPermissions.Default, L("Permission:AddressManagement.Default"));
   addressGroup.AddPermission(AddressPermissions.Create, L("Permission:AddressManagement.Create"));
   addressGroup.AddPermission(AddressPermissions.Edit, L("Permission:AddressManagement.Edit"));
   addressGroup.AddPermission(AddressPermissions.Delete, L("Permission:AddressManagement.Delete"));
   ```

**Affected Components:**
- App.Contracts: `Permissions/AddressPermissions.cs` — new
- App.Contracts: `Permissions/ProductsPermissionDefinitionProvider.cs` — add group and permissions

**Dependencies:** None

---

### SHARED-004 — Create Address DTOs

**Layer:** Application.Contracts
**Required by:** T-001, T-002, T-003, T-004, T-006 (OrderDto uses AddressDto)

**Technical Description:**
Create the DTOs that define request and response shapes: CreateAddressDto, UpdateAddressDto, AddressDto, GetAddressesInput.

**Approach:**
Create files in `src/Products.Application.Contracts/Addresses/`:

1. **CreateAddressDto.cs** with `[Required]` and `[StringLength]` attributes on all fields (customerId, name, addressLine1, city, state, country, postalCode, phone); optional `isDefaultBilling`, `isDefaultShipping`.
2. **UpdateAddressDto.cs** — same properties but all optional (no `[Required]`).
3. **AddressDto.cs** — all properties from spec response: `Id`, `CustomerId`, `Name`, `AddressLine1`, `AddressLine2` (nullable), `City`, `State`, `Country`, `PostalCode`, `Phone`, `IsDefaultBilling`, `IsDefaultShipping`, `IsActive`, `CreationTime`.
4. **GetAddressesInput.cs** — properties: `Filter` (string), `IsActive` (bool?), `MaxResultCount` (int, default 10, max 100), `SkipCount` (int), `Sorting` (string). Consider adding range validation attributes if desired.

Use namespace: `Products.Addresses` or `Products.Addresses.Dtos`. Follow existing DTO style (e.g., `CreateUpdateCustomerDto`).

**Affected Components:**
- App.Contracts: `Addresses/CreateAddressDto.cs`
- App.Contracts: `Addresses/UpdateAddressDto.cs`
- App.Contracts: `Addresses/AddressDto.cs`
- App.Contracts: `Addresses/GetAddressesInput.cs`

**Dependencies:** None

---

### SHARED-005 — Define IAddressAppService Interface

**Layer:** Application.Contracts
**Required by:** T-001, T-002, T-003, T-004, T-005

**Technical Description:**
Define the interface that declares all address endpoints. It will be implemented by AddressAppService.

**Approach:**
Create `src/Products.Application.Contracts/Addresses/IAddressAppService.cs`:
```csharp
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Products.Addresses
{
    public interface IAddressAppService : IApplicationService
    {
        Task<ResponseDataDto<object>> CreateAsync(CreateAddressDto input);
        Task<ResponseDataDto<object>> GetAsync([Required] Guid id);
        Task<ResponseDataDto<object>> GetListAsync(GetAddressesInput input);
        Task<ResponseDataDto<object>> UpdateAsync([Required] Guid id, UpdateAddressDto input);
        Task<ResponseDataDto<object>> DeleteAsync([Required] Guid id);
    }
}
```
Return type `ResponseDataDto<object>` matches project convention.

**Affected Components:**
- App.Contracts: `Addresses/IAddressAppService.cs`

**Dependencies:** SHARED-003, SHARED-004

---

## Endpoint Implementation Tasks

---

### T-001 — Implement CreateAsync (POST /api/app/addresses)

**Layer:** Application
**Implements:** US-001
**Permission:** AddressManagement.Create

**Technical Description:**
Full implementation of address creation with validation, uniqueness check, ownership enforcement, and default address management (only one default billing and one default shipping per customer). Must return 200 with created AddressDto.

**Approach:**
Create `src/Products.Application/Addresses/AddressAppService.cs` implementing `IAddressAppService`.

Inject: `IRepository<Address, Guid> _addressRepository`, `ILogger<AddressAppService> _logger`, `IMapper _mapper`, `ICurrentUser _currentUser` (or `IAbpSession`).

Implement `CreateAsync(CreateAddressDto input)`:
- Validate input: required fields, string lengths. Throw `UserFriendlyException` with messages matching spec.
- Verify `input.CustomerId == _currentUser.Id` else throw.
- Check that the customer exists (optional if foreign key ensures).
- Check duplicate name: `await _addressRepository.AnyAsync(x => x.CustomerId == input.CustomerId && x.Name == input.Name)`; if exists throw.
- Handle default flags: if input.IsDefaultBilling==true, find other addresses with same flag and set to false; same for IsDefaultShipping.
- Map DTO to entity: `var address = _mapper.Map<CreateAddressDto, Address>(input);` Ensure AutoMapper maps CustomerId correctly.
- Insert: `await _addressRepository.InsertAsync(address);`
- Map to AddressDto: `var dto = _mapper.Map<Address, AddressDto>(address);`
- Return success ResponseDataDto.

Add `[Authorize(AddressPermissions.Create)]`.

**Affected Components:**
- Application: `Addresses/AddressAppService.cs` — new

**Dependencies:** SHARED-001, SHARED-002, SHARED-003, SHARED-004, SHARED-005

---

### T-002 — Implement GetAsync (GET /api/app/addresses/{id})

**Layer:** Application
**Implements:** US-005
**Permission:** AddressManagement.Default

**Technical Description:**
Retrieve a single address by ID, ensuring ownership returns 404 if not found or not owned.

**Approach:**
In `AddressAppService`, implement `GetAsync(Guid id)`:
- Find address: `var address = await _addressRepository.FindAsync(id);`
- If null, return 404 ResponseDataDto.
- Verify `address.CustomerId == _currentUser.Id`; if not, return 404.
- Map to AddressDto and return 200.

Add `[Authorize(AddressPermissions.Default)]`.

**Affected Components:**
- Application: `Addresses/AddressAppService.cs`

**Dependencies:** SHARED-001, SHARED-004, SHARED-005

---

### T-003 — Implement GetListAsync (GET /api/app/addresses)

**Layer:** Application
**Implements:** US-004
**Permission:** AddressManagement.Default

**Technical Description:**
Paginated, filterable list of addresses belonging to the current customer. Supports `filter` (search in Name, AddressLine1, AddressLine2, City, State, Country), `isActive` (true/false/null), pagination (`skipCount`, `maxResultCount`), and `sorting` default "name asc".

**Approach:**
Implement `GetListAsync(GetAddressesInput input)`:
- Get current user ID.
- Build query: `var query = await _addressRepository.GetQueryableAsync();`
- Apply filter: `query = query.Where(a => a.CustomerId == currentUserId);`
- Apply isActive: if `input.IsActive.HasValue` add `a.IsActive == input.IsActive.Value`; else no filter.
- Apply text filter: if `!string.IsNullOrWhiteSpace(input.Filter)`, add conditions with `EF.Functions.Like` or `.Contains()` for each field.
- Get total count: `var totalCount = await AsyncExecuter.CountAsync(query);`
- Apply sorting: default to `a.Name`; parse input.Sorting if needed.
- Apply pagination: `.Skip(input.SkipCount).Take(input.MaxResultCount)`
- Execute: `var items = await AsyncExecuter.ToListAsync(query);`
- Map to `List<AddressDto>`: `var dtos = _mapper.Map<List<Address>, List<AddressDto>>(items);`
- Return `ResponseDataDto<object>` with `Data = new { totalCount, items = dtos }`.

Add `[Authorize(AddressPermissions.Default)]`.

**Affected Components:**
- Application: `Addresses/AddressAppService.cs`

**Dependencies:** SHARED-001, SHARED-004, SHARED-005

---

### T-004 — Implement UpdateAsync (PUT /api/app/addresses/{id})

**Layer:** Application
**Implements:** US-002
**Permission:** AddressManagement.Edit

**Technical Description:**
Partial update of address fields. Must enforce validation and handle default flag exclusivity.

**Approach:**
Implement `UpdateAsync(Guid id, UpdateAddressDto input)`:
- Validate via `ValidateInputAsync(input, id)`:
  - Check any provided string fields for length (max 100/200) if not null
  - Ensure CustomerId matches current user (by fetching address first)
  - If name changed, check uniqueness across other addresses
- Fetch address; if null return 404; check ownership.
- Map: `_mapper.Map(input, address);` (AutoMapper configured to ignore nulls)
- Handle default flags: if `input.IsDefaultBilling == true`, set all other billing defaults false; same for shipping.
- Update: `await _addressRepository.UpdateAsync(address);`
- Map to AddressDto and return 200.

Add `[Authorize(AddressPermissions.Edit)]`.

**Affected Components:**
- Application: `Addresses/AddressAppService.cs`

**Dependencies:** SHARED-001, SHARED-004, SHARED-005

---

### T-005 — Implement DeleteAsync (DELETE /api/app/addresses/{id})

**Layer:** Application
**Implements:** US-003
**Permission:** AddressManagement.Delete

**Technical Description:**
Soft delete address (set IsActive=false) with business rule validation: cannot delete if address is used in an active order.

**Approach:**
Implement `DeleteAsync(Guid id)`:
- Fetch address; if null return 404; check ownership.
- Inject `IRepository<Order, Guid> _orderRepository`.
- Check for active orders: `await _orderRepository.AnyAsync(o => (o.BillingAddressId == id || o.ShippingAddressId == id) && o.Status == "Pending");` (adjust statuses as needed: "Pending", "Processing", "Shipped"? Spec says "active order" — define set). If any found, return 400 ResponseDataDto with message.
- Set `address.IsActive = false;`
- Update: `await _addressRepository.UpdateAsync(address);`
- Return 200 with `Data = true`.

Add `[Authorize(AddressPermissions.Delete)]`.

**Affected Components:**
- Application: `Addresses/AddressAppService.cs` (add order repository injection)

**Dependencies:** SHARED-001, SHARED-005

---

### T-006 — Enhance Order Creation with Address Selection (POST /api/app/orders)

**Layer:** Application
**Implements:** US-006
**Permission:** Order.Create

**Technical Description:**
Extend Order functionality to support selecting billing and shipping addresses from customer's saved addresses, with fallback to defaults. Must also extend the Order entity to store address snapshot data and update/create order accordingly.

**Approach:**

**Step 1: Extend Domain Entity**
- Modify `src/Products.Domain/Entities/Orders/Order.cs`:
  - Add `OrderNumber` (string, maybe unique) if not present.
  - Add billing fields: `BillingAddressId` (Guid?), `BillingName`, `BillingAddressLine1`, `BillingAddressLine2`, `BillingCity`, `BillingState`, `BillingCountry`, `BillingPostalCode`, `BillingPhone`.
  - Add shipping fields: `ShippingAddressId`, `ShippingName`, `ShippingAddressLine1`, etc.
  - Ensure max lengths match address fields (100, 200, etc.) or use larger if needed.
- Note: Storing both ID (for reference) and full address fields (for snapshot) is recommended.

**Step 2: Update DbContext**
- In `ProductsDbContext.OnModelCreating`, configure these new properties with `HasMaxLength` and nullable as appropriate. Add columns to Order table.
- Generate migration.

**Step 3: Update DTOs**
- In `src/Products.Application.Contracts/Orders/CreateOrderDto.cs`:
  - Add `Guid? BillingAddressId` and `Guid? ShippingAddressId` (both optional).
- In `OrderDto.cs`:
  - Add `AddressDto BillingAddress { get; set; }` and `AddressDto ShippingAddress { get; set; }` (these will be populated from the snapshot fields).

**Step 4: Update AutoMapper Profile**
- In `src/Products.Application/ProductsApplicationAutoMapperProfile.cs`:
  - Create map `CreateMap<Address, AddressDto>()`
  - Create map `CreateMap<CreateAddressDto, Address>()`
  - Create map `CreateMap<UpdateAddressDto, Address>()`
  - Update `CreateMap<Order, OrderDto>()` to map billing/shipping fields to nested AddressDto objects using `ForMember` with `MapFrom` or a custom resolver that constructs `AddressDto` from the separate columns.
  - Update `CreateMap<CreateOrderDto, Order>()` to ignore the DTO's address IDs and let service manual copy, or map them separately.

**Step 5: Implement OrderAppService.CreateAsync**
- Inject `IRepository<Address, Guid> _addressRepository`.
- In `CreateAsync(CreateOrderDto input)`:
  - Validate required fields (product, quantity, price, customerId).
  - Check `input.CustomerId == _currentUser.Id`.
  - Resolve billing address:
    - If `input.BillingAddressId.HasValue`, fetch address, verify ownership, and copy its data to order's billing fields (`BillingAddressId`, `BillingName`, `BillingAddressLine1`, etc.). Throw 400 if not found or not owned.
    - Else, find default billing: `await _addressRepository.FirstOrDefaultAsync(a => a.CustomerId == currentUserId && a.IsDefaultBilling && a.IsActive)`. If none, throw 400 "No billing address selected and no default set". Copy data.
  - Resolve shipping address: same logic with `IsDefaultShipping`.
  - Generate `OrderNumber`: if Order entity doesn't have a setter for auto-generation, set here using a pattern like `ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid():N.Substring(0,6)}` or a sequential number. Simpler: use a timestamp + random.
  - Set other order properties: `Product`, `Quantity`, `Price`, `Status = "Pending"`.
  - Compute `TotalAmount = Quantity * Price` and set.
  - Insert order: `await _orderRepository.InsertAsync(order);`
  - Map to `OrderDto` (include BillingAddress and ShippingAddress from the snapshot fields).
  - Return success Response.

**Affected Components:**
- Domain: `Entities/Orders/Order.cs`
- Infrastructure: `ProductsDbContext.cs` + migration
- App.Contracts: `Orders/CreateOrderDto.cs`, `OrderDto.cs`
- Application: `AppServices/Orders/OrderAppService.cs`
- Application: `ProductsApplicationAutoMapperProfile.cs`

**Dependencies:** SHARED-004 (AddressDto), existing Order entity, T-001 (Address entity/repository exists)

---

## Task Dependencies

```
SHARED-001 (Address entity)
  └── SHARED-002 (DbContext + migration)

SHARED-003 (Permissions)
SHARED-004 (DTOs)

SHARED-004
  └── T-006 (Order changes need AddressDto)

SHARED-003 + SHARED-004
  └── SHARED-005 (IAddressAppService)
        ├── T-001 (CreateAsync)
        ├── T-002 (GetAsync)
        ├── T-003 (GetListAsync)
        ├── T-004 (UpdateAsync)
        └── T-005 (DeleteAsync)
              └── T-006 (OrderAppService uses Address repository)
                    └── T-007 (Tests)

T-001, T-002, T-003, T-004, T-005, T-006
  └── T-007 (Unit tests)
```

---

## Recommended Implementation Order

| Order | Task | Reason |
|---|---|---|
| 1 | SHARED-001 | Domain foundation |
| 2 | SHARED-002 | Needs domain entity; DB table |
| 3 | SHARED-003 | Permissions — independent |
| 4 | SHARED-004 | DTOs — independent |
| 5 | SHARED-005 | Interface needs DTOs and permissions |
| 6 | T-002 | Simple read — good warm-up |
| 7 | T-003 | List with filters — moderate |
| 8 | T-001 | Create — core validation logic |
| 9 | T-004 | Update — similar to create |
| 10 | T-005 | Delete — simpler but order check |
| 11 | T-006 | Order enhancement — most complex, touches many areas |
| 12 | T-007 | Tests after all implementation stable |

---

## Open Technical Questions

| # | Question | Decision Needed | Blocks |
|---|---|---|---|
| 1 | Should orders store full address snapshot as separate columns or as JSON? | Separate columns (as drafted) matches normalization but more work. JSON simpler but less queryable. | T-006 |
| 2 | What exact statuses count as "active order" for delete constraint? | Define set: Pending, Processing, Shipped (maybe). Exclude Completed, Cancelled. | T-005 |
| 3 | How to generate unique OrderNumber? | Use `ORD-{yyyyMMdd}-{increment}` or timestamp-based. Could use a database sequence or just GUID fragment. | T-006 |
| 4 | Should UpdateAddressDto allow changing CustomerId? | No — address belongs to customer; CustomerId should never change. Exclude from DTO mapping. | T-004 |
| 5 | Default address behavior: when creating an address with `isDefaultBilling=true`, should it automatically clear previous default? | Yes — spec business rule says only one default per customer. | T-001, T-004 |
| 6 | Should soft-deleted addresses be included in list when `isActive=null`? | Yes, to show history. But ensure defaults only consider active addresses. | T-003 |

---

*Generated by spec-to-technical skill*
*API Spec: docs/customer-management-api-spec-2026-03-19.md*
*Project auto-detected from src/*
