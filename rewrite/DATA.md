# Data model and persistence

## Conceptual model

The legacy implementation has only `Product` and `Price` as explicit model classes (`_Sell/Model/Product.cs`, `Price.cs`). Cart and invoice data are implicit in UI lists and collections (`_Sell/MainWindow.xaml.cs`). The following is a proposed web model, not an existing schema.

### Product configuration

- `Product`: stable ID, name, optional display label, price in integer minor units, display/order position, enabled flag.
- Currency is currently EUR. Keep price as integer cents. A product tile displays the optional display label when present and otherwise the name.
- Store the authoritative catalog in PostgreSQL and manage it through the settings UI. Seed it from the 13 current entries in `_Sell/Service/DefaultProductRegistry.cs` during database initialization. The browser may cache the fetched catalog to support checkout after connectivity is lost; cache is not authoritative. Product editing requires the server.

### Active cart

- `CartLine`: product ID when catalog-backed (nullable for a custom item), description/name snapshot, unit price in cents, positive integer quantity.
- `Cart`: lines and derived total. Proposed default is not to persist an active sale; offline reload/recovery behavior remains to be confirmed in [OPEN_QUESTIONS.md](OPEN_QUESTIONS.md).
- Keep distinct custom items as separate lines, or merge only identical description + unit-price lines. Historical line values must not be read back from mutable product config.

### Saved invoice (PostgreSQL)

- Saved invoices are required as informational/analysis records, not statutory invoices. No VAT/tax fields or sequential human invoice numbering are required.
- Keep the PostgreSQL shape intentionally small: invoice primary key, `timestamptz`, total in integer cents, and a `jsonb` data column containing a free-form invoice representation. Include a stable invoice ID, actor email, line snapshots, quantities, chargeable totals, donation details, and other useful snapshot values in JSON. This denormalization preserves historical product names/prices.
- The browser keeps an offline queue/cache and uploads queued invoices when the server returns. Generate a stable client-side primary key before queueing and make API writes idempotent by key so retries cannot duplicate invoices. Offline storage must survive reloads after the initial app/config load.

### Cash drawer and audit ledger

- Cash tracking is required. Keep current balance and an append-only audit ledger in browser local storage; expose a small view to inspect it. Suggested entry fields: stable ID, timestamp, amount delta, resulting balance or absolute-set amount, reason (`sale`, `donation`, `manual set`, `manual subtract`), and actor email. The drawer can be set to an absolute balance at any time. Since this is browser storage, it is best-effort history, can be cleared/modified by the browser user, and is not tamper-evident.
- Regular sale additions affect drawer balance, consistent with legacy coupling; donations affect drawer balance but are excluded from invoice total. Undo/clear behavior must create compensating ledger entries rather than silently deleting history. Exact timing/clear semantics remain open below.
- The ledger is per browser profile/device, not shared through PostgreSQL. No inventory entity or stock movement exists or is required by current evidence.

## Persistence boundaries to decide

PostgreSQL owns products and saved invoices. Rust provides the API; React caches the catalog and pending invoice queue locally for offline checkout. The cash balance/audit ledger stays browser-local. Offline support begins only after the app and product configuration have first loaded successfully online; first-visit offline page loading/PWA precaching is deferred.

Authentik provides OIDC. The frontend handles tokens and obtains the current user's email for audit. No backend sessions or in-app RBAC are wanted. The API still needs a defined way to authenticate/validate frontend requests; see [OPEN_QUESTIONS.md](OPEN_QUESTIONS.md).

## Existing data and migration

- No legacy data migration is required. Seed the new PostgreSQL product table from `_Sell/Service/DefaultProductRegistry.cs` (13 current entries) at database initialization. The owner confirmed the code is current and the screenshot is outdated.
- Legacy receipt files, invoice numbering, and cash-balance files are not to be imported. Existing receipts are plain text with formatted display strings and header rows, not clean structured records.
- No external database, POS device, printer, payment terminal, cloud API, or other integration is referenced in the app source. Existing external interaction is limited to local filesystem receipt/config files and opening Windows Explorer from Tools.
