# customer-address-management-TC-013: Successful flow

**Scenario:** A — Successful flow
**Feature:** customer-address-management
**Status:** pending
**Priority:** High
**Type:** Functional
**Tags:** @smoke

**Preconditions:**

**Steps:**
1. Given I am logged in as an order processor → selector: (discovered by explorer)
2. When I open an order in the order management system → selector: (discovered by explorer)
3. Then I see the billing address and shipping address selected by the customer → selector: (discovered by explorer)
4. When the customer used a saved address, I see the full address details from their address book → selector: (discovered by explorer)
5. When the customer entered a one-time address, I see those manually-entered details → selector: (discovered by explorer)
6. And I can verify the addresses for shipping and billing purposes → selector: (discovered by explorer)

**Expected Result:**

**Test Data:**

**Result:**
**Error:**
**Recovery Action:**
**Screenshot:**
