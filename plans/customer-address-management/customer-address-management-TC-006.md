# customer-address-management-TC-006: Alternate flow

**Scenario:** C — Alternate flow
**Feature:** customer-address-management
**Status:** pending
**Priority:** Medium
**Type:** Functional
**Tags:** @regression

**Preconditions:**

**Steps:**
1. Given I have an address currently set as my default billing address → selector: (discovered by explorer)
2. When I select a different address as the new default billing → selector: (discovered by explorer)
3. Then the previous default billing address loses its default status → selector: (discovered by explorer)
4. And only the newly selected address is marked as default billing → selector: (discovered by explorer)

**Expected Result:**

**Test Data:**

**Result:**
**Error:**
**Recovery Action:**
**Screenshot:**
