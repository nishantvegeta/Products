# Domain Context – Access Control Management System
> Auto-generated from `AccessControlManagementSystemDbContext.cs` on 2026-03-18 03:39  
> **Do not edit manually.** Re-run `generate_domain_context.py` to refresh.

## Table of Contents
- [ABP / Infrastructure](#abp--infrastructure)
- [Application & Tasks](#application--tasks)
- [API Clients](#api-clients)
- [Workflow Engine](#workflow-engine)
- [Operations](#operations)
- [Workflow Instances](#workflow-instances)
- [Dynamic UI](#dynamic-ui)
- [Notifications](#notifications)
- [Webhooks & Events](#webhooks--events)
- [Reference / Lookup](#reference--lookup)

---
## ABP / Infrastructure

### `IdentityUser` → `IdentityUsers`
*ABP framework-managed entity — avoid direct manipulation.*


---
## Application & Tasks

### `Application` → `Applications`
**Required:** `Name`, `IsActive`
**Optional:** `TenantId`, `Description`

### `ApplicationTask` → `ApplicationTasks`
**Required:** `Name`, `IsActive`
**Optional:** `Code`, `Description`

**Relationships:**
  - *──1 Application  (FK: ApplicationId)

### `ApplicationTaskForm` → `ApplicationTaskForms`
**Optional:** `TenantId`

### `ApplicationTaskRule` → `ApplicationTaskRules`
**Required:** `ExecutionOrder`
**Optional:** `TenantId`

**Relationships:**
  - *──1 ApplicationTask  (FK: ApplicationTaskId)
  - *──1 BusinessRule  (FK: BusinessRuleId)

---
## API Clients

### `ApiClient` → `ApiClients`
**Required:** `IsActive`, `DisplayName`, `SystemName`
**Large/JSON:** `RequestConfig`, `ResponseConfig`

### `ApiClientConfig` → `ApiClientConfigs`
**Required:** `AuthorizationTypeId`

**Relationships:**
  - 1──1 ApiClient  (FK: ApiClientId)

### `ApiClientCredential` → `ApiClientCredentials`
**Required:** `SystemName`, `DisplayName`, `ApiAuthenticationTypeId`, `IsActive`
**Optional:** `Description`
**Large/JSON:** `Data`

---
## Workflow Engine

### `WorkflowDefinition` → `WorkflowDefinitions`
**Required:** `Version`, `IsActive`
**Large/JSON:** `DefinitionId`, `Definition`, `Description`

### `WorkflowStage` → `WorkflowStages`
**Required:** `SystemName`, `DisplayName`, `IsActive`, `IsOperationLevel`
**Optional:** `Description`

### `WorkflowSubStage` → `WorkflowSubStages`
**Required:** `WorkflowStageId`, `SystemName`, `DisplayName`, `IsActive`
**Optional:** `Description`

**Relationships:**
  - *──1 WorkflowStage  (FK: WorkflowStageId)

### `WorkflowEvent` → `WorkflowEvents`
**Required:** `EventName`, `IsCompleted`
**Optional:** `EventData`

**Relationships:**
  - *──1 ApplicationWorkflowInstance  (FK: ApplicationWorkflowInstanceId)
  - *──1? OperationWorkflowInstance  (FK: OperationWorkflowInstanceId)

### `OperationWorkflowConfiguration` → `OperationWorkflowConfigurations`
**Required:** `IsActive`, `IsDefault`, `ExecutionOrder`
**Optional:** `Description`, `TenantId`, `Code`, `ParentId`

**Relationships:**
  - *──1? ?  (FK: ParentId)
  - *──1 WorkflowDefinition  (FK: WorkflowDefinitionId)
  - *──1 ApplicationTask  (FK: ApplicationTaskId)

> ℹ Self-ref hierarchy: Parent (parent) → Children (children)

### `OperationWorkflowReversal` → `OperationWorkflowReversals`

**Relationships:**
  - *──1 OperationWorkflowConfiguration  (FK: OriginalOperationWorkflwConfigurationId)
  - *──1 OperationWorkflowConfiguration  (FK: ReversalOperationWorkflowConfigurationId)

---
## Operations

### `Operation` → `Operations`
**Required:** `SystemName`, `DisplayName`, `IsActive`
**Optional:** `Description`, `TenantId`

**Relationships:**
  - self──* self  (FK: ReversalOperationId)

### `OperationForm` → `OperationForms`
**Required:** `OperationId`, `DynamicFormId`, `DisplayOrder`
**Optional:** `TenantId`

### `OperationRule` → `OperationRules`
**Required:** `ExecutionOrder`
**Optional:** `TenantId`

**Relationships:**
  - *──1 Operation  (FK: OperationId)
  - *──1 BusinessRule  (FK: BusinessRuleId)

### `OperationUserAccountType` → `OperationUserAccountTypes`
**Optional:** `TenantId`

**Relationships:**
  - *──1 Operation  (FK: OperationId)
  - *──1 UserAccountType  (FK: UserAccountTypeId)

### `OperationRequestVisibility` → `OperationRequestVisibilities`
**Required:** `PermissionName`
**Optional:** `ProviderKey`, `ProviderName`

**Relationships:**
  - *──1? OperationWorkflowInstance  (FK: OperationWorkFlowInstanceId)

### `OperationEventSource` → `OperationEventSources`
**Required:** `IsDefault`
**Optional:** `TenantId`
**Large/JSON:** `EventSourceConfiguration`

**Relationships:**
  - *──1 Consumer  (FK: ConsumerId)
  - *──1 Operation  (FK: OperationId)
  - *──1 EventSource  (FK: EventSourceId)

### `OperationJob` → `OperationJobs`
**Required:** `JobNumber`, `OperationJobStatusId`, `MaxRetryCount`
**Optional:** `TenantId`

**Relationships:**
  - *──1 Consumer  (FK: ConsumerId)
  - *──1 Operation  (FK: OperationId)
  - *──1 EventSource  (FK: EventSourceId)

**Unique indexes:**
  - `(JobNumber)`

### `OperationJobInstance` → `OperationJobInstances`
**Required:** `InstanceNumber`, `OperationJobInstanceStatusId`, `RetryCount`
**Optional:** `TenantId`, `EventId`

**Relationships:**
  - *──1 OperationJob  (FK: OperationJobId)
  - *──1? OperationWorkflowInstance  (FK: OperationWorkflowInstanceId)

**Unique indexes:**
  - `(InstanceNumber)`

---
## Workflow Instances

### `OperationWorkflowInstance` → `OperationWorkflowInstances`
**Required:** `TenantId`, `RequestNumber`, `WorkflowStageDate`, `UserAccountIdentifier`, `UserAccountName`, `RequestedBy`, `StartedBy`, `StartedDate`, `RequestedDate`
**Optional:** `ParentWorkflowInstanceId`, `FormData`, `FormSchema`
**Large/JSON:** `Remarks`, `InitialData`

**Relationships:**
  - *──1 Operation  (FK: OperationId)
  - *──1 WorkflowStage  (FK: WorkflowStageId)
  - *──1? IdentityUser  (FK: WorkflowStageBy)
  - *──1? UserAccountType  (FK: UserAccountTypeId)

**Unique indexes:**
  - `(RequestNumber)`

### `OperationWorkflowInstanceTag` → `OperationWorkflowInstanceTags`
**Required:** `Key`, `Value`
**Optional:** `TenantId`

### `OperationWorkflowInstanceForm` → `OperationWorkflowInstanceForms`
**Optional:** `TenantId`

**Relationships:**
  - *──1 OperationWorkflowInstance  (FK: OperationWorkflowInstanceId)
  - *──1 DynamicForm  (FK: DynamicFormId)

### `ApplicationWorkflowInstance` → `ApplicationWorkflowInstances`
**Required:** `TenantId`, `TaskNumber`
**Optional:** `InstanceId`, `RequestedBy`, `FormData`, `FormSchema`
**Large/JSON:** `Remarks`, `InitialData`, `IntermediateData`

**Relationships:**
  - *──1 OperationWorkflowInstance  (FK: OperationWorkflowInstanceId)
  - *──1 OperationWorkflowConfiguration  (FK: OperationWorkflowConfigurationId)
  - *──1 WorkflowStage  (FK: StageId)
  - *──1? WorkflowSubStage  (FK: SubStageId)

### `ApplicationWorkflowInstanceLog` → `ApplicationWorkflowInstanceLogs`
**Optional:** `TenantId`
**Large/JSON:** `LogMessage`, `Errors`, `Remarks`

**Relationships:**
  - *──1 ApplicationWorkflowInstance  (FK: ApplicationWorkflowInstanceId)
  - *──1 WorkflowStage  (FK: StageId)
  - *──1? WorkflowSubStage  (FK: SubStageId)

### `ApplicationWorkflowInstanceForm` → `ApplicationWorkflowInstanceForms`
**Optional:** `TenantId`

**Relationships:**
  - *──1 ApplicationWorkflowInstance  (FK: ApplicationWorkflowInstanceId)
  - *──1 DynamicForm  (FK: DynamicFormId)

---
## Dynamic UI

### `DynamicForm` → `DynamicForms`
**Required:** `Name`, `FormIdentifier`, `Slug`, `IsActive`
**Optional:** `Description`
**Large/JSON:** `JsonSchema`, `UISchema`

### `DynamicFormRule` → `DynamicFormRules`
**Required:** `ValidateIfOnlyNoLoginRequired`, `ExecutionOrder`
**Optional:** `TenantId`

**Relationships:**
  - *──1 DynamicForm  (FK: DynamicFormId)
  - *──1 BusinessRule  (FK: BusinessRuleId)

### `DynamicTable` → `DynamicTables`
**Required:** `EntityName`, `TableName`, `TableIdentifier`
**Optional:** `Description`, `MenuName`, `MenuDisplayOrder`, `MenuType`, `MenuIcon`
**Large/JSON:** `ImportConfig`, `ExportConfig`

### `DynamicTableRule` → `DynamicTableRules`
**Required:** `ExecutionOrder`
**Optional:** `TenantId`

**Relationships:**
  - *──1 DynamicTable  (FK: DynamicTableId)
  - *──1 BusinessRule  (FK: BusinessRuleId)

### `DynamicMenu` → `DynamicMenus`
**Required:** `PortalTypeId`, `Name`, `DisplayName`, `DisplayOrder`, `IsActive`
**Optional:** `TenantId`, `Icon`, `Description`

### `DynamicPage` → `DynamicPages`
**Required:** `Name`, `IsActive`
**Optional:** `TenantId`, `DynamicTableId`, `Description`
**Large/JSON:** `TableSchema`, `ImportConfig`, `ExportConfig`, `JsonSchema`, `UiSchema`

### `DynamicPageMapping` → `DynamicPageMappings`
**Optional:** `TenantId`

**Unique indexes:**
  - `(DynamicMenuId, DynamicPageId)`

### `MenuCategory` → `MenuCategories`
**Required:** `SystemName`, `DisplayName`, `IsActive`
**Optional:** `TenantId`, `Description`

---
## Notifications

### `Notification` → `Notifications`
**Required:** `Title`, `NotificationMediumId`
**Optional:** `SentOn`, `RecipientId`, `IsSeen`, `SeenOn`, `NotificationRecipientTypeId`
**Large/JSON:** `Body`

### `NotificationType` → `NotificationTypes`
**Required:** `SequenceNumber`, `SystemName`, `DisplayName`

### `NotificationMedium` → `NotificationMediums`
**Required:** `SystemName`, `DisplayName`

### `NotificationMatrix` → `NotificationMatrices`
**Required:** `IsActive`

### `NotificationRecipientType` → `NotificationRecipientTypes`
**Required:** `SystemName`, `DisplayName`

---
## Webhooks & Events

### `WebhookEvent` → `WebhookEvents`
**Required:** `EventName`, `CreateNocoDBRecord`, `PublishWorkflowEvent`, `IsActive`
**Optional:** `NocoDbTableName`, `Description`
**Large/JSON:** `AllowedStagesToPublishEvent`, `WorkflowEventMetadata`

### `WebhookSubscription` → `WebhookSubscriptions`
**Required:** `IsActive`
**Optional:** `TenantId`

**Relationships:**
  - *──1 Consumer  (FK: ConsumerId)
  - *──1 ApiClient  (FK: ApiClientId)

### `WebhookLog` → `WebhookLogs`
**Required:** `ResponseStatusCode`, `AttemptedAt`, `Succeeded`, `RetryCount`, `MaxRetryCount`
**Optional:** `TenantId`
**Large/JSON:** `ResponseBody`, `Errors`

**Relationships:**
  - *──1 WebhookSubscription  (FK: WebhookSubscriptionId)

### `EventSource` → `EventSources`
**Required:** `SystemName`, `DisplayName`, `IsActive`
**Optional:** `Description`

### `Consumer` → `Consumers`
**Required:** `Name`, `Email`, `ConsumerIdentifier`, `IsActive`
**Optional:** `TenantId`, `ContactNo`, `Address`

---
## Reference / Lookup

### `UserAccountType` → `UserAccountTypes`
**Required:** `TenantId`, `SystemName`, `DisplayName`, `Description`, `IsActive`

**Relationships:**
  - *──1? DataSourceConfiguration  (FK: DataSourceConfigurationId)

### `DataSourceConfiguration` → `DataSourceConfigurations`
**Required:** `Name`, `IsActive`
**Optional:** `TenantId`, `RequestConfig`, `ResponseConfig`

**Relationships:**
  - *──1 ApiClient  (FK: ApiClientId)

### `BusinessRule` → `BusinessRules`
**Required:** `SystemName`, `DisplayName`, `ScriptTypeId`, `RuleTypeId`, `IsActive`
**Optional:** `TenantId`, `Description`
**Large/JSON:** `Content`

### `Configuration` → `Configurations`
**Required:** `Key`, `Value`, `IsActive`
**Optional:** `TenantId`, `Description`

### `Template` → `Templates`
**Required:** `SystemName`, `DisplayName`, `IsActive`
**Optional:** `Description`
**Large/JSON:** `Templates`
