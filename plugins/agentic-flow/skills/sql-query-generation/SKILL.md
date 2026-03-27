---
name: postgresql-query-generation
version: 1.0
description: Generates PostgreSQL SQL queries for any .NET project using Entity Framework Core (ABP Framework, custom DDD, or other architectures). Automatically discovers domain entity structure, analyzes relationships, and adapts query generation to the project's conventions. Use this skill whenever someone asks to "generate a query", "write SQL for this", "create a PostgreSQL query", "get data from this table", or needs database queries for EF Core or ABP Framework projects.
---

# PostgreSQL Query Generation Skill (Domain-Agnostic)

## Overview

This skill generates PostgreSQL SQL queries for **any .NET project using Entity Framework Core** (ABP Framework, custom DDD, or other architectures). It automatically discovers the domain entity structure, analyzes relationships, and adapts to project-specific conventions.

**CORE CAPABILITY: AUTOMATIC DOMAIN DISCOVERY**

The skill employs intelligent code analysis to:
1. **Scan** the project's domain entity structure dynamically
2. **Analyze** entity properties, relationships, and inheritance hierarchies
3. **Map** C# types to PostgreSQL types automatically
4. **Identify** foreign key relationships and navigation properties
5. **Discover** base class patterns (FullAuditedAggregateRoot, SetupEntity, etc.)
6. **Adapt** query generation to any domain structure

This makes the skill **universally portable** across different projects and domain models, including:
- **ABP Framework** applications (ACMS, CRM, ERP, etc.)
- **Custom DDD** architectures
- **Layered monoliths** with Entity Framework Core
- **Multi-tenant** and single-tenant systems
- Projects with or without soft delete, auditing, etc.

---

## Dynamic Domain Discovery System

### Pre-Execution Analysis Phase

Before generating any query, the skill performs automatic analysis:

**Step 1: Entity Discovery**
```bash
# Symbolic scan of domain entities
- Search for all entity classes inheriting from:
  * FullAuditedAggregateRoot<Guid>
  * AuditedAggregateRoot<Guid>
  * AggregateRoot<Guid>
  * SetupEntity
  * Entity<Guid>
  * IMultiTenant implementers

- Extract from each entity:
  * Namespace (for table name inference)
  * Base classes and interfaces
  * Public properties with types
  * Data annotations ([Required], [StringLength], etc.)
  * Navigation properties (relationship detection)
```

**Step 2: Relationship Mapping**
```bash
# Foreign key detection patterns:
- Properties ending with "Id" (except standard fields)
  Examples: ApplicationId, WorkflowStageId, OperationId

- Navigation property analysis:
  * ICollection<T> → One-to-Many
  * T → Many-to-One
  * Nullable Guid? → Optional relationship

- Join table detection via naming:
  * Compound names: {Entity1}{Entity2}s
  * Contains both foreign key patterns
```

**Step 3: Schema Inference**
```bash
# Table name rules:
1. Entity class name (pluralized) → Table name
   Example: OperationWorkflowInstance → "OperationWorkflowInstances"

2. Schema rules:
   - Domain entities: "public" schema
   - ABP Identity: "AbpIdentity" schema
   - Multi-tenant entities include TenantId

# Column name rules:
- Direct property → Column (preserve casing with quotes)
- Foreign key: {NavigationProperty}Id
- Base class fields inherited automatically
```

### Domain Analysis Commands

The skill uses Serena tools to analyze the codebase:

```
Command: Get entity structure
mcp__serena__find_symbol(name_path_pattern="*", relative_path="src/.../Domain/Entities", depth=1)

Command: Get entity properties
mcp__serena__find_symbol(name_path_pattern="EntityName", include_body=true)

Command: Find relationships
mcp__serena__find_referencing_symbols(name_path="EntityName/PropertyName", relative_path="path/to/entity")

Command: Explore domain layer
Task(subagent_type=Explore, prompt="Analyze domain entities, relationships, and base classes")
```

### Entity Hierarchy & Base Classes

**DISCOVERED DYNAMICALLY** - The skill automatically identifies base class patterns from the codebase:

```
Common Base Patterns:
FullAuditedAggregateRoot<Guid>, IMultiTenant
├── Inherits: AuditedAggregateRoot<Guid> + ISoftDelete
├── Standard fields (all inheriting):
│   ├── Id (Guid) - Primary Key
│   ├── TenantId (Guid?) - Multi-tenancy (if IMultiTenant)
│   ├── CreationTime (DateTime)
│   ├── CreatorId (Guid?)
│   ├── LastModificationTime (DateTime?)
│   ├── LastModifierId (Guid?)
│   └── IsDeleted (bool)
└── Extensions may add: SystemName, DisplayName, Description, IsActive, Name, Code

SetupEntity (ABP Convention)
├── Inherits: FullAuditedAggregateRoot<Guid>
└── Adds: Name, Description, Code, IsActive

Entity<TKey> (EF Core base)
├── Id (TKey) - Primary Key
└── Minimal base

AuditedAggregateRoot<TKey>
├── CreationTime, CreatorId
├── LastModificationTime, LastModifierId
└── No soft delete
```

**Discovery Process:**
```
1. Scan entity files in Domain/Entities directory
2. Parse: namespace, base class, interfaces
3. Identify common properties across entities
4. Map to PostgreSQL types (see Type Mapping below)
5. Build relationship graph from:
   - Foreign key property names (ending in "Id")
   - Navigation property types (ICollection<T>, T)
   - Explicit [ForeignKey] attributes
```

**Automatic Base Class Detection:**
- `FullAuditedAggregateRoot<Guid>` → `Id`, `TenantId?`, `CreationTime`, `IsDeleted`, audit fields
- `SetupEntity` → inherits FullAudited + `Name`, `Description`, `Code`, `IsActive`
- `Entity<Guid>` → just `Id`
- `IMultiTenant` → adds `TenantId` pattern
- Any custom base → inherit all public properties

**Note:** Actual base classes vary by project. The skill inspects the `src/*.Domain/Entities/Base/` directory to identify project-specific base classes.

### Database Schema Patterns

#### Table Naming
- **Domain entities**: Plural PascalCase (e.g., `Applications`, `WorkflowStages`, `Operations`)
- **ABP identity tables**: Schema-prefixed: `AbpIdentity.Users`, `AbpIdentity.Roles`
- **Many-to-many join tables**: Compound name (e.g., `OperationUserAccountTypes`)

#### Column Naming
- **Primary keys**: `Id` (UUID/Guid)
- **Foreign keys**: `{EntityName}Id` pattern (e.g., `ApplicationId`, `WorkflowStageId`)
- **Common fields**:
  - `TenantId` (UUID, nullable) - Multi-tenancy filter
  - `SystemName` (VARCHAR) - Programmatic identifier
  - `DisplayName` (VARCHAR) - Human-readable name
  - `Description` (TEXT, nullable)
  - `IsActive` (BOOLEAN, default true)
  - Audit: `CreationTime`, `CreatorId`, `LastModificationTime`, `LastModifierId`
  - Soft delete: `IsDeleted` (BOOLEAN, default false)

#### PostgreSQL Data Types

