# API Template + Utilities

Template API berbasis .NET 8 dengan berbagai utilitas yang memudahkan pengembangan API.

## Fitur Utama

### 1. Endpoint Generator **(PLAN)**

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
- Hot-reload pesan tanpa restart aplikasi
- Fallback ke default language

### 3. Middleware

- API Key Authentication dengan multiple validation methods:
  - User-based authentication
  - Client-based authentication
  - Environment-based authentication
  - Temporary access tokens
- Rate Limiting berbasis IP
- Swagger/OpenAPI Documentation
  - Pengurutan dokumen otomatis
  - Pengelompokan endpoint berdasarkan tags
  - Multiple documentation versions (Main/Demo)
  - Kustomisasi tema UI (Light/Dark mode)
  - Security schemes (API Key)
  - XML documentation support

### 4. Utilitas Database

- Koneksi database yang thread-safe
- Dukungan untuk multiple database engines
- Query builder
- Pagination support
- Connection string parser
- In-memory SQLite support

### 5. Response Handling

- Format response API yang terstandarisasi
- Dukungan untuk pagination
- Error handling yang konsisten
- Method chaining untuk konfigurasi response
- Model state validation
- Exception filters

### 6. Sistem Logging

- Multi-destination logging:
  - Windows Event Log
  - Unix Syslog
  - File system dengan daily rotation
- Pembersihan log otomatis
- Konfigurasi level log (Information, Warning, Error)
- Exception logging dengan stack trace
- Custom log directory dan base path
- Thread-safe operations

### 7. Konfigurasi Aplikasi

- Manajemen konfigurasi berbasis JSON
- Thread-safe operations
- Hot-reload konfigurasi
- Nested configuration support
- Environment-specific settings
- Default value fallback

### 8. Caching System

- Thread-safe in-memory caching
- Configurable expiration time
- Automatic cleanup of expired items
- Generic type support
- CRUD operations untuk cached items

### 9. Regular Expression Collections

- Pre-compiled regex patterns untuk:
  - Email validation
  - Phone numbers
  - Log parsing
  - Connection strings
  - Date/time formats
  - Security patterns (password, JWT)
  - HTML/XML parsing
  - Color codes
  - Markdown syntax
  - Social media handles

### 10. Security Features

- CORS policy management
- API Key generation dan validation
- Strong password validation
- JWT token validation **(PLAN)**
- Rate limiting
- Secure headers

## Fitur Tambahan

### 11. Dependency Injection

- Built-in DI container
- Scoped, Singleton, dan Transient services
- Auto-registration untuk services
- Lazy loading support
- Conditional service registration

### 12. Validation System

- Model validation
- Custom validation attributes
- Fluent validation support
- Validation response formatting
- Cross-property validation

### 13. Log File Management

- File upload/download handling
- Stream processing
- Temporary file management
- File type validation
- Auto-cleanup mechanisms

### 14. Background Services

- Background task queuing
- Scheduled tasks
- Long-running operations
- Task cancellation support
- Progress reporting

### 15. Health Checks

- Database connectivity checks
- External service monitoring
- Custom health check implementations
- Health status reporting
- Metrics collection

## Troubleshooting

### Common Issues

1. Database Connection

   - Periksa connection string
   - Verifikasi credentials
   - Cek firewall settings
2. API Key Issues

   - Validasi format key
   - Periksa expiration
   - Cek permission levels
3. Logging Problems

   - Verifikasi write permissions
   - Cek disk space
   - Monitor log rotation

### Debug Mode

- Set `ASPNETCORE_ENVIRONMENT=Development`
- Enable detailed error messages
- Use logging levels appropriately
- Monitor performance counters

## Tools & Resources

### Development Tools

- Visual Studio 2022
- VS Code
- Postman
- Docker Desktop
- Git

### Documentation

- XML Documentation
- Swagger/OpenAPI
- Architecture diagrams
- API guidelines

### Supported Databases

#### SQLite (via `SQLiteHelper`)

- Support in-memory database
- Support shared/private cache
- Support read-only/read-write mode

#### PostgreSQL (via `PostgreHelper`)

- Support connection pooling
- Support transaction
- Support JSON operations

#### MongoDB (planned via `MongoDBHelper`)

- Masih dalam tahap pengembangan
- Connection string parser sudah tersedia di `CommonConnectionString`

> **Note**`CommonConnectionString` juga sudah menyediakan parser dan generator untuk:
>
> - MariaDB
> - SQL Server
> - Oracle
> - Cassandra

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

## Konfigurasi

1. Sesuaikan `appsettings.json` untuk konfigurasi dasar
2. Atur `appconfigs.jsonc` untuk konfigurasi detail:
   - Swagger/OpenAPI settings
   - Database connections
   - Security settings
   - Logging options
   - Caching parameters

## Lisensi

Proprietary License - Lihat [LICENSE](IDC.Template/wwwroot/openapi/license.html) untuk detail lebih lanjut.

## Kontak

Untuk pertanyaan dan dukungan, silakan hubungi:

- Email: fadhly.permata@idxpartners.com
- Website: [IDX Partners](https://idxpartners.com/contact-us/)
