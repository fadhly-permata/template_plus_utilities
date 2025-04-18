# Changelog

Semua perubahan penting pada proyek ini akan didokumentasikan dalam file ini.

## [1.0.6] - 2024-03-17

### Ditambahkan
- Penambahan file build script untuk berbagai platform:
  * Build script untuk PowerShell (.vscode/build.ps1)
  * Build script untuk Bash (.vscode/build.sh)
  * Build script untuk Windows Batch (.vscode/build.bat)
- Penambahan file rebuild script untuk berbagai platform:
  * Rebuild script untuk PowerShell (.vscode/rebuild.ps1)
  * Rebuild script untuk Bash (.vscode/rebuild.sh)
  * Rebuild script untuk Windows Batch (.vscode/rebuild.bat)
- Penambahan konfigurasi VS Code tasks (.vscode/tasks.json):
  * Task untuk security scanning (Ubuntu & Windows)
  * Task untuk cleaning solution (Ubuntu & Windows)
  * Task untuk building solution (Ubuntu & Windows)
  * Task untuk rebuilding solution (Ubuntu & Windows)
- Penambahan tema Swagger UI:
  * Tema Monokai dengan dukungan mode gelap/terang otomatis
  * Tema Material
  * Tema Feeling Blue
  * Tema Flattop
  * Tema Muted
  * Tema Newspaper
  * Tema Outline
- Penambahan konfigurasi keamanan:
  * File .trivyignore untuk pengecualian scan keamanan
  * File .trivyrc untuk konfigurasi scanner Trivy

### Diubah
- Peningkatan konfigurasi Swagger UI:
  * Penambahan dukungan tema dinamis
  * Peningkatan styling untuk mode gelap
  * Optimasi animasi dan transisi
  * Penyesuaian tata letak dan responsivitas
- Pembaruan konfigurasi build:
  * Optimasi proses build untuk berbagai platform
  * Penanganan dependencies yang lebih baik
  * Pembersihan file-file temporary

### File yang Diubah
- .vscode/build.ps1 (Baru)
- .vscode/build.sh (Baru)
- .vscode/build.bat (Baru)
- .vscode/rebuild.ps1 (Baru)
- .vscode/rebuild.sh (Baru)
- .vscode/rebuild.bat (Baru)
- .vscode/tasks.json (Diperbarui)
- .trivyignore (Baru)
- .trivyrc (Baru)
- IDC.Template/wwwroot/themes/theme-monokai-dark.css (Baru)
- IDC.Template/wwwroot/themes/theme-material.css (Baru)
- IDC.Template/wwwroot/themes/theme-feeling-blue.css (Baru)
- IDC.Template/wwwroot/themes/theme-flattop.css (Baru)
- IDC.Template/wwwroot/themes/theme-muted.css (Baru)
- IDC.Template/wwwroot/themes/theme-newspaper.css (Baru)
- IDC.Template/wwwroot/themes/theme-outline.css (Baru)
- IDC.Template/wwwroot/css/swagger-custom.css (Diperbarui)
- IDC.Template/wwwroot/js/swagger-theme-switcher.js (Diperbarui)
- IDC.Template/Program.Swagger.cs (Diperbarui)

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
