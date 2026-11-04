# UI and interaction brief

## Existing application

The main window is borderless and maximized (`_Sell/MainWindow.xaml`). It is a single-screen till:

- **Left/top:** 3×4 orange product/action tiles, with price and keyboard hotkey; four page controls sit below them. First-page special actions are `Sonstiges` (custom-priced item) and `Spende` (donation), followed by catalog products.
- **Left/bottom:** numeric display with a second line for optional cash balance; `SAVE`, `CE`, `GGB`, backspace, enter, and digit buttons. The hardware keyboard supports digit entry, Enter, Backspace, Delete/CE, period/save, Print Screen/save, arithmetic keys, and left/right page navigation. Tile hotkeys use Q/W/E, A/S/D, Y/X/C, R/F/V in row order (`_Sell/Action/GridHotKeys.cs`).
- **Right:** sale lines and total. Line text displays item, unit price, quantity, and extended price.
- **Footer:** one status message such as “Bereit!”, save status, or an error.
- **Context menus/tools:** Enter's menu contains minimize/close, Tools, and cash set/subtract. Tools opens Windows Explorer at the invoice folder or app folder (`Tools.xaml`, `Tools.xaml.cs`).

There is no separate navigation or configuration UI. The screenshot is an outdated visual snapshot; use the current code catalog as initial seed data.

## Proposed web interaction

Use a responsive checkout as the default view, optimized for a touchscreen/tablet or desktop keyboard. Authentik provides OIDC sign-in, with token handling in the frontend. Use the current user's `email` claim for audit attribution even if `email_verified` is false; users are manually provisioned by admins. There are no in-app roles. Do not copy the legacy proportions/colors unless desired by the product owner.

1. **Catalog region:** visible product tiles with clear name and formatted price; search/category filters are not evidenced as needed and should be omitted unless catalog size grows. A product tile adds one unit. Fetch product configuration at page load; do not live-sync product edits into an already-loaded checkout. Refresh/reload fetches the latest catalog.
2. **Cart region:** item name, unit price, quantity, line total, decrement/increment or quantity editor, and remove action. Mixed-price custom items must not be merged into a misleading line. Show total prominently.
3. **Custom item:** an explicit control opens a small form/dialog with fixed description `Sonstiger Artikel` and amount. Accept decimal currency input (e.g. `2.50`) with clear EUR labeling; normalize to cents. This is a web adaptation—the old app entered raw cents through a digit pad. Different amounts remain separate lines.
4. **Quantity:** use inline quantity editing or a clear quantity dialog. Do not depend on hidden “press multiply, then tile, but not Enter” state. Validate a positive integer and make cancellation leave the cart unchanged.
5. **Donation:** keep a special action. It adds a donation amount to cash-drawer tracking but does not contribute to invoice total. Clearly distinguish it from an item charge in the cart and saved invoice data.
6. **Change helper:** provide “Amount received” and “Change due”; this is for cash and is only a calculation. Handle received amount below total visibly; do not treat it as payment processing.
7. **Actions/status:** label the sale actions clearly, e.g. `Speichern` and `Verwerfen`, so save and discard cannot be confused. Discard reverses this unsaved cart's cash-ledger changes. Show invoice status as pending offline or stored on the server; preserve cart/queue on failure and provide an actionable error.
8. **Configuration:** provide a separate online settings view to maintain server-backed product name, display label, price, and order/active status. Product editing is unavailable offline; use cached catalog for checkout after initial load.
9. **Cash drawer and audit:** initialize balance to €0, show current balance, allow setting or subtracting at any time, and clamp all operations at zero. Provide a small ledger/history view sourced from browser local storage. Log user email, time, amount delta/result, and reason (sale item, donation, set, or subtract). The ledger is local to the browser profile.
10. **Offline status:** indicate connectivity loss and queued invoice count/status. After initial app/config load, allow checkout and local queueing offline only while the already-loaded page remains open, even after the access token expires; attribute local events to the identity captured at authentication. Pending invoices persist locally and upload next time the app is online with a fresh valid token. Do not implement offline page reload/PWA precaching or unsaved-cart recovery in the first iteration.
11. **Invoices:** store invoices in PostgreSQL only. Do not add an invoice history/detail screen, reports, or export.

## Accessibility and state

Buttons and dialogs should be keyboard reachable, have accessible names, and work at touch size. Maintain clear focus and announce cart-total/save-status changes to assistive technology. Cache the server catalog for the loaded session and pending invoice queue in local storage. Do not restore an unsaved active cart after reload in the first iteration.
