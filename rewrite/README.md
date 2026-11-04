# POS rewrite specification

This folder captures the behavior of the existing `_Sell` Windows/WPF point-of-sale app and a small, web-oriented implementation brief for a Rust + React + PostgreSQL rewrite. It is written for an implementing agent; it is not a design mandate to reproduce the old code or screen layout.

## Documents

- [REQUIREMENTS.md](REQUIREMENTS.md) — workflows, observed rules/quirks, and acceptance criteria.
- [UI.md](UI.md) — existing interaction model and proposed browser interactions.
- [DATA.md](DATA.md) — conceptual entities, persistence boundaries, and legacy data.
- [OPEN_QUESTIONS.md](OPEN_QUESTIONS.md) — decisions that materially affect implementation.

## Purpose and scope

The confirmed application is a small single-operator till for selecting configured items, totaling an in-progress sale, optionally tracking a cash-drawer balance, and saving a plain-text receipt. There is no customer/account workflow, inventory, tax calculation, card processing, or multi-user collaboration in the inspected code.

The rewrite should preserve the quick sale-entry use case while adapting it for browser use. The desired stack is Rust backend, React frontend, and PostgreSQL. Product configuration is expected to persist in the browser; persistence of completed invoices is still a decision. Keep the application single-user and without live synchronization unless the owner decides otherwise.

## Evidence labels

- **Confirmed (legacy):** behavior directly visible in the checked-in source.
- **Proposed (web):** implementation-oriented adaptation, not a claim about the old app.
- **Needs decision:** an ambiguity, implementation choice, or apparent legacy defect that should not be guessed silently.

Useful source references are relative to the repository root. The primary logic is in `_Sell/MainWindow.xaml.cs`, the catalog in `_Sell/Service/DefaultProductRegistry.cs`, money in `_Sell/Model/Price.cs`, layout in `_Sell/MainWindow.xaml`, and file persistence in `_Sell/App.xaml.cs`.
