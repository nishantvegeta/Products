# API Specification
## Customer Address Management

---

**Version:** 1.0
**Date:** 2026-03-19
**Status:** Draft
**Source:** requirements.md
**Base URL:** /api/app
**Auth:** Bearer JWT — required on all endpoints unless marked public

---

## Overview

This API specification defines the customer address management endpoints that allow customers to create, view, update, delete, and select multiple saved addresses with support for default billing and shipping addresses during order creation. The specification is based on the requirement that "the system shall allow each customer to create, view, update, delete, and select multiple saved addresses, with support for default billing and shipping addresses during order creation."

---

## Endpoints Summary

| Method | Path | Description | Auth | Story |
|---|---|---|---|---|
| POST | /api/app/addresses | Create a new address for the customer | Required | US-001 |
| GET | /api/app/addresses | List addresses for customer | Required | US-004 |
| GET | /api/app/addresses/{id} | Get address by ID | Required | US-005 |
| PUT | /api/app/addresses/{id} | Update an existing address | Required | US-002 |
| DELETE | /api/app/addresses/{id} | Delete an address | Required | US-003 |
| POST | /api/app/orders | Create an order with selected addresses | Required | US-006 |

---

## Endpoints

---

### POST /api/app/addresses

**Description:** Create a new address for the authenticated customer
**User Story:** US-001 — As a customer, I want to create and save multiple addresses so that I can reuse them for future orders without re-entering information
**Permission:** `Address.Create` — Customer only

**Request Body:**

| Field | Type | Required | Description | Constraints |
|---|---|---|---|---|
| name | string | Yes | Address name/label | Max 100 chars |
| addressLine1 | string | Yes | First address line | Max 200 chars |
| addressLine2 | string | No | Second address line | Max 200 chars |
| city | string | Yes | City name | Max 100 chars |
| state | string | Yes | State/province | Max 100 chars |
| country | string | Yes | Country name | Max 100 chars |
| postalCode | string | Yes | ZIP/Postal code | Max 20 chars |
| phone | string | Yes | Contact phone | Max 20 chars |
| isDefaultBilling | boolean | No | Mark as default billing | Defaults to false |
| isDefaultShipping | boolean | No | Mark as default shipping | Defaults to false |

**Example Request:**
```json
{
  "name": "Home",
  "addressLine1": "123 Main St",
  "addressLine2": "Apt 4B",
  "city": "New York",
  "state": "NY",
  "country": "USA",
  "postalCode": "10001",
  "phone": "+1-555-0123",
  "isDefaultBilling": true,
  "isDefaultShipping": true
}
```

**Response: 200 OK**

| Field | Type | Description |
|---|---|---|
| id | string (uuid) | Generated identifier |
| customerId | string (uuid) | Customer identifier |
| name | string | Address name/label |
| addressLine1 | string | First address line |
| addressLine2 | string | Second address line |
| city | string | City name |
| state | string | State/province |
| country | string | Country name |
| postalCode | string | ZIP/Postal code |
| phone | string | Contact phone |
| isDefaultBilling | boolean | Default billing flag |
| isDefaultShipping | boolean | Default shipping flag |
| isActive | boolean | Always true on creation |
| createdAt | string (date-time) | Creation timestamp |

**Example Response:**
```json
{
  "success": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Home",
    "addressLine1": "123 Main St",
    "addressLine2": "Apt 4B",
    "city": "New York",
    "state": "NY",
    "country": "USA",
    "postalCode": "10001",
    "phone": "+1-555-0123",
    "isDefaultBilling": true,
    "isDefaultShipping": true,
    "isActive": true,
    "createdAt": "2026-03-19T10:00:00Z"
  }
}
```

**Error Responses:**

| Status | Reason |
|---|---|
| 400 | Validation failed — missing required field or invalid format |
| 400 | Address name already exists for this customer |
| 401 | Missing or invalid auth token |
| 403 | Caller does not have Address.Create permission |
| 403 | Customer ID does not match authenticated user |

---

### GET /api/app/addresses

