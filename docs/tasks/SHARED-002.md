# SHARED-002 — Add Address to DbContext and Generate Migration

**Layer:** Infrastructure
**Required by:** All address-related tasks
**Estimate:** 1 hour

---

## Linked User Story

| ID | Title | File |
|---|---|---|
| US-001 | Create Address | [../user-stories/US-001.md](../user-stories/US-001.md) |

---

## Technical Description

The Address entity needs to be registered in the `ProductsDbContext` so EF Core can manage its persistence. A database migration must be generated to create the corresponding table with proper column constraints, relationships, and the unique constraint on (CustomerId, Name) as implied by the business rule "Address name already exists for this customer".

---

## Approach

Open `src/Products.EntityFrameworkCore/EntityFrameworkCore/ProductsDbContext.cs` and add a `DbSet<Address>` property in the region where other entity DbSets are defined (near Categories and Products). Follow the same pattern: `public DbSet<Address> Addresses { get; set; }`.

In the `OnModelCreating` method, add a configuration block for the Address entity using `builder.Entity<Address>(b => { ... })`. Apply the same conventions as other entities:

- Use `b.ToTable("Addresses")` to name the table
- Use `b.ConfigureByConvention()` to auto-configure base class properties
- Configure required fields with `IsRequired()` and maximum lengths with `HasMaxLength()` matching the validation rules in the spec
- Configure the foreign key relationship to Customer: `b.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Cascade)` or appropriate cascade rule
- Add a unique index constraint on `(CustomerId, Name)` to enforce the business rule that address names must be unique per customer

After updating the DbContext, generate a new EF Core migration using the ABP migration pattern. Run the appropriate CLI command or use the DbMigrator project to create and apply the migration.

---

## Affected Components

- **Infrastructure:** `src/Products.EntityFrameworkCore/EntityFrameworkCore/ProductsDbContext.cs` — add DbSet and entity configuration
- **Infrastructure:** `src/Products.EntityFrameworkCore/Migrations/` — new migration file with timestamp prefix

---

## Dependencies

| Task | Title | File |
|---|---|---|
| SHARED-001 | Create Address domain entity | [../tasks/SHARED-001.md](./SHARED-001.md) |

---

## Acceptance Criteria

- [ ] DbSet<Address> property added to ProductsDbContext
- [ ] Entity configuration in OnModelCreating includes all column constraints (max lengths, required fields)
- [ ] Unique constraint on (CustomerId, Name) is defined
- [ ] Foreign key relationship to Customer is properly configured
- [ ] Migration file generated and applies without errors to a test database
- [ ] Migration follows existing naming convention (e.g., `20260319000000_AddAddressTable`)

---

*Part of technical plan: customer-address-management-technical-plan-2026-03-19.md*
