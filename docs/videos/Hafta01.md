# Hafta 01 — Video senaryosu

**Dosya:** `Hafta01_Damgio.mp4` · **Süre:** en az 5 dk · **Son yükleme:** 25 Ekim 2026 Pazar 23:59
**Lider / sunan:** Aslınur Demir

> Kod anlatılırken yorum satırı okunmaz. Her bölümü kendi cümlelerinle anlat.

| Süre | Bölüm | Ekranda |
|---|---|---|
| 0:00–0:30 | Giriş: "Hafta 1 lideri Aslınur Demir. Proje Damgio: NFC/QR dijital damga kartı." | Portal ana sayfası |
| 0:30–1:30 | Kim ne yaptı: Aslınur iskelet + veritabanı, Zehra Identity + roller | GitHub → Commits / Pull requests sayfası |
| 1:30–2:30 | Mimari: neden 3 katman (Core / Infrastructure / Web); Core'un hiçbir şeye bağımlı olmaması | Solution Explorer |
| 2:30–3:45 | Veritabanı: 6 tablo + Identity, bire-çok ilişkiler (Shop→Programs, Program→StampCards, StampCard→StampEvents); unique index'ler | `AppDbContext.OnModelCreating`, pgAdmin / `\dt` |
| 3:45–4:30 | Migration: `dotnet ef database update` canlı çalıştır; tabloların oluştuğunu göster | Terminal + pgAdmin |
| 4:30–5:15 | Gizli bilgi: bağlantı dizesi neden `user-secrets`'ta | `appsettings.json` (şifre yok) |
| 5:15–5:45 | Zehra'nın kısmı: kayıt ol → giriş → Admin sayfası rol kontrolü | Tarayıcı |
| 5:45–6:15 | Gelecek hafta: Shop/Program modelleri, Admin panelinde CRUD + form doğrulama | `docs/tasks/Hafta02.md` |

## Olası sorular (gösterim için hazırlık)

- **Bire-çok ilişki nerede, nasıl tanımlandı?** `HasMany(...).WithOne(...).HasForeignKey(...)`
- **Migration nedir, neden elle tablo açmadık?** Şema değişikliğinin versiyonlanması, her ortamda aynı şema.
- **Neden PostgreSQL / Npgsql?** Ders isteri; ücretsiz, üretimde yaygın.
- **`StampCard`'da neden `UserId + ProgramId` unique?** Bir müşterinin bir programda tek kartı olur.
- **Damga neden istemciden yazılmıyor?** Sahtecilik; kural ve cooldown sunucuda transaction içinde.
