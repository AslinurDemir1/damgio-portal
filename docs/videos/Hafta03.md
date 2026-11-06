# Hafta 03 — Video Senaryosu

**Dosya:** `Hafta03_Damgio.mp4` · **Süre:** en az 5 dk · **Son yükleme:** 8 Kasım 2026 Pazar 23:59  
**Lider / sunan:** Aslınur Demir

> Kod anlatılırken yorum satırı okunmaz. Her bölümü kendi cümlelerinle anlat.

| Süre | Bölüm | Ekranda |
|---|---|---|
| 0:00–0:30 | **Giriş:** "Hafta 3 lideri Aslınur Demir. Bu haftaki odak noktamız: Damga kartları Web API (tam CRUD), Swagger dokümantasyonu, iOS sözleşmesi (`contracts/openapi-v1.json`) ve işletmelere personel/kasiyer atama altyapısı." | Admin paneli & Swagger UI |
| 0:30–1:15 | **İş Dağılımı:** Aslınur Web API full CRUD, Swagger altyapısı, OpenAPI sözleşmesi, ShopMembers backend ve birim testleri; Zehra iOS veri modelleri hazırlığı ve uç nokta testleri | GitHub → Commits / PR sayfası |
| 1:15–2:15 | **Web API Mimarisi & Modeller:** `/api/v1/stampcards` mimarisi, `StampCardDto`, `CreateStampCardRequest`, `UpdateStampCardRequest` ve kuralları | `StampCardsController.cs`, `StampCardDto.cs` |
| 2:15–3:30 | **Swagger UI Üzerinde Canlı CRUD Testi:** Swagger UI ekranı açılır (`/swagger`). GET ile kart listeleme, POST ile yeni kart oluşturma (201 Created), mükerrer kart denemesinde 409 Conflict, PUT ile damga ekleme ve hedef tamamlandığında ödül döngüsü (D08), DELETE ile kart silme gösterilir. | Tarayıcıda `/swagger` |
| 3:30–4:15 | **ShopMembers (Personel/Kasiyer Yönetimi):** İşletme detayında (`/Shops/Details/1`) yeni bir personelin e-posta ile kasiyer olarak atanması ve yetkilendirilmesi | Tarayıcıda `/Shops/Details/1` |
| 4:15–4:45 | **Birim Testleri & OpenAPI Sözleşmesi:** `dotnet test` ile tüm testlerin (59 test) çalıştırılması; iOS projesi için üretilen `contracts/openapi-v1.json` dosyasının gösterilmesi | Terminal (`dotnet test`) & `contracts/openapi-v1.json` |
| 4:45–5:15 | **Gelecek Hafta:** Hafta 4 — Müşteri kart görüntüleme ekranı (`/s/{slug}`), mobil uyumlu UI ve NFC tap akışı | `docs/tasks/Hafta04.md` / Plan |

## Olası Sorular (Gösterim İçin Hazırlık)

- **Neden Web API için ayrı DTO'lar kullandınız?** Veritabanı varlıklarını doğrudan dışarı açmamak, hassas verileri gizlemek, gereksiz döngüsel referansları önlemek ve API sözleşmesini veritabanı şemasından bağımsız tutmak için DTO kullandık.
- **D08 Ödül döngüsü nasıl çalışıyor?** Bir kartta damga sayısı programın `RequiredStamps` değerine ulaştığında `RewardEarned` olayı veritabanına eklenir, mevcut damga (`CurrentStamps`) sıfırlanır; fakat toplam kazanılan damgayı takip eden `LifetimeStamps` artmaya devam eder.
- **iOS istemcisi bu API'yi nasıl kullanacak?** `contracts/openapi-v1.json` dosyasındaki OpenAPI standardındaki şema sayesinde iOS uygulamasında Swift modelleri ve ağ katmanı birebir uyumlu olarak üretilebiliyor.
- **Tekillik kontrolü nerede sağlanıyor?** Hem PostgreSQL veritabanı seviyesinde `(UserId, ProgramId)` unique index'i ile hem de controller içinde `AnyAsync` kontrolüyle çift katmanlı doğrulanır; mükerrer istekte `409 Conflict` döner.
