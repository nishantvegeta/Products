# Requirements Gap Report
## Customer Address Management Feature

---

**Generated:** 2026-03-17
**Requirements Source:** `src/business-document-customer-address.md`
**Codebase Scanned:** All .cs files in `src/Products.*` projects
**Total Requirements Analyzed:** 22 (8 Functional + 8 Non-Functional + 6 Business Rules)
**Implementation Status:** **0% Complete** — Feature not implemented

---

## 🚨 Executive Summary

| Metric | Count | % |
|---|---|---|
| ✅ Fully Implemented | 0 | 0% |
| ⚠️ Partially Implemented | 0 | 0% |
| ❌ Missing from Code | **22** | **100%** |
| 📝 Undocumented Logic | 0 | — |
| 📝 Undocumented Rules | 0 | — |

**Overall Health:** 🔴 **Critical** — **No implementation exists** for the Customer Address Management feature. The feature is purely documented but not coded.

---

## Background

### What We Found

**Requirements Documentation (Complete):**
- ✅ Business case with ROI analysis
- ✅ Detailed functional requirements (FR-001 through FR-014)
- ✅ Non-functional requirements (NFR-001 through NFR-008)
- ✅ Business rules (BR-001 through BR-006)
- ✅ Technical design document with full code samples
- ✅ Data dictionary with field specifications

**Actual Code Implementation (None):**
- ❌ No `Address` entity class
- ❌ No `AddressDto` or other DTOs
- ❌ No `IAddressAppService` interface
- ❌ No `AddressAppService` implementation
- ❌ No `DbSet<Address>` in `ProductsDbContext`
- ❌ No database migrations for `Addresses` table
- ❌ No API endpoints (`/api/app/address/*`)
- ❌ No permission definitions (`Addresses.*`)
- ❌ No repository implementation
- ❌ No AutoMapper profiles
- ❌ No unit tests
- ❌ No UI pages/components

### Current State

The only address-related items in the codebase are:
1. `src/customer-address-feature-overview.md` — Feature overview (this spec)
2. `src/Products-Address-Technical-Doc.md` — Technical design with code samples
3. `src/Products-Address-Business-Doc.md` — Business case
4. `src/business-document-customer-address.md` — Detailed requirements (structured)

**Important:** These files contain **documentation and proposed code examples only**, not actual compiled code. The code samples in the technical doc are **not present** in the actual project.

---

## Direction 1 — Requirements → Code Full Traceability Matrix

| Req ID | Requirement | Status | Evidence |
|---|---|---|---|
| FR-001 | Store multiple addresses per customer | ❌ MISSING | No Address entity, no repository, no service |
| FR-002 | Address types: billing/shipping/delivery | ❌ MISSING | No AddressType enum or boolean flags |
| FR-003 | Mark one default per type | ❌ MISSING | No default selection logic |
| FR-004 | Prevent deletion of only address | ❌ MISSING | No delete logic implemented |
| FR-005 | Update addresses without affecting past orders | ❌ MISSING | No update operation |
| FR-006 | Store audit information (who/when) | ❌ MISSING | No entity inheriting from FullAuditedAggregateRoot |
| FR-007 | Addresses persisted across sessions | ❌ MISSING | No database table |
| FR-008 | Customers only access own addresses | ❌ MISSING | No authorization checks |
| FR-009 | Validate required address fields | ❌ MISSING | No DTO validation rules |
| FR-010 | Reject invalid addresses with errors | ❌ MISSING | No validation service |
| FR-011 | Enforce maximum field lengths | ❌ MISSING | No length constraints defined |
| FR-012 | Select saved address during checkout | ❌ MISSING | No checkout integration |
| FR-013 | Populate checkout form from saved address | ❌ MISSING | No form population logic |
| FR-014 | Add new address during checkout (inline) | ❌ MISSING | No inline creation flow |
| NFR-001 | 30% faster checkout with saved address | ❌ MISSING | Performance untestable without implementation |
| NFR-002 | All endpoints require authentication | ❌ MISSING | No endpoints exist |
| NFR-003 | Encrypt addresses at rest (PII compliance) | ❌ MISSING | No encryption configured |
| NFR-004 | 99.9% uptime for address operations | ❌ MISSING | Not applicable yet |
| NFR-005 | 90% success rate first attempt adding address | ❌ MISSING | No UI testing possible |
| NFR-006 | API response time < 500ms (95th pct) | ❌ MISSING | Not testable |
| NFR-007 | Support up to 10 saved addresses per customer | ❌ MISSING | No limit enforcement |
| NFR-008 | All changes logged (audit trail) | ❌ MISSING | No audit logging configured |
| BR-001 | Always have at least one address | ❌ MISSING | No deletion protection |
| BR-002 | Email uniqueness across customers | ⚠️ PARTIAL* | **Customer email uniqueness exists** but for Customer entity, not Address |
| BR-003 | Default changes automatically | ❌ MISSING | No default management |
| BR-004 | Address edits don't affect historical orders | ❌ MISSING | No order relationship defined |
| BR-005 | Only owning customer can modify; admins separate | ❌ MISSING | No authorization layer |
| BR-006 | Soft delete for addresses | ❌ MISSING | No soft delete implementation |

