# API Release Documentation
## Products E-Commerce Platform

---

**Version:** 1.0.0
**Release Date:** 2025-03-16
**Status:** Ready for Release
**Prepared By:** api-release-documentation skill
**Sources:**
- API Specification: `src/api-spec.md`
- Service Interfaces: `ICustomerAppService.cs`, `IOrderAppService.cs`
- DTOs: `CustomerDto.cs`, `CreateUpdateCustomerDto.cs`, `OrderDto.cs`, `CreateOrderDto.cs`, `UpdateOrderDto.cs`

---

## 1. Release Summary

This release introduces **Customer Management** and **Order Management** API modules. These endpoints provide full CRUD operations for managing customer accounts and customer orders, including custom actions for customer activation and order cancellation.

**Modules Affected:** Customers, Orders
**Total New Endpoints:** 11 (5 Customer + 6 Order including deprecated)
**Total New DTOs:** 5 (2 for Customers, 3 for Orders)
**Breaking Changes:** No
**Deprecated Endpoints:** 1 (Order Cancel endpoint)

---

## 2. New Endpoints

### 2.1 Endpoint Summary

| Method | Path | Description | Auth Required | Module |
|---|---|---|---|
| POST | /api/customers | Create Customer | Yes | Customers |
| GET | /api/customers/{id} | Get Customer by ID | Yes | Customers |
| PUT | /api/customers/{id} | Update Customer | Yes | Customers |
| DELETE | /api/customers/{id} | Delete Customer | Yes | Customers |
| POST | /api/customers/{id}/activate | Activate Customer | Yes | Customers |
| POST | /api/orders | Create Order | Yes | Orders |
| GET | /api/orders/{id} | Get Order by ID | Yes | Orders |
| PUT | /api/orders/{id} | Update Order | Yes | Orders |
| DELETE | /api/orders/{id} | Delete Order | Yes | Orders |
| POST | /api/orders/{id}/cancel | Cancel Order (Deprecated) | Yes | Orders |

---

### 2.2 Endpoint Details

---

#### POST /api/customers

**Summary:** Create a new customer
**C# Method:** `CreateAsync`
**Permission Required:** `ProductsPermissions.Customers.Create` (to be defined)

**Request Body:**

```json
{
  "name": "John Doe",
  "email": "john@example.com",
  "phone": "+1-555-0123",
  "preferredLanguage": "en"
}
```

| Field | Type | Required | Validation |
|---|---|---|---|
| name | string | Yes | Max length: 256 (inferred) |
| email | string | Yes | Must be unique across customers |
| phone | string | Yes | Max length: 50 (inferred) |
| preferredLanguage | string | No | Optional |

**Response:** `200 OK`

```json
{
  "success": true,
  "message": "Customer created successfully",
  "data": {
    "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "name": "John Doe",
    "email": "john@example.com",
    "phone": "+1-555-0123",
    "isActive": true,
    "creationTime": "2025-03-16T10:30:00Z"
  },
  "code": 200
}
```

**Error Responses:**

| Status | Code | Description |
|---|---|---|
| 400 | Bad Request | Validation failed — missing required fields or invalid email format |
| 409 | Conflict | Email already exists for another customer |

---

#### GET /api/customers/{id}

**Summary:** Retrieve a customer by ID
**C# Method:** `GetAsync`
**Permission Required:** `ProductsPermissions.Customers.Default`

**Path Parameters:**

| Parameter | Type | Description |
|---|---|---|
| id | Guid | Customer unique identifier |

**Response:** `200 OK`

```json
{
  "success": true,
  "message": "Customer retrieved successfully",
  "data": {
    "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "name": "John Doe",
    "email": "john@example.com",
    "phone": "+1-555-0123",
    "isActive": true,
    "creationTime": "2025-03-16T10:30:00Z"
  },
  "code": 200
}
```

**Error Responses:**

| Status | Code | Description |
|---|---|---|
| 404 | Not Found | Customer with specified ID does not exist |

---

#### PUT /api/customers/{id}

**Summary:** Update an existing customer
**C# Method:** `UpdateAsync`
**Permission Required:** `ProductsPermissions.Customers.Edit`

