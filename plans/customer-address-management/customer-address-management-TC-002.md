# customer-address-management-TC-002: Validation / error flow

**Scenario:** B — Validation / error flow
**Feature:** customer-address-management
**Status:** pending
**Priority:** High
**Type:** Functional
**Tags:** @regression

**Preconditions:**

**Steps:**
1. Given I am adding a new address → selector: (discovered by explorer)
2. When I submit the address form with missing required fields (street, city, state, postal code, or country) → selector: (discovered by explorer)
3. Then the system displays validation errors and prevents the address from being saved → selector: (discovered by explorer)
4. And I remain on the form to correct the errors → selector: (discovered by explorer)

**Expected Result:**

**Test Data:**

**Result:**
**Error:**
**Recovery Action:**
**Screenshot:**