| C# Type | PostgreSQL Type | Usage |
|---------|----------------|-------|
| `Guid` | `UUID` | Primary/Foreign keys |
| `string` (short) | `VARCHAR(n)` | Names, codes, identifiers |
| `string` (long) | `TEXT` | Descriptions, configs, FormData |
| `bool` | `BOOLEAN` | Flags, IsActive, IsDeleted |
| `DateTime` | `TIMESTAMPTZ` | Timestamps with timezone |
| `DateTime?` | `TIMESTAMPTZ` | Nullable timestamps |
| `JObject`/`object` | `JSONB` | Structured data, schemas, configs |
| `int` | `INTEGER` | Order, counts, enums |
| `enum` | `INTEGER` | Enum values stored as integers |

### Entity Relationships

#### One-to-Many
**Pattern**: ParentEntity → ChildEntityCollection
```
Entity: Parent
└── ICollection<Child> Children  (in Parent class)
Entity: Child
└── ParentId (foreign key) + Parent (navigation property)
```

**Discovery**: Skill automatically detects:
- ICollection<T> property in parent → identifies as collection navigation
- Child entity having parent's Id as foreign key
- Creates join on: `JOIN "Child" c ON c."ParentId" = parent."Id"`

#### Many-to-One
**Pattern**: Child → Parent reference
```
Entity: Child
└── ParentId (Guid) + Parent (navigation property)
Entity: Parent
└── (no direct reference to children)
```

**Discovery**: Any property with type equal to another entity (excluding primitive and collection types) is treated as navigation. Matching foreign key by {NavigationPropertyName}Id pattern.

#### Many-to-Many (via join entities)
**Pattern**: EntityA ↔ EntityB via JoinEntity
```
Entity: EntityA
└── ICollection<EntityB> EntitiesB  (optional)
Entity: EntityB
└── ICollection<EntityA> EntitiesA  (optional)
Join Table:
├── EntityAId (FK to EntityA)
└── EntityBId (FK to EntityB)
```

**Discovery**:
- Join entities named: `{EntityA}{EntityB}` or `{EntityA}{EntityB}s`
- Contains exactly 2 FK properties matching other entity names
- May include additional fields (ExecutionOrder, etc.)

#### Self-Referencing (Hierarchical)
**Pattern**: Entity with ParentId
```
Entity: Category
├── ParentId (Guid?) - nullable FK to self
└── Parent (navigation to same entity type)
```

**Discovery**:
- Property type matches the entity itself
- Property name = "{EntityName}Id" or "ParentId"
- Enables recursive CTE queries

---

**IMPORTANT**: Relationship discovery is **fully automatic**. The skill:
1. Analyzes all entity property types
2. Matches {EntityName}Id patterns to entity tables
3. Builds relationship graph
4. Suggests appropriate JOIN conditions
5. Detects join tables for many-to-many relationships

No hardcoded relationships required!

### Indexes & Constraints

**Dynamically Discovered from Entity Configuration**

The skill analyzes the DbContext OnModelCreating to determine:

#### Unique Constraints
```
Pattern: [Index(IsUnique = true)] attribute or HasIndex(...).IsUnique()
Examples from any project:
- Entities with natural keys: {TenantId, SystemName}
- Business keys: {TenantId, Code}, {TenantId, Name}
- Request numbers: RequestNumber, JobNumber, InvoiceNumber
```

**Discovery**: Parses entity configuration files or uses `FindReferencingSymbols` on `HasIndex` calls.

#### Composite Indexes
```
Discovered from:
- Primary foreign key combinations (typical: {ParentEntityId, SortOrder})
- Query patterns with multiple filters
- Unique constraints on multiple columns
```

**Skill inference**:
```sql
-- Common patterns automatically identified:
-- Pattern 1: Parent + child relationship queries
CREATE INDEX "IX_ChildEntity_Parent_Order"
ON "ChildEntities" ("ParentId", "Order");

-- Pattern 2: Status filtering with date
CREATE INDEX "IX_Entity_Status_Date"
ON "Entities" ("Status", "CreatedDate" DESC);

-- Pattern 3: Tenant + active (ABP convention)
CREATE INDEX "IX_Entity_Tenant_Active"
ON "Entities" ("TenantId")
WHERE "IsDeleted" = false AND "IsActive" = true;
```

#### Foreign Key Constraints
**Project-specific deletion strategy** (detected from DbContext):
```csharp
// Check OnModelCreating for:
builder.HasOne(...).WithMany(...).OnDelete(DeleteBehavior.Restrict);  // ABP default
builder.HasOne(...).WithMany(...).OnDelete(DeleteBehavior.Cascade);    // Custom
```

**Skill adapts**:
- RESTRICT → Prevent orphaned records, manual deletion
- CASCADE → Delete children automatically
- SET NULL → Optional relationships

#### Filtered Indexes (Soft Delete)
**Common for ABP projects**:
```sql
-- Pattern: Tenant + active records (IsDeleted filter)
CREATE INDEX "IX_{Entity}_{FilterColumn}"
ON "{Schema}"."{Entity}" ("{Column}")
WHERE "IsDeleted" = false;

-- Example discovered dynamically based on:
-- 1. Entities implementing ISoftDelete
-- 2. Common query patterns in repository methods
```

**Discovery**: Searches for `ISoftDelete` interface in entity base classes.

## Cross-Project Portability Features

### How the Skill Adapts to Different Domains

**1. Schema Discovery Phase**
```
When invoked, the skill:

Step 1: Locate Domain Entities
├── Search: src/*/Domain/Entities/**/*.cs
├── Identify: Entity classes (FullAuditedAggregateRoot, SetupEntity, etc.)
├── Parse: Namespaces → Table names (with schema)
└── Build: Master entity catalog

Step 2: Analyze Base Classes
├── Read: src/*/Domain/Entities/Base/*.cs
├── Identify: Common properties (Id, TenantId, IsDeleted, etc.)
├── Determine: Multi-tenancy pattern (TenantId presence)
└── Detect: Soft delete support (IsDeleted property)

Step 3: Map Relationships
├── Scan: All entity properties
├── Match: {EntityName}Id patterns to known entities
├── Identify: ICollection<T> as collection navigation
├── Build: Relationship graph with cardinality
└── Detect: Join tables (entities with 2+ FK to related entities)

Step 4: Infer Types & Constraints
├── Map: C# types → PostgreSQL types
├── Detect: Required vs optional (nullable)
├── Identify: String length limits (StringLength attribute)
└── Build: Type mapping dictionary for this project
```

**2. Dynamic Query Construction**
```
Instead of hardcoded table/column names:

OLD (static):
SELECT * FROM "OperationWorkflowInstances"
WHERE "WorkflowStageId" = @stageId

NEW (dynamic):
Table: Discovered from entity OperationWorkflowInstance
Columns: Retrieved from entity property analysis
FK relationships: Auto-joined based on navigation properties
Filters: Based on discovered property names
```

**3. Conventions Discovery**
```
ABP Framework → TenantId, IsDeleted, CreationTime, audit fields
Custom Project → Detected from base class properties
Naming → PascalCase → quoted identifiers (e.g., "SystemName")
Schemas → "public" for domain, "AbpIdentity" for identity
```

### Supported Frameworks

