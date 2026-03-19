# SHARED-003 — Create Address Permission Constants

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

Permission constants define the access levels for each endpoint as specified in the API spec's Permissions Reference section. These constants are used by the ABP permission system to authorize users and will be referenced in the AddressAppService and potentially in the PermissionDefinitionProvider to register the new permissions.

---

## Approach

Two files need to be updated:

1. **Create or extend `AddressPermissions` static class** in `src/Products.Application.Contracts/Permissions/` or a dedicated Addresses permissions file. Following the existing pattern (e.g., `ProductsPermissions`), define a constant for each required permission:
   - `public const string GroupName = "AddressManagement";`
   - `public const string Default = GroupName + ".Default";`
   - `public const string Create = GroupName + ".Create";`
   - `public const string Edit = GroupName + ".Edit";`
   - `public const string Delete = GroupName + ".Delete";`

2. **Update `ProductsPermissionDefinitionProvider`** in `src/Products.Application.Contracts/Permissions/ProductsPermissionDefinitionProvider.cs` to register these new permissions in the permission group. Add them under the appropriate group (either create a new "AddressManagement" group or add to existing "Products" group depending on project conventions).

Follow the ABP convention of using localized strings for display names (`L("Permission:AddressManagement")`). Ensure the permission names match exactly what will be used in the AppService `[Authorize]` attributes.

---

## Affected Components

- **App.Contracts:** `src/Products.Application.Contracts/Permissions/AddressPermissions.cs` — new static class with permission constants (or extend existing ProductsPermissions)
- **App.Contracts:** `src/Products.Application.Contracts/Permissions/ProductsPermissionDefinitionProvider.cs` — register new permissions in the `Define` method

---

## Dependencies

| Task | Title | File |
|---|---|---|
| — | No dependencies | — |

---

## Acceptance Criteria

- [ ] Permission constants defined for Address.Default, Address.Create, Address.Edit, Address.Delete
- [ ] Permissions are registered in PermissionDefinitionProvider
- [ ] Constants follow the naming pattern `{Group}.{Action}`
- [ ] Code compiles without errors
- [ ] Localization keys are correctly formatted (if used)

---

*Part of technical plan: customer-address-management-technical-plan-2026-03-19.md*
