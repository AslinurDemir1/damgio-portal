# API sözleşmesi

`openapi-v1.json` — Damgio Web API'nin (v1) OpenAPI tanımı. **Hafta 3**'te StampCards API ile birlikte eklenir.

- Kaynak: `Damgio.Web` çalışırken `/swagger/v1/swagger.json`.
- Tüketici: [damgio-ios](https://github.com/zehravural1/damgio-ios) — Swift istemcisi bu dosyadan üretilir.
- Kural: API'de kırıcı değişiklik yapılırsa önce bu dosya güncellenir ve iOS tarafına haber verilir. Kırıcı değişiklik `v2` ile yapılır.
