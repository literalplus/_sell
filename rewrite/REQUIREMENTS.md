# Requirements and acceptance criteria

## Core workflows

### Configure and browse products

**Confirmed (legacy):** `_Sell/Service/DefaultProductRegistry.cs` defines 13 products with integer IDs, names, euro prices, and optional display labels. `_Sell/Action/ProductGrid.cs` and `ProductGridPage.cs` place actions in a 3-column by 4-row grid; the first page also contains `Sonstiges` and `Spende`, leaving three catalog products on the second page. Page navigation wraps at either end. Product buttons show a name, price, and keyboard shortcut (`_Sell/Action/GridHotKeys.cs`). The catalog is compiled into the app, not edited in a settings screen.

**Proposed (web):** provide a product configuration view for adding/editing, ordering, enabling, and pricing the small catalog; persist that configuration in this browser. In the checkout view, show products as selectable tiles, grouped across a scrollable or paged area. A checkout should continue to work without requiring a server round-trip for each tile selection.

### Build and edit a sale

**Confirmed (legacy):** selecting a regular product adds one unit. The item list groups quantities by product `Name`; each click/order operation is also retained separately. The displayed line includes name, unit price, quantity, and extended price; the total is incremented in integer minor units (`_Sell/MainWindow.xaml.cs:101-105, 154-207`). `Price` stores integer cents and formats as euro with comma decimal separator (`_Sell/Model/Price.cs`).

The keypad appends digits to a numeric display; physical number keys and numpad work too. Backspace removes the last digit, the context-menu “Auf Null setzen” resets the entry, and `CE`/Delete removes the most recently added item operation (or clears an already-empty cart) (`MainWindow.xaml:82-159`, `MainWindow.xaml.cs:455-500`). A separate clear-all handler exists but is not directly exposed as a main button.

The `×` keyboard action captures the current integer as a quantity; selecting a product then adds that quantity. The `ENTER` key while waiting for multiplication instead evaluates a number multiplication in the display, so it does not add a product. Other add/subtract/divide handlers implement display arithmetic but are not ordinary sale actions. `GGB` subtracts the current sale total from the amount entered and displays the difference; it is a calculation only, not a payment transaction (`MainWindow.xaml.cs:257-316, 532-600`).

`Sonstiges` prompts for an amount and adds a line named `Sonstiger Artikel`; the amount is interpreted as cents (e.g. `250` means €2.50), because it is passed directly to `Price(int rawValue)` (`MainWindow.xaml.cs:209-223`). `Spende` similarly prompts for an integer-cent amount (`:324-338`).

**Proposed (web):** the cart is an explicit list of lines; each line has quantity controls and a remove action, and the user can undo/remove a line without changing another line's price. The numeric entry workflow for custom items and quantity entry must clearly label units and handle cancellation. Use integer cents end-to-end, with displayed currency formatting separate from entered values. Recalculate totals from cart lines, rather than maintaining a separately mutable total.

### Complete / save a sale

**Confirmed (legacy):** `SAVE` (also `.` or Print Screen) writes `rechnung_NNNN_yyyy-MM-dd_HHmm.txt` under the per-user AppData `.../.xxyy/_sell/rechnungen/` directory, with date/time, line text, total and a thank-you footer (`MainWindow.xaml.cs:386-424`; `App.xaml.cs:19-24`). The invoice number starts from a small text config and increments after save. The in-progress list is cleared after the save attempt, including when an exception is caught. No receipt printer is invoked.

**Proposed (web):** saving a sale must be a deliberate action with a clear success/failure result. Do not clear the active cart on failed persistence. Saved invoice retention, numbering, and export/download behavior depend on decisions in [OPEN_QUESTIONS.md](OPEN_QUESTIONS.md). If invoices are persisted, store item name and unit-price snapshots so later catalog edits cannot rewrite historical sales.

## Business rules and anomalies

