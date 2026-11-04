# Open questions and implementation decisions

Resolve the first four before finalizing persistence and sale semantics. The remaining questions depend on whether the corresponding legacy feature is retained.

1. **Persistence and PostgreSQL role:** Should saved invoices be durable in PostgreSQL? If not, what data is PostgreSQL expected to own, given that product configuration is requested as browser-local? Should invoice persistence be mandatory or optional in the product?

A: yes saved invoices can be stored in postgres (keep it simple e.g. a table of pk, timestamptz, total in cents, a jsonb data column that contains free json incl. a representation of the "invoice"). the products themselves however should actually be maintained in postgres via the settings screen. the suggested denormalisation already protects against product changes affecting invoices.

2. **Donation semantics:** Should `Spende` contribute its entered amount to the sale total and cash balance, or is it intentionally informational/non-chargeable? Current code displays the amount as a line but adds zero to total and cash.

A: intention of this feature is to improve accuracy of the cash drawer tracking for large donations i.e. it should not be added to the invoice total but should affect the cash drawer.

1. **Invoice rules:** Are saved invoices required? Should they be downloadable/printable, and must they have sequential numbers or only unique identifiers? Are these receipts informal records, or must they meet local legal/fiscal requirements (VAT fields, immutable numbering, retention, correction/refund process)? No such requirements are implemented today.

A: no, these are solely for information / interest / analysis. no VAT or similar is needed since this is designed for nonprofits that are exempt from taxation and invoicing requirements.

2. **Data migration:** Are existing `%AppData%` receipt text files, invoice number state, or cash-balance value present and required on the target installation? If no real legacy installation data is needed, seed a reviewed catalog and start fresh.

A: no migration; the product catalog can be seeded with the current values at db-init but that's it.

3. **Terminal/offline model:** Is this one browser profile on one device, with network required only to save invoices, or must sales/configuration continue during network/server outages? Browser-local config does not automatically follow the operator to another device.

A: the product config is on the server. after initial loading, the app should in principle function offline. e.g. invoice saving can be implemented via a queue/cache in local storage and if the server is offline it can be uploaded later. product editing is obviously not available if offline. a PWA-like offline support s.t. the page can be loaded locally without the initial connection to fetch config is a future plan but should not be implemented yet.

4. **Cash tracking:** Should the optional drawer balance/set/subtract behavior be retained? Is an opening balance or an audit history required? In the old app the fresh-install controls cannot be enabled through the visible UI, and drawer updates are coupled to sale-line additions rather than an explicit payment.

A: an audit ledger for audit in local storage is a good addition, we can add a small view to access it. the cash drawer is a required feature and it's allowed to set the balance at any time. coupling the updates to the sale-line additions is fine for my purposes.

5. **Payment/tender:** Is `GGB` change calculation sufficient, or is any payment recording/payment-terminal integration intended? The inspected application processes no payments.

A: this is correct, all payments happen in cash. a nicer view that allows the user to select received money directly as banknotes/coins could be added in the future, but not yet.

6. **Catalog seed:** Confirm whether the 13 products/prices in `_Sell/Service/DefaultProductRegistry.cs` are still correct. They differ from the historic screenshot. Should `Sonstiges` and `Spende` remain special checkout actions?

A: the code is current, the screenshot is outdated. the special entries shall remain.

7.  **Custom item behavior:** Should custom entries have an editable description (legacy always calls them `Sonstiger Artikel`)? Should same-description items at different prices remain separate lines? Proposed behavior is to keep amounts correct and lines unambiguous.

A: keep it simple, this is an exit hatch rather than a feature we need often. description remains fixed. multiple copies with differing per-item amounts should remain separate.

8.  **Deployment/security:** Where will the Rust/PostgreSQL service run (local machine, private network, or hosted server), and is any authentication needed even for a single operator? This affects how a browser can reach the API and how invoice data is protected. No user roles or authentication exist in the legacy app.

A: this will be deployed in a k8s cluster behind a login page (poor person's WAF) with Authentik (this happens at the network level and the application is unaware). the app itself uses OIDC to connect to an Authentik instance, with the token handling in the frontend (no backend sessions needed). there are no roles, but i will configure authentik to limit access to the whole application on WAF level -> no RBAC needed in the app itself. the app should be aware of the current user's email address (we don't care about email changes) for auditing purposes.

## Confirmed non-requirements unless scope changes

No evidence supports multiple users/roles, live synchronization, inventory, customer records, card processing, external services, POS hardware, or a receipt printer. Add any of these only following an explicit decision.
