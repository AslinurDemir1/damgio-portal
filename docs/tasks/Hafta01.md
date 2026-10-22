# Hafta 01 — Görevler (21–25 Ekim 2026)

**Lider:** Aslınur Demir · **Video:** `Hafta01_Damgio.mp4` → Drive, **25 Ekim Pazar 23:59**

## Aslınur — iskelet ve veritabanı

- [x] Public repo, README, `.gitignore`, `.editorconfig`
- [x] Solution: `Damgio.Core`, `Damgio.Infrastructure`, `Damgio.Web` (MVC), `Damgio.Tests`
- [x] Varlıklar: `Shop`, `Program`, `StampCard`, `StampEvent`, `ShopMember`, `Device`, `AppUser`
- [x] `AppDbContext` (Identity tabloları dahil) + Npgsql + `InitialCreate` migration
- [x] Bağlantı dizesi `user-secrets` ile (repoda şifre yok)
- [x] CI: GitHub Actions build + test
- [x] El kitabı `docs/ENGINEERING.md`

## Zehra — Identity ve roller

Branch: `feat/identity-roles` → PR → Aslınur review eder.

- [ ] `Damgio.Core/Constants/Roles.cs` içindeki rolleri (`Admin`, `Cashier`, `Customer`) uygulama açılışında oluşturan seed (`RoleManager`)
- [ ] Geliştirme ortamı için bir demo Admin kullanıcısı (şifre `user-secrets`'tan okunur, koda yazılmaz)
- [ ] `AccountController`: Register (yeni kullanıcı → `Customer` rolü), Login, Logout
- [ ] `RegisterViewModel` / `LoginViewModel` — DataAnnotations doğrulama (zorunlu alan, e-posta, telefon, şifre uzunluğu, şifre tekrarı)
- [ ] Görünümler: `Views/Account/Login.cshtml`, `Register.cshtml`; navbar'da giriş/çıkış durumu
- [ ] `[Authorize(Roles = Roles.Admin)]` ile korunan boş bir `/Admin` sayfası — rol kontrolünün çalıştığını videoda göstermek için

İpucu: `Program.cs` içinde Identity zaten `AddIdentity<AppUser, IdentityRole>()` ile kayıtlı; cookie yolları `/Account/Login` olarak ayarlı.

## Hafta sonu kabul

- `dotnet ef database update` boş veritabanında çalışır, tablolar oluşur
- Kayıt ol → giriş yap → çıkış yap akışı çalışır
- Admin olmayan kullanıcı `/Admin`'e giremez
