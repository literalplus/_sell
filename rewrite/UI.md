# UI and interaction brief

## Existing application

The main window is borderless and maximized (`_Sell/MainWindow.xaml`). It is a single-screen till:

- **Left/top:** 3×4 orange product/action tiles, with price and keyboard hotkey; four page controls sit below them. First-page special actions are `Sonstiges` (custom-priced item) and `Spende` (donation), followed by catalog products.
- **Left/bottom:** numeric display with a second line for optional cash balance; `SAVE`, `CE`, `GGB`, backspace, enter, and digit buttons. The hardware keyboard supports digit entry, Enter, Backspace, Delete/CE, period/save, Print Screen/save, arithmetic keys, and left/right page navigation. Tile hotkeys use Q/W/E, A/S/D, Y/X/C, R/F/V in row order (`_Sell/Action/GridHotKeys.cs`).
- **Right:** sale lines and total. Line text displays item, unit price, quantity, and extended price.
- **Footer:** one status message such as “Bereit!”, save status, or an error.
- **Context menus/tools:** Enter's menu contains minimize/close, Tools, and cash set/subtract. Tools opens Windows Explorer at the invoice folder or app folder (`Tools.xaml`, `Tools.xaml.cs`).

There is no separate navigation or configuration UI. The screenshot is only a historic visual snapshot and conflicts with the current source catalog.

## Proposed web interaction

Use a responsive, single-page checkout as the default route, optimized for a touchscreen/tablet or desktop keyboard. Do not copy the legacy proportions/colors unless desired by the product owner.

1. **Catalog region:** visible product tiles with clear name and formatted price; search/category filters are not evidenced as needed and should be omitted unless catalog size grows. A product tile adds one unit. Keep paging or scrolling predictable and retain keyboard shortcuts only if they do not interfere with normal browser typing/accessibility.
2. **Cart region:** item name, unit price, quantity, line total, decrement/increment or quantity editor, and remove action. Mixed-price custom items must not be merged into a misleading line. Show total prominently.
3. **Custom item:** an explicit control opens a small form/dialog with description and amount. Accept decimal currency input (e.g. `2.50`) with clear EUR labeling; normalize to cents. This is a web adaptation—the old app entered raw cents through a digit pad.
4. **Quantity:** use inline quantity editing or a clear quantity dialog. Do not depend on hidden “press multiply, then tile, but not Enter” state. Validate a positive integer and make cancellation leave the cart unchanged.
5. **Change helper:** if retained, provide “Amount received” and “Change due” with explicit non-payment wording. Handle received amount below total visibly; do not mutate the cart or a drawer balance.
6. **Actions/status:** expose Clear cart and Save invoice as distinct actions. Confirm destructive clear if an accidental tap is plausible. Display save errors without clearing cart; provide a success confirmation and, if supported, invoice details/download.
7. **Configuration:** provide a separate settings view or dialog to maintain product name, display label, price, and order/active status. Explain that browser-local configuration belongs to that browser/device; a cleared browser profile may lose it unless an export/backup option is chosen.
8. **Cash drawer:** only show drawer balance and set/subtract tools if the owner confirms this legacy feature should remain. Put them in a clear, accessible settings/admin section rather than a hidden right-click menu.

## Accessibility and state

Buttons and dialogs should be keyboard reachable, have accessible names, and work at touch size. Maintain clear focus and announce cart-total/save-status changes to assistive technology. Persist configuration independently of the transient active cart unless the owner specifically requests draft recovery. Do not silently restore an old cart into a new sale.
