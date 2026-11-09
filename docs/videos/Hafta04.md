# Hafta 04 — Video Kayıt Senaryosu ve Konuşma Metni

**Dosya:** `Hafta04_Damgio.mp4` · **Hedef Süre:** 5:30 dk (En az 5 dk kuralı)  
**Lider / Sunan:** Aslınur Demir · **Teslim:** 15 Kasım 2026 Pazar 23:59  

---

## 🎬 Video Öncesi Hazırlık (Masayı ve Ekranı Ayarlama)

1. **Uygulamayı Çalıştır:**  
   Terminalde `Damgio.Web` projesini çalıştırın:
   ```bash
   dotnet run --project src/Damgio.Web
   ```
   Uygulama `https://localhost:7180` (veya varsayılan portunuzda) çalışır olsun.

2. **Tarayıcı Sekmelerini Hazırla:**
   - **Sekme 1:** `https://localhost:7180/` (Ana sayfa: 4. Hafta mobil kart tanıtımı ve hızlı erişim butonları).
   - **Sekme 2:** `https://localhost:7180/s/kadikoy-damgio-cafe` (Gizli sekmede veya çıkış yapılmış halde: Ziyaretçi önizleme görünümü).
   - **Sekme 3:** `https://localhost:7180/s/kadikoy-damgio-cafe` (Müşteri ile giriş yapılmış: `musteri@damgio.com` / `Musteri123*`).
   - **Sekme 4:** `https://localhost:7180/swagger` (Swagger UI: `/api/v1/me/cards` ve `/api/v1/tap` uç noktaları).
   - **Sekme 5:** GitHub PR / commits sayfası (`aslinur` branch'i ve Zehra'nın PR incelemesi).

