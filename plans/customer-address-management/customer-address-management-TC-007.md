# customer-address-management-TC-007: Successful flow

**Scenario:** A — Successful flow
**Feature:** customer-address-management
**Status:** pending
**Priority:** High
**Type:** Functional
**Tags:** @smoke

**Preconditions:**

**Steps:**
1. Given I have at least one saved address in my address book → selector: (discovered by explorer)
2. When I proceed to the checkout page while creating a new order → selector: (discovered by explorer)
3. Then I see options to either select a saved billing address or enter a new one → selector: (discovered by explorer)
4. And I see options to either select a saved shipping address or enter a new one → selector: (discovered by explorer)
5. When I select a saved address for billing and/or shipping → selector: (discovered by explorer)
6. Then the order form is automatically populated with the selected address details → selector: (discovered by explorer)
7. And I can complete the order without manual address entry → selector: (discovered by explorer)

**Expected Result:**

**Test Data:**

**Result:**
**Error:**
**Recovery Action:**
**Screenshot:**
