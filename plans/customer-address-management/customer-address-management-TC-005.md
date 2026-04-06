# customer-address-management-TC-005: Validation / error flow

**Scenario:** B — Validation / error flow
**Feature:** customer-address-management
**Status:** pending
**Priority:** High
**Type:** Functional
**Tags:** @regression

**Preconditions:**

**Steps:**
1. Given I have only one saved address → selector: (discovered by explorer)
2. When I attempt to delete that address while it is still set as my default billing or shipping address → selector: (discovered by explorer)
3. Then the system prevents deletion or prompts me to first select a different default address → selector: (discovered by explorer)
4. And my address remains unchanged → selector: (discovered by explorer)

**Expected Result:**

**Test Data:**

**Result:**
**Error:**
**Recovery Action:**
**Screenshot:**
