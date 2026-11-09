# Hafta 04 — Görevler (9–15 Kasım 2026)

**Lider:** Aslınur Demir · **Video:** `Hafta04_Damgio.mp4` → Drive, **15 Kasım 2026 Pazar 23:59**

---

## 4. Hafta Hedefi
Müşterilerin masadaki NFC veya QR kodunu taratarak tarayıcıda açtığı mobil uyumlu dijital damga kartı arayüzünün (`/s/{slug}`), NFC sayaç tokmak dokunma akışının (`/s/{slug}/tap?k=...` ve `POST /api/v1/tap`), soğuma süresi (D07) ve ödül döngüsü (D08) kurallarının tamamlanması; kasiyer masaüstü/kasa yönetim ekranının (`/Desk`) ve kardes iOS uygulamasının (`damgio-ios`) entegrasyonu.

---

## Aslınur — Mobil Kart Arayüzü, NFC Damga Akışı, API ve Testler

Branch: `aslinur` → PR → Zehra review eder (D11: merge commit).

- [x] **NFC Tokmak Anahtarı ve Güvenlik Altyapısı (`Damgio.Core/Common/DeviceKeyHelper.cs`):**
  - Karar D09 gereği sayaç cihazlarının `k` parametresi veritabanında asla açık metin saklanmaz; tek yönlü SHA-256 özeti (`KeyHash`) üretilir ve doğrulanır (`ComputeHash`, `VerifyKey`).
- [x] **Müşteri Kartı Web Denetleyicisi (`Damgio.Web/Controllers/CustomerCardController.cs`):**
  - `GET /s/{slug}`: Mobil uyumlu müşteri kartı görüntüleme ekranı.
  - Otomatik Kayıt (Auto-enrollment): Oturum açmış müşteri işletme sayfasına girdiğinde kartı yoksa otomatik 0 damga ile kart oluşturulur.
  - Giriş yapmamış ziyaretçiler için işletme ve ödül önizleme görünümü; `returnUrl` ile giriş/kayıt yönlendirmesi.
  - `GET /s/{slug}/tap?k=...`: NFC sayaç tokmak dokunma uç noktası.
    - Cihaz SHA-256 doğrulaması (D09).
    - Soğuma süresi denetimi (D07 - 180 saniye): Süre dolmadan basılan mükerrer damgaların engellenmesi.
    - Ödül döngüsü (D08): Hedef damgaya (ör. 7) ulaşıldığında ödülün hak edilmesi (`RewardEarned` olayı), damgaların 0'a sıfırlanması ve yeni döngünün başlaması.
    - Normal damga basımı (`Stamp` olayı, sayaç cihaz kimliği kaydı).
- [x] **Mobil Uyumlu Tasarım ve Tasarım Token'ları (`design/tokens.json` & `site.css`):**
  - `tokens.json` renk paleti (`#fbf8f3`, `#2d5e4e`, `#c26c55`, `#3b2e2b`), kart yarıçapı (24px) ve buton yarıçapı (20px) ile responsive mobil deneyim.
  - Görsel damga ızgarası (Stamp grid): Dolu damgalar için kahve damgası animasyonu, boş yuvalar, ödül yuvası ve ilerleme çubuğu.
  - Dinamik JavaScript geri sayım sayacı (soğuma süresi için canlı saniye sayımı).
  - Tarayıcı üzerinden test için demo NFC dokunma simülatörü.
- [x] **Müşteri Web API Uç Noktaları (`Damgio.Web/Controllers/Api/CustomerController.cs`):**
  - `GET /api/v1/me/cards`: Oturum açmış müşterinin tüm damga kartlarını ve soğuma durumunu listeler.
  - `POST /api/v1/tap`: Mobil ve iOS istemcileri için JSON tabanlı tokmak dokunma uç noktası (`TapRequest` -> `TapResponseDto`).
- [x] **OpenAPI Sözleşmesi Güncellemesi (`contracts/openapi-v1.json`):**
  - `/api/v1/me/cards` ve `/api/v1/tap` uç noktaları ve şemaları (`CustomerCardSummaryDto`, `TapRequest`, `TapResponseDto`) OpenAPI sözleşmesine eklendi.
