# Hafta 02 — Video Kayıt Senaryosu ve Konuşma Metni

**Dosya:** `Hafta02_Damgio.mp4` · **Hedef Süre:** 5:30 dk (En az 5 dk kuralı)  
**Lider / Sunan:** Aslınur Demir · **Teslim:** 1 Kasım 2026 Pazar 23:59  

---

## 🎬 Video Öncesi Hazırlık (Masayı ve Ekranı Ayarlama)

1. **Uygulamayı Çalıştır:** Terminalde `Damgio.Web` projesini çalıştırın (`dotnet run`), tarayıcıda `https://localhost:7001` adresinde hazır olsun.
2. **Admin ile Giriş Yap:** Birinci tarayıcı sekmesinde Admin hesabıyla oturum açın (Navbar'da *İşletmeler*, *Programlar* ve *Admin Paneli* görünsün).
3. **Gizli Sekme Hazırla:** İkinci bir pencerede giriş yapılmamış gizli sekme açın.
4. **GitHub Sekmesi:** `https://github.com/AslinurDemir1/damgio-portal/commits/main` sekmesi açık olsun.
5. **VS Code Sekmeleri (Sırasıyla açık kalsın):**
   - Tab 1: `Shop.cs` ve `LoyaltyProgram.cs`
   - Tab 2: `AppDbContext.cs`
   - Tab 3: `SlugHelper.cs`
   - Tab 4: `SlugHelperTests.cs`
   - Tab 5: `ShopInputModel.cs`
   - Tab 6: `ShopsController.cs`

---

## 🎙️ Sahne Sahne Çekim ve Konuşma Rehberi

---

### [00:00 – 00:40] Bölüm 1: Giriş ve Proje Özeti
* **Ekranda Ne Olacak:** Tarayıcı açık; Damgio ana sayfası ve ardından tıkla `/Admin` dashboard ekranı.
* **Ne Yapılacak:** Fare ile ana sayfadaki Damgio logosu ve ardından Admin Paneli kartları gösterilir.
* **Seslendirilecek Metin:**
> *"Merhaba hocam, BLM4581 Ağ Tabanlı Teknolojiler ve Uygulamaları dersi için geliştirdiğimiz Damgio projesinin 2. hafta tanıtım videosuna hoş geldiniz. Ben bu haftanın grup lideri Aslınur Demir.*  
>  
> *Damgio; kafeler ve işletmeler için geliştirdiğimiz, kâğıt damga kartlarının yerini alan, NFC ve QR tabanlı dijital bir sadakat kartı platformudur.*  
>  
> *Bu haftaki ana hedefimiz; işletmeler ve damga programları için veri modellerinin hazırlanması, güvenli URL slug altyapısının kurulması, Admin yönetim paneli için tam MVC CRUD ekranlarının yazılması ve dersimizin zorunlu isterlerinden olan form doğrulamalarının (validation) hem istemci hem sunucu tarafında hayata geçirilmesiydi."*

---

### [00:40 – 01:25] Bölüm 2: Haftalık Görev Dağılımı ve Git Süreci
* **Ekranda Ne Olacak:** GitHub repo sayfası → `Commits` ve `Network/Graph` görünümü.
* **Ne Yapılacak:** Ekranda Aslınur ve Zehra'nın commitleri, branch birleştirmeleri ve merge commit'ler fare ile gösterilir.
* **Seslendirilecek Metin:**
> *"Proje el kitabımızdaki kurallar doğrultusunda bu hafta iş bölümümüzü şu şekilde yaptık:*  
>  
> *Ben grup lideri olarak; veri modellerinin kısıtlamalarını, Türkçe karakterleri URL'e dönüştüren ve rezerve rotaları koruyan `SlugHelper` sınıfını, bunların birim testlerini ve CRUD için gereken form modellerini hazırladım.*  
>  
> *Arkadaşım Zehra ise; 1. haftanın Identity ve rol atama altyapısını tamamladıktan sonra, bu modeller üzerinden işletme ve programların yönetimini sağlayan MVC CRUD controller'larını ve arayüzlerini geliştirdi.*  
>  
> *Git sürecinde `aslinur` ve `zehra` dallarımızı kullandık, kodları inceledikten sonra projenin merge commit kuralına uygun şekilde `main` dalında birleştirdik."*

---

### [01:25 – 02:25] Bölüm 3: Modeller, İlişkiler ve Veritabanı Kısıtlamaları
* **Ekranda Ne Olacak:** VS Code → `Shop.cs`, `LoyaltyProgram.cs` ve `AppDbContext.cs`.
* **Ne Yapılacak:** `Shop` içindeki `ICollection<LoyaltyProgram>`, `LoyaltyProgram` içindeki `ShopId` ve `AppDbContext`'teki `CK_Programs_RequiredStamps` check constraint satırları gösterilir.
* **Seslendirilecek Metin:**
> *"Şimdi kod tarafına bakalım. Bu haftaki çekirdek modellerimiz `Shop` ve `LoyaltyProgram`.*  
>  
> *Gördüğünüz gibi bir işletmenin birden fazla damga programı olabiliyor; burada bire-çok (1─N) ilişki kurduk. İşletme silindiğinde programlarının da cascade olarak temizlenmesini sağladık.*  
>  
> *`AppDbContext` içinde ise PostgreSQL seviyesinde check constraint'ler ekledik. Örneğin bir programın hedef damga sayısı (`RequiredStamps`) 1 ile 50 arasında olmak zorunda, soğuma süresi negatif olamaz. Ayrıca her işletmenin internette benzersiz bir adrese sahip olması için `Slug` alanına veritabanında unique index uyguladık."*

---

### [02:25 – 03:25] Bölüm 4: SlugHelper Altyapısı ve Birim Testleri
* **Ekranda Ne Olacak:** VS Code → `SlugHelper.cs`, ardından `SlugHelperTests.cs` ve Terminal (`dotnet test`).
* **Ne Yapılacak:** `SlugHelper.ToSlug` içindeki Türkçe harf eşlemeleri gösterilir. Rezerve slug listesi (`admin`, `api`, `login`) vurgulanır. Terminalde `dotnet test` yazılarak çalıştırılır ve tüm testlerin yeşil geçtiği gösterilir.
* **Seslendirilecek Metin:**
> *"Müşteriler masadaki NFC'ye dokunduğunda veya QR okuttuğunda `/s/kadikoy-damgio-cafe` gibi temiz bir URL'e giderler. İşletme adındaki 'ç, ğ, ı, ö, ş, ü' gibi Türkçe karakterlerin bozulmadan URL formatına dönüşmesi için `SlugHelper` sınıfını yazdık.*  
>  
> *Ayrıca sistemin kendi rotaları olan 'admin', 'api', 'login' veya 'swagger' gibi kelimelerin kafe slug'ı olarak seçilmesini engelleyen rezerve rota denetimi ekledik.*  
>  
> *Yazdığımız bu mantığı `SlugHelperTests` birim testleriyle güvence altına aldık. Terminalden `dotnet test` komutunu çalıştırdığımızda tüm Türkçe karakter ve rezerve kelime testlerimizin başarıyla geçtiğini görüyoruz."*

---

### [03:25 – 04:45] Bölüm 5: Canlı Demo — Admin CRUD ve Form Doğrulamaları (Validation)
* **Ekranda Ne Olacak:** Tarayıcı (Admin açık).
* **Ne Yapılacak (Adım Adım Canlı Uygulama):**
  1. Üst menüden **"İşletmeler"** bağlantısına tıklayın (`/Shops`).
  2. **"+ Yeni İşletme Ekle"** butonuna basın.
  3. Formu **boş bırakıp "Kaydet"** butonuna basın -> Ekranda anında çıkan kırmızı Türkçe hata mesajlarını gösterin.
  4. Slug alanına `admin` yazın -> *"Sistem tarafından rezerve edilmiş"* hatasını gösterin.
  5. Doğru bilgiler girin: Ad: `Damgio Moda Kahvecisi`, Adres: `Kadıköy/İstanbul`. Slug'ı boş bırakın.
  6. "Kaydet"e basın -> Listede otomatik oluşan `damgio-moda-kahvecisi` slug'ını gösterin.
  7. Menüden **"Programlar"** sekmesine geçin (`/Programs`).
  8. **"+ Yeni Program Ekle"** diyerek az önce eklediğiniz işletmeyi seçin.
  9. Damga sayısına `99` yazın -> 1–50 arası sınır uyarısını gösterin; ardından `7` damga, `1 Ücretsiz Filtre Kahve` girerek kaydedin.
* **Seslendirilecek Metin:**
> *"Şimdi uygulamanın çalışan arayüzünü inceleyelim. Menüde sadece Admin rolüne açık olan 'İşletmeler' ve 'Programlar' bağlantılarımız yer alıyor.*  
>  
> *'İşletmeler' sekmesine girip 'Yeni İşletme Ekle' diyorum. Dersimizin zorunlu isterlerinden olan Form Doğrulamasını (Validation) test edelim:*  
>  
> *Formu boş gönderdiğimde, hem istemci tarafında jQuery Unobtrusive hem de sunucu tarafında `ModelState.IsValid` devreye giriyor ve sayfa yenilenmeden alanların zorunlu olduğu uyarısı veriliyor.*  
>  
> *Slug alanına 'admin' yazdığımda sistemimizin rezerve kontrolü tetikleniyor. Şimdi geçerli bir işletme adı giriyorum ve slug alanını boş bırakıyorum; kaydettiğimde sistemin işletme adını otomatik olarak temiz ve Türkçe uyumlu bir slug'a dönüştürdüğünü görüyoruz.*  
>  
> *Aynı şekilde 'Programlar' sekmesinden bu işletmeye bağlı bir sadakat programı tanımlıyoruz. 1-50 arası sınır doğrulamamız burada da çalışıyor. Detay, düzenleme ve silme işlemlerimiz eksiksiz işliyor."*

---

### [04:45 – 05:15] Bölüm 6: Yetkilendirme (Authorization) Güvenlik Testi
* **Ekranda Ne Olacak:** Giriş yapılmamış Gizli Tarayıcı Penceresi.
* **Ne Yapılacak:** Adres çubuğuna doğrudan `https://localhost:7001/Shops` ve ardından `https://localhost:7001/Admin` yazıp Enter'a basın. Sayfanın derhal `/Account/Login` sayfasına yönlendiğini gösterin.
* **Seslendirilecek Metin:**
> *"Bir diğer önemli konu ise güvenlik ve yetkilendirme. Normal bir müşteri veya sisteme hiç giriş yapmamış birisi bu yönetim ekranlarına erişemez.*  
>  
> *Bunu kanıtlamak için gizli sekme açıyorum ve adres çubuğuna elle `/Shops` yazıyorum. Gördüğünüz gibi `[Authorize(Roles = Roles.Admin)]` filtresi devreye giriyor ve kullanıcıyı doğrudan giriş ekranına yönlendiriyor. Admin olmayan hiç kimse bu sayfalara erişemez."*

---

### [05:15 – 05:40] Bölüm 7: Kapanış ve Gelecek Hafta Hedefleri
* **Ekranda Ne Olacak:** VS Code `docs/tasks/Hafta02.md` veya `docs/ENGINEERING.md`.
* **Ne Yapılacak:** Hafta 2'deki tüm maddelerin yeşil tikli (`[x]`) olduğu gösterilir, Hafta 3 planına değinilir.
* **Seslendirilecek Metin:**
> *"Özetle 2. haftada; işletme ve program modellerimizi, Türkçe destekli slug yapımızı, Admin MVC CRUD sayfalarımızı, birim testlerimizi ve hem istemci hem sunucu taraflı form doğrulamalarımızı tamamladık.*  
>  
> *3. haftada ise müşterilerin damga kartlarını yöneteceğimiz StampCards için tam CRUD Web API'yi ve Swagger dokümantasyonunu geliştireceğiz.*  
>  
> *Bizi dinlediğiniz için teşekkür ederiz."*

---

## 💡 Hoca Sorarsa — Hızlı Cevap Rehberi

* **Soru: Validasyonu sadece tarayıcıda mı yaptınız, sunucuda mı?**
  * **Cevap:** *"İki tarafta da yaptık hocam. İstemci tarafında `_ValidationScriptsPartial` ile jQuery Validate anlık olarak formun boş gitmesini engelliyor; sunucu tarafında ise Controller'da `ModelState.IsValid` ile gelen verinin arka planda doğruluğunu garanti altına alıyoruz."*
* **Soru: Slug neden gerekli, ID kullansak olmaz mıydı?**
  * **Cevap:** *"Müşteri masadaki NFC veya QR kodu telefonuna okuttuğunda `/s/5` yerine `/s/kadikoy-moda-cafe` gibi kurumsal, akılda kalıcı ve okunabilir bir adresle karşılaşması kullanıcı deneyimi ve güvenlik açısından çok daha sağlıklıdır."*
* **Soru: Silme işleminde veritabanında ne oluyor?**
  * **Cevap:** *"Shop ile LoyaltyProgram arasında cascade delete tanımlıdır. Bir şube kapatılıp silindiğinde, ona bağlı damga programları da veritabanı tutarlılığı gereği otomatik olarak temizlenir."*