3. **VS Code Sekmelerini Sırasıyla Açık Bırak:**
   - **Tab 1:** `src/Damgio.Web/Controllers/CustomerCardController.cs` (Mobil Kart & Tap Controller)
   - **Tab 2:** `src/Damgio.Web/Views/CustomerCard/Index.cshtml` (Mobil Uyumlu Razor View)
   - **Tab 3:** `src/Damgio.Web/wwwroot/css/site.css` (Tasarım token'ları & animasyonlar)
   - **Tab 4:** `src/Damgio.Core/Common/DeviceKeyHelper.cs` (SHA-256 tek yönlü anahtar özeti)
   - **Tab 5:** `src/Damgio.Web/Controllers/Api/CustomerController.cs` (Mobil/iOS API uç noktaları)
   - **Tab 6:** `contracts/openapi-v1.json` (Güncellenen OpenAPI sözleşmesi)
   - **Tab 7:** `tests/Damgio.Tests/CustomerCardTests.cs` (Birim testleri)

4. **Terminal:**
   Açık ve temiz bir pencere olsun, `dotnet test` komutunu çalıştırmaya hazır beklesin.

---

## 🎙️ Sahne Sahne Çekim ve Konuşma Rehberi

---

### [00:00 – 00:45] Bölüm 1: Giriş ve 4. Hafta Hedefleri
* **Ekranda Ne Olacak:** Ana sayfa (`https://localhost:7180/`) ve üst menüdeki "Hafta 4 Müşteri Kartı (/s)" butonu.
* **Ne Yapılacak:** Fare ile ana sayfadaki 4. Hafta kartı ve menü gösterilir.
* **Seslendirilecek Metin:**
> *"Merhaba hocam, BLM4581 Ağ Tabanlı Teknolojiler ve Uygulamaları dersi için geliştirdiğimiz Damgio projesinin 4. hafta tanıtım videosuna hoş geldiniz. Ben grup lideri Aslınur Demir.*  
>  
> *Geçtiğimiz 3. haftada damga kartlarımızın Web API full CRUD mimarisini, Swagger dokümantasyonunu ve iOS sözleşmesini tamamlamıştık.*  
>  
> *Bu 4. haftamızda ise projemizin kalbi olan müşteri deneyimine ve mobil arayüze odaklandık. Müşterilerimizin masadaki QR kodunu okutarak veya NFC'ye dokunarak tarayıcıda açtığı `/s/{slug}` mobil damga kartını, tokmak dokunma akışını (`/s/{slug}/tap`), soğuma süresi (D07) ve ödül döngüsü (D08) iş kurallarını geliştirdik. Ayrıca iOS istemcimiz için yeni müşteri API uç noktalarımızı entegre ettik."*

---

### [00:45 – 01:30] Bölüm 2: Görev Paylaşımı ve Git İş Akışı
* **Ekranda Ne Olacak:** GitHub commits ve Pull Requests sayfası.
* **Ne Yapılacak:** `aslinur` branch'i, commit geçmişi ve merge kuralları ekranda vurgulanır.
* **Seslendirilecek Metin:**
> *"Mühendislik el kitabımızın D11 ve D12 kurallarına uygun olarak bu hafta da görev dağılımımızı branch ve PR yapısıyla yürüttük.*  
>  
> *Ben Aslınur olarak; `/s/{slug}` mobil kart arayüzünü, `DeviceKeyHelper` ile NFC cihazlarının SHA-256 güvenlik katmanını, tokmak dokunma ve ödül döngüsü algoritmasını, `/api/v1/me/cards` ve `/api/v1/tap` API servislerini ve kapsamlı birim testlerini hazırladım.*  
>  
> *Arkadaşım Zehra ise; hazırladığım `aslinur` branch'i için GitHub üzerinde kod incelemesini (code review) gerçekleştirdi ve merge commit ile ana dalımızla birleştirdi. Ayrıca kasiyerlerin masaüstünden manuel damga basıp ödül kullandıracağı `/Desk` ekranının ve `damgio-ios` mobil istemcimizin müşteri modellerinin entegrasyonu üzerinde çalıştı."*

---

### [01:30 – 02:45] Bölüm 3: Mobil Müşteri Kartı Arayüzü (`/s/{slug}`)
* **Ekranda Ne Olacak:** Tarayıcıda mobil görünüm moduna (F12 -> Responsive / iPhone boyutu) alınmış `/s/kadikoy-damgio-cafe` sayfası.
* **Ne Yapılacak:**
  1. Önce giriş yapılmamış durum gösterilir: İşletme adı, 7 damga hedefi, "Giriş Yap ve Kartı Gör" butonu.
  2. Müşteri hesabı ile giriş yapılır (`musteri@damgio.com` / `Musteri123*`).
  3. Otomatik kayıt (auto-enrollment) sayesinde kartın açıldığı, 7 adet dairesel yuva, kahve damgaları, ödül yuvası ve ilerleme çubuğu gösterilir.
* **Seslendirilecek Metin:**
> *"Şimdi projemizin en önemli arayüzünü canlı olarak inceleyelim. Bir müşteri kafeye gittiğinde masadaki QR kodu tarattığında doğrudan `/s/kadikoy-damgio-cafe` adresine ulaşır.*  
>  
> *Eğer müşteri ilk defa geliyorsa ve henüz giriş yapmamışsa, sistem işletme logosunu, adresini ve kazanacağı ödülü göstererek sade bir giriş ve kayıt ekranı sunar.*  
>  
> *Müşteri giriş yaptığında ise sistem otomatik olarak (auto-enrollment) müşteriye bu işletme için dijital bir kart tanımlar. Tasarımımızda `design/tokens.json` dosyasındaki sıcak kafe renklerini (terracotta, forest yeşili ve krem) kullandık. Kart üzerinde 7 adet damga yuvası, +1 hediye kutusu yuvası, mevcut damga ilerleme çubuğu ve son hareket geçmişi yer alıyor.*  
>  
> *Uygulama gerektirmeden, her türlü akıllı telefon tarayıcısında anında ve akıcı bir native mobil hissi vermektedir."*

---

### [02:45 – 04:00] Bölüm 4: NFC Tokmak Dokunma Akışı, Soğuma Süresi ve Ödül Döngüsü
* **Ekranda Ne Olacak:** Müşteri kartı açıkken alt kısımdaki demo tokmak simülatörüne tıklanır veya URL ile `/s/kadikoy-damgio-cafe/tap?k=damgio-tokmak-secret-2026` tetiklenir.
* **Ne Yapılacak:**
  1. Tokmak butonuna basılır -> 1 damga eklenir, yeşil onay bildirimi çıkar ve damga sayısı artar.
  2. Hemen ardından bir daha basılır -> "Çok hızlı damga basamazsınız! Lütfen bekleyin" uyarısı çıkar ve **canlı geri sayım sayacı** (cooldown) gösterilir.
  3. Ardından hedef damgaya (7/7) ulaşıldığında -> Tebrikler kutusu patlar, "Ödül Kazandınız!" kutlaması çıkar, damga sayısı 0'a sıfırlanır ve yeni döngü başlar (D08 kuralı).
* **Seslendirilecek Metin:**
> *"Şimdi NFC tokmak akışımızı test edelim. Masada veya kasada duran NFC tokmak cihaza telefon dokundurulduğunda cihazın NTAG çipi `/s/{slug}/tap?k=...` adresini tetikler.*  
>  
> *Burada Karar D09 gereği cihazın `k` anahtarı veritabanında asla açık tutulmaz; SHA-256 özeti `Devices` tablomuzla eşleştirilir. Geçersiz veya yabancı bir cihaz anında reddedilir.*  
>  
> *Butona basarak dokunmayı simüle ediyoruz: Gördüğünüz gibi 1 damgamız başarıyla eklendi ve işlem geçmişine NFC dokunma olarak işlendi.*  
>  
> *Şimdi kötüye kullanımı önleyen Karar D07 soğuma süresini (cooldown) test edelim. Arka arkaya hızlıca tekrar dokunulduğunda sistem isteği engeller ve ekranda kalan saniyeyi gösteren canlı bir geri sayım sayacı çalışır.*  
>  
> *Damgalarımız 7'ye ulaştığında ise Karar D08 devreye girer: Tebrik kutusu çıkar, müşteriye 1 Adet Filtre Kahve ödülü tanımlanır ve döngü sıfırlanarak müşterinin yeni ödül için damga toplaması sağlanır."*

---

### [04:00 – 04:45] Bölüm 5: Web API Uç Noktaları ve Kod İncelemesi
* **Ekranda Ne Olacak:** Swagger UI (`/swagger`) ve ardından VS Code'da `CustomerCardController.cs` ile `DeviceKeyHelper.cs`.
* **Ne Yapılacak:** Swagger'da `/api/v1/me/cards` ve `/api/v1/tap` uç noktaları gösterilir; ardından VS Code'da kodlar gösterilir.
* **Seslendirilecek Metin:**
> *"Aynı iş mantığını kardes iOS istemcimizin de kullanabilmesi için Web API tarafına taşıdık. Swagger dokümantasyonumuzda görebileceğiniz gibi:*  
>  
> *`GET /api/v1/me/cards` uç noktası müşterinin sahip olduğu tüm kartları ve soğuma durumlarını JSON olarak döner.*  
> *`POST /api/v1/tap` uç noktası ise iOS cihazından NFC okutulduğunda aynı güvenlik ve ödül döngüsünü işleterek güncel kart sonucunu iletir.*  
>  
> *Kod tarafında `CustomerCardController` katmanlı mimarimize tam uyumlu olarak EF Core ile çalışır, `DeviceKeyHelper` sınıfımız tek yönlü SHA-256 doğrulamasını gerçekleştirir ve `contracts/openapi-v1.json` dosyamız iOS ekibi için güncel şemayı sunar."*

---

### [04:45 – 05:30] Bölüm 6: Birim Testleri ve Kapanış
* **Ekranda Ne Olacak:** Terminal penceresi tam ekran.
* **Ne Yapılacak:** Terminalde `dotnet test` komutu çalıştırılır ve tüm testlerin yeşil geçtiği gösterilir (79 passed).
* **Seslendirilecek Metin:**
> *"Son olarak geliştirdiğimiz tüm bu iş kurallarının ve güvenlik mantığının xUnit birim testlerini çalıştıralım.*  
>  
> *(Terminalde `dotnet test` komutu girilir)*  
>  
> *Gördüğünüz gibi; SHA-256 anahtar doğrulama, soğuma süresi ihlalleri, hedef damga ödül döngüsü sıfırlaması, DTO validasyonları ve otomatik kayıt mantığı dahil olmak üzere toplam 79 birim testimizin tamamı yeşil olarak hatasız geçmiştir.*  
>  
> *Böylece 4. haftamızı başarıyla tamamladık. Gelecek 5. haftamızda ise kasiyer kasa panelinin arayüzünü (`/Desk`) ve istatistik raporlama ekranlarımızı genişleteceğiz. Bizi dinlediğiniz için teşekkür ederiz."*
