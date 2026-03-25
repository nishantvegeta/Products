# Products System Requirements

## Executive Summary

**System:** Products E-commerce Platform
**Type:** Enterprise Order Management System
**Domain:** Product catalog, customer management, order processing
**Technology:** ABP Framework (v7.x+), .NET 8.0, Entity Framework Core
**Status:** Active development with existing customer address management initiative

---

## 1. Project Context

### 1.1 Business Purpose

The Products system is an enterprise e-commerce and order management platform that enables:
- Product catalog management with categories and pricing
- Customer account management and order history
- Order creation, processing, and fulfillment workflows
- Integration with payment and shipping systems

### 1.2 Current State

**Existing Implementation:**
- Product and Category management (CRUD operations)
- Basic customer entity and DTOs
- Order entity with product references (stored as plain text)
- Sample test scaffolding (non-functional)

**Active Initiative:** Customer Address Management (March 2026)
- Feature spec: `docs/feat/customer-address-management/customer-address-management-feature-spec-2026-03-23.md`
- Technical plan: `docs/feat/customer-address-management/customer-address-management-technical-plan-2026-03-23.md`
- Generated user stories: `US-001.md` through `US-003.md`
- Generated tasks: `T-001.md` through `T-027.md`

---

## 2. Functional Requirements

### 2.1 Core Domains

#### 2.1.1 Product Management
- **Products:** Create, read, update, delete product catalog items
  - Fields: Name, SKU, Description, Price, Stock quantity, Status (Active/Discontinued)
  - Category association (many-to-many)
  - Product images/attachments
  - Price history and tiered pricing (optional)

- **Categories:** Hierarchical product categorization
  - Parent-child relationships (nested categories)
  - Display order and SEO metadata
  - Filter products by category

#### 2.1.2 Customer Management
- **Customers:** Customer account profiles
  - Personal information (name, email, phone)
  - Account status (Active, Suspended)
  - Registration and verification workflow
  - Order history and preferences

- **Customer Addresses** (In Progress)
  - Multiple addresses per customer
  - Address types: Billing, Shipping
  - Default address selection per type
  - CRUD operations with validation
  - Address reuse across orders

#### 2.1.3 Order Management
- **Orders:** Complete order lifecycle
  - Order creation with line items
  - Order status: Draft → Pending → Confirmed → Shipped → Delivered → Cancelled
  - Quantity validation against stock
  - Price locking at order time
  - Order notes and internal comments

- **Order Items:** Individual products within an order
  - **Current:** Product name stored as plain text
  - **Required:** Foreign key to Product entity
  - Quantity and unit price snapshot
  - Line item total calculation

- **Order History:** Audit trail
  - Status change logs with timestamps
  - User tracking (who made changes)
  - Revert/rollback capability (optional)

### 2.2 User Roles & Permissions

#### 2.2.1 Role Definitions

| Role | Permissions |
|------|-------------|
| **Admin** | Full access to all entities and settings |
| **Manager** | Create/update products, categories, orders; view all customers; assign orders |
| **Customer** | View own profile, address management, place orders, view own order history |
| **Guest** | Browse catalog, add to cart (if implemented), limited order creation |

#### 2.2.2 Permission Scopes

Per-module permissions using ABP's permission system:
- `Products.Create`, `Products.Update`, `Products.Delete`, `Products.View`
- `Categories.Create`, `Categories.Update`, `Categories.Delete`, `Categories.View`
- `Customers.ViewAll`, `Customers.ViewOwn`, `Customers.Update`
- `Orders.Create`, `Orders.Update`, `Orders.Delete`, `Orders.ViewAll`, `Orders.ViewOwn`
- `Addresses.Manage`, `Addresses.ViewOwn`

### 2.3 API Requirements

#### 2.3.1 General API Standards

- **Base Path:** `/api/app`
- **Versioning:** v1 in URL path or header-based
- **Response Format:** ABP's standard wrapper with `result`, `target`, `success`, `error`, `unAuthorizedRequest`
- **Authentication:** JWT Bearer token required for protected endpoints
- **Authorization:** Role-based via ABP permissions

#### 2.3.2 Required Endpoints

