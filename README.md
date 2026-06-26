# DocConverter

<p align="center">
  <img
    src="https://github.com/user-attachments/assets/41fb7a54-dc2f-4888-8fa2-44ce8f5d35c6"
    alt="Dashboard"
    width="1024"
  />
</p>

DocConverter es una plataforma web para la conversión y procesamiento de documentos desarrollada con ASP.NET Core y Angular.

La aplicación permite a los usuarios registrarse, autenticarse mediante JWT y ejecutar tareas de procesamiento de documentos, incluyendo:

- Conversión de Word a PDF (word-to-pdf).
- Unión de múltiples archivos PDF (merge-pdf).
- División de PDFs por rango de páginas (split-pdf).
- Descarga de archivos procesados.
- Seguimiento del estado de las conversiones mediante polling.

---

## Arquitectura General

El proyecto está dividido en dos aplicaciones independientes (ya que primero desarrollé el backend y luego el frontend):

```text
DocConverter
│
├── src/
│   ├── DocConverter.API
│   ├── DocConverter.Application
│   ├── DocConverter.Domain
│   └── DocConverter.Infrastructure
│
└── doc-converter-web/
    └── Angular 21
```

---

## Backend

El backend está desarrollado siguiendo los principios de Clean Architecture y Domain-Driven Design (DDD).

```text
src/
├── DocConverter.Domain
├── DocConverter.Application
├── DocConverter.Infrastructure
└── DocConverter.API
```

### Responsabilidades

#### Domain

Contiene:

- Entidades
- Enums
- Interfaces del dominio
- Excepciones de negocio

#### Application

Contiene:

- Casos de uso
- DTOs
- Servicios de aplicación
- Validaciones con FluentValidation

#### Infrastructure

Contiene:

- Entity Framework Core
- SQL Server
- Repositorios
- Almacenamiento de archivos
- Procesamiento de PDFs
- Workers en segundo plano
- JWT

#### API

Contiene:

- Controllers
- Middlewares
- Configuración de autenticación
- Configuración de Swagger
- Endpoints REST

---

## Frontend

El frontend está desarrollado con Angular 21 utilizando Standalone Components.

### Características

- Angular 21
- Angular Material
- Standalone Components
- Signals
- Reactive Forms
- Route Guards
- HTTP Interceptors
- Responsive Design

### Estructura

```text
src/app
│
├── core
│   ├── guards
│   ├── interceptors
│   ├── models
│   └── services
│
├── features
│   ├── auth
│   ├── dashboard
│   └── conversions
│
└── shared
    └── components
```

---

## Flujo de Conversión

Todas las operaciones de procesamiento se ejecutan en segundo plano.

### 1. El usuario envía un archivo

```http
POST /api/conversions/word-to-pdf
```

<p align="center">
  <img
    src="https://github.com/user-attachments/assets/d49497c0-616f-4d0f-8821-0cb60f8f63ec"
    alt="Captura_Upload"
    width="1024"
  />
</p>

### 2. La API responde

```json
{
  "jobId": "guid",
  "status": "Pending",
  "createdAt": "2026-06-25T20:00:00"
}
```
<p align="center">
  <img
    src="https://github.com/user-attachments/assets/7ae93f4b-5712-45b5-b45d-48cdb2648322"
    alt="Captura_Pendiente"
    width="1024"
  />
</p>

### 3. El frontend inicia polling

```http
GET /api/conversions/status/{jobId}
```

cada 3 segundos.

<p align="center">
  <img
    src="https://github.com/user-attachments/assets/b2f03a9e-a010-445c-91d1-286509db6c97"
    alt="Captura_Pendiente"
    width="1024"
  />
</p>

### 4. Cuando el estado cambia

```json
{
  "jobId": "guid",
  "status": "Completed"
}
```

se habilita la descarga.

<p align="center">
  <img
    src="https://github.com/user-attachments/assets/320ae8d9-39fa-4dbf-a5db-0cda6225e197"
    alt="Captura_Completado"
    width="1024"
  />
</p>

### 5. Descarga

```http
GET /api/conversions/download/{jobId}
```

---

## Funcionalidades

### Autenticación

- Registro de usuarios
- Inicio de sesión
- JWT Bearer Authentication
- Auth Guard
- Auth Interceptor

### Word → PDF

- Soporte para .doc
- Soporte para .docx
- Conversión asíncrona

### PDF Merge

- Múltiples archivos PDF
- Conserva el orden enviado

### PDF Split

- División por rango de páginas
- Validación de páginas

### Descarga

- Descarga segura de resultados
- Archivos protegidos por usuario

---

## Tecnologías Utilizadas

### Backend

- .NET 10
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- JWT Bearer Authentication
- FluentValidation
- BCrypt.Net
- PdfSharp
- SautinSoft.Document

### Frontend

- Angular 21
- Angular Material
- TypeScript
- RxJS
- Angular Signals
- SCSS

---

## Seguridad

La autenticación se realiza mediante JWT.

Después del login:

```json
{
  "id": "guid",
  "email": "user@email.com",
  "token": "jwt-token"
}
```

El token es almacenado en el cliente y enviado automáticamente mediante un HTTP Interceptor:

```http
Authorization: Bearer <token>
```

Todas las rutas de conversión requieren autenticación.

---

## Instalación

### Backend

#### Clonar repositorio

```bash
git clone https://github.com/martincodesenicen/docconverter.git
cd docconverter
```

#### Configurar appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=DocConverterDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "Secret": "YOUR_SECRET_KEY",
    "Issuer": "DocConverterAPI",
    "Audience": "DocConverterUsers"
  }
}
```

#### Ejecutar migraciones

```bash
dotnet ef database update \
--project src/DocConverter.Infrastructure \
--startup-project src/DocConverter.API
```

#### Ejecutar API

```bash
dotnet run --project src/DocConverter.API
```

API:

```text
http://localhost:5217
```

Swagger:

```text
http://localhost:5217/swagger
```

---

### Frontend

Entrar al proyecto Angular:

```bash
cd ..
cd doc-converter-web
```

Instalar dependencias:

```bash
npm install
```

Configurar:

```ts
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5217/api'
};
```

Ejecutar:

```bash
ng serve
```

Aplicación:

```text
http://localhost:4200
```

---

## Capturas

### Login

<p align="center">
  <img
    src="https://github.com/user-attachments/assets/e0cc7779-d306-4699-8482-6c88b7545662"
    alt="Captura_Login"
    width="1024"
  />
</p>

### PDF Merge / Combinar PDF

<p align="center">
  <img
    src="https://github.com/user-attachments/assets/34453745-a8bf-4c72-a112-8664fb856916"
    alt="Captura_MergePdf"
    width="1024"
  />
</p>

### PDF Split / Dividir PDF

<p align="center">
  <img
    src="https://github.com/user-attachments/assets/4a0c0b91-acfa-48c7-9ce5-bbc7611aaf7d"
    alt="Captura_SplitPdf"
    width="1024"
  />
</p>

---

## Roadmap

Posibles mejoras futuras:

- Historial de conversiones.
- Refresh Tokens.
- Notificaciones en tiempo real con SignalR.
- Procesamiento distribuido.
- Docker Compose.
- CI/CD con GitHub Actions.
- Almacenamiento en Azure Blob Storage o AWS S3.

---

## Licencia

Este proyecto se distribuye bajo la licencia MIT.