**Description:** Returns a paginated, filterable list of addresses for the authenticated customer
**User Story:** US-004 — As a customer, I want to list all my saved addresses so that I can see which addresses I have stored
**Permission:** `Address.Default` — Customer only

**Query Parameters:**

| Parameter | Type | Required | Default | Description |
|---|---|---|---|---|
| filter | string | No | null | Search by name or address line (contains) |
| isActive | boolean | No | true | Filter by active status. null = all |
| maxResultCount | integer | No | 10 | Max records per page (max: 100) |
| skipCount | integer | No | 0 | Number of records to skip (pagination) |
| sorting | string | No | "name asc" | Sort field and direction |

**Example Request:**
```
GET /api/app/addresses?filter=home&maxResultCount=20&skipCount=0
```

**Response: 200 OK**

| Field | Type | Description |
|---|---|---|
| totalCount | integer | Total matching records |
| items | array | Array of address objects |

**Example Response:**
```json
{
  "success": true,
  "data": {
    "totalCount": 2,
    "items": [
      {
        "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        "name": "Home",
        "addressLine1": "123 Main St",
        "addressLine2": "Apt 4B",
        "city": "New York",
        "state": "NY",
        "country": "USA",
        "postalCode": "10001",
        "phone": "+1-555-0123",
        "isDefaultBilling": true,
        "isDefaultShipping": true,
        "isActive": true,
        "createdAt": "2026-03-19T10:00:00Z"
      }
    ]
  }
}
```

**Error Responses:**

| Status | Reason |
|---|---|
| 401 | Missing or invalid auth token |
| 403 | Insufficient permissions |

---

### GET /api/app/addresses/{id}

**Description:** Get a specific address by ID
**User Story:** US-005 — As a customer, I want to view the full details of a specific saved address so that I can verify it before selecting it for an order
**Permission:** `Address.Default` — Customer only

**Path Parameters:**

| Parameter | Type | Description |
|---|---|---|
| id | string (uuid) | The address identifier |

**Response: 200 OK**

| Field | Type | Description |
|---|---|---|
| id | string (uuid) | Identifier |
| customerId | string (uuid) | Customer identifier |
| name | string | Address name/label |
| addressLine1 | string | First address line |
| addressLine2 | string | Second address line |
| city | string | City name |
| state | string | State/province |
| country | string | Country name |
| postalCode | string | ZIP/Postal code |
| phone | string | Contact phone |
| isDefaultBilling | boolean | Default billing flag |
| isDefaultShipping | boolean | Default shipping flag |
| isActive | boolean | Whether address is active |
| createdAt | string (date-time) | Creation timestamp |

**Example Response:**
```json
{
  "success": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Home",
    "addressLine1": "123 Main St",
    "addressLine2": "Apt 4B",
    "city": "New York",
    "state": "NY",
    "country": "USA",
    "postalCode": "10001",
    "phone": "+1-555-0123",
    "isDefaultBilling": true,
    "isDefaultShipping": true,
    "isActive": true,
    "createdAt": "2026-03-19T10:00:00Z"
  }
}
```

**Error Responses:**

| Status | Reason |
|---|---|
| 401 | Missing or invalid auth token |
| 403 | Insufficient permissions |
| 404 | Address with given id not found |
| 403 | Address ID does not belong to authenticated user |

---

### PUT /api/app/addresses/{id}

**Description:** Updates an existing address
**User Story:** US-002 — As a customer, I want to update my saved addresses so that I can keep my information current and correct
**Permission:** `Address.Edit` — Customer only

**Path Parameters:**

| Parameter | Type | Description |
|---|---|---|
| id | string (uuid) | The address identifier |

**Request Body:** *(all fields optional — only provided fields are updated)*

| Field | Type | Required | Description | Constraints |
|---|---|---|---|---|
| name | string | No | Address name/label | Max 100 chars |
| addressLine1 | string | No | First address line | Max 200 chars |
| addressLine2 | string | No | Second address line | Max 200 chars |
| city | string | No | City name | Max 100 chars |
| state | string | No | State/province | Max 100 chars |
| country | string | No | Country name | Max 100 chars |
| postalCode | string | No | ZIP/Postal code | Max 20 chars |
| phone | string | No | Contact phone | Max 20 chars |
| isDefaultBilling | boolean | No | Mark as default billing | |
| isDefaultShipping | boolean | No | Mark as default shipping | |