- **Money:** legacy prices and totals are integer euro cents. No tax/VAT, discounts, rounding policy, or fractional quantities are implemented. Use integer minor units; do not use binary floating-point for money.
- **Quantity:** regular product button adds one; multiplication mode adds the captured integer quantity when a product is selected. The legacy code does not validate quantity as positive; zero/negative values can arise via arithmetic/display paths. Proposed validation should require a positive whole quantity, subject to owner confirmation.
- **Custom item grouping defect:** every custom item is named `Sonstiger Artikel`. The old cart groups by name and overwrites the displayed unit price while summing operation totals. Adding custom items at different prices can therefore show a line extension that does not agree with the sale total. Proposed cart lines should preserve each entered price or group only when unit prices match.
- **Donation discrepancy:** donation uses `AddItemToLists(..., addToTotal: false)` (`MainWindow.xaml.cs:324-330`): it appears as a line priced at the entered amount but contributes zero to the total and does not increase cash balance. Its intended accounting meaning is unknown; do not infer it.
- **Undo/cash discrepancy:** undo subtracts the last operation's amount from total and cash, even if cash logging is disabled (`MainWindow.xaml.cs:351-379`). For custom items, the grouped line's price/quantity may become inaccurate. Donation undo uses a zero bookkeeping amount. Replace this with line-based undo and only update a cash balance according to the chosen semantics.
- **Undo crash at default setup:** cash logging is disabled when the stored balance is `-1`; in that state `_cash` is never initialized (`MainWindow.xaml.cs:107-118`). Undo nevertheless dereferences `_cash` unconditionally at `:374`, so undoing a sale can throw. The product-index map also is not adjusted after removing a row, so later cart updates can address the wrong displayed row (`:169-206, 351-379`). These are defects, not intended rules.
- **Cash drawer:** optional balance is stored as an integer in `cash.int.cfg.txt`; sales added through `AddPriceToList` increase it, and context-menu actions set an absolute amount or subtract a typed amount (`MainWindow.xaml.cs:107-138, 225-255, 579-587`; `App.xaml.cs:65-89, 117-139`). There are no payment methods, tender records, reconciliation, or cash validation. On a fresh install, both cash controls are disabled and only become enabled when a saved balance is not `-1`; the UI has no visible action to enable/set the initial balance. Treat drawer tracking as optional and clarify whether it is wanted.
- **Amount received/change:** `GGB` computes entered amount minus total and can show a negative result; it neither validates tender nor records payment. It should not be described as integrated payment processing.
- **Receipt defects:** invoice text generation writes the UI's header/separator rows as if they were sale lines (`MainWindow.xaml.cs:401-408`). The file name has minute precision and may collide/overwrite; there is no collision handling. A list-length mismatch returns before closing the file, while the method still reaches clear-all. These are implementation defects, not target requirements.
- **Catalog evidence conflict:** the source registry is the reliable current catalog, but `screenshot.png` shows different names/prices (for example, screenshot prices for some items do not match the registry). Seed data should be reviewed by the owner instead of treating the screenshot as authoritative.

## Acceptance criteria for the web core

1. The operator can configure a product's name, price in cents, and optional display name, and those settings survive a reload in the same browser.
2. The operator can select products quickly, enter a positive whole quantity, add custom-priced items, and see line extensions and the sale total recomputed exactly in cents.
3. Removing/undoing an item changes only the intended cart line and leaves every displayed line extension equal to its quantity × unit price. The displayed total equals the sum of line extensions, subject to the confirmed donation decision.
4. The operator can clear a cart and can distinguish clearing from saving. Invalid, empty, overflowing, or cancelled numeric input cannot silently create a malformed line.
5. The operator can calculate change due from an amount tendered, if retaining the legacy `GGB` helper; the result is labelled as a calculation and does not imply that a payment was processed.
6. When invoice persistence is selected, successful save produces a durable invoice with line snapshots, exact total, and an unambiguous identifier; failed save preserves the cart and reports an actionable error.
7. The app is usable by one operator on one browser/device and does not depend on concurrent users, live updates, inventory, external payment, or POS hardware unless explicitly added by a decision below.