**Products**
- `GET /api/app/v1/products` — List with filtering, sorting, pagination
- `GET /api/app/v1/products/{id}` — Get by ID
- `POST /api/app/v1/products` — Create
- `PUT /api/app/v1/products/{id}` — Update
- `DELETE /api/app/v1/products/{id}` — Delete (soft delete preferred)

**Categories**
- `GET /api/app/v1/categories` — List with hierarchy support
- `GET /api/app/v1/categories/{id}` — Get by ID
- `POST /api/app/v1/categories` — Create
- `PUT /api/app/v1/categories/{id}` — Update
- `DELETE /api/app/v1/categories/{id}` — Delete

**Customers**
- `GET /api/app/v1/customers` — List with filters ( Admin/Manager only)
- `GET /api/app/v1/customers/{id}` — Get by ID (own or with permission)
- `PUT /api/app/v1/customers/{id}` — Update profile
- `GET /api/app/v1/customers/{id}/orders` — Customer's order history

**Addresses** (for customer address management)
- `GET /api/app/v1/customers/{customerId}/addresses` — List customer addresses
- `GET /api/app/v1/addresses/{id}` — Get by ID
- `POST /api/app/v1/customers/{customerId}/addresses` — Create address
- `PUT /api/app/v1/addresses/{id}` — Update address
- `DELETE /api/app/v1/addresses/{id}` — Delete address
- `PUT /api/app/v1/addresses/{id}/set-default` — Mark as default billing/shipping

**Orders**
- `GET /api/app/v1/orders` — List with filters (role-based)
- `GET /api/app/v1/orders/{id}` — Get by ID
- `POST /api/app/v1/orders` — Create (from cart or manual entry)
- `PUT /api/app/v1/orders/{id}/status` — Update status
- `GET /api/app/v1/customers/{customerId}/orders` — Customer's orders

#### 2.3.3 Filtering, Sorting, Pagination

All list endpoints must support:
- **Pagination:** `skip` (default 0), `maxResultCount` (default 10, max 100)
- **Sorting:** `sorting` parameter (e.g., `name asc`, `price desc`)
- **Filtering:** `filter` parameter for text search across appropriate fields
- **Response:** `PagedResultDto<T>` with total count

---

## 3. Data Model Requirements

### 3.1 Entity Conventions

- **Base:** `AggregateRoot` or `Entity<TKey>` (ABP conventions)
- **Primary Key:** `long` (identity) or `Guid` based on needs
- **Auditing:** `CreationTime`, `CreatorId`, `LastModificationTime`, `LastModifierId` (ABP automatic)
- **Soft Delete:** `IsDeleted` flag (ABP automatic)
- **Concurrency:** `RowVersion` (byte[]) for optimistic locking

### 3.2 Required Entities

#### Product
```csharp
public class Product : AggregateRoot<long>
{
    public string Name { get; set; }
    public string Sku { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public ProductStatus Status { get; set; }
    // ABP: CreationTime, CreatorId, etc.
}
```

#### Category
```csharp
public class Category : AggregateRoot<long>
{
    public string Name { get; set; }
    public Guid? ParentId { get; set; }  // Self-reference for hierarchy
    public int DisplayOrder { get; set; }
    public ICollection<ProductCategory> ProductCategories { get; }
}
```

#### ProductCategory (Many-to-Many)
```csharp
public class ProductCategory : Entity<long>
{
    public long ProductId { get; set; }
    public long CategoryId { get; set; }
    public Product Product { get; }
    public Category Category { get; }
}
```

#### Customer
```csharp
public class Customer : AggregateRoot<long>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string? Phone { get; set; }
    public CustomerStatus Status { get; set; }
    public DateTime? RegistrationDate { get; set; }
    public ICollection<Address> Addresses { get; }
    public ICollection<Order> Orders { get; }
}
```

#### Address (In Progress)
```csharp
public class Address : Entity<long>
{
    public long CustomerId { get; set; }
    public string RecipientName { get; set; }
    public string StreetLine1 { get; set; }
    public string? StreetLine2 { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }
    public AddressType Type { get; set; }  // Billing, Shipping
    public bool IsDefault { get; set; }
    public Customer Customer { get; }
}
```

