# POS rewrite specification

This folder captures the behavior of the existing `_Sell` Windows/WPF point-of-sale app and a small, web-oriented implementation brief for a Rust + React + PostgreSQL rewrite. It is written for an implementing agent; it is not a design mandate to reproduce the old code or screen layout.

## Documents

- [REQUIREMENTS.md](REQUIREMENTS.md) — workflows, observed rules/quirks, and acceptance criteria.
- [UI.md](UI.md) — existing interaction model and proposed browser interactions.
- [DATA.md](DATA.md) — conceptual entities, persistence boundaries, and legacy data.
- [OPEN_QUESTIONS.md](OPEN_QUESTIONS.md) — decisions that materially affect implementation.

## Purpose and scope

The confirmed application is a small till for selecting configured items, totaling an in-progress sale, tracking a cash-drawer balance, and saving a plain-text receipt. The rewrite is for nonprofit use: no VAT/tax or formal invoicing rules are required. There is no customer/account workflow, inventory, card processing, or POS hardware integration in the inspected code.

The rewrite should preserve quick sale entry while adapting it for browser use. Use a Rust backend, React frontend, and PostgreSQL. Products are managed in PostgreSQL through a settings screen and seeded from the current code catalog at database initialization. Completed informational invoices are saved to PostgreSQL; after connectivity is lost, the already-loaded app queues invoices locally and uploads them when connectivity returns. Cash drawer state and its audit ledger are local to the browser. Authentik provides OIDC authentication; the frontend handles tokens, and the app uses the current user's email claim for audit even when `email_verified` is false. There are no in-app roles or live synchronization, and only one user is expected to use the app at a time. Expired tokens do not block offline checkout; queued invoices require a fresh valid token to upload. Offline refresh/page recovery is deferred; offline use is only promised while the app loaded online remains open. See [OPEN_QUESTIONS.md](OPEN_QUESTIONS.md) for recorded decisions.

## Evidence labels

- **Confirmed (legacy):** behavior directly visible in the checked-in source.
- **Proposed (web):** implementation-oriented adaptation, not a claim about the old app.
- **Needs decision:** an ambiguity, implementation choice, or apparent legacy defect that should not be guessed silently.

Useful source references are relative to the repository root. The primary logic is in `_Sell/MainWindow.xaml.cs`, the catalog in `_Sell/Service/DefaultProductRegistry.cs`, money in `_Sell/Model/Price.cs`, layout in `_Sell/MainWindow.xaml`, and file persistence in `_Sell/App.xaml.cs`.
