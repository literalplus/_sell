# Decisions recorded and remaining questions

## Decisions recorded

1. **Persistence and PostgreSQL:** Products are stored and edited in PostgreSQL. Informational invoices are saved in PostgreSQL using a compact record (primary key, timestamp, total cents, JSONB invoice data). The browser caches product configuration for offline checkout and queues invoices locally for later upload.
2. **Donation:** A donation is an informational line in the cart/saved invoice data, is excluded from invoice total, and increases the cash-drawer balance.
3. **Invoice purpose:** Invoices are for information/analysis only. No VAT/tax fields, statutory invoicing rules, or sequential human numbering are needed.
4. **Migration/seed:** Do not migrate legacy receipts, invoice sequence, or cash config. Seed PostgreSQL with the 13 current product entries from `_Sell/Service/DefaultProductRegistry.cs`; the source is current and the screenshot is outdated. Keep `Sonstiges` and `Spende`.
5. **Offline scope:** After the page and product configuration have loaded online, checkout should continue offline and invoices should queue locally for later upload. Product editing needs the server. First-visit offline loading/PWA-style precaching is future scope and must not be implemented now.
6. **Cash ledger:** Cash tracking is required. Allow setting the balance at any time. Keep a local-storage audit ledger and provide a small view to inspect it. Coupling cash adjustments to sale-line additions is acceptable.
7. **Payments:** Payments are cash. The app does not process/record payments; retain a change calculation. Choosing received denominations is future scope.
8. **Custom items:** Keep the fixed description `Sonstiger Artikel`; differently priced entries remain separate lines.
9. **Authentication:** Deploy in Kubernetes behind Authentik access control. The app also uses OIDC, with token handling in the frontend. No in-app roles or backend sessions. The app needs the current user's email for audit records.

## Remaining questions

1. **Number of operators/terminals:** Can different Authentik users or devices use the app, including concurrently, or is it one operator/terminal at a time? PostgreSQL products/invoices are shared, but the cash balance and audit ledger are browser-local and there is no live sync. Multiple terminals would therefore have separate local drawer balances.

A: only one concurrent user. products are global and only fetched on page load, cash balance and cash ledger remain local. concurrent use is not a relevant concern.

2. **Cash effects of an unfinished sale:** Cash balance changes occur when sale lines/donations are added, as approved. Proposed behavior is to append compensating ledger entries when an item is removed, and when an unsaved cart is cleared; after an invoice is saved or queued offline, its cash changes remain. Confirm whether clearing an unsaved cart should reverse its sale/donation entries.

A: correct; maybe we should relabel the save vs. clear-all buttons to e.g. "Speichern" and "Verwerfen"

3. **Offline page reload and draft recovery:** The spec caches the fetched catalog and pending invoices in browser storage. Is checkout expected to keep working after refreshing/reopening the browser while offline, or only while the already-loaded page remains open? If a browser closes unexpectedly with an unsaved cart, should it restore that draft? This matters because sale additions already affect the local cash ledger. First-visit offline loading remains out of scope.

A: support for refresh while offline is not necessary in the first iteration. if we add this then we will do it properly, later.

4. **OIDC/API contract:** Which Authentik issuer URL, frontend client ID/redirect URLs, audience, and scopes should be used? Proposed flow is frontend OIDC authorization code with PKCE, with the frontend sending a bearer access token and the Rust API validating it (no backend session) and deriving the email from a trusted claim. Confirm the flow, email claim name, and that the claim is present on API tokens.

A: OIDC URLs etc are configuration set at runtime. Flow is correct. 

example token:
{
    "iss": "https://auth.nowak.cloud/application/o/sell-guy/",
    "sub": "lit",
    "aud": "LfKXVdKWYzPVwT9HiKrr2hgghgUMcjDTpLEjnw1m",
    "exp": 1793832649,
    "iat": 1793830849,
    "auth_time": 1793830849,
    "acr": "goauthentik.io/providers/oauth2/default",
    "jti": "Q3EzUU8lv9zkc5u3vHHrY2UDFLDVzv1PmnmmR1ZD",
    "email": "aa@example.com",
    "email_verified": false,
    "name": "Full name",
    "preferred_username": "lit"
}

1. **Initial cash balance:** Legacy data will not be migrated. Should a fresh installation start at €0, requiring the operator to set the actual opening balance, or should setup require an opening-balance entry before checkout?

A: zero is fine, no such requirement

2. **Cash balance bounds:** Legacy manual subtraction can produce a negative balance. Preserve that behavior, or reject/clamp adjustments that would make the tracked balance negative?

A: clamp to zero

3. **Invoice history/analysis:** The UI proposal includes a simple stored-invoice list/detail view. Is that useful, or is PostgreSQL storage alone sufficient? Are CSV export or summary reports required for analysis, or should these stay out of scope?

A: only store no view; no reports or similar.

4. **Quantity validation:** The legacy allows zero/negative quantities through arithmetic paths. The rewrite proposes positive whole-number quantities only. Confirm that zero and negative quantities should be rejected.

A: yes

## Confirmed exclusions

No inventory, customer records, card/payment-terminal integration, receipt printer, or cash denomination selection is required. Do not add in-app RBAC or live synchronization unless the scope is changed.
