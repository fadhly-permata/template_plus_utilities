# API Template + Utilities

Template API berbasis .NET 8 dengan berbagai utilitas yang memudahkan pengembangan API.

## Fitur Utama

### 1. Endpoint Generator
- Membuat endpoint API secara dinamis dari konfigurasi JSON
- Mendukung berbagai metode HTTP (GET, POST, PUT, DELETE)
- Konfigurasi routing dan parameter yang fleksibel
- Dukungan untuk callback functions
- Mendukung operasi database (SQLite, PostgreSQL, MongoDB)

### 2. Sistem Lokalisasi
- Manajemen pesan multi-bahasa menggunakan JSON
- Thread-safe operations
- Dukungan untuk nested message paths
- Kemudahan dalam menambah/mengubah pesan

### 3. Middleware
- API Key Authentication
- Rate Limiting berbasis IP
- Swagger/OpenAPI Documentation
  - Pengurutan dokumen otomatis
  - Pengelompokan endpoint
  - Kustomisasi tampilan

### 4. Utilitas Database
- Koneksi database yang thread-safe
- Dukungan untuk multiple database engines
- Query builder
- Pagination support

### 5. Response Handling
- Format response API yang terstandarisasi
- Dukungan untuk pagination
- Error handling yang konsisten
- Method chaining untuk konfigurasi response

### 6. Sistem Logging
- Multi-destination logging (Windows Event Log, Syslog, File)
- Rotasi log otomatis
- Pembersihan log lama
- Exception logging dengan stack trace

### 7. Konfigurasi Aplikasi
- Manajemen konfigurasi berbasis JSON
- Thread-safe operations
- Hot-reload konfigurasi
- Nested configuration support

## Persyaratan Sistem
- .NET 8.0 SDK
- Visual Studio 2022 atau VS Code
- PostgreSQL (opsional)
- MongoDB (opsional)

## Instalasi

1. Clone repository:
```bash
git clone https://github.com/fadhly-permata/template_plus_utilities.git
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Build solution:
```bash
dotnet build
```

## Change Notes

### v1.0.0 (main)
- Initial release
- Basic API template structure
- Core utilities implementation
- Basic middleware setup
- Database helpers
- Basic documentation

### v1.1.0 (dev)
- Endpoint Generator implementation
- Enhanced API response handling
- Improved documentation
- Bug fixes and performance improvements

### v1.2.0 (feature/endpoint-generator)
- Advanced Endpoint Generator features
- Dynamic routing support
- Database integration
- Callback function support
- Documentation updates

### v1.3.0 (feature/language-handler)
- Multi-language support
- JSON-based message management
- Thread-safe operations
- Documentation improvements

### v1.4.0 (feature/api-key-auth)
- API Key Authentication middleware
- Security enhancements
- Documentation updates
- Performance optimizations

### v1.5.0 (feature/rate-limiting)
- Rate limiting middleware
- IP-based request tracking
- Configurable limits
- Documentation updates

### v1.6.0 (feature/swagger-enhancement)
- Enhanced Swagger/OpenAPI documentation
- Custom document filters
- Grouping and sorting features
- UI improvements

## Lisensi

Proprietary License - Lihat [LICENSE](license.html) untuk detail lebih lanjut.

## Kontak

Untuk pertanyaan dan dukungan, silakan hubungi:
- Email: fadhly.permata@idxpartners.com
- Website: [IDX Partners](https://idxpartners.com/contact-us/)