| Framework | Detection Strategy | Adaptations |
|-----------|-------------------|-------------|
| **ABP Framework** | FullAuditedAggregateRoot, IMultiTenant | Auto-add TenantId filters, soft delete checks |
| **Entity Framework Core** | Entity<T>, DbContext | Basic entity patterns |
| **DDD (Custom)** | AggregateRoot, ValueObject | Respect aggregate boundaries |
| **Multi-Tenancy** | TenantId property | Always filter by @tenantId |
| **Soft Delete** | ISoftDelete, IsDeleted | Auto-add IsDeleted = false |
| **CRUD Patterns** | Repository<T> | Suggest standard query patterns |

---

## Query Generation Patterns

### 1. Multi-Tenancy Awareness

**Always filter by TenantId for tenant-aware entities:**
```sql
-- Good (tenant-specific)
SELECT * FROM "Applications"
WHERE "TenantId" = 'tenant-guid-here' AND "IsActive" = true;

-- If system-wide (no tenant), TenantId is NULL
SELECT * FROM "Applications"
WHERE "TenantId" IS NULL AND "IsActive" = true;

-- To get both tenant and system records
SELECT * FROM "Applications"
WHERE "TenantId" IS NULL OR "TenantId" = 'tenant-guid-here';
```

**Tenant-aware entities** (all implementing `IMultiTenant`):
- All domain entities except ABP framework tables
- IdentityUser uses TenantId from AbpIdentity

### 2. Soft Delete Pattern

```sql
-- Include IsDeleted filter in all queries
SELECT * FROM "Applications" WHERE "IsDeleted" = false AND "IsActive" = true;

-- With tenant filter and soft delete
SELECT * FROM "Applications"
WHERE "TenantId" = @tenantId
  AND "IsDeleted" = false
  AND "IsActive" = true;
```

**Exception**: Some queries may intentionally include deleted records for admin/recovery purposes.

### 3. JSONB Querying (PostgreSQL Specific)

```sql
-- JSONB fields: InitialData, FormData, FormSchema, JsonSchema, UISchema, ImportConfig, ExportConfig, EventSourceConfiguration, OperationParameters

-- Check if JSONB field contains key
SELECT * FROM "OperationJobs"
WHERE "OperationParameters" @> '{"priority": "high"}';

-- Extract value from JSONB
SELECT
  "OperationParameters"->>'priority' as priority
FROM "OperationJobs";

-- Query JSONB array
SELECT * FROM "OperationJobs"
WHERE "OperationParameters" @> '{"tags": ["urgent"]}';

-- JSONB exists check
SELECT * FROM "DynamicTables"
WHERE "TableSchema" ? 'columns';

-- JSONB path query
SELECT
  jsonb_path_query("FormData", '$.fields[*].name') as field_name
FROM "ApplicationWorkflowInstances";
```

### 4. Date/Time Queries

```sql
-- TIMESTAMPTZ fields with timezone awareness
SELECT * FROM "OperationWorkflowInstances"
WHERE "WorkflowStageDate" >= '2024-01-01 00:00:00+00';

-- Date truncation for grouping
SELECT
  date_trunc('day', "CreationTime") as day,
  count(*) as count
FROM "ApplicationWorkflowInstances"
WHERE "TenantId" = @tenantId
GROUP BY date_trunc('day', "CreationTime")
ORDER BY day DESC;

-- Timezone conversion
SELECT
  "CreationTime" AT TIME ZONE 'UTC' as utc_time,
  "CreationTime" AT TIME ZONE 'Asia/Kathmandu' as local_time
FROM "OperationWorkflowInstances";
```

### 5. Common Query Templates

#### Get Active Records with Filters
```sql
SELECT
  e.*,
  -- Joins for related data
  a."Name" as "ApplicationName",
  ws."DisplayName" as "WorkflowStageName"
FROM "ApplicationTasks" e
JOIN "Applications" a ON a."Id" = e."ApplicationId" AND a."TenantId" = e."TenantId"
LEFT JOIN "WorkflowStages" ws ON ws."Id" = e."WorkflowStageId" AND ws."TenantId" = e."TenantId"
WHERE e."TenantId" = @tenantId
  AND e."IsDeleted" = false
  AND e."IsActive" = true
  AND a."IsActive" = true
ORDER BY e."CreationTime" DESC
LIMIT @limit OFFSET @offset;
```

#### Count with Filters (for pagination)
```sql
SELECT COUNT(*) as total
FROM "ApplicationWorkflowInstances" awi
JOIN "OperationWorkflowInstances" owf ON owf."Id" = awi."OperationWorkflowInstanceId"
WHERE awi."TenantId" = @tenantId
  AND awi."IsDeleted" = false
  AND awi."StageId" = @stageId;
```

#### Get Hierarchical Data (Recursive CTE)
```sql
WITH RECURSIVE workflow_config_tree AS (
  -- Anchor: root configurations (ParentId IS NULL)
  SELECT
    "Id",
    "ParentId",
    "Code",
    "ExecutionOrder",
    1 as level
  FROM "OperationWorkflowConfigurations"
  WHERE "ParentId" IS NULL
    AND "TenantId" = @tenantId
    AND "IsDeleted" = false

  UNION ALL

  -- Recursive: children
  SELECT
    c."Id",
    c."ParentId",
    c."Code",
    c."ExecutionOrder",
    t.level + 1
  FROM "OperationWorkflowConfigurations" c
  INNER JOIN workflow_config_tree t ON c."ParentId" = t."Id"
  WHERE c."TenantId" = @tenantId
    AND c."IsDeleted" = false
)
SELECT * FROM workflow_config_tree
ORDER BY level, "ExecutionOrder";
```

#### Tag/Category Filtering (OperationWorkflowInstanceTags)
```sql
SELECT owfi.*
FROM "OperationWorkflowInstances" owfi
JOIN "OperationWorkflowInstanceTags" owft
  ON owft."OperationWorkflowInstanceId" = owfi."Id"
  AND owft."TenantId" = owfi."TenantId"
WHERE owfi."TenantId" = @tenantId
  AND owfi."IsDeleted" = false
  AND (owft."Key" = 'priority' AND owft."Value" = 'high')
  AND (owft."Key" = 'department' AND owft."Value" = 'finance');
```

#### Full-Text Search (like pattern)
```sql
-- SystemName, DisplayName searches (case-insensitive)
SELECT * FROM "Applications"
WHERE "TenantId" = @tenantId
  AND "IsDeleted" = false
  AND (
    "SystemName" ILIKE '%search%'
    OR "DisplayName" ILIKE '%search%'
  );

-- Description search (TEXT field)
SELECT * FROM "Configurations"
WHERE "TenantId" = @tenantId
  AND "Description" ILIKE '%config%';
```

