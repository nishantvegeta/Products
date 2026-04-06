# customer-address-management-TC-008: Validation / error flow

**Scenario:** B — Validation / error flow
**Feature:** customer-address-management
**Status:** pending
**Priority:** High
**Type:** Functional
**Tags:** @regression

**Preconditions:**

**Steps:**
1. Given I am placing an order and select a saved address → selector: (discovered by explorer)
2. When I proceed to confirm the order → selector: (discovered by explorer)
3. Then the system validates that both billing and shipping addresses are provided → selector: (discovered by explorer)
4. If I have no default addresses set, the system prompts me to select addresses manually → selector: (discovered by explorer)
5. And I cannot complete the order until valid addresses are provided → selector: (discovered by explorer)

**Expected Result:**

**Test Data:**

**Result:**
**Error:**
**Recovery Action:**
**Screenshot:**
