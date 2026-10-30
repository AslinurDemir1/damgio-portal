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
`INVALID_DEVICE`, `COOLDOWN`, `SLUG_TAKEN`, `ALREADY_USED`, `RATE_LIMITED`, `VALIDATION`.

## 6. Course requirements checklist

- [x] ASP.NET Core MVC
- [x] PostgreSQL + EF Core + migrations
- [x] ≥ 4–5 tables, ≥ 1 one-to-many
- [ ] Web API full CRUD + Swagger (W3)
- [ ] Form validation (W2)
- [ ] Login + ≥ 2 roles (W1–W2, Zehra)
- [ ] Admin panel (W3–W7)
- [ ] Mobile-friendly UI (W4, W8)
- [x] README with setup + migration steps
- [x] No secrets in repo

## 7. Work log

Newest first.

### 2026-10-30 — Week 2: Models, SlugHelper and Admin CRUD foundation (Aslınur)

- Added `SlugHelper` in `Damgio.Core/Common` with Turkish transliteration (`ç->c`, `ğ->g`, `ı->i`, `ö->o`, `ş->s`, `ü->u`, `İ/I->i`) and reserved slug check (`admin`, `api`, `desk`, `s`, `account`, `login`, `swagger`).
- Unit tests: `SlugHelperTests` added to `Damgio.Tests`.
- Admin InputModels: `ShopInputModel`, `ProgramInputModel` created with comprehensive DataAnnotations validation.
- Documentation: `docs/tasks/Hafta02.md` and video script `docs/videos/Hafta02.md` created.
- UI: Added Admin navigation links (Management, Shops, Programs) in `_Layout.cshtml`.
- **Left for Zehra (W2):** MVC CRUD Controllers (`ShopsController`, `ProgramsController`) with Razor views and client/server validation.
- **Next:** Zehra's CRUD PR review and video demo recording.

### 2026-10-22 — Week 1 skeleton (Aslınur)

- Solution: `Damgio.Core`, `Damgio.Infrastructure`, `Damgio.Web`, `Damgio.Tests` on .NET 10.
- Domain entities + `AppDbContext` (Identity tables included) + `InitialCreate` migration on PostgreSQL.
- README, CI (build + test), PR/issue templates, video script `docs/videos/Hafta01.md`.
- **Left for Zehra (W1):** role seeding (Admin/Cashier/Customer), login/register/logout UI — see `docs/tasks/Hafta01.md`.
- **Next:** W2 — Shop/Program CRUD in Admin area with validation.