*Note: BR-002 mentions email uniqueness for **Customer** entity, which is already implemented in `CustomerAppService.cs` (lines 245-253). However, this requirement is in the **Address** feature document but applies to Customer. This is likely a misplaced requirement or belongs in Customer spec.*

---

## Direction 2 — Code → Requirements

**Result:** No code exists that implements address functionality, so there is no undocumented logic to report. All business logic in the address feature would be documented in the requirements already.

---

## Detailed Gap Analysis

### Missing Components Checklist

#### Domain Layer (Products.Domain)
- [ ] **Address Entity** (`Entities/Addresses/Address.cs`)
  - Must inherit from `FullAuditedAggregateRoot<Guid>`
  - Properties: CustomerId, StreetAddress, City, StateProvince, PostalCode, Country, AddressType flags, IsActive
  - Relationships: `Customer Customer { get; set; }`, `Guid CustomerId { get; set; }`
- [ ] **AddressType Enum** (`Enums/AddressType.cs`)
  - Flags enum: Billing = 1, Shipping = 2, Delivery = 4 (allows combinations)
- [ ] **Repository Interface** (`IRepository<Address, Guid>`) — optional (ABP provides default)
- [ ] **Domain Services** (if any complex validation)

#### Application Contracts Layer (Products.Application.Contracts)
- [ ] **DTOs** (`Customers/` or `Addresses/`)
  - `CreateAddressDto` — [Required] fields, max lengths, address type flags
  - `UpdateAddressDto` — all fields optional/nullable except Id
  - `AddressDto` — output model with all properties
  - `AddressSelectionDto` — lightweight for dropdowns (Id + display string)
  - `GetAddressesInput` — pagination/filtering DTO
- [ ] **Interface** (`IAddressAppService`)
  - `Task<ResponseDataDto<object>> CreateAsync(CreateAddressDto input)`
  - `Task<ResponseDataDto<object>> GetAsync(Guid id)`
  - `Task<ResponseDataDto<PagedResultDto<AddressDto>>> GetListAsync(GetAddressesInput input)`
  - `Task<ResponseDataDto<object>> UpdateAsync(Guid id, UpdateAddressDto input)`
  - `Task<ResponseDataDto<object>> DeleteAsync(Guid id)`
  - `Task<ResponseDataDto<object>> SetDefaultAsync(Guid id, bool isBilling, bool isShipping, bool isDelivery)`
  - `Task<ResponseDataDto<AddressDto>> GetByCustomerAsync(Guid customerId)`
- [ ] **Permissions** (`Permissions/ProductsPermissions.cs`)
  - `Addresses.Default` (view)
  - `Addresses.Create`
  - `Addresses.Edit`
  - `Addresses.Delete`

#### Application Layer (Products.Application)
- [ ] **AppService Implementation** (`AppServices/Addresses/AddressAppService.cs`)
  - Full CRUD with business rule enforcement
  - Authorization checks (`[Authorize]` or permission checks)
  - Validation: required fields, max lengths, Customer ownership
  - Protect against deleting only address (BR-001)
  - Auto-manage default flags (BR-003)
  - Soft delete (IsActive = false) instead of hard delete (BR-006)
  - Prevent cross-customer access (BR-005)
  - Logging (audit trail exists via FullAuditedAggregateRoot)