**Path Parameters:**

| Parameter | Type | Description |
|---|---|---|
| id | Guid | Customer unique identifier |

**Request Body:**

```json
{
  "name": "Jane Doe",
  "email": "jane@example.com",
  "phone": "+1-555-0456",
  "preferredLanguage": "es"
}
```

| Field | Type | Required | Validation |
|---|---|---|---|
| name | string | No | If provided, not empty |
| email | string | No | If provided, must be unique (excluding current record) |
| phone | string | No | If provided, not empty |
| preferredLanguage | string | No | Optional |

**Response:** `200 OK`

```json
{
  "success": true,
  "message": "Customer updated successfully",
  "data": {
    "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "name": "Jane Doe",
    "email": "jane@example.com",
    "phone": "+1-555-0456",
    "isActive": true,
    "lastUpdatedTime": "2025-03-16T14:20:00Z"
  },
  "code": 200
}
```

**Error Responses:**

| Status | Code | Description |
|---|---|---|
| 404 | Not Found | Customer not found |
| 400 | Bad Request | Validation failed or email conflict |

---

#### DELETE /api/customers/{id}

**Summary:** Delete a customer (soft delete)
**C# Method:** `DeleteAsync`
**Permission Required:** `ProductsPermissions.Customers.Delete`

**Path Parameters:**

| Parameter | Type | Description |
|---|---|---|
| id | Guid | Customer unique identifier |

**Response:** `200 OK`

```json
{
  "success": true,
  "message": "Customer deleted successfully",
  "data": null,
  "code": 200
}
```

**Error Responses:**

| Status | Code | Description |
|---|---|---|
| 404 | Not Found | Customer not found or already deleted |

---

#### POST /api/customers/{id}/activate

**Summary:** Activate a customer account
**C# Method:** `ActivateAsync`
**Permission Required:** `ProductsPermissions.Customers.Edit`

**Path Parameters:**

| Parameter | Type | Description |
|---|---|---|
| id | Guid | Customer unique identifier |

**Request Body:** None

**Response:** `200 OK`

```json
{
  "success": true,
  "message": "Customer activated successfully",
  "data": {
    "id": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "status": "activated"
  },
  "code": 200
}
```

**Error Responses:**

| Status | Code | Description |
|---|---|---|
| 404 | Not Found | Customer not found |

---

#### POST /api/orders

**Summary:** Create a new order
**C# Method:** `CreateAsync`
**Permission Required:** `ProductsPermissions.Orders.Create`

**Request Body:**

```json
{
  "customerId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
  "product": "Widget",
  "quantity": 2,
  "price": 29.99
}
```

| Field | Type | Required | Validation |
|---|---|---|---|
| customerId | Guid | Yes | Must reference existing customer |
| product | string | Yes | Max length: 256 (inferred) |
| quantity | integer | Yes | Must be >= 1 |
| price | decimal | Yes | Must be >= 0 |

**Response:** `200 OK`

```json
{
  "success": true,
  "message": "Order created successfully",
  "data": {
    "id": "b2c3d4e5-f678-9012-abcd-ef1234567891",
    "customerId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "product": "Widget",
    "quantity": 2,
    "price": 29.99,
    "status": "pending",
    "creationTime": "2025-03-16T11:00:00Z"
  },
  "code": 200
}
```

**Error Responses:**

| Status | Code | Description |
|---|---|---|
| 400 | Bad Request | Validation failed |
| 404 | Not Found | Customer (referenced by customerId) not found |

---

#### GET /api/orders/{id}

**Summary:** Retrieve an order by ID
**C# Method:** `GetAsync`
**Permission Required:** `ProductsPermissions.Orders.Default`

**Path Parameters:**

| Parameter | Type | Description |
|---|---|---|
| id | Guid | Order unique identifier |

**Response:** `200 OK`

```json
{
  "success": true,
  "message": "Order retrieved successfully",
  "data": {
    "id": "b2c3d4e5-f678-9012-abcd-ef1234567891",
    "customerId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "product": "Widget",
    "quantity": 2,
    "price": 29.99,
    "status": "pending",
    "creationTime": "2025-03-16T11:00:00Z"
  },
  "code": 200
}
```

