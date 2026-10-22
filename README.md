# Damgio Portal

NFC/QR tabanlı, uygulama gerektirmeyen dijital damga kartı web portalı.
Müşteri masadaki NFC/QR ile kartını tarayıcıda açar, kasada damga düşer, 7+1 gibi bir kuralla ödül kazanır.

BLM4581 Ağ Tabanlı Teknolojiler ve Uygulamaları — 2026–2027 Güz dönem projesi.

| | |
|---|---|
| Grup | **Damgio** — Aslınur Demir ([@AslinurDemir1](https://github.com/AslinurDemir1)), Zehra Vural ([@zehravural1](https://github.com/zehravural1)) |
| Yığın | ASP.NET Core MVC (.NET 10) · PostgreSQL · Entity Framework Core · ASP.NET Core Identity · Bootstrap 5 · Swagger |
| iOS istemcisi | [zehravural1/damgio-ios](https://github.com/zehravural1/damgio-ios) — aynı Web API'yi kullanır |
| Mühendislik el kitabı | [docs/ENGINEERING.md](docs/ENGINEERING.md) |

## Proje yapısı

```
src/
  Damgio.Core/            Varlıklar ve iş kuralları (hiçbir projeye bağımlı değil)
  Damgio.Infrastructure/  EF Core DbContext, migration'lar, Identity
  Damgio.Web/             MVC arayüz + Web API (/api/v1) + Swagger
tests/
  Damgio.Tests/           xUnit testleri
contracts/                iOS uygulamasının kullandığı OpenAPI sözleşmesi
design/                   Ortak tasarım token'ları (web + iOS)
docs/                     El kitabı, haftalık görevler, video senaryoları
```

## Gereksinimler

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- PostgreSQL 15+ (yerelde kurulu ya da `docker compose up -d`)
- EF Core CLI: `dotnet tool install --global dotnet-ef`

## Kurulum

### 1. Veritabanı

**Seçenek A — Docker:**

```bash
docker compose up -d
```

**Seçenek B — Yerel PostgreSQL:** `damgio` adında bir veritabanı ve kullanıcı oluşturun:

```sql
CREATE USER damgio WITH PASSWORD 'kendi-sifreniz';
CREATE DATABASE damgio OWNER damgio;
```

### 2. Bağlantı dizesi (gizli bilgi, repoya girmez)

```bash
cd src/Damgio.Web
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5432;Database=damgio;Username=damgio;Password=kendi-sifreniz"
```

> Şifre `appsettings.json` içine **yazılmaz**. Ortam değişkeni de kullanılabilir:
> `ConnectionStrings__Default=...`

### 3. Migration

```bash
dotnet ef database update --project src/Damgio.Infrastructure --startup-project src/Damgio.Web
```

Yeni migration eklemek için:

```bash
dotnet ef migrations add <Ad> --project src/Damgio.Infrastructure --startup-project src/Damgio.Web --output-dir Data/Migrations
```

### 4. Çalıştırma

```bash
dotnet run --project src/Damgio.Web
```

| Adres | İçerik |
|---|---|
| `https://localhost:7180` | Portal |
| `https://localhost:7180/swagger` | Web API (Swagger) |

### 5. Test

```bash
dotnet test
```

## Çalışma kuralları

- Her iş bir **issue** + **feature branch** (`feat/shop-crud`) + **pull request**.
- PR'ı diğer üye inceler, **merge commit** ile birleştirilir (squash yok — katkılar kişiye sayılır).
- Commit mesajları İngilizce, conventional: `feat:`, `fix:`, `docs:`, `chore:`, `test:`.
- Her üye **kendi GitHub hesabıyla** push yapar; en az haftada bir kez.
- Haftalık görevler: [docs/tasks](docs/tasks) · video senaryoları: [docs/videos](docs/videos)

## Liderlik

| Hafta | Lider |
|---|---|
| 1–5 | Aslınur Demir |
| 6–9 | Zehra Vural |
