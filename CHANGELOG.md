# Changelog

Semua perubahan penting pada proyek ini akan didokumentasikan dalam file ini.

## [1.0.3] - 2024-03-17

### Ditambahkan
- Peningkatan dokumentasi pada README.md:
  * Penambahan deskripsi fitur-fitur baru
  * Revisi konten untuk kejelasan dan akurasi
  * Pembaruan informasi versi
  * Perbaikan format dan struktur dokumentasi

### File yang Diubah
- README.md

## [1.0.2] - 2024-03-17

### Ditambahkan
- Implementasi pengelompokan API berdasarkan tags di Swagger:
  * Filter untuk menghormati tags yang sudah ada
  * Pengelompokan otomatis untuk endpoint tanpa tag
  * Pengurutan dan pengorganisasian tag
  * Dokumentasi komprehensif untuk DefaultGroupDocFilter
  * Konfigurasi UI Swagger yang dapat disesuaikan
  * Dukungan tema yang ditingkatkan untuk endpoint terkelompok

### Diubah
- Peningkatan RateLimitingMiddleware:
  * Optimasi penanganan request
  * Peningkatan sistem pelacakan IP
  * Implementasi algoritma pembatasan yang lebih efisien
- Optimasi strategi caching:
  * Implementasi algoritma caching yang lebih efisien
  * Peningkatan performa penyimpanan cache
- Pembaruan .gitignore:
  * Penambahan pola baru untuk mengabaikan build artifacts
  * Pengecualian file-file yang tidak diperlukan
- Pembaruan file log:
  * Pencatatan aktivitas sistem terbaru
  * Dokumentasi error dan penanganannya

### File yang Diubah
- IDC.Template/Utilities/Middlewares/Swagger/DefaultGroupDocFilter.cs:
  * Peningkatan filter untuk tag yang ada
  * Penambahan pengelompokan otomatis
  * Implementasi pengurutan tag
  * Penambahan dokumentasi lengkap
- IDC.Template/Program.Swagger.cs:
  * Pembaruan konfigurasi Swagger
  * Integrasi DefaultGroupDocFilter
- IDC.Template/wwwroot/appconfigs.jsonc:
  * Penambahan opsi konfigurasi UI Swagger
- IDC.Template/wwwroot/css/swagger-custom.css:
  * Pembaruan styling untuk pengelompokan tag
- IDC.Template/wwwroot/js/swagger-theme-switcher.js:
  * Peningkatan dukungan tema
- RateLimitingMiddleware.cs:
  * Optimasi penanganan request
  * Peningkatan pelacakan IP
- Caching.cs:
  * Implementasi algoritma caching baru
- .gitignore:
  * Penambahan pola pengecualian
- logs-20250317.txt:
  * Pembaruan log aktivitas

## [1.0.1] - 2024-03-17

### Keamanan
- Implementasi autentikasi API key:
  * Sistem validasi API key
  * Pengelolaan akses berbasis token
- Penerapan pembatasan rate:
  * Konfigurasi batas request per IP
  * Sistem pencatatan dan monitoring
- Peningkatan keamanan sistem:
  * Validasi input yang lebih ketat
  * Enkripsi data sensitif
  * Penanganan error yang aman

### File yang Diubah
- Program.cs
- Startup.cs
- Security/ApiKeyAuthenticationHandler.cs
- Middleware/RateLimitingMiddleware.cs
- Configuration/SecuritySettings.cs
