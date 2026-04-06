# customer-address-management-TC-014: Validation / error flow

**Scenario:** B — Validation / error flow
**Feature:** customer-address-management
**Status:** pending
**Priority:** High
**Type:** Functional
**Tags:** @regression

**Preconditions:**

**Steps:**
1. Given I am viewing an order that references a saved address → selector: (discovered by explorer)
2. When that customer's saved address has been deleted or is no longer accessible → selector: (discovered by explorer)
3. Then the system still displays the address information as it was recorded at the time of order (historical snapshot) → selector: (discovered by explorer)
4. And I can proceed with order fulfillment using the preserved address data → selector: (discovered by explorer)

**Expected Result:**

**Test Data:**

**Result:**
**Error:**
**Recovery Action:**
**Screenshot:**