**Error Responses:**

| Status | Code | Description |
|---|---|---|
| 404 | Not Found | Order not found |

---

#### PUT /api/orders/{id}

**Summary:** Update an existing order
**C# Method:** `UpdateAsync`
**Permission Required:** `ProductsPermissions.Orders.Edit`

**Path Parameters:**

| Parameter | Type | Description |
|---|---|---|
| id | Guid | Order unique identifier |

**Request Body:**

```json
{
  "product": "Widget XL",
  "quantity": 3,
  "price": 39.99,
  "status": "confirmed"
}
```

| Field | Type | Required | Validation |
|---|---|---|---|
| product | string | No | If provided, not empty |
| quantity | integer | No | If provided, >= 1 |
| price | decimal | No | If provided, >= 0 |
| status | string | No | If provided, valid status enum (pending, confirmed, shipped, delivered, cancelled) |

**Response:** `200 OK`

```json
{
  "success": true,
  "message": "Order updated successfully",
  "data": {
    "id": "b2c3d4e5-f678-9012-abcd-ef1234567891",
    "customerId": "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
    "product": "Widget XL",
    "quantity": 3,
    "price": 39.99,
    "status": "confirmed",
    "lastUpdatedTime": "2025-03-16T14:15:00Z"
  },
  "code": 200
}
```

**Error Responses:**

| Status | Code | Description |
|---|---|---|
| 404 | Not Found | Order not found |
| 400 | Bad Request | Validation failed |

---

#### DELETE /api/orders/{id}

**Summary:** Delete an order (soft delete)
**C# Method:** `DeleteAsync`
**Permission Required:** `ProductsPermissions.Orders.Delete`

**Path Parameters:**

| Parameter | Type | Description |
|---|---|---|
| id | Guid | Order unique identifier |

**Response:** `200 OK`

```json
{
  "success": true,
  "message": "Order deleted successfully",
  "data": null,
  "code": 200
}
```

**Error Responses:**

| Status | Code | Description |
|---|---|---|
| 404 | Not Found | Order not found or already deleted |

---

#### POST /api/orders/{id}/cancel ⚠️ DEPRECATED

**Summary:** Cancel an order (deprecated — use status update instead)
**C# Method:** `CancelAsync`
**Permission Required:** `ProductsPermissions.Orders.Edit`

**Path Parameters:**

| Parameter | Type | Description |
|---|---|---|
| id | Guid | Order unique identifier |

**Request Body:** None

**Response:** `200 OK`

```json
{
  "success": true,
  "message": "Order cancelled successfully",
  "data": {
    "id": "b2c3d4e5-f678-9012-abcd-ef1234567891",
    "status": "cancelled"
  },
  "code": 200
}
```

**Migration Notes:**
- **Deprecated Since:** This release (v1.0.0)
- **Removal Target:** Next major version (v2.0.0)
- **Replacement:** Use `PUT /api/orders/{id}` with `{ "status": "cancelled" }`
- **Action Required:** API consumers should migrate by v2.0.0

---

## 3. Data Transfer Objects (DTOs)

### 3.1 New DTOs

| DTO Name | Used In | Description |
|---|---|---|
| CustomerDto | All customer responses | Customer response data shape |
| CreateUpdateCustomerDto | Create/Update Customer | Input for creating and updating customers |
| OrderDto | All order responses | Order response data shape |
| CreateOrderDto | Create Order | Input for creating orders |
| UpdateOrderDto | Update Order | Input for updating orders |

---

### 3.2 DTO Field Reference

#### CustomerDto

| Field | Type | Nullable | Description |
|---|---|---|---|
| Id | Guid | No | Unique customer identifier |
| Name | string | No | Customer full name |
| Email | string | No | Customer email address (unique) |
| Phone | string | No | Customer phone number |
| IsActive | boolean | No | Account activation status |
| CreationTime | DateTime | No | Timestamp when customer was created |

#### CreateUpdateCustomerDto