- [x] **Veritabanı Tohumlama Zenginleştirmesi (`DbSeeder.cs`):**
  - Demo işletme: "Kadıköy Damgio Cafe" (`kadikoy-damgio-cafe`).
  - Demo program: "7 Kahveye 1 İkram" (7 damga, "1 Adet Filtre Kahve", 180 sn soğuma).
  - Demo NFC tokmak: "Kasa 1 NFC Tokmak" (`damgio-tokmak-secret-2026` ve SHA-256 özeti).
  - Demo kullanıcılar: `musteri@damgio.com` (Müşteri) ve `kasiyer@damgio.com` (Kasiyer).
- [x] **Birim Testleri (`tests/Damgio.Tests/DeviceKeyHelperTests.cs` ve `CustomerCardTests.cs`):**
  - SHA-256 hash hesaplama, büyük/küçük harf duyarsızlığı ve doğrulama testleri.
  - İlerleme yüzdesi ve kalan damga hesaplama testleri.
  - Tap akışı: normal damga artışı, ödül döngüsü sıfırlaması, soğuma süresi engellemesi ve yetkisiz cihaz engelleme testleri (toplam 79 test yeşil).
- [x] **Dokümantasyon & Video:**
  - `docs/tasks/Hafta04.md` ve `docs/videos/Hafta04.md` senaryosu.

---

## Zehra — Kasiyer Ekranı (`/Desk`), iOS Entegrasyonu ve İnceleme

Branch: `zehra` → PR / Review → Aslınur merge eder.

- [ ] **Kasiyer Yönetim Ekranı (`/Desk` — MVC):**
  - `DeskController` oluşturulması (`[Authorize(Roles = $"{Roles.Cashier},{Roles.Admin}")]`).
  - Kasiyerin işletmeyi seçip müşteri telefon numarası, e-posta veya kart numarası ile müşterinin kartını ekrana getirmesi.
  - **Manuel Damga Basma:** Kasiyer butonuna tıklandığında müşteriye 1 damga basılması (`Source = StampSource.Manual`, `ActorUserId = kasiyerId`).
  - **Ödül Kullanma (Redeem):** Müşteri hak ettiği ücretsiz kahveyi/ikramı aldığında kasiyerin "Ödülü Kullan" butonuna basarak `StampEventType.RewardUsed` olayını kaydetmesi.
- [ ] **iOS İstemcisi Entegrasyonu (`zehravural1/damgio-ios`):**
  - Güncellenen `contracts/openapi-v1.json` sözleşmesini repoya çekip Swift modellerini (`CustomerCardSummaryDto`, `TapRequest`, `TapResponseDto`) güncelleme.
  - "Kartlarım" ekranında `/api/v1/me/cards` çağrısını bağlama ve kartları listeleme.
  - NFC okuyucu veya test butonu ile `/api/v1/tap` isteğini test etme.
- [ ] **Kod İncelemesi (Code Review & Merge):**
  - Aslınur'un açtığı 4. Hafta PR'ını GitHub üzerinden inceleme, testleri denetleme ve **merge commit** (D11 kuralı) ile `main` dalına birleştirme.

---

## Hafta Sonu Kabul Kriterleri

1. Tüm birim testleri (`dotnet test`) hatasız geçer (79 test).
2. Tarayıcıda `/s/kadikoy-damgio-cafe` açıldığında:
   - Giriş yapılmamışsa işletme bilgisi ve "Giriş Yap / Kayıt Ol" butonları görüntülenir.
   - Giriş yapıldığında müşterinin 7 yuvalı damga kartı, mevcut damga sayısı ve ödül bilgisi mobil uyumlu olarak görüntülenir.
3. `/s/kadikoy-damgio-cafe/tap?k=damgio-tokmak-secret-2026` linkine tıklandığında veya sayfadaki test butonuna basıldığında:
   - Giriş yapılmamışsa önce `/Account/Login` sayfasına yönlendirir, ardından damgayı işler.
   - Damga sayısı 1 artar, işlem geçmişine NFC dokunma olayı eklenir ve başarı bildirimi verilir.
   - 180 saniye içinde tekrar tıklandığında soğuma süresi uyarısı verilir ve canlı geri sayım sayacı çalışır.
   - 7. damgaya ulaşıldığında ödül kazanma tebriki gösterilir, damgalar sıfırlanır ve yeni ödül döngüsü başlar (D08).
4. Geçersiz bir `k` anahtarı veya geçersiz slug girildiğinde yetkisiz cihaz / bulunamadı uyarısı verilir.
5. `/api/v1/me/cards` ve `/api/v1/tap` uç noktaları Swagger'da ve `contracts/openapi-v1.json` dosyasında yer alır.
