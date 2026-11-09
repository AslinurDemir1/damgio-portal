# Damgio Portal — Engineering Handbook

> Single source of truth for this repo. Read before writing code.
> Every work session ends with a dated entry in §7 (work log).
> Product text is Turkish; code, commits, identifiers are English.

| Field | Value |
|---|---|
| Course | BLM4581 Ağ Tabanlı Teknolojiler ve Uygulamaları (2026–2027 Güz) |
| Group | Damgio — Aslınur Demir (lead W1–5), Zehra Vural (lead W6–9) |
| Repo | `github.com/AslinurDemir1/damgio-portal` (public) |
| Sister repo | `github.com/zehravural1/damgio-ios` (BLM4587, SwiftUI) |
| Predecessor | `TapKart` (Flutter + Firebase, private) — domain reference only |

---

## 1. Product

App-less NFC/QR stamp card for cafes. The customer taps the table NFC / scans QR,
opens their card in the browser; staff (or the counter NFC "tokmak") adds a stamp.
After `RequiredStamps` (e.g. 7+1) a reward is earned.

| Actor | Surface |
|---|---|
| Customer | Mobile web (`/s/{slug}`), later iOS app |
| Admin (shop owner) | Admin panel (`/Admin`) |
| Cashier | Desk screen (`/Desk`) — stamp + redeem only |
| iOS app | REST API `/api/v1` with JWT |

## 2. Locked decisions

| ID | Decision |
|---|---|
| D01 | ASP.NET Core **MVC** on **.NET 10 (LTS)** — matches course material. |
| D02 | **PostgreSQL** via **EF Core (Npgsql)**; schema only via migrations. |
| D03 | One backend for web **and** iOS. iOS consumes `/api/v1` (OpenAPI contract in `contracts/`). |
| D04 | Layers: `Damgio.Core` (domain, rules) ← `Damgio.Infrastructure` (EF, Identity) ← `Damgio.Web` (MVC + API). Core references nothing. |
| D05 | Auth: ASP.NET Core Identity. Cookie for MVC, JWT bearer for API. |
| D06 | Roles: `Admin`, `Cashier`, `Customer`. |
| D07 | Stamps are written **only server-side** inside a transaction with a cooldown (default 180 s). |
| D08 | Reward cycle: `CurrentStamps >= RequiredStamps` → `RewardEarned` event, `CurrentStamps = 0`, `LifetimeStamps` keeps counting. |
| D09 | NFC tokmak = NTAG URL `/s/{slug}/tap?k=…`; only the SHA-256 hash of `k` is stored. |
| D10 | No secrets in git. Connection strings via `dotnet user-secrets` or environment variables. |
| D11 | Git: feature branch → PR → review by the other member → **merge commit** (no squash; authorship is graded). |
| D12 | Conventional commits: `feat:`, `fix:`, `docs:`, `chore:`, `test:`, `refactor:`. |

## 3. Open decisions

| ID | Question | Default |
|---|---|---|
| O01 | Customer login on web | Identity account (phone + password) |
| O02 | Organizations / chains (one card across branches) | After the course |
| O03 | Hosting for production | After the course |

## 4. Data model

```
AspNetUsers (AppUser) 1─N StampCards
                      1─N ShopMembers
Shops     1─N Programs
          1─N ShopMembers
          1─N Devices
Programs  1─N StampCards
StampCards 1─N StampEvents
```

| Table | Key columns |
|---|---|
| `Shops` | Name, Slug (unique), Address, IsActive |
| `Programs` | ShopId, Name, RequiredStamps, RewardName, StampCooldownSeconds |
| `StampCards` | UserId, ProgramId, CurrentStamps, LifetimeStamps, LastStampAt (unique UserId+ProgramId) |
| `StampEvents` | StampCardId, Type (Stamp / RewardEarned / RewardUsed), Source (Tap / Manual), ActorUserId, CreatedAt |
| `ShopMembers` | ShopId, UserId, Role (Owner / Cashier) (unique ShopId+UserId) |
| `Devices` | ShopId, Name, KeyHash, IsActive |

Reserved slugs: `admin`, `api`, `desk`, `s`, `account`, `login`, `swagger`.

## 5. API (v1)

| Method | Route | Who |
|---|---|---|
| GET/POST/PUT/DELETE | `/api/v1/stampcards` | Admin (course: full CRUD + Swagger) |
| POST | `/api/v1/auth/token` | anyone (JWT for iOS) |
| GET | `/api/v1/me/cards` | Customer |
| POST | `/api/v1/tap` | Customer |

Error codes: `UNAUTHENTICATED`, `PERMISSION_DENIED`, `NOT_FOUND`, `SHOP_INACTIVE`,
`INVALID_DEVICE`, `COOLDOWN`, `SLUG_TAKEN`, `ALREADY_USED`, `RATE_LIMITED`, `VALIDATION`, `ALREADY_EXISTS`.

## 6. Course requirements checklist

