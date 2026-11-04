# Data model and persistence

## Conceptual model

The legacy implementation has only `Product` and `Price` as explicit model classes (`_Sell/Model/Product.cs`, `Price.cs`). Cart and invoice data are implicit in UI lists and collections (`_Sell/MainWindow.xaml.cs`). The following is a proposed web model, not an existing schema.

### Product configuration

- `Product`: stable ID, name, optional display label, price in integer minor units, display/order position, enabled flag.
- Currency is currently EUR. Keep price as integer cents. A product tile displays the optional display label when present and otherwise the name.
- Store the authoritative catalog in PostgreSQL and manage it through the settings UI. Seed it from the 13 current entries in `_Sell/Service/DefaultProductRegistry.cs` during database initialization. The browser may cache the fetched catalog to support checkout after connectivity is lost; cache is not authoritative. Product editing requires the server.

### Active cart

- `CartLine`: product ID when catalog-backed (nullable for a custom item), description/name snapshot, unit price in cents, positive integer quantity.
- `Cart`: lines and derived total. Keep the active sale in memory only; do not restore it after reload/close in the first iteration. A discard action writes compensating cash-ledger entries; a save/queued invoice keeps its drawer changes.
- Keep distinct custom items as separate lines, or merge only identical description + unit-price lines. Historical line values must not be read back from mutable product config.

### Saved invoice (PostgreSQL)

- Saved invoices are required as informational/analysis records, not statutory invoices. No VAT/tax fields or sequential human invoice numbering are required.
- Keep the PostgreSQL shape intentionally small: invoice primary key, `timestamptz`, total in integer cents, and a `jsonb` data column containing a free-form invoice representation. Include a stable invoice ID, actor email, line snapshots, quantities, chargeable totals, donation details, and other useful snapshot values in JSON. This denormalization preserves historical product names/prices.
- The browser keeps an offline queue/cache and uploads queued invoices when the server returns. Generate a stable client-side primary key before queueing and make API writes idempotent by key so retries cannot duplicate invoices. Offline storage must survive reloads after the initial app/config load.

### Cash drawer and audit ledger

- Cash tracking is required. Initialize the balance at €0. Keep current balance and an append-only audit ledger in browser local storage; expose a small view to inspect it. Suggested entry fields: stable ID, timestamp, amount delta, resulting balance or absolute-set amount, reason (`sale`, `donation`, `manual set`, `manual subtract`, `reversal`), and actor email. The drawer can be set at any time; setting/subtraction must clamp the balance at €0 so it cannot become negative. Since this is browser storage, it is best-effort history, can be cleared/modified by the browser user, and is not tamper-evident.
- Regular sale additions affect drawer balance, consistent with legacy coupling; donations affect drawer balance but are excluded from invoice total. Undo/discard creates compensating ledger entries rather than silently deleting history.
- The ledger is per browser profile/device, not shared through PostgreSQL. No inventory entity or stock movement exists or is required by current evidence.

## Persistence boundaries to decide

PostgreSQL owns products and saved invoices. Rust provides the API; React fetches the catalog on page load and keeps it for the active session. There is one concurrent user and no live sync. The pending invoice queue persists in browser local storage; after offline use it uploads when the app is next open online. The cash balance/audit ledger stays browser-local. Offline support begins only after app/catalog load and applies only while that loaded page remains open. Offline page refresh/PWA precaching and active-cart recovery are deferred.

Authentik provides OIDC. OIDC issuer/client/audience/scope/redirect settings are supplied at runtime. The frontend uses authorization code with PKCE and handles tokens; the stateless API validates bearer tokens and obtains the `email` claim for audit, regardless of `email_verified`. Users are manually provisioned by admins. No backend sessions or in-app RBAC are wanted. Local offline work can continue after token expiry using the identity captured at authentication; uploading queued invoices requires a fresh valid token. Derive identity from validated token claims, never a client-supplied email field.

## Existing data and migration

- No legacy data migration is required. Seed the new PostgreSQL product table from `_Sell/Service/DefaultProductRegistry.cs` (13 current entries) at database initialization. The owner confirmed the code is current and the screenshot is outdated.
- Legacy receipt files, invoice numbering, and cash-balance files are not to be imported. Existing receipts are plain text with formatted display strings and header rows, not clean structured records.
- No external database, POS device, printer, payment terminal, cloud API, or other integration is referenced in the app source. Existing external interaction is limited to local filesystem receipt/config files and opening Windows Explorer from Tools.
