# Data model and persistence

## Conceptual model

The legacy implementation has only `Product` and `Price` as explicit model classes (`_Sell/Model/Product.cs`, `Price.cs`). Cart and invoice data are implicit in UI lists and collections (`_Sell/MainWindow.xaml.cs`). The following is a proposed web model, not an existing schema.

### Product configuration

- `Product`: stable ID, name, optional display label, price in integer minor units, display/order position, enabled flag.
- Currency is currently EUR. Keep price as integer cents. A product tile displays the optional display label when present and otherwise the name.
- Persist product configuration in browser storage as requested. IndexedDB is suitable if an exportable/versioned configuration model is desired; localStorage is adequate for a tiny catalog. Keep a schema/version field so a future config update can migrate safely.

### Active cart

- `CartLine`: product ID when catalog-backed (nullable for a custom item), description/name snapshot, unit price in cents, positive integer quantity.
- `Cart`: lines and derived total. Do not persist the active sale by default; decide whether interrupted-sale recovery is needed.
- Keep distinct custom items as separate lines, or merge only identical description + unit-price lines. Historical line values must not be read back from mutable product config.

### Optional saved invoice (PostgreSQL candidate)

- `Invoice`: durable ID, created timestamp, optional human invoice number, currency, computed total in cents.
- `InvoiceLine`: invoice ID, description/name snapshot, optional product ID reference, unit price cents, quantity, extended amount cents.
- Enforce a foreign key from invoice lines to invoice; historical data should survive product rename/price changes (product reference may be null if deletion is allowed). Store a total derived from the lines or validate it transactionally on save. Avoid floating-point numeric storage.
- If the invoice number must be sequential, allocate it transactionally in PostgreSQL; legacy filename numbering is not a reliable uniqueness mechanism. Otherwise use an opaque unique ID and avoid imposing fiscal numbering requirements without owner confirmation.

### Optional cash drawer

Legacy cash is one integer balance in a config file. If retained, model it as one current balance and preferably append-only adjustment/sale events rather than silently overwriting the balance. Whether drawer state belongs in browser storage or PostgreSQL depends on terminal, recovery, and audit decisions. No inventory entity or stock movement exists or is required by current evidence.

## Persistence boundaries to decide

The desired stack includes PostgreSQL, but the requested product configuration is browser-local and saved invoices are optional. A coherent minimal arrangement is: React owns local configuration and transient cart; Rust exposes invoice endpoints backed by PostgreSQL only if invoice persistence is selected. If invoices are disabled, the required role of Rust/PostgreSQL remains unclear. Do not add sync, accounts, or multi-terminal semantics merely to justify the backend.

Clarify whether browser-local configuration is the source of truth and whether export/import/backup is needed. Browser-local data is per browser profile and is not shared across devices. If PostgreSQL must be authoritative for configuration, that changes the stated local-first boundary and needs explicit agreement.

## Existing data and migration

- The catalog exists only in source code: `_Sell/Service/DefaultProductRegistry.cs` (13 current entries). There is no product database or editable product file. Recreate reviewed seed values in the new configuration; do not import the stale `screenshot.png` contents as canonical.
- Legacy receipt files, if present on an old installation, are plain text in `%AppData%/.xxyy/_sell/rechnungen/`. Invoice numbering and cash balance are plain-text files `rechnungen.int.cfg.txt` and `cash.int.cfg.txt` in that app directory (`_Sell/App.xaml.cs:19-24, 40-89`). These files are not present in this repository.
- Receipt text embeds formatted display strings and header rows, not a clean structured line format. Migration to invoice records is possible only with a parser and review; determine whether any actual historical files must be imported. Otherwise start new persistence without legacy-history migration.
- No external database, POS device, printer, payment terminal, cloud API, or other integration is referenced in the app source. Existing external interaction is limited to local filesystem receipt/config files and opening Windows Explorer from Tools.