- [x] ASP.NET Core MVC
- [x] PostgreSQL + EF Core + migrations
- [x] ≥ 4–5 tables, ≥ 1 one-to-many
- [x] Web API full CRUD + Swagger (W3)
- [x] Form validation (W2)
- [x] Login + ≥ 2 roles (W1–W2, Zehra)
- [ ] Admin panel (W3–W7)
- [x] Mobile-friendly UI (W4, W8)
- [x] README with setup + migration steps
- [x] No secrets in repo

## 7. Work log

Newest first.

### 2026-11-09 — Week 4 completion: Mobile-friendly Customer Card (/s/{slug}), NFC/QR Tap Flow & API (Aslınur & Zehra)

- Customer Card Surface: `CustomerCardController` (`/s/{slug}`) implemented with responsive mobile layout, styled using shared design tokens (`design/tokens.json`).
- Auto-enrollment: Automatically creates a digital stamp card for authenticated customers visiting the shop's URL.
- NFC Tap Flow: `/s/{slug}/tap?k=...` endpoint handles counter device taps, SHA-256 key hash verification (D09), cooldown checking (D07) and D08 reward cycle resets.
- Web API Endpoints: `CustomerController` (`/api/v1/me/cards` and `/api/v1/tap`) for iOS client (`damgio-ios`) and mobile apps.
- OpenAPI Contract: Updated `contracts/openapi-v1.json` with customer card summary and tap endpoints.
- UI & Interaction: Real-time cooldown countdown timer in JS, animated stamp grid, celebration notice upon reward earn, and demo tap simulator.
- Seed Data: Demo shop ("Kadıköy Damgio Cafe"), program ("7 Kahveye 1 İkram"), NFC device, demo customer and cashier accounts added to `DbSeeder.cs`.
- Unit Tests: 79 unit tests passing (`dotnet test`), verifying SHA-256 hashing, tap logic, cooldown enforcement, and auto-enrollment.
- Documentation: `docs/tasks/Hafta04.md` and `docs/videos/Hafta04.md` completed.
- **Next:** W5 — Cashier Desk screen (`/Desk`), manual stamp & reward redemption, and metrics reporting.

### 2026-11-06 — Week 3 completion: StampCards Web API (Full CRUD + Swagger) & ShopMembers (Aslınur & Zehra)

- Web API: `StampCardsController` (`/api/v1/stampcards`) implemented with complete RESTful CRUD operations (GET all with filters, GET by id with event history, POST create, PUT update with reward cycle D08, DELETE).
- OpenAPI / Swagger: Swashbuckle OpenAPI v1 configured with XML documentation comments, available at `/swagger`.
- Contract: Generated and published `contracts/openapi-v1.json` for the iOS application (`zehravural1/damgio-ios`).
- ShopMembers: Staff/Cashier management integrated into Admin Shop Details (`/Shops/Details/{id}`); support for adding and removing shop members with role assignments (`Cashier`, `Owner`) and uniqueness validation.
- Unit Tests: 59 unit tests passing (`dotnet test`), verifying DTO validations, reward cycle resets, cooldown logic, and member constraints.
- Admin UI & Layout: Direct shortcuts to Swagger UI and raw API endpoints added to Admin Dashboard and main navigation.
- Documentation: `docs/tasks/Hafta03.md` and `docs/videos/Hafta03.md` created.
- **Next:** W4 — Customer card display screen (`/s/{slug}`), mobile-friendly UI and NFC tap flow.

### 2026-10-31 — Week 2 completion: Admin CRUD for Shops and Programs (Aslınur & Zehra)

- MVC Controllers: `ShopsController` and `ProgramsController` implemented with full CRUD operations (`Index`, `Create`, `Edit`, `Details`, `Delete`).
- Form Validation: Client-side (jQuery Unobtrusive) and server-side (`ModelState.IsValid`) validation with localized Turkish messages across all forms.
- URL Slug: Automatic slug generation, reserved route validation (`admin`, `api`, `login`, etc.) and database uniqueness checks.
- Admin UI: Razor views created for Shops and Programs management; Admin Dashboard (`/Admin`) enhanced with shortcut cards.
- Layout: Cleaned and integrated navigation bar with role-based visibility (`Roles.Admin`).
- Verification: All Week 2 checklist items completed and verified.
- **Next:** W3 — StampCards Web API (full CRUD with Swagger) + ShopMembers.

### 2026-10-22 — Week 1 skeleton (Aslınur)

- Solution: `Damgio.Core`, `Damgio.Infrastructure`, `Damgio.Web`, `Damgio.Tests` on .NET 10.
- Domain entities + `AppDbContext` (Identity tables included) + `InitialCreate` migration on PostgreSQL.
- README, CI (build + test), PR/issue templates, video script `docs/videos/Hafta01.md`.
- **Left for Zehra (W1):** role seeding (Admin/Cashier/Customer), login/register/logout UI — see `docs/tasks/Hafta01.md`.
- **Next:** W2 — Shop/Program CRUD in Admin area with validation.
