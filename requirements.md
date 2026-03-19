# Requirements
## Products Project Enhancements

**Date:** 2026-03-19  
**Status:** Draft

## Suggested Enhancements

This project can be enhanced in these areas:

- Add **customer address management** so customers can save billing and shipping addresses and reuse them during ordering.
- Add **proper permissions and authorization** for product, category, customer, and order operations.
- Improve **order modeling** by linking orders to actual products instead of storing product name as plain text.
- Add **list, filter, and paging APIs** for customers and orders, similar to the existing product/category flows.
- Add **real automated tests** for application services and API behavior, since the current test suite is mostly sample scaffolding.

## Recommended First Enhancement

The highest-value enhancement is **customer address management** because the project already has customers and orders, but no reusable address workflow.

## Short Requirement

The system shall allow each customer to create, view, update, delete, and select multiple saved addresses, with support for default billing and shipping addresses during order creation.

## Minimum Implementation Scope

To support this requirement, the project should add:

- an `Address` entity linked to `Customer`
- address DTOs and an `IAddressAppService`
- CRUD endpoints for customer addresses
- default billing/shipping address logic
- validation for required address fields
- tests for address CRUD and default selection behavior
