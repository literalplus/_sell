# Decisions recorded and remaining questions

## Decisions recorded

1. **Concurrency and catalog:** only one user is expected to use the app concurrently. Products are global in PostgreSQL and fetched on page load only; no live updates. Cash balance and ledger remain browser-local.
2. **Discarding a sale:** removing a line or discarding an unsaved cart appends compensating cash-ledger entries. Use clearly distinct sale actions, e.g. `Speichern` and `Verwerfen`.
3. **Offline behavior:** offline use is supported only after the app/catalog have loaded online and only while that page remains open. Offline refresh/reopen and unsaved-cart recovery are not required in the first iteration. Pending invoices remain in local storage and upload when the app next runs online.
4. **OIDC flow/configuration:** use frontend authorization code with PKCE and a stateless bearer-token API; no backend sessions. Issuer, client, audience/scopes, and redirect settings are runtime configuration. Use the token's `email` claim for audit even when `email_verified` is false; users are manually provisioned by admins.
5. **Cash balance:** initialize at €0. The balance must not go below zero; manual subtraction/set operations clamp at €0.
6. **Invoices:** store informational invoices in PostgreSQL only. No in-app invoice history, reports, or export.
7. **Quantity:** accept positive whole quantities only; reject zero and negative quantities.
8. **Offline token expiry:** continue local checkout and queueing after the access token expires, using the identity captured when the page authenticated. A fresh valid token is required to upload queued invoices.

## Remaining questions

No open decisions are currently blocking implementation. The Rust API must derive identity from validated token claims, not a client-supplied email field.

## Confirmed exclusions

No inventory, customer records, card/payment-terminal integration, receipt printer, cash denomination selection, in-app RBAC, or live synchronization is required.