| Field | Type | Required | Validation |
|---|---|---|---|
| name | string | Yes | Required, max 256 chars |
| email | string | Yes | Required, valid email format, unique |
| phone | string | Yes | Required, max 50 chars |
| preferredLanguage | string | No | Optional language code (e.g., "en", "es") |

#### OrderDto

| Field | Type | Nullable | Description |
|---|---|---|---|
| Id | Guid | No | Unique order identifier |
| CustomerId | Guid | No | Reference to customer who placed order |
| Product | string | No | Product name or description |
| Quantity | integer | No | Number of units ordered |
| Price | decimal | No | Unit price |
| Status | string | No | Order status (pending, confirmed, shipped, delivered, cancelled) |
| CreationTime | DateTime | No | Timestamp when order was created |

#### CreateOrderDto

| Field | Type | Required | Validation |
|---|---|---|---|
| customerId | Guid | Yes | Must reference existing customer |
| product | string | Yes | Required, max 256 chars |
| quantity | integer | Yes | Required, minimum 1 |
| price | decimal | Yes | Required, minimum 0.00 |

#### UpdateOrderDto

| Field | Type | Required | Validation |
|---|---|---|---|
| product | string | No | If provided, max 256 chars |
| quantity | integer | No | If provided, minimum 1 |
| price | decimal | No | If provided, minimum 0.00 |
| status | string | No | If provided, must be valid status value |

---

## 4. Permissions

### 4.1 New Permission Definitions

Add the following to your `PermissionDefinitionProvider`:

```csharp
using Products.Permissions;

public override void Define(IPermissionDefinitionContext context)
{
    var productsGroup = context.AddGroup(ProductsPermissions.GroupName);

    // Customer Permissions
    var customerGroup = productsGroup.AddGroup(
        ProductsPermissions.Customers,
        L("CustomerManagement")
    );

    customerGroup.AddPermission(
        ProductsPermissions.CustomersCreate,
        L("CreateCustomer")
    );

    customerGroup.AddPermission(
        ProductsPermissions.CustomersEdit,
        L("EditCustomer")
    );

    customerGroup.AddPermission(
        ProductsPermissions.CustomersDelete,
        L("DeleteCustomer")
    );

    customerGroup.AddPermission(
        ProductsPermissions.CustomersDefault,
        L("ViewCustomers")
    );

    // Order Permissions
    var orderGroup = productsGroup.AddGroup(
        ProductsPermissions.Orders,
        L("OrderManagement")
    );

    orderGroup.AddPermission(
        ProductsPermissions.OrdersCreate,
        L("CreateOrder")
    );

    orderGroup.AddPermission(
        ProductsPermissions.OrdersEdit,
        L("EditOrder")
    );

    orderGroup.AddPermission(
        ProductsPermissions.OrdersDelete,
        L("DeleteOrder")
    );

    orderGroup.AddPermission(
        ProductsPermissions.OrdersDefault,
        L("ViewOrders")
    );
}

private static LocalizableString L(string name)
{
    return LocalizableString.Create<ProductsResource>(name);
}
```

### 4.2 Permission Constants

Add to `src/Products.Application.Contracts/Permissions/ProductsPermissions.cs`:

```csharp
namespace Products.Permissions;

public static class ProductsPermissions
{
    public const string GroupName = "Products";

    // Customer Permissions
    public const string Customers = GroupName + ".Customers";
    public const string CustomersCreate = Customers + ".Create";
    public const string CustomersEdit = Customers + ".Edit";
    public const string CustomersDelete = Customers + ".Delete";
    public const string CustomersDefault = Customers + ".Default";

    // Order Permissions
    public const string Orders = GroupName + ".Orders";
    public const string OrdersCreate = Orders + ".Create";
    public const string OrdersEdit = Orders + ".Edit";
    public const string OrdersDelete = Orders + ".Delete";
    public const string OrdersDefault = Orders + ".Default";
}
```

### 4.3 Permission Reference