#### Complex Joins (Application Task Workflow)
```sql
SELECT
  awi."Id",
  awi."TaskNumber",
  awi."WorkflowStageDate",
  app."Name" as "ApplicationName",
  task."Name" as "TaskName",
  stage."DisplayName" as "StageName",
  substage."DisplayName" as "SubStageName",
  owf."RequestNumber" as "OperationRequestNumber",
  u."UserName" as "RequestedByUserName"
FROM "ApplicationWorkflowInstances" awi
JOIN "Applications" app ON app."Id" = awi."ApplicationWorkflowConfigurationId" -- Through configuration
JOIN "OperationWorkflowInstances" owf ON owf."Id" = awi."OperationWorkflowInstanceId"
JOIN "WorkflowStages" stage ON stage."Id" = awi."StageId"
LEFT JOIN "WorkflowSubStages" substage ON substage."Id" = awi."SubStageId"
LEFT JOIN "AbpIdentity"."Users" u ON u."Id"::uuid = awi."RequestedBy"::uuid
WHERE awi."TenantId" = @tenantId
  AND awi."IsDeleted" = false
ORDER BY awi."WorkflowStageDate" DESC;
```

### 6. Pagination with Count

```sql
-- Two-query pattern (common in ABP)
WITH filtered AS (
  SELECT awi.*
  FROM "ApplicationWorkflowInstances" awi
  WHERE awi."TenantId" = @tenantId
    AND awi."IsDeleted" = false
    AND awi."IsActive" = true
)
SELECT
  (SELECT COUNT(*) FROM filtered) as "TotalCount",
  f.*
FROM filtered f
ORDER BY f."CreationTime" DESC
LIMIT @maxResultCount OFFSET @skipCount;
```

### 7. Upsert Pattern (INSERT ... ON CONFLICT)

```sql
-- For entities with natural keys (SystemName + TenantId)
INSERT INTO "Applications" ("Id", "TenantId", "SystemName", "DisplayName", "IsActive")
VALUES (@id, @tenantId, @systemName, @displayName, true)
ON CONFLICT ("TenantId", "SystemName")
WHERE "IsDeleted" = false
DO UPDATE SET
  "DisplayName" = EXCLUDED."DisplayName",
  "LastModificationTime" = NOW();
```

### 8. Conditional Updates

```sql
-- Update only if record is active and not deleted
UPDATE "OperationWorkflowInstances"
SET
  "WorkflowStageId" = @newStageId,
  "LastModificationTime" = NOW()
WHERE "Id" = @id
  AND "TenantId" = @tenantId
  AND "IsDeleted" = false
  AND "IsActive" = true
RETURNING *;
```

### 9. Bulk Operations

```sql
-- Insert multiple rows
INSERT INTO "DynamicFormRules" ("Id", "DynamicFormId", "BusinessRuleId", "ExecutionOrder", "TenantId")
VALUES
  (@id1, @formId1, @ruleId1, 100, @tenantId),
  (@id2, @formId2, @ruleId2, 200, @tenantId);

-- Bulk update with CASE
UPDATE "DynamicTableRules"
SET "ExecutionOrder" = CASE "Id"
  WHEN @id1 THEN 100
  WHEN @id2 THEN 200
  WHEN @id3 THEN 300
  ELSE "ExecutionOrder"
END
WHERE "Id" IN (@id1, @id2, @id3)
  AND "TenantId" = @tenantId;
```

### 10. Window Functions for Analytics

```sql
-- Row number per group
SELECT
  "OperationId",
  "RequestNumber",
  "WorkflowStageId",
  ROW_NUMBER() OVER (
    PARTITION BY "OperationId"
    ORDER BY "WorkflowStageDate" DESC
  ) as stage_sequence
FROM "OperationWorkflowInstances"
WHERE "TenantId" = @tenantId
  AND "IsDeleted" = false;

-- First/Last value
SELECT DISTINCT ON ("OperationId")
  "OperationId",
  FIRST_VALUE("RequestNumber") OVER (
    PARTITION BY "OperationId"
    ORDER BY "WorkflowStageDate" DESC
  ) as latest_request
FROM "OperationWorkflowInstances";
```

### 11. Permission & Security Queries

```sql
-- Get user's operations via OperationUserAccountTypes
SELECT DISTINCT
  o.*
FROM "Operations" o
JOIN "OperationUserAccountTypes" ovat
  ON ovat."OperationId" = o."Id"
  AND ovat."TenantId" = o."TenantId"
JOIN "UserAccountTypes" uat
  ON uat."Id" = ovat."UserAccountTypeId"
  AND uat."TenantId" = ovat."TenantId"
JOIN "AbpIdentity"."Users" u ON u."UserName" = @userName
WHERE o."TenantId" = @tenantId
  AND o."IsDeleted" = false
  AND uat."Name" IN (
    SELECT "Name"
    FROM "UserAccountTypes"
    WHERE "Id" IN (
      SELECT "UserAccountTypeId"
      FROM "AbpIdentity"."UserRoles" ur
      JOIN "AbpIdentity"."Roles" r ON r."Id" = ur."RoleId"
      WHERE ur."UserId" = u."Id"
    )
  );
```

### 12. Webhook & Integration Queries

```sql
-- Get active webhook subscriptions for an event
SELECT
  ws.*,
  c."Email" as "ConsumerEmail",
  ac."SystemName" as "ApiClientSystemName",
  ac."RequestConfig" as "RequestConfig"
FROM "WebhookSubscriptions" ws
JOIN "Consumers" c ON c."Id" = ws."ConsumerId" AND c."TenantId" = ws."TenantId"
JOIN "ApiClients" ac ON ac."Id" = ws."ApiClientId" AND ac."TenantId" = ws."TenantId"
JOIN "WebhookEvents" we ON we."Id" = ws."WebhookEventId" AND we."TenantId" = ws."TenantId"
WHERE ws."TenantId" = @tenantId
  AND ws."IsDeleted" = false
  AND ws."IsActive" = true
  AND we."EventName" = @eventName;

-- Get webhook retry attempts
SELECT
  wl.*,
  ws."ConsumerId",
  ac."SystemName" as "ApiClientName"
FROM "WebhookLogs" wl
JOIN "WebhookSubscriptions" ws ON ws."Id" = wl."WebhookSubscriptionId"
JOIN "ApiClients" ac ON ac."Id" = ws."ApiClientId"
WHERE wl."Succeeded" = false
  AND wl."RetryCount" < wl."MaxRetryCount"
  AND wl."AttemptedAt" > NOW() - INTERVAL '1 hour'
ORDER BY wl."AttemptedAt" ASC;
```

## Entity Property Reference

### SetupEntity Base Properties
| Property | Type | Constraints | Description |
|----------|------|-------------|-------------|
| Id | UUID | PK | Unique identifier |
| TenantId | UUID | FK (nullable) | Multi-tenancy |
| Name | VARCHAR(128) | Required | Entity name |
| Description | TEXT | Nullable | Detailed description |
| Code | VARCHAR(64) | Nullable | Programmatic code |
| IsActive | BOOLEAN | Required, default true | Active status |
| CreationTime | TIMESTAMPTZ | Required | Audit |
| CreatorId | UUID | Nullable | Audit |
| LastModificationTime | TIMESTAMPTZ | Nullable | Audit |
| LastModifierId | UUID | Nullable | Audit |
| IsDeleted | BOOLEAN | Required, default false | Soft delete |

### FullAuditedAggregateRoot Additional Properties
- Extends `AuditedAggregateRoot` with soft delete
- Includes `DeleterId`, `DeletionTime` when soft deleted

### DescriptiveEntityBase
- Same as SetupEntity but for entities that need custom inheritance
- Located in: `Entities.Base.DescriptiveEntityBase`