- [ ] **AutoMapper Profile** (`ProductsApplicationAutoMapperProfile.cs` — extend)
  - Map `Address ↔ AddressDto`
  - Map `CreateAddressDto → Address`
  - Map `Address → UpdateAddressDto` (reverse)

#### Infrastructure Layer (Products.EntityFrameworkCore)
- [ ] **DbContext Update** (`ProductsDbContext.cs`)
  - Add `public DbSet<Address> Addresses { get; set; }`
  - Configure relationships: `builder.Entity<Address>(b => { b.HasOne(a => a.Customer).WithMany().HasForeignKey(a => a.CustomerId); })`
- [ ] **Migrations**
  - Create new migration: `AddAddressEntity`
  - Generated table: `Addresses` with columns as specified in data dictionary
  - Add indexes: `CustomerId`, (CustomerId + IsActive), maybe `AddressType` if used in queries
- [ ] **Repository Implementation** — default ABP repository is fine

#### API Layer (Products.HttpApi)
- [ ] **Controllers** — auto-generated by ABP from `IAddressAppService`
  - No manual work needed if following ABP conventions

#### Web Layer (Products.Web)
- [ ] **UI Pages/Components** (depends on UI framework)
  - Address list page (`/customer/addresses`)
  - Add/Edit address form modal/page
  - Address selection component for checkout
  - Default address toggles
  - Delete confirmation dialog
- [ ] **Menu items** for address management
- [ ] **Permission checks** in UI (hide buttons if no permission)

#### Testing Layer
- [ ] **Unit Tests** (`Products.Application.Tests/Addresses/`)
  - CreateAsync with valid data → success
  - CreateAsync with duplicate (if any business rule)
  - UpdateAsync preserves order history
  - DeleteAsync prevents deleting only address
  - Authorization: User cannot access other customer's address
- [ ] **Integration Tests** (`Products.HttpApi.Tests/`)
  - End-to-end API tests
  - Checkout flow with saved address
- [ ] **UI Tests** (if applicable)

#### Security Layer
- [ ] **Permission definition** in `ProductsPermissions.cs`
- [ ] **Permission check** in `AddressAppService` — every method should check `await CheckPermissionAsync("Addresses.*")`
- [ ] **Data isolation** — queries must filter by `CurrentUser.GetId()` or `CustomerId` (unless admin override)

---

## Implementation Order (Recommended)

### Sprint 1: Foundation (Week 1-2)
1. **Domain**: Create `Address` entity, `AddressType` enum
2. **DbContext**: Add `DbSet<Address>`, configure relationships
3. **Migrations**: Create and apply database migration
4. **Contracts**: Create all DTOs, `IAddressAppService` interface, permissions
5. **Basic CRUD**: Implement `AddressAppService` without advanced rules (just CRUD)

### Sprint 2: Business Logic (Week 3)
1. **Validation**: Required fields, max lengths, ownership checks
2. **Business Rules**: Implement BR-001 (protect only address), BR-003 (default management), BR-006 (soft delete)
3. **Authorization**: Add permission checks to every method
4. **AutoMapper**: Configure all mappings
5. **Unit Tests**: Core business logic tests

### Sprint 3: Integration & UI (Week 4)
1. **Checkout Integration**: Connect to existing order flow (FR-012 to FR-014)
2. **UI Pages**: Build address management UI
3. **UI Tests**: User acceptance testing
4. **API Testing**: Integration tests

### Sprint 4: Polish & Deploy (Week 5)
1. **Performance**: Ensure <500ms response times (NFR-006)
2. **Security Review**: Verify data isolation (BR-005), PII encryption (NFR-003)
3. **Load Testing**: Support 10 addresses per customer (NFR-007)
4. **Documentation**: Update API release notes, user guides
5. **Deployment**: Roll out to production

---

## Customer Entity ≠ Address Feature

**Important Distinction:**

The `Customer` entity (already implemented in `Customer.cs`) is **NOT** the same as the `Address` management feature. The business doc `business-document-customer-address.md` is about a **separate Address entity** that relates to Customer as a **one-to-many relationship**.

**Current Implementation:**
- ✅ `Customer` entity exists with: Name, Email, Phone, IsActive, PreferredLanguage
- ✅ `CustomerAppService` has CRUD and email uniqueness validation
- ✅ `Customer` email uniqueness works (BR-002 compliance for Customer)