| Endpoint | Required Permission |
|---|---|
| POST /api/customers | ProductsPermissions.Customers.Create |
| GET /api/customers/{id} | ProductsPermissions.Customers.Default |
| PUT /api/customers/{id} | ProductsPermissions.Customers.Edit |
| DELETE /api/customers/{id} | ProductsPermissions.Customers.Delete |
| POST /api/customers/{id}/activate | ProductsPermissions.Customers.Edit |
| POST /api/orders | ProductsPermissions.Orders.Create |
| GET /api/orders/{id} | ProductsPermissions.Orders.Default |
| PUT /api/orders/{id} | ProductsPermissions.Orders.Edit |
| DELETE /api/orders/{id} | ProductsPermissions.Orders.Delete |
| POST /api/orders/{id}/cancel | ProductsPermissions.Orders.Edit |

---

## 5. Breaking Changes

✅ **No breaking changes in this release.** All endpoints are new additions. The deprecated Order Cancel endpoint does not constitute a breaking change as it remains functional.

---

## 6. Deprecated Endpoints

#### POST /api/orders/{id}/cancel

**Status:** Deprecated
**Deprecated Since:** v1.0.0
**Target Removal:** v2.0.0
**Replacement:** `PUT /api/orders/{id}` with `{ "status": "cancelled" }`

**Migration Guide:**

**Before:**
```http
POST /api/orders/{orderId}/cancel
```

**After:**
```http
PUT /api/orders/{orderId}
Content-Type: application/json

{
  "status": "cancelled"
}
```

**Why:** The order cancellation endpoint is redundant — the same functionality can be achieved by updating the order status via the standard update endpoint. This simplifies the API surface and provides more flexibility (any status change in one endpoint).

---

## 7. Database Changes

### 7.1 New Tables

| Table | Maps To Entity | Description |
|---|---|---|
| Customers | `Customer` | Stores customer account information |
| Orders | `Order` | Stores customer orders |

### 7.2 Required Migration

The following entities have already been added to the `ProductsDbContext`. Generate and apply migrations:

```bash
# Generate migration (if not already created)
dotnet ef migrations add AddCustomerAndOrderTables \
  --project src/Products.EntityFrameworkCore \
  --startup-project src/Products.Web

# Apply migration to database
dotnet ef database update \
  --project src/Products.EntityFrameworkCore \
  --startup-project src/Products.Web
```

**Expected Table Schema:**

**Customers Table:**

| Column | SQL Type | Nullable | Constraints |
|---|---|---|---|
| Id | uuid | No | PRIMARY KEY, DEFAULT gen_random_uuid() |
| Name | varchar(256) | No | NOT NULL |
| Email | varchar(256) | No | NOT NULL, UNIQUE |
| Phone | varchar(50) | No | NOT NULL |
| IsActive | boolean | No | NOT NULL DEFAULT true |
| PreferredLanguage | varchar(10) | Yes | NULL |
| CreationTime | timestamp | No | NOT NULL |
| CreatorId | uuid | Yes | NULL |
| LastModificationTime | timestamp | Yes | NULL |
| LastModifierId | uuid | Yes | NULL |
| IsDeleted | boolean | No | NOT NULL DEFAULT false |

**Orders Table:**

| Column | SQL Type | Nullable | Constraints |
|---|---|---|---|
| Id | uuid | No | PRIMARY KEY, DEFAULT gen_random_uuid() |
| CustomerId | uuid | No | NOT NULL, FOREIGN KEY (Customers.Id) ON DELETE CASCADE |
| Product | varchar(256) | No | NOT NULL |
| Quantity | integer | No | NOT NULL, DEFAULT 1 |
| Price | numeric(18,2) | No | NOT NULL |
| Status | varchar(50) | No | NOT NULL DEFAULT 'pending' |
| CreationTime | timestamp | No | NOT NULL |
| CreatorId | uuid | Yes | NULL |
| LastModificationTime | timestamp | Yes | NULL |
| LastModifierId | uuid | Yes | NULL |
| IsDeleted | boolean | No | NOT NULL DEFAULT false |

### 7.3 Rollback Script

If you need to rollback this release:

```sql
-- WARNING: This will permanently delete all Customer and Order data!
DROP TABLE IF EXISTS "Orders" CASCADE;
DROP TABLE IF EXISTS "Customers" CASCADE;
```

