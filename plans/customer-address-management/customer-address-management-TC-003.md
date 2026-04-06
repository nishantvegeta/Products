# customer-address-management-TC-003: Alternate flow

**Scenario:** C — Alternate flow
**Feature:** customer-address-management
**Status:** pending
**Priority:** Medium
**Type:** Functional
**Tags:** @regression

**Preconditions:**

**Steps:**
1. Given I have a saved address → selector: (discovered by explorer)
2. When I update the address details and save → selector: (discovered by explorer)
3. Then the updated information is stored and reflected in my address list → selector: (discovered by explorer)
4. And any orders that reference this address (if allowed by business rules) maintain the historical snapshot → selector: (discovered by explorer)

**Expected Result:**

**Test Data:**

**Result:**
**Error:**
**Recovery Action:**
**Screenshot:**
