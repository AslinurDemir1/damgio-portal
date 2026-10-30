# Hafta 02 — Video Senaryosu

**Dosya:** `Hafta02_Damgio.mp4` · **Süre:** en az 5 dk · **Son yükleme:** 1 Kasım 2026 Pazar 23:59  
**Lider / sunan:** Aslınur Demir

> Kod anlatılırken yorum satırı okunmaz. Her bölümü kendi cümlelerinle anlat.

| Süre | Bölüm | Ekranda |
|---|---|---|
| 0:00–0:30 | **Giriş:** "Hafta 2 lideri Aslınur Demir. Bu hafta odak: İşletme ve program modelleri, URL slug yönetimi, Admin CRUD sayfaları ve form doğrulamaları." | Admin paneli / Dashboard |
| 0:30–1:15 | **İş Dağılımı:** Aslınur modeller, `SlugHelper`, validasyon kuralları ve testler; Zehra Admin MVC CRUD ekranları ve form validation | GitHub → Commits / PR sayfası |
| 1:15–2:15 | **Modeller ve Kurallar:** `Shop` ve `LoyaltyProgram` modelleri; bire-çok ilişkisi (`Shop 1─N Programs`), check constraint'ler (`RequiredStamps` 1–50) | `Shop.cs`, `LoyaltyProgram.cs`, `AppDbContext.cs` |
| 2:15–3:15 | **Slug Yönetimi & Testler:** Türkçe karakterlerin (`ç, ğ, ı, ö, ş, ü`) URL uyumlu formata dönüştürülmesi; rezerve slug koruması (`admin`, `api`, `swagger`); birim testlerinin çalıştırılması | `SlugHelper.cs`, `SlugHelperTests.cs` (`dotnet test`) |
| 3:15–4:30 | **Zehra'nın Kısmı — Admin CRUD & Form Doğrulama:** Yeni işletme ekleme (`Create`), düzenleme (`Edit`); DataAnnotations ile boş alan veya geçersiz damga girildiğinde anlık Türkçe validasyon mesajları | Tarayıcıda `/Admin/Shops` ve `/Admin/Programs` |
| 4:30–5:15 | **Yetkilendirme:** Müşteri veya giriş yapmamış kullanıcının Admin CRUD sayfalarına erişemediğinin gösterilmesi | Gizli sekme / Tarayıcı |
| 5:15–5:45 | **Gelecek Hafta:** Hafta 3 Web API (tam CRUD + Swagger) ve ShopMembers altyapısı | `docs/tasks/Hafta03.md` / Plan belgesi |

## Olası Sorular (Gösterim İçin Hazırlık)

- **Slug neden gereklidir, neden doğrudan ID kullanmıyoruz?** Müşteriler masadaki NFC veya QR kodunu okuttuğunda `/s/kadikoy-cafe` gibi akılda kalıcı, kullanıcı dostu ve SEO uyumlu URL ile kartlarına erişirler.
- **Slug'da Türkçe karakterler ve rezerve kelimeler nasıl yönetildi?** `SlugHelper` ile Türkçe karakterler ASCII eşdeğerlerine dönüştürülür, sistem rotalarıyla (`/admin`, `/api`, `/account`) çakışan kelimeler engellenir.
- **Form doğrulaması (Validation) nasıl yapıldı?** Hem istemci tarafında jQuery Validate Unobtrusive ile sayfa yenilenmeden anlık geri bildirim verilir, hem de sunucu tarafında `ModelState.IsValid` ile güvenli kontrol sağlanır.
- **Program ve Shop arasındaki ilişki nedir?** Bir işletmenin (Shop) birden fazla damga programı (LoyaltyProgram) olabilir (`1─N`); işletme silindiğinde programları da cascade olarak silinir.