**Example Request:**
```json
{
  "addressLine1": "456 Elm St",
  "city": "Brooklyn",
  "isDefaultShipping": false
}
```

**Response: 200 OK**
*(Same shape as GET by ID response)*

**Error Responses:**

| Status | Reason |
|---|---|
| 400 | Validation failed |
| 400 | Address name already exists for this customer |
| 401 | Missing or invalid auth token |
| 403 | Insufficient permissions |
| 404 | Address not found |
| 403 | Address ID does not belong to authenticated user |

---

### DELETE /api/app/addresses/{id}

**Description:** Delete an address (soft delete)
**User Story:** US-003 — As a customer, I want to delete my saved addresses so that I can remove outdated or incorrect addresses
**Permission:** `Address.Delete` — Customer only

**Path Parameters:**

| Parameter | Type | Description |
|---|---|---|
| id | string (uuid) | The address identifier |

**Response: 200 OK**

```json
{
  "success": true,
  "data": true
}
```

**Error Responses:**

| Status | Reason |
|---|---|
| 401 | Missing or invalid auth token |
| 403 | Insufficient permissions |
| 403 | Address ID does not belong to authenticated user |
| 404 | Address not found |

---

### POST /api/app/orders

**Description:** Create an order with selected billing and shipping addresses from saved addresses
**User Story:** US-006 — As a customer, I want to select from my saved billing and shipping addresses when creating an order so that I can quickly checkout without re-entering address details
**Permission:** `Order.Create` — Customer only

**Request Body:**

| Field | Type | Required | Description | Constraints |
|---|---|---|---|---|
| customerId | string (uuid) | Yes | The customer identifier | Must match authenticated user |
| billingAddressId | string (uuid) | No | Address ID for billing | Must belong to customer |
| shippingAddressId | string (uuid) | No | Address ID for shipping | Must belong to customer |
| product | string | Yes | Product name | Max 200 chars |
| quantity | integer | Yes | Quantity | Must be > 0 |
| price | decimal | Yes | Unit price | Must be >= 0 |

**Notes:**
- If `billingAddressId` is not provided, the system will use the customer's default billing address (if set).
- If `shippingAddressId` is not provided, the system will use the customer's default shipping address (if set).
- If neither default is set, the order creation will fail with a validation error.
- The product reference is currently stored as plain text; future enhancement will link to Product entity.

**Example Request:**
```json
{
  "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "billingAddressId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "shippingAddressId": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
  "product": "Laptop",
  "quantity": 1,
  "price": 999.99
}
```

**Response: 200 OK**

| Field | Type | Description |
|---|---|---|
| id | string (uuid) | Order identifier |
| orderNumber | string | Unique order number |
| customerId | string (uuid) | Customer identifier |
| billingAddress | AddressDto | Full billing address details |
| shippingAddress | AddressDto | Full shipping address details |
| product | string | Product name |
| quantity | integer | Quantity |
| price | decimal | Unit price |
| totalAmount | decimal | quantity * price |
| status | string | Order status (e.g., "Pending") |
| createdAt | string (date-time) | Creation timestamp |

**Example Response:**
```json
{
  "success": true,
  "data": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa8",
    "orderNumber": "ORD-20260319-001",
    "customerId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "billingAddress": {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "name": "Home",
      "addressLine1": "123 Main St",
      "city": "New York",
      "state": "NY",
      "country": "USA",
      "postalCode": "10001",
      "phone": "+1-555-0123"
    },
    "shippingAddress": {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
      "name": "Office",
      "addressLine1": "789 Broadway",
      "city": "New York",
      "state": "NY",
      "country": "USA",
      "postalCode": "10002",
      "phone": "+1-555-0124"
    },
    "product": "Laptop",
    "quantity": 1,
    "price": 999.99,
    "totalAmount": 999.99,
    "status": "Pending",
    "createdAt": "2026-03-19T10:30:00Z"
  }
}
```