## Generic Query Templates (Portable)

### Template: Get Active Records by Entity

```sql
-- TEMPLATE: Generic SELECT with filters
-- Replace {Entity} with actual entity name from your domain
SELECT
  e."Id",
  e."{DisplayNameColumn}",  -- e.g., "Name", "SystemName", "Title"
  e."{SortColumn}"          -- e.g., "CreationTime", "Order"
FROM "{Entity}" e
WHERE e."TenantId" = @tenantId
  AND e."IsDeleted" = false
  AND e."IsActive" = true
ORDER BY e."{SortColumn}" DESC
LIMIT @limit OFFSET @offset;
```

**Auto-discovery fills in:**
- `{Entity}` → Table name from entity class
- `{DisplayNameColumn}` → Name, SystemName, DisplayName, Title (detected by common naming)
- `{SortColumn}` → CreationTime, ModifiedDate, Order

### Template: Get by Foreign Key

```sql
-- TEMPLATE: Find child records by parent
SELECT
  child.*,
  parent."{ParentDisplayColumn}" as "ParentName"
FROM "{ChildEntity}" child
JOIN "{ParentEntity}" parent
  ON parent."Id" = child."{ParentEntityId}"
WHERE child."{ParentEntityId}" = @parentId
  AND child."TenantId" = @tenantId
  AND child."IsDeleted" = false
ORDER BY child."CreationTime" DESC;
```

**Auto-discovery determines:**
- `{ChildEntity}` → From navigation property
- `{ParentEntity}` → Type of navigation property
- `{ParentEntityId}` → Foreign key property name
- `{ParentDisplayColumn}` → Identifier column for parent

### Template: Count with Filters (Pagination)

```sql
WITH filtered AS (
  SELECT e.*
  FROM "{Entity}" e
  WHERE e."TenantId" = @tenantId
    AND e."IsDeleted" = false
    AND e."{FilterColumn1}" = @filterValue1
    AND e."{FilterColumn2}" LIKE '%' || @search || '%'
)
SELECT
  (SELECT COUNT(*) FROM filtered) as "TotalCount",
  f.*
FROM filtered f
ORDER BY f."CreationTime" DESC
LIMIT @maxCount OFFSET @skipCount;
```

### Template: Recursive Hierarchy (Self-Referencing)

```sql
WITH RECURSIVE entity_tree AS (
  -- Anchor: root nodes (ParentId IS NULL)
  SELECT
    e."Id",
    e."ParentId",
    e."{DisplayNameColumn}",
    1 as "Level"
  FROM "{Entity}" e
  WHERE e."ParentId" IS NULL
    AND e."TenantId" = @tenantId
    AND e."IsDeleted" = false

  UNION ALL

  -- Recursive: children
  SELECT
    c."Id",
    c."ParentId",
    c."{DisplayNameColumn}",
    t."Level" + 1
  FROM "{Entity}" c
  INNER JOIN entity_tree t ON c."ParentId" = t."Id"
  WHERE c."TenantId" = @tenantId
    AND c."IsDeleted" = false
)
SELECT * FROM entity_tree
ORDER BY "Level", "{SortColumn}";
```

### Template: Many-to-Many via Join Table

```sql
SELECT
  e1.*,
  e2.*,
  jt."{AdditionalField1}",  -- optional extra columns in join table
  jt."{AdditionalField2}"
FROM "{Entity1}" e1
JOIN "{JoinEntity}" jt ON jt."{Entity1}Id" = e1."Id"
JOIN "{Entity2}" e2 ON e2."Id" = jt."{Entity2}Id"
WHERE e1."Id" = @entity1Id
  AND e1."TenantId" = @tenantId
  AND e2."TenantId" = @tenantId
  AND jt."IsDeleted" = false;
```

---

**Dynamic Discovery fills all {...} placeholders automatically based on:**
- Entity property inspections
- Navigation property analysis
- Common naming patterns (Id, Name, DisplayName, etc.)
- Project's base class conventions

---

## Project-Specific Examples

The following examples demonstrate PostgreSQL patterns **specific to the ACMS project**.

### Common Abbreviations in Table/Column Names (ACMS-specific)

- `owi` - OperationWorkflowInstance
- `awi` - ApplicationWorkflowInstance
- `owc` - OperationWorkflowConfiguration
- `uat` - UserAccountType
- `br` - BusinessRule
- `dt` - DynamicTable
- `df` - DynamicForm
- `ws` - WorkflowStage
- `wss` - WorkflowSubStage

## Important Notes

### 1. ABP Framework Conventions (Auto-Detected)

When the skill encounters an ABP-based project, it automatically detects and applies:
- **Primary Key**: `Id` (Guid, int, long, etc. - whatever the base class uses)
- **Multi-tenancy**: `TenantId` property + `IMultiTenant` interface detection
- **Soft Delete**: `IsDeleted` property + `ISoftDelete` interface
- **Auditing**: `CreationTime`, `CreatorId`, `LastModificationTime`, `LastModifierId`
- **Auto-populated**: Convention-based automatic population by framework

**For non-ABP projects**: The skill adapts to whatever base class properties are discovered.

### 2. PostgreSQL Optimizations
- `JSONB` is preferred over `JSON` for indexing and performance
- GIN indexes recommended on JSONB columns frequently queried
- Use `EXPLAIN ANALYZE` for query performance analysis
- Consider partial indexes for frequently filtered subsets

### 3. Multi-Tenancy Best Practices
- Always include `TenantId` in WHERE clauses
- Use parameterized queries to prevent SQL injection
- TenantId should be validated and sanitized
- Cross-tenant queries require explicit permission

### 4. Transaction Management
- ABP uses Unit of Work pattern automatically
- Multiple repository calls in same service method share transaction
- Use `[UnitOfWork]` attribute for manual control
- PostgreSQL transaction isolation level: READ COMMITTED

### 5. Naming Conventions for Generated SQL
- Schema: "public" for domain tables, "AbpIdentity" for identity
- Double quotes for case-sensitive identifiers
- Use snake_case for generated aliases
- Prefix parameters with `@` for consistency (Npgsql uses `@`)

## Example Use Cases

### Use Case 1: Get Pending Approval Tasks for User
```sql
SELECT DISTINCT
  at."Name" as "TaskName",
  at."Code" as "TaskCode",
  awi."TaskNumber",
  ws."DisplayName" as "CurrentStage",
  owf."RequestNumber",
  app."DisplayName" as "ApplicationName"
FROM "ApplicationWorkflowInstances" awi
JOIN "OperationWorkflowInstances" owf ON owf."Id" = awi."OperationWorkflowInstanceId"
JOIN "WorkflowStages" ws ON ws."Id" = awi."StageId"
JOIN "OperationWorkflowConfigurations" owc ON owc."Id" = awi."OperationWorkflowConfigurationId"
JOIN "ApplicationTasks" at ON at."Id" = owc."ApplicationTaskId"
JOIN "Applications" app ON app."Id" = at."ApplicationId"
WHERE awi."TenantId" = @tenantId
  AND awi."IsDeleted" = false
  AND ws."IsActive" = true
  AND ws."IsOperationLevel" = false  -- Sub-stage level
  AND owf."WorkflowStageId" IN (
    -- Get stages assigned to user's UserAccountType
    SELECT "WorkflowStageId"
    FROM "WorkflowSubStages"
    WHERE "Id" IN (
      SELECT "SubStageId"
      FROM "OperationUserAccountTypes"
      WHERE "UserAccountTypeId" = @userAccountTypeId
    )
  )
ORDER BY awi."WorkflowStageDate" DESC;
```