#### Order
```csharp
public class Order : AggregateRoot<long>
{
    public string OrderNumber { get; set; }  // Generated, human-readable
    public long CustomerId { get; set; }
    public OrderStatus Status { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal ShippingAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Currency { get; set; } = "USD";
    public long? ShippingAddressId { get; set; }
    public long? BillingAddressId { get; set; }
    public ICollection<OrderItem> Items { get; }
    public ICollection<OrderStatusHistory> StatusHistory { get; }
}
```

#### OrderItem
```csharp
public class OrderItem : Entity<long>
{
    public long OrderId { get; set; }
    public long ProductId { get; set; }
    public string ProductName { get; set; }      // Snapshot at order time
    public decimal UnitPrice { get; set; }       // Snapshot at order time
    public int Quantity { get; set; }
    public decimal LineTotal { get; set; }
    public Order Order { get; }
    public Product Product { get; }
}
```

#### OrderStatusHistory (Optional but Recommended)
```csharp
public class OrderStatusHistory : Entity<long>
{
    public long OrderId { get; set; }
    public OrderStatus FromStatus { get; set; }
    public OrderStatus ToStatus { get; set; }
    public string? Notes { get; set; }
    public Guid ChangedByUserId { get; set; }
    public DateTime ChangeTime { get; set; }
}
```

### 3.3 Enumerations

```csharp
public enum ProductStatus { Active, Discontinued, OutOfStock }
public enum CustomerStatus { Active, Suspended, Inactive }
public enum AddressType { Billing, Shipping }
public enum OrderStatus
{
    Draft,
    Pending,
    Confirmed,
    Processing,
    Shipped,
    Delivered,
    Cancelled,
    Refunded
}
```

---

## 4. Non-Functional Requirements

### 4.1 Performance
- API response time: < 500ms for 90th percentile
- Database query optimization with proper indexes on foreign keys and filter columns
- Support 100+ concurrent users without degradation
- Pagination to limit data transfer (max 100 items per page)

### 4.2 Reliability & Availability
- 99.5% uptime target
- Graceful error handling with user-friendly messages
- Transactional integrity for order creation (all-or-nothing)
- Database transaction boundaries properly defined

### 4.3 Security
- OWASP Top 10 compliance
- All input validated (server-side)
- SQL injection protection (EF Core parameterized queries)
- XSS protection in API responses
- JWT token expiration: 1 hour (configurable)
- Rate limiting: 100 requests/minute per user/IP
- Secrets stored in environment variables (never in code)

### 4.4 Maintainability
- Clean Architecture: Domain → Application → Infrastructure → Presentation
- Dependency injection throughout (constructor injection)
- Interface segregation (depend on abstractions)
- Comprehensive unit tests (>80% coverage target)
- Integration tests for API endpoints
- XML documentation for all public APIs

