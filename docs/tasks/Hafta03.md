# Hafta 03 — Görevler (2–8 Kasım 2026)

**Lider:** Aslınur Demir · **Video:** `Hafta03_Damgio.mp4` → Drive, **8 Kasım 2026 Pazar 23:59**

## Aslınur — Web API (Full CRUD), Swagger & ShopMembers Altyapısı

Branch: `aslinur` → PR → Zehra review eder.

- [x] Web API mimarisi ve Swagger/OpenAPI v1 entegrasyonu (`Damgio.Web/Program.cs`, XML dokümantasyon üretimi)
- [x] Damga kartı API modelleri ve validasyon sözleşmeleri:
  - `StampCardDto`, `StampCardDetailDto`, `StampEventDto`
  - `CreateStampCardRequest`, `UpdateStampCardRequest`
  - Standart hata formatı `ApiErrorResponse`
- [x] `StampCardsController` (Web API — `/api/v1/stampcards`):
  - `GET /api/v1/stampcards`: Kartları listeleme (`userId`, `programId`, `shopId` filtreleri ile)
  - `GET /api/v1/stampcards/{id}`: Kart detayları ve geçmiş işlem olayları (`Events`)
  - `POST /api/v1/stampcards`: Yeni kart oluşturma (tekillik `(UserId, ProgramId)`, aktiflik ve varlık denetimleri)
  - `PUT /api/v1/stampcards/{id}`: Damga güncelleme, D08 ödül döngüsü kuralı (hedefe ulaşınca ödül olayı ve sıfırlama)
  - `DELETE /api/v1/stampcards/{id}`: Kart ve bağlı işlem geçmişini silme
- [x] iOS istemcisi (zehravural1/damgio-ios) için OpenAPI sözleşmesi (`contracts/openapi-v1.json`)
- [x] İşletme personel / kasiyer yönetimi (`ShopMembers`) arka plan mantığı ve MVC arayüz entegrasyonu (`ShopsController.Details`, `AddMember`, `RemoveMember`)
- [x] Kapsamlı birim testleri (`tests/Damgio.Tests/StampCardTests.cs`, `ShopMemberTests.cs`)
- [x] Admin Dashboard ve Layout güncellemesi: Swagger ve API hızlı erişim kartı
- [x] 3. Hafta video senaryosu `docs/videos/Hafta03.md`

## Zehra — API & iOS Entegrasyonu, Kasiyer Rolü ve İnceleme

Branch: `zehra` → PR / Review → Aslınur merge eder.

- [ ] `contracts/openapi-v1.json` sözleşmesini `zehravural1/damgio-ios` (Swift) projesine dahil etme ve DTO eşleştirmelerini tamamlama
- [ ] Swagger arayüzü (`/swagger`) üzerinden API uç noktalarını (GET, POST, PUT, DELETE) test etme
- [ ] Şubeye yeni personel atandığında kullanıcının sistemdeki `Cashier` rolünü ve yetkilerini doğrulama
- [ ] Aslınur'un açtığı 3. Hafta PR'ını inceleme ve onaylama (D11: merge commit)

## Hafta Sonu Kabul Kriterleri

- Tüm birim testleri (`dotnet test`) hatasız geçer (59 test).
- `/swagger` adresi tarayıcıda açılır, XML açıklamalarıyla tüm `/api/v1/stampcards` uç noktaları listelenir ve test edilebilir.
- Postman veya Swagger üzerinden bir kullanıcı için kart oluşturulabilir (`201 Created`), mevcut kart getirilebilir (`200 OK`), damga güncellenebilir (`200 OK`) ve silinebilir (`204 No Content`).
- Aynı kullanıcıya aynı program için mükerrer kart oluşturulması `409 Conflict` (`ALREADY_EXISTS`) ile engellenir.
- Bir kartta damga sayısı hedef damga sayısına (ör. 7) ulaştığında `RewardEarned` olayı tetiklenir ve kart damgası sıfırlanır (D08 kuralı).
- Admin panelinde işletme detayından (`/Shops/Details/{id}`) sisteme kayıtlı bir kullanıcı işletmeye kasiyer/owner olarak atanabilir ve çıkarılabilir.
- `contracts/openapi-v1.json` güncel ve geçerlidir.
