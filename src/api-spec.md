# API Specification - Customer & Order

---

## Customer Endpoints

### Create Customer
POST /api/customers

Request:
```json
{
  "name": "string",
  "email": "string",
  "phone": "string"
}
```

Response:
```json
{
  "id": "guid",
  "name": "string",
  "email": "string",
  "phone": "string",
  "isActive": true,
  "creationTime": "2024-01-01T00:00:00Z"
}
```

### Get Customer
GET /api/customers/{id}

Response:
```json
{
  "id": "guid",
  "name": "string",
  "email": "string",
  "phone": "string",
  "isActive": true,
  "creationTime": "2024-01-01T00:00:00Z"
}
```

### Update Customer
PUT /api/customers/{id}

Request:
```json
{
  "name": "string",
  "email": "string",
  "phone": "string",
  "preferredLanguage": "string"
}
```

Response:
```json
{
  "id": "guid",
  "name": "string",
  "email": "string",
  "phone": "string",
  "isActive": true,
  "lastUpdatedTime": "2024-01-02T12:00:00Z"
}
```

### Delete Customer
DELETE /api/customers/{id}

Response:
```json
{
  "success": true
}
```

### Activate Customer
POST /api/customers/{id}/activate

Response:
```json
{
  "id": "guid",
  "status": "activated"
}
```

---

## Order Endpoints

### Create Order
POST /api/orders

Request:
```json
{
  "customerId": "guid",
  "product": "string",
  "quantity": 1,
  "price": 0.0
}
```

Response:
```json
{
  "id": "guid",
  "customerId": "guid",
  "product": "string",
  "quantity": 1,
  "price": 0.0,
  "status": "pending",
  "creationTime": "2024-01-01T00:00:00Z"
}
```

### Get Order
GET /api/orders/{id}

Response:
```json
{
  "id": "guid",
  "customerId": "guid",
  "product": "string",
  "quantity": 1,
  "price": 0.0,
  "status": "pending",
  "creationTime": "2024-01-01T00:00:00Z"
}
```

### Update Order
PUT /api/orders/{id}

Request:
```json
{
  "product": "string",
  "quantity": 1,
  "price": 0.0,
  "status": "string"
}
```

Response:
```json
{
  "id": "guid",
  "customerId": "guid",
  "product": "string",
  "quantity": 1,
  "price": 0.0,
  "status": "confirmed",
  "lastUpdatedTime": "2024-01-02T12:00:00Z"
}
```

### Delete Order
DELETE /api/orders/{id}

Response:
```json
{
  "success": true
}
```

### Cancel Order (Deprecated)
POST /api/orders/{id}/cancel

Response:
```json
{
  "id": "guid",
  "status": "cancelled"
}
```

**Deprecated**: Use status update endpoint instead (`PUT /api/orders/{id}` with `status="cancelled"`).  

---

## Notes

- Both Customer and Order endpoints have full CRUD
- Includes custom actions (Customer: activate, Order: cancel)
- Includes optional/deprecated fields for testing release doc generation
- JSON blocks will map to DTOs automatically