### Use Case 2: Find Applications with Specific Business Rules
```sql
SELECT
  app.*,
  array_agg(DISTINCT br."SystemName") as "BusinessRules"
FROM "Applications" app
JOIN "ApplicationTasks" at ON at."ApplicationId" = app."Id" AND at."TenantId" = app."TenantId"
JOIN "OperationWorkflowConfigurations" owc ON owc."ApplicationTaskId" = at."Id"
JOIN "ApplicationTaskRules" atr ON atr."ApplicationTaskId" = at."Id"
JOIN "BusinessRules" br ON br."Id" = atr."BusinessRuleId"
WHERE app."TenantId" = @tenantId
  AND app."IsDeleted" = false
  AND br."SystemName" IN ('Rule1', 'Rule2', 'Rule3')
GROUP BY app."Id"
HAVING COUNT(DISTINCT br."Id") = 3;  -- All rules matched
```

### Use Case 3: Dynamic Form Schema Analysis
```sql
-- Extract field names from JSON schema of DynamicForms
SELECT
  df."Id",
  df."Name" as "FormName",
  jsonb_array_elements(
    jsonb_path_query_array(df."JsonSchema", '$.properties.*')
  )->>'name' as "FieldName",
  jsonb_path_query_first(df."JsonSchema", '$.properties.*') as "FieldSchema"
FROM "DynamicForms" df
WHERE df."TenantId" = @tenantId
  AND df."IsDeleted" = false
  AND df."IsActive" = true
  AND df."JsonSchema" ? 'properties';
```

### Use Case 4: Workflow Instance Timeline
```sql
SELECT
  owfi."RequestNumber",
  owfi."WorkflowStageDate",
  ws."DisplayName" as "StageName",
  wss."DisplayName" as "SubStageName",
  u."UserName" as "StageByUser",
  COALESCE(awil."LogMessage", 'N/A') as "LogMessage"
FROM "OperationWorkflowInstances" owfi
JOIN "WorkflowStages" ws ON ws."Id" = owfi."WorkflowStageId"
LEFT JOIN "WorkflowSubStages" wss ON wss."Id" = owfi."SubStageId"
LEFT JOIN "AbpIdentity"."Users" u ON u."Id" = owfi."WorkflowStageBy"
LEFT JOIN "ApplicationWorkflowInstanceLogs" awil
  ON awil."ApplicationWorkflowInstanceId" = owfi."Id"
  AND awil."StageId" = owfi."WorkflowStageId"
WHERE owfi."RequestNumber" = @requestNumber
  AND owfi."TenantId" = @tenantId
ORDER BY owfi."WorkflowStageDate" DESC;
```

### Use Case 5: Configuration Key-Value Lookup
```sql
-- Efficient key lookup with tenant fallback
SELECT
  COALESCE(ttenant."Value", ssystem."Value") as "EffectiveValue"
FROM (
  SELECT "Value"
  FROM "Configurations"
  WHERE "TenantId" = @tenantId
    AND "Key" = @configKey
    AND "IsDeleted" = false
    AND "IsActive" = true
) ttenant
FULL OUTER JOIN (
  SELECT "Value"
  FROM "Configurations"
  WHERE "TenantId" IS NULL
    AND "Key" = @configKey
    AND "IsDeleted" = false
    AND "IsActive" = true
) ssystem ON true;
```

## Schema Evolution & Migrations

### Adding New Column
```sql
-- Add nullable column (safe)
ALTER TABLE "Applications"
ADD COLUMN "CustomField" VARCHAR(255) NULL;

-- Add with default (batched for large tables)
ALTER TABLE "Applications"
ADD COLUMN "IsFeatured" BOOLEAN NOT NULL DEFAULT false;

-- For existing data, use two-phase:
-- 1. Add nullable
ALTER TABLE "Applications" ADD COLUMN "NewFeatureFlag" BOOLEAN NULL;
-- 2. Update in batches
UPDATE "Applications" SET "NewFeatureFlag" = false WHERE "NewFeatureFlag" IS NULL;
-- 3. Set NOT NULL
ALTER TABLE "Applications" ALTER COLUMN "NewFeatureFlag" SET NOT NULL;
```

### Creating Indexes
```sql
-- Single column
CREATE INDEX CONCURRENTLY "IX_Applications_SystemName"
ON "Applications" ("SystemName")
WHERE "IsDeleted" = false;

-- Composite index
CREATE INDEX CONCURRENTLY "IX_OperationWorkflowInstances_Stage_Requested"
ON "OperationWorkflowInstances" ("WorkflowStageId", "RequestedDate" DESC)
WHERE "IsDeleted" = false;

-- GIN index for JSONB
CREATE INDEX CONCURRENTLY "IX_OperationJobs_Parameters_GIN"
ON "OperationJobs" USING GIN ("OperationParameters");

-- Partial index for active records only
CREATE INDEX CONCURRENTLY "IX_DynamicTables_Active"
ON "DynamicTables" ("TableName")
WHERE "IsDeleted" = false AND "IsActive" = true;
```

## Tips for Query Optimization

1. **Use indexes wisely**:
   - Foreign key columns are automatically indexed by `OnModelCreating`
   - Add indexes for frequently filtered columns (`SystemName`, `IsActive`)
   - Use partial indexes for soft delete queries

2. **Avoid SELECT ***:
   ```sql
   -- Bad
   SELECT * FROM "Applications";

   -- Good (only needed columns)
   SELECT "Id", "SystemName", "DisplayName", "IsActive"
   FROM "Applications";
   ```

3. **Use EXISTS instead of IN for subqueries**:
   ```sql
   -- Good for checking existence
   SELECT * FROM "Applications" a
   WHERE EXISTS (
     SELECT 1 FROM "ApplicationTasks" t
     WHERE t."ApplicationId" = a."Id"
       AND t."IsActive" = true
   );
   ```

4. **Batch JSONB operations**:
   ```sql
   -- Instead of multiple JSON extractions in WHERE
   SELECT * FROM "OperationJobs"
   WHERE ("OperationParameters" @> '{"status": "pending"}')
     AND ("OperationParameters" @> '{"priority": "high"}');
   ```

5. **Connection pooling**:
   - Use Npgsql connection pooling (default: 100)
   - Set `Maximum Pool Size` in connection string for high-traffic scenarios

## Error Handling

### Common PostgreSQL Errors

| Error | Cause | Solution |
|-------|-------|----------|
| `23505` (unique_violation) | Duplicate key | Check tenant + natural key uniqueness |
| `23503` (foreign_key_violation) | Missing parent | Ensure parent record exists before child |
| `23502` (not_null_violation) | Required field null | Check column constraints |
| `42703` (undefined_column) | Column doesn't exist | Verify column name and schema |
| `42P01` (undefined_table) | Table missing | Check table name and casing with quotes |
| `55P03` (lock_not_available) | Deadlock | Use proper transaction ordering |

