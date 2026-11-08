# Hafta 03 — Video Kayıt Senaryosu ve Konuşma Metni

**Dosya:** `Hafta03_Damgio.mp4` · **Hedef Süre:** 5:30 dk (En az 5 dk kuralı)  
**Lider / Sunan:** Aslınur Demir · **Teslim:** 8 Kasım 2026 Pazar 23:59  

---

## 🎬 Video Öncesi Hazırlık (Masayı ve Ekranı Ayarlama)

1. **Uygulamayı Çalıştır:**  
   Terminalde `Damgio.Web` projesini çalıştırın:
   ```bash
   dotnet run --project src/Damgio.Web
   ```
   Uygulama `https://localhost:7246` (veya projenizin varsayılan portunda) hazır olsun.

2. **Tarayıcı Sekmelerini Hazırla:**
   - **Sekme 1:** `https://localhost:7246/Admin` (Admin ile giriş yapılmış olsun: `admin@damgio.com` / `Admin123*`).
   - **Sekme 2:** `https://localhost:7246/swagger` (Swagger UI tam ekran test edilmeye hazır).
   - **Sekme 3:** `https://localhost:7246/Shops/Details/1` (ShopMembers personel yönetiminin gösterileceği şube detayı).
   - **Sekme 4:** `https://github.com/AslinurDemir1/damgio-portal/pull/2` (Zehra'nın onaylayıp merge ettiği 3. Hafta PR sayfası).

3. **VS Code Sekmelerini Sırasıyla Açık Bırak:**
   - **Tab 1:** `src/Damgio.Web/Controllers/Api/StampCardsController.cs` (Web API Controller)
   - **Tab 2:** `src/Damgio.Web/Models/Api/StampCards/StampCardDto.cs` (DTO modelleri)
   - **Tab 3:** `src/Damgio.Web/Program.cs` (SwaggerGen & XML entegrasyonu)
   - **Tab 4:** `contracts/openapi-v1.json` (iOS sözleşmesi)
   - **Tab 5:** `src/Damgio.Web/Controllers/ShopsController.cs` (ShopMembers aksiyonları)
   - **Tab 6:** `tests/Damgio.Tests/StampCardTests.cs` (Birim testleri)

4. **Terminal:**
   Açık ve temiz bir pencere olsun, `dotnet test` komutunu çalıştırmaya hazır beklesin.

---

## 🎙️ Sahne Sahne Çekim ve Konuşma Rehberi

---

### [00:00 – 00:40] Bölüm 1: Giriş ve 3. Hafta Hedefleri
* **Ekranda Ne Olacak:** Tarayıcı açık; `/Admin` paneli ve ardından yeni eklenen "Web API & Swagger (v1)" kartı.
* **Ne Yapılacak:** Fare ile Admin dashboard'undaki Web API kartı ve üst menüdeki "Swagger API" bağlantısı gösterilir.
* **Seslendirilecek Metin:**
> *"Merhaba hocam, BLM4581 Ağ Tabanlı Teknolojiler ve Uygulamaları dersi için geliştirdiğimiz Damgio projesinin 3. hafta tanıtım videosuna hoş geldiniz. Ben bu haftanın grup lideri Aslınur Demir.*  
>  
> *Geçtiğimiz hafta işletme ve programlarımızın MVC CRUD ekranlarını, URL slug yapısını ve form validasyonlarını tamamlamıştık.*  
>  
> *Bu 3. haftamızda ise dersimizin en kritik zorunlu isterlerinden olan Web API altyapısına odaklandık. Müşteri damga kartları (`StampCards`) için tam teşekküllü bir RESTful CRUD API geliştirdik, Swagger ile etkileşimli dokümantasyonunu sağladık, iOS istemcimiz için OpenAPI sözleşmesini oluşturduk ve işletmelere kasiyer/personel atamamızı sağlayan ShopMembers modülünü tamamladık."*

---

### [00:40 – 01:20] Bölüm 2: Haftalık Görev Dağılımı ve Git PR Süreci
* **Ekranda Ne Olacak:** GitHub repo sekmesi → PR #2 (`feat: 3. Hafta - StampCards Web API full CRUD...`) ve commit geçmişi.
* **Ne Yapılacak:** Zehra'nın PR'ı incelediği ve `main` branch'ine **merge commit** ile birleştirdiği (`d004ceb`) ekranda fare ile gösterilir.
* **Seslendirilecek Metin:**
> *"Mühendislik el kitabımızdaki kurallara uygun olarak iş dağılımımızı ve versiyon kontrol sürecimizi titizlikle yürüttük:*  
>  
> *Ben grup lideri olarak; Web API mimarisini, DTO veri transfer modellerini, `/api/v1/stampcards` altındaki tüm CRUD uç noktalarını, D08 ödül döngüsü iş mantığını, Swagger XML entegrasyonunu ve birim testlerini `aslinur` branch'i üzerinde geliştirdim.*  
>  
> *Arkadaşım Zehra ise; hazırladığım `aslinur` branch'i için GitHub üzerinde Pull Request #2'yi açtı, kod incelemesini (code review) gerçekleştirdi ve merge commit kuralımıza uygun olarak ana `main` dalımızla birleştirdi. Ayrıca bu API sözleşmesinin iOS istemcimizdeki (`damgio-ios`) modellerle uyumunu denetledi."*

---

### [01:20 – 02:30] Bölüm 3: Kod İncelemesi — Web API, DTO'lar ve İş Kuralları
* **Ekranda Ne Olacak:** VS Code → `StampCardsController.cs`, `StampCardDto.cs` ve `Program.cs`.
* **Ne Yapılacak:** 
  1. `StampCardsController` sınıfındaki `[ApiController]` ve `[Route("api/v1/[controller]")]` öznitelikleri gösterilir.
  2. `StampCardDto` ve `CreateStampCardRequest` açılarak veritabanı varlıklarının doğrudan açılmadığı, DTO kullanıldığı vurgulanır.
  3. `Program.cs`'teki `IncludeXmlComments` satırı gösterilir.
  4. Controller içindeki tekillik denetimi (`AnyAsync` -> 409 Conflict) ve D08 ödül döngüsü kod bloğu gösterilir.
* **Seslendirilecek Metin:**
> *"Şimdi kod mimarimize bakalım:*  
>  
> *İlk olarak `StampCardsController` sınıfımızı incelediğimizde; güvenlik ve katmanlı mimari ilkeleri gereği veritabanı Entity'lerini doğrudan dış dünyaya açmadık. `StampCardDto`, `CreateStampCardRequest` ve `UpdateStampCardRequest` gibi güçlü tipli DTO nesneleri ve DataAnnotations doğrulamaları kullandık.*  
>  
> *`Program.cs` içerisinde Swashbuckle SwaggerGen yapılandırmamıza XML dokümantasyon dosyasını dahil ettik. Böylece C# kodlarımızdaki XML özetleri doğrudan Swagger UI ekranına yansıyor.*  
>  
> *İş mantığı tarafında iki kritik kuralı garanti altına aldık:*  
> *Birincisi; bir müşterinin aynı sadakat programında yalnızca tek bir kartı olabilir. Bunu hem veritabanındaki benzersiz indeksle hem de controller seviyesinde `409 Conflict` ve `ALREADY_EXISTS` hata koduyla koruyoruz.*  
> *İkincisi ise proje şartnamemizdeki D08 Ödül Döngüsü Kuralı: Damga sayısı hedef damgaya (örneğin 7) ulaştığında sistem otomatik olarak `RewardEarned` olayı oluşturur, mevcut damgayı sıfırlar ve `LifetimeStamps` değerini artırarak yeni döngüyü başlatır."*

---

### [02:30 – 03:55] Bölüm 4: Canlı Uygulama — Swagger UI ile Tam CRUD Testi
* **Ekranda Ne Olacak:** Tarayıcıda Swagger UI (`https://localhost:7246/swagger`).
* **Ne Yapılacak (Sırasıyla Canlı Çalıştırma):**
  1. **GET `/api/v1/StampCards`:** "Try it out" -> "Execute" diyerek 200 OK yanıtını ve kart listesini gösterin.
  2. **POST `/api/v1/StampCards`:** "Try it out" deyin. Örnek JSON ile yeni bir kart oluşturun:
     ```json
     {
       "userId": "admin-id-veya-kullanici-id",
       "programId": 1,
       "initialStamps": 2
     }
     ```
     Execute'a basın -> `201 Created` yanıtını ve dönen DTO'yu gösterin.
  3. **409 Conflict Testi:** Aynı verilerle tekrar "Execute" butonuna basın -> Ekranda dönen `409 Conflict` ve *"Bu müşteri için belirtilen programda zaten tanımlı bir damga kartı mevcuttur"* uyarısını gösterin.
  4. **GET `/api/v1/StampCards/{id}`:** Az önce oluşturulan kartın ID'sini yazıp çalıştırın -> Detayları ve `events` geçmişindeki manuel başlangıç damgasını gösterin.
  5. **PUT `/api/v1/StampCards/{id}`:** Hedef 7 damga ise `currentStamps: 7` gönderin -> Yanıtta damganın sıfırlandığını (`currentStamps: 0`), `lifetimeStamps`'in 7 olduğunu ve ödül döngüsünün çalıştığını gösterin.
  6. **DELETE `/api/v1/StampCards/{id}`:** Kart ID'sini girip çalıştırın -> `204 No Content` yanıtını gösterin.
* **Seslendirilecek Metin:**
> *"Şimdi geliştirdiğimiz API'yi canlı olarak Swagger arayüzünde test edelim:*  
>  
> *Gördüğünüz gibi `/swagger` ekranımızda tüm uç noktalar, parametre açıklamaları ve HTTP durum kodları detaylıca yer alıyor.*  
>  
> *Öncelikle `GET /api/v1/StampCards` ile sistemdeki kartları listeliyoruz. Filtreleme desteğimiz sayesinde kullanıcıya veya işletmeye göre de sorgu atabiliyoruz.*  
>  
> *Şimdi `POST` metodunu kullanarak yeni bir damga kartı tanımlıyorum. 2 başlangıç damgası veriyorum; Execute dediğimde `201 Created` koduyla kartımız anında oluşuyor.*  
>  
> *Aynı kullanıcıya aynı program için mükerrer bir kart oluşturmayı denediğimde, sistemimizin bunu engelleyerek `409 Conflict` ve açıklayıcı Türkçe hata mesajı verdiğini görüyoruz.*  
>  
> *`GET by ID` ile bu kartın işlem olay geçmişine erişebiliyoruz.*  
>  
> *Şimdi `PUT` ile damga sayısını program hedefi olan 7'ye güncelliyorum. Execute dediğimizde D08 kuralımız devreye giriyor: Kart damgası otomatik olarak sıfırlanıyor, toplam ömür boyu damga 7'ye yükseliyor ve ödül kazanma olayı kaydediliyor.*  
>  
> *Son olarak `DELETE` isteği ile kartı sildiğimizde `204 No Content` ile işlemin başarıyla tamamlandığını doğruluyoruz."*

---

### [03:55 – 04:40] Bölüm 5: Canlı Uygulama — İşletme Personel / Kasiyer Yönetimi (`ShopMembers`)
* **Ekranda Ne Olacak:** Tarayıcı → `/Shops/Details/1` sayfası.
* **Ne Yapılacak:**
  1. İşletme detayında "Kasiyerler ve Personel (ShopMembers)" tablosunu gösterin.
  2. "Personel / Kasiyer Ata" formuna kayıtlı bir kullanıcının e-postasını (`kasiyer@damgio.com` vb.) yazın.
  3. Rol olarak "Kasiyer (Cashier)" seçip "+ Personel Olarak Ekle" butonuna basın.
  4. Sayfa yenilendiğinde yeşil alert mesajını ve tabloda beliren kasiyer satırını gösterin.
* **Seslendirilecek Metin:**
> *"3. haftanın bir diğer önemli geliştirmesi ise İşletme Personel ve Kasiyer Yönetimi (`ShopMembers`).*  
>  
> *Kafelerde damga basacak veya ödül kullandıracak personellerin sisteme tanımlanması gerekiyor. Bunun için işletme detay ekranımıza personel yönetim modülünü entegre ettik.*  
>  
> *Burada mevcut personelleri görebiliyoruz. Şimdi formdan sisteme kayıtlı bir kullanıcının e-postasını girip 'Kasiyer' rolünü seçerek ekliyorum.*  
>  
> *Backend'de kullanıcının varlığı, aynı işletmeye mükerrer atanıp atanmadığı denetleniyor ve Identity üzerinden kullanıcıya otomatik olarak `Cashier` rolü tanımlanıyor. İstenildiğinde personeli şubeden tek tıkla çıkarabiliyoruz."*

---

### [04:40 – 05:10] Bölüm 6: Birim Testleri & iOS OpenAPI Sözleşmesi
* **Ekranda Ne Olacak:** Terminalde `dotnet test` çalıştırması ve VS Code `contracts/openapi-v1.json`.
* **Ne Yapılacak:**
  1. Terminale geçip `dotnet test` yazın ve Enter'a basın -> **59 testin tamamının Passed** olduğunu gösterin.
  2. VS Code'da `contracts/openapi-v1.json` dosyasını açıp birkaç satır gösterin.
* **Seslendirilecek Metin:**
> *"Sistemimizin güvenilirliğini birim testleriyle garanti altına aldık. Terminalde `dotnet test` komutunu çalıştırıyorum.*  
>  
> *Gördüğünüz gibi DTO validasyonları, D08 ödül döngüsü, damga soğuma süreleri ve personel kurallarını test eden 59 birim testimizin tamamı sıfır hata ile başarıyla geçiyor.*  
>  
> *Ayrıca projemizin `contracts` klasöründe yer alan `openapi-v1.json` dosyasını ürettik. Bu dosya, Zehra'nın geliştirdiği kardeş iOS uygulamamız (`damgio-ios`) için tek doğruluk kaynağıdır (Single Source of Truth); Swift tarafındaki modeller ve ağ istekleri doğrudan bu OpenAPI şemasına göre üretilmektedir."*

---

### [05:10 – 05:35] Bölüm 7: Kapanış, Mühendislik El Kitabı ve Gelecek Hafta
* **Ekranda Ne Olacak:** VS Code → `docs/ENGINEERING.md` (Checklist ve Work Log kısmı).
* **Ne Yapılacak:** `docs/ENGINEERING.md` dosyasındaki "Web API full CRUD + Swagger (W3)" maddesinin işaretli olduğu gösterilir.
* **Seslendirilecek Metin:**
> *"Özetle 3. haftamızda; Web API tam CRUD operasyonlarımızı, Swagger UI entegrasyonumuzu, iOS API sözleşmemizi, ShopMembers personel yönetimimizi ve 59 birim testimizi eksiksiz olarak tamamladık.*  
>  
> *Gelecek 4. haftamızda ise müşterilerin kartlarını tarayıcıda açacağı `/s/{slug}` mobil uyumlu arayüzünü ve NFC/QR damga akışını geliştireceğiz.*  
>  
> *Bizi dinlediğiniz için teşekkür ederiz."*

---

## 💡 Hoca Sorarsa — Hızlı Cevap Rehberi

* **Soru 1: Entity'leri doğrudan Controller'dan dönmek yerine neden DTO kullandınız?**
  * **Cevap:** *"İki kritik sebeple hocam: Birincisi güvenlik; Entity içindeki hassas kolonları veya Identity bağımlılıklarını dışarıya açmamak. İkincisi ise mimari tutarlılık; Entity'ler arasındaki döngüsel referansların (circular reference) JSON serileştirmede hata vermesini önlemek ve mobil istemci ile olan sözleşmemizi veritabanı şemasından bağımsız kılmak için DTO kullandık."*

* **Soru 2: Swagger arayüzündeki açıklamalar nereden geliyor?**
  * **Cevap:** *"C# kodlarımızdaki XML dokümantasyon etiketlerinden (`<summary>`, `<param>`, `<response>`) geliyor hocam. `.csproj` dosyamızda `<GenerateDocumentationFile>true</GenerateDocumentationFile>` ayarını açtık ve `Program.cs`'te `options.IncludeXmlComments(xmlPath)` ile SwaggerGen motoruna bağladık."*

* **Soru 3: D08 Ödül Döngüsü iş kuralı nasıl çalışıyor?**
  * **Cevap:** *"Bir kartta damga sayısı programın `RequiredStamps` değerine ulaştığında `RewardEarned` olayı veritabanına eklenir. `CurrentStamps` sıfırlanarak yeni döngü başlar, ancak müşterinin geçmişte kazandığı tüm damgaları takip eden `LifetimeStamps` artmaya devam eder."*

* **Soru 4: Mükerrer kart oluşturma nasıl engelleniyor?**
  * **Cevap:** *"Çift katmanlı koruma sağladık: Veritabanında `(UserId, ProgramId)` üzerinde Unique Index kısıtı bulunuyor. Controller katmanında ise istek geldiğinde önce `AnyAsync` ile kontrol edilerek müşteriye anlaşılır bir `409 Conflict` (`ALREADY_EXISTS`) HTTP durum kodu döndürülüyor."*

* **Soru 5: iOS uygulaması ile Web API nasıl haberleşecek?**
  * **Cevap:** *"Ürettiğimiz `contracts/openapi-v1.json` sözleşmesi sayesinde iOS istemcimiz (SwiftUI) backend modellerini ve API rotalarını birebir OpenAPI standartlarında tüketiyor. Böylece iki platform arasında sözleşme ihlali yaşanmıyor."*