**Error Responses:**

| Status | Reason |
|---|---|
| 400 | Validation failed — missing required fields |
| 400 | Billing address not found or does not belong to customer |
| 400 | Shipping address not found or does not belong to customer |
| 400 | No billing address selected and no default set |
| 400 | No shipping address selected and no default set |
| 401 | Missing or invalid auth token |
| 403 | Caller does not have Order.Create permission |
| 403 | Customer ID does not match authenticated user |

---

## Permissions Reference

| Permission | Who Has It | Endpoints |
|---|---|---|
| Address.Default | Customer only | GET list, GET by id |
| Address.Create | Customer only | POST |
| Address.Edit | Customer only | PUT |
| Address.Delete | Customer only | DELETE |
| Order.Create | Customer only | POST |

---

## Field Validation Rules

### Address Fields

| Field | Rule | Error Message |
|---|---|---|
| name | Required (POST), Optional (PUT), max 100 chars | "Address name is required" / "Address name cannot exceed 100 characters" |
| addressLine1 | Required (POST), Optional (PUT), max 200 chars | "Address line 1 is required" / "Address line 1 cannot exceed 200 characters" |
| city | Required (POST), Optional (PUT), max 100 chars | "City is required" / "City cannot exceed 100 characters" |
| state | Required (POST), Optional (PUT), max 100 chars | "State is required" / "State cannot exceed 100 characters" |
| country | Required (POST), Optional (PUT), max 100 chars | "Country is required" / "Country cannot exceed 100 characters" |
| postalCode | Required (POST), Optional (PUT), max 20 chars | "Postal code is required" / "Postal code cannot exceed 20 characters" |
| phone | Required (POST), Optional (PUT), max 20 chars | "Phone is required" / "Phone cannot exceed 20 characters" |

### Order Fields

| Field | Rule | Error Message |
|---|---|---|
| customerId | Required, valid UUID | "Customer ID is required" |
| product | Required, max 200 chars | "Product is required" / "Product cannot exceed 200 characters" |
| quantity | Required, > 0 | "Quantity must be greater than zero" |
| price | Required, >= 0 | "Price must be non-negative" |
| billingAddressId | Optional, but required if no default billing | "Billing address required — select or set a default" |
| shippingAddressId | Optional, but required if no default shipping | "Shipping address required — select or set a default" |

---

## Business Rules Reflected in API

| Rule | Enforced At |
|---|---|
| Only one default billing address per customer | POST and PUT on addresses — sets others to false if true |
| Only one default shipping address per customer | POST and PUT on addresses — sets others to false if true |
| Customer can only manage their own addresses | All address endpoints — 403 if address belongs to another customer |
| Cannot delete if address is currently used in an active order | DELETE on address — 400 if address is referenced |
| If explicit address IDs are provided in order, they must belong to the customer | POST /orders — 400 if not |
| If explicit billingAddressId not provided in order, use customer's default billing address | POST /orders — 400 if no default |
| If explicit shippingAddressId not provided in order, use customer's default shipping address | POST /orders — 400 if no default |
| Addresses used in order should be immutable — copy data into order | POST /orders — address data copied at time of order |

---

## Open Questions

| # | Question | Affects |
|---|---|---|
| 1 | Should addresses be soft-deleted or hard-deleted? | DELETE endpoint behavior, Order reference validation |
| 2 | What is the maximum number of addresses per customer? | POST validation |
| 3 | Should we validate address format per country? | POST/PUT validation |
| 4 | Should the order creation endpoint support multiple products (order items)? | Future enhancement scope |
| 5 | How should the order status be initialized? | Business rule for order lifecycle |
| 6 | Should address changes after order placement affect existing orders? | Business rule confirms copy |

---

*Generated by user-stories-to-api-spec skill*
*Source: requirements.md*
*Feed this file into the api-spec-to-technical-plan skill*