### 4.5 Observability
- Structured logging with Serilog (or ABP's built-in)
- Request/response logging for API diagnostics
- Performance counters for slow queries
- Exception tracking and alerting (optional integration)

---

## 5. Technical Requirements

### 5.1 ABP Framework Conventions

- Base classes: `CrudAppService<TEntity, TKey, TDto>` for standard CRUD
- Use `IRepository<TEntity, TKey>` for data access
- AutoMapper configurations in `Profile` classes
- Localization via `IStringLocalizer` in Resources folder
- Permission definitions in `Permissions` class
- Feature flags via `IFeatureChecker` for progressive rollout

### 5.2 Database

**Development:** PostgreSQL (Docker)
**Production:** SQL Server

- Code-first migrations with `dotnet ef migrations add`
- Seed data for development and testing
- Migration reviews for destructive changes
- Transaction management in Application layer

### 5.3 API Contract

**Request/Response Wrapper:**
```json
{
  "result": { /* object */ },
  "target": "/api/app/v1/products",
  "success": true,
  "error": null,
  "unAuthorizedRequest": false
}
```

**Error Response (400 Bad Request):**
```json
{
  "result": null,
  "target": "/api/app/v1/products",
  "success": false,
  "error": {
    "code": "InvalidInput",
    "message": "One or more validation errors occurred.",
    "details": [
      { "field": "Price", "message": "Price must be greater than 0." }
    ]
  },
  "unAuthorizedRequest": false
}
```

### 5.4 Testing

**Unit Tests:**
- Domain services and entities
- Application services with mocked repositories
- DTO validation and mapping

**Integration Tests:**
- API controllers with real database (SQLite in-memory)
- End-to-end request/response validation
- Database migration tests

**E2E Tests (Future):**
- Complete user workflows
- Multi-step order creation
- Authentication and authorization scenarios

---

## 6. Current Implementation Gaps

### 6.1 Critical
1. **Order → Product Relationship** - Orders reference product names as strings; must link to Product entity
2. **Real Automated Tests** - Current tests are scaffolding; need actual coverage

### 6.2 High Priority
3. **Proper Permissions** -RBAC not fully implemented
4. **Customer Address Management** - In progress but not yet integrated
5. **List, Filter, Paging APIs** - Need full implementation for Customers and Orders
6. **Order Status Workflow** - Status transitions need validation rules

### 6.3 Medium Priority
7. **Order History/Audit** - Track status changes and user actions
8. **Default Address Selection** - During order creation, allow selection of saved addresses
9. **Product Search** - Full-text search across name, description, SKU
10. **Category Hierarchy** - Parent-child relationships in API responses

---

## 7. Definition of Ready (DoR)

Before a feature/task can be started:
- [ ] User story/task description is clear and unambiguous
- [ ] Acceptance criteria defined (Given-When-Then format)
- [ ] Database schema changes identified (if any)
- [ ] API endpoint(s) specified with request/response examples
- [ ] Permission requirements identified
- [ ] Dependencies on other tasks understood
- [ ] Test strategy defined (unit, integration, or E2E)

---

## 8. Definition of Done (DoD)

A feature/task is complete when:
- [ ] **Implementation:** Code follows ABP conventions and project standards
- [ ] **Tests:** All required tests written and passing (>80% coverage for new code)
- [ ] **Code Review:** At least 1 peer review completed and approved
- [ ] **Documentation:** XML comments on public APIs; Swagger annotations if needed
- [ ] **Database:** Migrations reviewed and tested on dev database
- [ ] **API:** Endpoints tested with Postman or Swagger UI
- [ ] **Permissions:** Tested with appropriate roles
- [ ] **No Breaking Changes:** Existing functionality verified (regression testing)
- [ ] **Git:** Commit message follows Conventional Commits format
- [ ] **GitLab:** Related issues referenced in commit messages (e.g., `Closes #10`)

---

## 9. Milestones & Phasing

### Phase 1: Foundation (Complete)
- Basic ABP project setup
- Product and Category CRUD
- Sample scaffolding tests

### Phase 2: Order Model Fix (Next Priority)
- Refactor Orders to reference Product entity (FK)
- Implement proper OrderItem relationship
- Update existingOrders API responses
- Add database migration with data preservation strategy
- Add tests for order creation with valid product references

### Phase 3: Address Management (In Progress)
- Complete customer address feature
- Integrate addresses into order creation flow
- Default billing/shipping address logic
- Address validation

### Phase 4: Customers & Permissions
- Full customer CRUD APIs
- Role-based access control implementation
- Customer order history
- Address ownership enforcement

### Phase 5: Order Workflow
- Status transition validation
- Order status history tracking
- Manager assignment capabilities
- Cancellation and refund workflows

### Phase 6: Enhancement & Polish
- Product search and advanced filtering
- Category hierarchy in API
- Reporting endpoints (sales by product, customer, date range)
- Performance optimization
- Documentation and API reference

---

## 10. Success Metrics

**Functional Completeness:**
- 100% of required use cases implemented and tested
- Zero broken API contracts

**Code Quality:**
- >80% unit test coverage
- Zero critical static analysis warnings
- All code reviews completed

**Performance:**
- 90th percentile API response < 500ms
- Database queries < 100ms (95th percentile)

**Security:**
- No OWASP Top 10 vulnerabilities
- All endpoints require authentication where appropriate
- Input validation on all public APIs

---

**Document Version:** 1.0
**Last Updated:** 2026-03-23
**Owner:** Products Development Team
**Related Documents:**
- `CLAUDE.md` — Project conventions and workflows
- `docs/feat/customer-address-management/` — Active feature work
- `docs/adr/` — Architecture decision records