## Reference

### Dynamic Domain Discovery Reference

#### How the Skill Discovers Domain Structure

```
Step 1: Entity Discovery
=======================
Search Pattern: src/*/Domain/Entities/**/*.cs
Action: Identify all classes that:
  - Inherit from: AggregateRoot<T>, Entity<T>, FullAuditedAggregateRoot<T>, etc.
  - Implement: IAggregateRoot, IEntity, IMultiTenant, ISoftDelete

Output: EntityCatalog
  - Entity: {ClassName, Namespace, BaseClass, Interfaces, Properties[], TableName}
```

```
Step 2: Base Class Analysis
===========================
Location: src/*/Domain/Entities/Base/*.cs
Action: Read base classes to determine common patterns:
  - What properties are defined (Id, TenantId, IsDeleted, etc.)
  - What interfaces are implemented
  - How entities are structured

Output: BaseClassModel
  - BaseClassName → PropertySet {standard properties}
  - MultiTenancyPattern → "HasTenantId", "OptionalTenantId", "None"
  - SoftDeletePattern → "IsDeleted", "IsActive", "None"
  - AuditPattern → "FullAudited", "Audited", "None"
```

```
Step 3: Relationship Graph Building
====================================
Algorithm:
  1. For each entity, analyze all properties:
     - Type = AnotherEntity → Navigation property
     - Name ends with "Id" + Type = primitive → Foreign key candidate
     - Type = ICollection<AnotherEntity> → Collection navigation

  2. Match FKs to entities:
     Property.Name: {NavigationProperty}Id
     → Links to entity: {NavigationProperty}

  3. Detect join tables:
     - Entity name pattern: {Entity1}{Entity2}s
     - Has exactly 2 FK properties: {Entity1}Id, {Entity2}Id
     - No primary reference outside the pair

  4. Build graph:
     Entity {Relationships: [
       {Type: OneToOne|OneToMany|ManyToMany, Target: EntityName, Via: JoinEntity}
     ]}

Output: RelationshipGraph
```

```
Step 4: Schema Inference
========================
Table Naming:
  - Default: Pluralize entity class name
  - Override: [Table("CustomName")] attribute
  - Schema: [Schema("Custom")] or convention-based

Column Naming:
  - Property name → Column name (preserve case)
  - Override: [Column("CustomColumn")] attribute

Type Mapping:
  C# Type          → PostgreSQL Type
  -------------------------------
  Guid             → UUID
  Guid?            → UUID (nullable)
  int              → INTEGER
  int?             → INTEGER
  long             → BIGINT
  long?            → BIGINT
  string           → VARCHAR(256) or TEXT (if long)
  bool             → BOOLEAN
  DateTime         → TIMESTAMPTZ
  DateTime?        → TIMESTAMPTZ
  decimal          → NUMERIC
  decimal?         → NUMERIC
  JsonDocument     → JSONB
  object           → JSONB
  enum             → INTEGER

Output: SchemaModel
  - Entity → {TableName, Schema, Columns[], PrimaryKey}
```

#### Using Discovered Schema for Query Generation

**Template Instantiation:**

```sql
-- Generic: SELECT with relationships
SELECT
  e."{DisplayColumn}",
  -- Relationship: {NavigationProperty}
  np."{DisplayColumn}" as "{NavigationProperty}{DisplayColumn}"
FROM "{EntityTable}" e
{JOIN clauses from RelationshipGraph}
WHERE {auto-generated filters}
```

**Auto-Generated Filters (based on BaseClassModel):**
```sql
-- If MultiTenancy = HasTenantId:
AND e."TenantId" = @tenantId

-- If SoftDelete = IsDeleted:
AND e."IsDeleted" = false

-- If HasAudit:
-- Include CreationTime, LastModificationTime in results or ORDER BY

-- If IsActive = true/false (common):
AND e."IsActive" = true
```

**Example Discovery → Query Flow:**

```
User Request: "Get all Products with Category name"

Discovery:
  Product: {Id, Name, CategoryId, TenantId, IsDeleted}
  Category: {Id, Name}
  Relationship: Product.CategoryId → Category.Id

Generated Query:
  SELECT
    p."Id",
    p."Name",
    p."Price",
    c."Name" as "CategoryName"
  FROM "Products" p
  JOIN "Categories" c ON c."Id" = p."CategoryId"
  WHERE p."TenantId" = @tenantId
    AND p."IsDeleted" = false
  ORDER BY p."CreationTime" DESC;
```

---

### ACMS Project Reference (Example Implementation)

The following demonstrates how discovery works on the ACMS project specifically. **For other projects, this section would be auto-generated.**

**Workflow Domain** (discovered pattern):
- `WorkflowStages` → `WorkflowSubStages` (One-to-Many)
- `OperationWorkflowInstances` → `WorkflowStage`, `Operation`, `UserAccountType` (ManyToOne)
- `ApplicationWorkflowInstances` → `OperationWorkflowInstance`, `WorkflowStage`, `WorkflowSubStage` (ManyToOne)

**Operations Domain**:
- `Operations` → `OperationUserAccountTypes` (Many-to-Many via join)
- `Operations` → `OperationRules` (One-to-Many)

## Skill Usage

### For the ACMS Project

When asked to generate PostgreSQL queries for ACMS:

1. **Identify the entity** from domain terminology
2. **Determine relationships** needed for the query
3. **Apply multi-tenancy** filter automatically
4. **Include soft delete** filter by default
5. **Use appropriate PostgreSQL types** (JSONB, TEXT, UUID)
6. **Follow naming conventions** (quoted identifiers, aliases)
7. **Add pagination** if needed (LIMIT/OFFSET)
8. **Consider indexes** for performance
9. **Include audit fields** when relevant
10. **Add proper ORDER BY** using `CreationTime` or business-relevant fields

### For Different Projects (Cross-Project Usage)

When using this skill with a **different project**:

**Automatic Discovery Phase** (runs on first query request):
```
1. Execute domain analysis:
   - Scan: src/{ProjectName}.Domain/Entities/**/*.cs
   - Identify: All entity classes and their base types
   - Extract: Properties, types, attributes
   - Detect: Relationships via navigation properties
   - Build: In-memory schema model

2. Cache schema model for session (reuse on subsequent queries)

3. Query generation uses discovered schema:
   - Table names: From entity class names (pluralized)
   - Column names: From property names
   - Joins: From navigation properties
   - Filters: From property existence and common patterns
```

**Project Adaptation Examples**:

| Project Feature | Detection | Adaptation |
|-----------------|-----------|------------|
| Different base class (e.g., `BaseEntity`) | Scan Base/*.cs | Inherit properties: Id, TenantId, etc. |
| No multi-tenancy | TenantId absent | Skip automatic TenantId filter |
| No soft delete | IsDeleted absent | Skip IsDeleted = false filter |
| Different PK type (int, long) | Entity<TKey> | Use appropriate database type |
| Composite keys | [Key] on multiple props | Generate composite primary key |
| Custom table names | [Table("Custom")] | Use attribute override |
| Different schemas | [Schema("Custom")] | Apply schema prefix |

**Invocation Pattern for New Project**:
```bash
User: "Generate query to get all active Products in processing stage"

Skill:
1. Discovers: Product entity (with properties: Id, Name, StatusId, TenantId, IsDeleted, IsActive)
2. Discovers: Status navigation (Product → Status)
3. Discovers: Status entity (with properties: Id, Name, IsProcessing boolean?)
4. Generates:
   SELECT p.*, s."Name" as "StatusName"
   FROM "Products" p
   JOIN "Statuses" s ON s."Id" = p."StatusId"
   WHERE p."TenantId" = @tenantId
     AND p."IsDeleted" = false
     AND p."IsActive" = true
     AND s."Name" = 'Processing'
```

**Key Insight**: The same skill works for ANY domain. Instead of hardcoding "OperationWorkflowInstance", it discovers "Product", "Order", "Invoice", "Task", etc. based on actual entity structure.

---

## Example Invocations

### Example 1: Original Request (ACMS)

```
User: "Create query to retrieve data from OperationWorkflowInstances table which is in processing stage."

Skill:
Step 1 - Discover entities:
  Found: OperationWorkflowInstance
  Properties: Id, RequestNumber, OperationId, WorkflowStageId, TenantId, IsDeleted, ...
  Relationships:
    - WorkflowStageId → WorkflowStage (FK)
    - OperationId → Operation (FK)

  Found: WorkflowStage
  Properties: Id, SystemName, DisplayName, IsActive, TenantId, IsDeleted

  Found: Operation
  Properties: Id, SystemName, DisplayName, IsActive, TenantId, IsDeleted

Step 2 - Generate query:
  SELECT
    owfi."Id",
    owfi."RequestNumber",
    owfi."WorkflowStageDate",
    ws."SystemName" as "StageSystemName",
    ws."DisplayName" as "StageDisplayName",
    o."SystemName" as "OperationName"
  FROM "OperationWorkflowInstances" owfi
  JOIN "WorkflowStages" ws ON ws."Id" = owfi."WorkflowStageId"
    AND ws."TenantId" = owfi."TenantId"
  JOIN "Operations" o ON o."Id" = owfi."OperationId"
    AND o."TenantId" = owfi."TenantId"
  WHERE owfi."TenantId" = @tenantId
    AND owfi."IsDeleted" = false
    AND ws."SystemName" ILIKE '%processing%'
  ORDER BY owfi."WorkflowStageDate" DESC;

Step 3 - Suggest parameters:
  - @tenantId (Guid): Tenant identifier
  - Optional: @maxResultCount (int), @skipCount (int) for pagination
```

### Example 2: Different Project (E-commerce)

```
User: "Get all Orders in 'Shipped' status with customer email"

Skill (in e-commerce project):
Step 1 - Discover entities:
  Found: Order
  Properties: Id, OrderNumber, StatusId, CustomerId, TotalAmount, TenantId, IsDeleted
  Relationships:
    - StatusId → OrderStatus (FK)
    - CustomerId → Customer (FK)

  Found: OrderStatus
  Properties: Id, Name, IsShipped (bool)

  Found: Customer
  Properties: Id, Email, Name, TenantId, IsDeleted

Step 2 - Generate query:
  SELECT
    o."Id",
    o."OrderNumber",
    o."TotalAmount",
    os."Name" as "Status",
    c."Email",
    c."Name" as "CustomerName"
  FROM "Orders" o
  JOIN "OrderStatuses" os ON os."Id" = o."StatusId"
  JOIN "Customers" c ON c."Id" = o."CustomerId"
  WHERE o."TenantId" = @tenantId
    AND o."IsDeleted" = false
    AND os."Name" = 'Shipped'
  ORDER BY o."CreationTime" DESC;
```

**Same skill, completely different domain!**

---

## Performance Considerations

### Discovery Caching

```
First Invocation (with new project):
  Discovery Time: ~500-2000ms (depends on entity count)
  Caching: Store SchemaModel in session/memory

Subsequent Invocations:
  Discovery Time: ~0ms (reuse cached schema)
  Query Generation Time: ~50-200ms

Cache Invalidation:
  - When project code changes (file timestamp check)
  - When DbContext modifications detected
  - Manual: /refresh-domain-discovery command
```

### Optimizing Discovery

For large projects (500+ entities):
1. **Incremental discovery**: Cache per-assembly, not per-file
2. **Parallel scanning**: Use parallel file I/O
3. **Selective re-analysis**: Only modified files since last scan
4. **Pre-compiled schemas**: For production, load from JSON snapshot

---

## Version Information

- **Generated**: 2026-03-20 (Updated for Cross-Project Portability)
- **Original**: 2025-03-20 (ACMS-specific version)
- **Target Framework**: .NET 6+ (Entity Framework Core / ABP Framework)
- **PostgreSQL**: 14+ (JSONB support required)
- **Discovery Mechanism**: Serena symbolic search tools (find_symbol, find_referencing_symbols, Explore)

---

## Change Log

### 2026-03-20 (Major Update - Cross-Project Portability)
- Added Dynamic Domain Discovery System
- Automatic entity relationship detection
- Generic query templates with placeholder substitution
- Cross-project compatibility matrix
- Performance caching strategies
- Troubleshooting guide for discovery issues
- Example usage for different project types
- Refactored ACMS-specific examples into "reference implementation" section
- Maintained backward compatibility with ACMS project

### 2025-03-20 (Initial)
- ACMS-specific PostgreSQL query generation patterns
- Entity relationship mappings for ACMS domain
- ABP Framework convention integration

## Maintenance

When updating this skill:

1. Re-analyze domain entities after significant schema changes
2. Update entity relationship mappings in new migrations
3. Refresh table/column constraints and indexes
4. Add new entity types to categorization
5. Update data type mappings if C# types change
6. Document new ABP framework conventions if adopted

---

## Quick Start for New Projects

### Initial Setup Checklist

```
□ Project Structure Validation
  - Confirm: Domain entities exist in src/{Project}.Domain/Entities/
  - Confirm: Entities inherit from recognized base class
  - Confirm: Entity classes are public and non-abstract

□ First-Time Discovery Validation
  - Invoke: "Analyze domain entities"
  - Verify: All expected entities are discovered
  - Verify: Relationships are correctly identified
  - Verify: Table names match database schema

□ Query Test
  - Generate: Simple query for one entity
  - Execute: Run against database (read-only)
  - Verify: Results match expectations
  - Adjust: If table/column names differ, check attribute overrides

□ Iteration
  - Add: More queries with joins
  - Refine: Understanding of project-specific patterns
  - Document: Any custom conventions discovered
```

### Troubleshooting Discovery Issues

| Symptom | Likely Cause | Solution |
|---------|--------------|----------|
| Entity not found | Custom directory structure | Manually specify entity location |
| Table name incorrect | Custom [Table] attribute ignored | Check naming conventions in skill config |
| FK relationships missing | Navigation properties not public | Ensure navigation properties are public |
| TenantId filter missing | Entity doesn't implement IMultiTenant | Verify base class or interface |
| Wrong column names | Property naming mismatch | Check for [Column] attributes |

---

**Note**: For the **ACMS project**, refer to the source code in `src/Amnil.AccessControlManagementSystem.Domain/Entities/` for specific entity details. For **other projects**, the skill automatically discovers and adapts to the domain structure.