**What's Missing:**
- ❌ `Address` entity (completely separate from Customer)
- ❌ One-to-many relationship: `Customer 1 → N Address`
- ❌ All address management operations

---

## Risks of Current State

| Risk | Probability | Impact | Mitigation |
|------|-------------|--------|------------|
| **Feature Not Shippable** | 100% | Critical | Must implement all 22 requirements first |
| **Requirements Outdated** | Medium | Medium | Review business doc for final sign-off before dev |
| **Scope Creep** | Medium | Medium | Freeze requirements before Sprint 1 |
| **Integration Issues** | Medium | High | Checkout flow changes need coordination with OrderAppService |
| **Performance Degradation** | Low | Medium | Address list pagination must be implemented correctly |
| **Security Breach** | Low | High | Authorization (BR-005) must be thoroughly tested |

---

## Comparison with Existing Customer Implementation

While the **Address feature is 0% implemented**, the existing **Customer feature is functional**:

| Customer Feature | Status |
|---|---|
| Create customer | ✅ Implemented |
| Get customer by ID | ✅ Implemented |
| Update customer | ✅ Implemented |
| Delete customer (soft/hard?) | ⚠️ Partial — `DeleteAsync` calls hard delete; docs suggest soft delete should be used |
| Activate customer | ✅ Implemented |
| Email uniqueness | ✅ Implemented (Create & Update) |
| Name/Email/Phone required | ✅ Implemented |
| Audit logging | ✅ Implemented (FullAuditedAggregateRoot) |
| Authorization | ❌ Missing — no permission checks in `CustomerAppService` |
| Pagination/Filtering | ❌ Missing — no GetListAsync method |

**Note:** `CustomerAppService.DeleteAsync` (line 166) calls `_customerRepository.DeleteAsync(customer)` which performs a **hard delete** in ABP by default. The business requirement BR-006 says "Soft delete shall be used for addresses" — this pattern should also be applied to Customer entity if consistent with platform.

---

## Recommendations

### Immediate Actions (Before Development)

1. **✅ Requirements Finalization** — The business doc is comprehensive. Review and approve:
   - Confirm address field specifications (max lengths)
   - Confirm address types (flags vs separate records)
   - Confirm phone format requirement: Is it mandatory? Length 32?
   - Sign off on out-of-scope items to prevent scope creep

2. **⚠️ Technical Spike** — Validate ABP patterns:
   - Review existing `Customer` entity as template
   - Confirm repository/permission patterns
   - Check if any custom repository needed (likely not)
   - Decide on encryption approach (NFR-003)

3. **📋 Task Breakdown** — Create tickets for each checklist item above
   - 22 subtasks minimum (entity, dbcontext, DTOs, service, UI, tests, etc.)
   - Estimate effort: Domain (2d), Contracts (2d), Service (3d), DbContext + Migration (1d), UI (5d), Tests (3d) = ~16 developer-days

4. **🔄 Alignment with Existing Code**
   - Follow `CustomerAppService` patterns exactly (logging, exception handling, response wrappers)
   - Reuse permission pattern from existing `ProductsPermissions.cs`
   - Match naming conventions (`FullAuditedAggregateRoot<Guid>`, `ApplicationService`, `ResponseDataDto<object>`)
   - Ensure `Address` uses same `[StringLength]` conventions as `Customer` and `Product`

### For Product Owner

1. **Prioritization** — This feature is **not in the code at all**. It will take **4-5 weeks** as documented. Do we start now or later?
2. **Scope Decision** — The technical doc includes optional features:
   - Address validation integration (postal API) — defer to v2?
   - Geolocation (lat/long) — out of scope per business doc
   - Bulk import/export — out of scope
3. **Checkout Integration** — Need to modify Order creation flow to allow selecting from saved addresses. Coordinate with OrderAppService team.

---

## Success Criteria — When Is This Done?

When **all 22 requirements** are marked ✅ and the following are verified:

1. **API Contract** matches the documented endpoints (see Technical Doc section 3.3)
2. **Database** has `Addresses` table with correct schema
3. **UI** allows full CRUD on addresses from customer account page
4. **Checkout** can select a saved address and populate form
5. **Tests** pass with >80% coverage on AddressAppService
6. **Security review** passed:
   - Customer A cannot access Customer B's addresses
   - Permissions enforced
   - (Optional if required) Data encrypted at rest
7. **Performance benchmarks** met:
   - GetListAsync with pagination < 500ms
   - CreateAsync < 200ms
8. **Documentation** updated:
   - API release notes generated
   - User guide written
   - Support team trained

---

## Evidence of Absence

To confirm the feature is not implemented, we searched:

```bash
# Entity search
find . -name "*Address*.cs" ! -path "*/obj/*" ! -path "*/bin/*"
# Result: 0 files

# DbSet search
grep -rn "DbSet<Address>" --include="*.cs" .
# Result: No matches

# Interface search
grep -rn "IAddressAppService" --include="*.cs" .
# Result: Only in documentation

# Migration search
grep -rn "Addresses" src/Products.EntityFrameworkCore/Migrations/
# Result: No Address table in any migration

# Permissions search
grep -rn "Addresses\." src/Products.Application.Contracts/Permissions/
# Result: No address permissions defined
```

**Conclusion**: The Customer Address Management feature exists only in documentation. Zero implementation in the compiled codebase.

---

## Next Steps

### Option A: Begin Development (Recommended if Priority is High)

1. Assign to development team with 4-5 week timeline
2. Use this gap report as the implementation checklist
3. First task: Create `Address` entity and migration
4. Follow technical doc `Products-Address-Technical-Doc.md` for code patterns
5. Re-run this gap analysis after each sprint to track progress

### Option B: Defer to Later Quarter

1. Move documentation to `/docs/planned-features/` (not main docs)
2. Add to roadmap with estimated quarter
3. No code changes required now
4. Re-evaluate in next planning cycle

---

## Appendix: Full Requirements List

### Functional Requirements (FR)

| ID | Description | Priority |
|---|---|---|
| FR-001 | Store multiple addresses per customer | Must |
| FR-002 | Support address types: billing, shipping, delivery | Must |
| FR-003 | Mark default address for each type | Must |
| FR-004 | Prevent deletion if it's the only address | Must |
| FR-005 | Allow updates without affecting historical orders | Must |
| FR-006 | Store audit metadata (created by, when, etc.) | Must |
| FR-007 | Persist addresses across login sessions | Must |
| FR-008 | Enforce per-customer data isolation | Must |
| FR-009 | Validate required fields before accept | Must |
| FR-010 | Reject invalid data with clear errors | Must |
| FR-011 | Enforce max field lengths | Must |
| FR-012 | Allow address selection during checkout | Must |
| FR-013 | Populate checkout form from saved address | Must |
| FR-014 | Add new address inline during checkout | Must |

### Non-Functional Requirements (NFR)

| ID | Category | Description | Target |
|---|---|---|---|
| NFR-001 | Performance | 30% faster checkout with saved address | Measured < 2s |
| NFR-002 | Security | All endpoints require authentication | 401 on unauthenticated |
| NFR-003 | Privacy | Encrypt addresses at rest (PII) | AES-256 or DB encryption |
| NFR-004 | Reliability | 99.9% uptime | Monitored availability |
| NFR-005 | Usability | 90% success rate first attempt | User testing |
| NFR-006 | Performance | API response < 500ms (95th pct) | Gateway metrics |
| NFR-007 | Scalability | Support 10 addresses per customer | Configurable limit |
| NFR-008 | Auditability | Log all changes with user/time | Enabled via base class |

### Business Rules (BR)

| ID | Rule | Applies To | Impact |
|---|---|---|---|
| BR-001 | Must always have at least one address | Delete | Blocks deletion if last |
| BR-002 | Customer emails must be unique | Customer entity | Prevents duplicate accounts |
| BR-003 | Changing default removes old default | SetDefault | Auto-unset previous |
| BR-004 | Address edits don't cascade to orders | Update | Orders keep snapshot |
| BR-005 | Only owner can modify; admins separate | All | Enforced per operation |
| BR-006 | Use soft delete for addresses | Delete | IsActive = false |

---

*Generated by requirements-gap-filler skill*
*Source: business-document-customer-address.md*
*Codebase scan date: 2026-03-17*