Then revert the database to the previous migration:

```bash
dotnet ef database update {PreviousMigrationName} \
  --project src/Products.EntityFrameworkCore \
  --startup-project src/Products.Web
```

---

## 8. Integration Checklist

### For Backend Developers

- [ ] Verify `DbSet<Customer>` and `DbSet<Order>` are added to `ProductsDbContext.cs`
- [ ] Verify entity configurations in `OnModelCreating` method
- [ ] Run EF Core migration: `dotnet ef database update`
- [ ] Add permission definitions to `ProductsPermissionDefinitionProvider.cs`
- [ ] Add permission localization strings to `ProductsResource` class (en.json)
- [ ] Verify AutoMapper profiles are registered (should be auto-registered if in Application project)
- [ ] Build solution: `dotnet build`
- [ ] Run unit tests: `dotnet test`
- [ ] Verify Swagger UI shows new endpoints at `/swagger`

### For Frontend / API Consumers

- [ ] Import updated Swagger spec into Postman or API client: `https://{host}/swagger/v1/swagger.json`
- [ ] Add new permission grants to roles in admin panel (Customers.*, Orders.*)
- [ ] Update any hardcoded endpoint paths if different from expectations
- [ ] Implement request/response error handling for 400, 404, 409 status codes
- [ ] Update API client libraries to use new DTO structures
- [ ] Replace usage of deprecated `POST /api/orders/{id}/cancel` with `PUT /api/orders/{id}`

### For DevOps / Release

- [ ] Deploy updated backend to staging environment
- [ ] Run database migration on staging DB
- [ ] Verify Swagger UI is accessible at `https://staging.example.com/swagger`
- [ ] Perform smoke tests on all new endpoints (see Section 9)
- [ ] Monitor application logs for errors after deployment
- [ ] Promote to production after QA sign-off
- [ ] Deploy to production with zero-downtime deployment strategy
- [ ] Run production database migration during maintenance window if needed

---

## 9. Testing Guide

### 9.1 Quick Smoke Tests

Replace `{host}` with your API base URL and `{token}` with a valid JWT bearer token.

```bash
# Health check
curl -X GET https://{host}/api/health \
  -H "Accept: application/json"

# Create Customer
curl -X POST https://{host}/api/customers \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{"name":"Test User","email":"test@example.com","phone":"+1-555-0000"}'

# Get Customer (use ID from create response)
curl -X GET https://{host}/api/customers/{customerId} \
  -H "Authorization: Bearer {token}"

# Create Order
curl -X POST https://{host}/api/orders \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{"customerId":"{customerId}","product":"Test Product","quantity":1,"price":9.99}'

# Get Order
curl -X GET https://{host}/api/orders/{orderId} \
  -H "Authorization: Bearer {token}"

# Update Order Status
curl -X PUT https://{host}/api/orders/{orderId} \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{"status":"confirmed"}'

# Activate Customer
curl -X POST https://{host}/api/customers/{customerId}/activate \
  -H "Authorization: Bearer {token}"
```

### 9.2 Recommended Test Coverage

| Scenario | Expected Result |
|---|---|
| Create Customer with valid data | 200 OK, returns CustomerDto with Id |
| Create Customer with missing name | 400 Bad Request, validation error |
| Create Customer with duplicate email | 409 Conflict, "Customer email must be unique" error |
| Get non-existent Customer | 404 Not Found |
| Update Customer with invalid email | 400 Bad Request |
| Delete Customer | 200 OK, soft delete applied |
| Activate inactive Customer | 200 OK, IsActive=true |
| Create Order with valid data | 200 OK, returns OrderDto with Id |
| Create Order with non-existent CustomerId | 404 Not Found (or 400 depending on implementation) |
| Update Order status to "cancelled" | 200 OK, status updated |
| Delete Order | 200 OK, soft delete applied |
| Call deprecated Cancel endpoint | 200 OK (for now), but warning logged |
| Request without auth token | 401 Unauthorized |
| Request with token lacking permission | 403 Forbidden |

---

## 10. Swagger UI Access

Once deployed, the API can be explored interactively at:

```
https://{host}/swagger
```

To test authenticated endpoints in Swagger UI:

1. Click the **Authorize** button (top right corner)
2. Enter: `Bearer {your_jwt_token}` (including the word "Bearer" and a space)
3. Click **Authorize**, then **Close**
4. All subsequent requests will include the authorization header

---

## 11. Postman Collection

To import into Postman:

1. Open Postman → **Import**
2. Select **Link** tab (or **Raw Text**)
3. Enter URL: `https://{host}/swagger/v1/swagger.json`
4. Click **Import**

All endpoints will be imported with:
- Correct HTTP methods
- Request body templates
- Response schemas
- Authorization pre-configured for Bearer token

---

## 12. Rollback Plan

If critical issues are found after deployment:

### Immediate Actions

1. **Feature flags:** If configured, disable the new endpoints by setting feature flag to `false`
2. **Database rollback:** Revert to previous migration
   ```bash
   dotnet ef database update {previous_migration_name} \
     --project src/Products.EntityFrameworkCore \
     --startup-project src/Products.Web
   ```
3. **Drop new tables** (only if migration revert fails and you must restore DB to pre-release state):
   ```sql
   DROP TABLE IF EXISTS "Orders" CASCADE;
   DROP TABLE IF EXISTS "Customers" CASCADE;
   ```
4. **Redeploy previous version** of the application (commit before this release)
5. **Notify API consumers** of rollback via appropriate channels
6. **Monitor** system stability post-rollback

### Rollback Decision Tree

```
Critical bug discovered?
      ↓
Yes → Can you revert DB migration? → Yes → Apply rollback → Done
      ↓                                    ↓
      No                                   Monitor
      ↓
Drop tables? → Yes → Drop → Redeploy old version → Done
      ↓
      No → Contact DB admin → Manual data recovery → Done
```

---

## 13. Support & Contacts

| Role | Contact |
|---|---|
| Development Team | dev-team@products.example.com |
| API Support | api-support@products.example.com |
| Product Owner | product-owner@products.example.com |
| Documentation | https://docs.products.example.com |
| On-call Engineer | PagerDuty: 555-0123 |

---

## 14. Changelog

### v1.0.0 — 2025-03-16

**New Features:**
- Customer Management API (5 endpoints)
- Order Management API (5 endpoints + 1 deprecated)

**DTOs Added:**
- `CustomerDto`
- `CreateUpdateCustomerDto`
- `OrderDto`
- `CreateOrderDto`
- `UpdateOrderDto`

**Permissions Added:**
- `ProductsPermissions.Customers.*` (4 permissions)
- `ProductsPermissions.Orders.*` (4 permissions)

**Deprecated:**
- `POST /api/orders/{id}/cancel` — use `PUT /api/orders/{id}` with status update instead

**Known Issues:**
- None at release time

---

## 15. Appendix

### A. Response Pattern Reference

All endpoints return JSON with the following structure:

```json
{
  "success": true|false,
  "message": "operation result description",
  "data": { ... }|null,
  "code": 200
}
```

- `success`: Boolean indicating operation success
- `message`: Human-readable message
- `data`: Payload (DTO on success, null on delete/failure)
- `code`: Numeric status code (200, 400, 404, etc.)

**Note:** This is a custom wrapper pattern used project-wide. Do not return bare DTOs.

### B. Error Handling

Validation errors throw `UserFriendlyException` with a clear message. These are caught by ABP's exception filter and converted to:

```json
{
  "success": false,
  "message": "Validation error details",
  "data": null,
  "code": 400,
  "details": [...]
}
```

### C. Pagination Notes

Current implementation does not include pagination for `GetListAsync` methods. If list size grows large, consider adding:

```csharp
Task<ResponseDataDto<PagedResultDto<CustomerDto>>> GetListAsync(PagedAndSortedResultRequestDto input);
```

And corresponding route: `GET /api/customers?skipCount=0&maxResultCount=20`

---

*Generated by api-release-documentation skill*
*Sources: src/api-spec.md, ICustomerAppService.cs, IOrderAppService.cs*
*Generated on: 2025-03-16*
