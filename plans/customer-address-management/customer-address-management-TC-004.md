# customer-address-management-TC-004: Successful flow

**Scenario:** A — Successful flow
**Feature:** customer-address-management
**Status:** pending
**Priority:** High
**Type:** Functional
**Tags:** @smoke

**Preconditions:**

**Steps:**
1. Given I have multiple saved addresses in my address book → selector: (discovered by explorer)
2. When I select one address as "Set as Default Billing" and another (or the same) as "Set as Default Shipping" → selector: (discovered by explorer)
3. Then the system marks those addresses as the respective defaults → selector: (discovered by explorer)
4. And the billing and shipping defaults are clearly indicated in my address list → selector: (discovered by explorer)
5. And only one address can be set as default billing and only one as default shipping at any time → selector: (discovered by explorer)

**Expected Result:**

**Test Data:**

**Result:**
**Error:**
**Recovery Action:**
**Screenshot:**
