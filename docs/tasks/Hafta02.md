# Hafta 02 — Görevler (26 Ekim – 1 Kasım 2026)

**Lider:** Aslınur Demir · **Video:** `Hafta02_Damgio.mp4` → Drive, **1 Kasım 2026 Pazar 23:59**

## Aslınur — Modeller, Slug Yönetimi ve Testler

Branch: `feat/slug-and-models` → PR → Zehra review eder.

- [x] URL için `SlugHelper` sınıfı (`Damgio.Core/Common/SlugHelper.cs`):
  - Türkçe karakter desteği (`ç->c`, `ğ->g`, `ı->i`, `ö->o`, `ş->s`, `ü->u`, `İ/I->i`)
  - URL uyumlu slug formatı (küçük harf, alfanümerik ve tek tire)
  - Rezerve slug koruması (`admin`, `api`, `desk`, `s`, `account`, `login`, `swagger`)
- [x] Birim testleri (`Damgio.Tests/SlugHelperTests.cs`):
  - Türkçe başlıkların slug dönüşüm testleri
  - Özel karakter ve birden fazla tire temizleme testleri
  - Rezerve slug tespit ve engelleme testleri
- [x] Model kısıtlamaları ve doğrulama kurallarının (`Shop`, `LoyaltyProgram`, `StampCard`) incelenmesi
- [x] Zehra için CRUD ViewModelleri (`ShopInputModel`, `ProgramInputModel`) ve validasyon sözleşmelerinin hazırlanması
- [x] 2. Hafta video senaryosu `docs/videos/Hafta02.md`
- [ ] Zehra'nın açtığı CRUD PR'larının incelenmesi (D11: merge commit) ve entegrasyonu

## Zehra — MVC CRUD ve Form Doğrulama (Validation)

Branch: `zehra` → PR → Aslınur review eder.

- [ ] `ShopsController` (MVC Controller — `[Authorize(Roles = Roles.Admin)]`):
  - `Index`: Tüm işletmeleri listeleme (aktiflik rozeti, slug, program sayısı)
  - `Create`: Yeni işletme ekleme (otomatik slug tamamlama / elle slug girişi)
  - `Edit`: İşletme bilgilerini ve aktiflik durumunu güncelleme
  - `Details`: İşletme detayları ve bağlı programların özeti
  - `Delete`: İşletmeyi silme veya pasife alma
- [ ] `ProgramsController` (MVC Controller — `[Authorize(Roles = Roles.Admin)]`):
  - Belirli bir işletmeye bağlı LoyaltyProgram oluşturma (`ShopId`, `Name`, `RequiredStamps`, `RewardName`, `StampCooldownSeconds`)
  - Program düzenleme ve listeleme
- [ ] Form Doğrulamaları (DataAnnotations + UI):
  - `ShopInputModel` / `ProgramInputModel` üzerinde zorunlu alanlar, metin uzunlukları, sayı aralıkları (`[Range(1, 50)]`)
  - İstemci taraflı anlık doğrulama (`_ValidationScriptsPartial`, jQuery Validate)
  - Sunucu taraflı doğrulama (`ModelState.IsValid`) ve model state hata mesajları
  - Slug çakışması ve rezerve slug hata mesajlarının formda gösterilmesi
- [ ] `Views/Shared/_Layout.cshtml`: Admin rolü için üst menüye "İşletmeler" ve "Programlar" bağlantılarının eklenmesi

## Hafta Sonu Kabul Kriterleri

- Tüm birim testleri (`dotnet test`) hatasız geçer.
- Admin hesabı ile giriş yapıldığında menüde işletme/program yönetimi görünür.
- Yeni bir işletme (ör. "Kadıköy Damgio Cafe", slug: `kadikoy-damgio-cafe`) eklenebilir.
- Formlarda boş bırakılan veya geçersiz alanlarda (ör. damga sayısı 0 veya 51 girildiğinde, rezerve slug yazıldığında) anında Türkçe doğrulama hatası verilir.
- Eklenen işletmeye ait bir damga programı (ör. "7 Kahveye 1 İkram", 7 damga, 180 sn soğuma) oluşturulabilir ve listelenebilir.
- Admin olmayan kullanıcılar bu yönetim sayfalarına doğrudan URL ile de erişemez (`403 Access Denied` veya login yönlendirmesi).
