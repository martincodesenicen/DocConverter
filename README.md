# DocConverter API

DocConverter es una API REST que diseñé para la gestión y procesamiento de documentos. Está desarrollada siguiendo los principios de **Clean Architecture** y **Domain-Driven Design (DDD)**, manteniendo separadas las reglas de negocio de la infraestructura y los detalles de implementación.

La plataforma permite gestionar usuarios y ejecutar tareas de procesamiento de archivos en segundo plano, como la conversión de Word a PDF, la fusión de múltiples PDFs y la división de documentos por rangos de páginas.

---

## Arquitectura

El proyecto está organizado siguiendo **Clean Architecture**, separando cada responsabilidad en capas independientes.

```text
src/
├── DocConverter.Domain/         # Entidades, enums, interfaces core y excepciones de dominio
├── DocConverter.Application/    # Casos de uso, DTOs, servicios de aplicación y validaciones
├── DocConverter.Infrastructure/ # Persistencia, manejo de archivos y procesamiento en segundo plano
└── DocConverter.API/            # Endpoints REST, middlewares, autenticación y configuración
```

### Componentes principales

* **Procesamiento asincrónico:** Las solicitudes de procesamiento de archivos no bloquean las peticiones HTTP. Las tareas se envían a canales en memoria mediante `System.Threading.Channels` y son procesadas por un `BackgroundService`.

* **Flujo REST para tareas en segundo plano:** Las operaciones retornan inmediatamente un código `202 Accepted` junto con un `JobId`. El cliente luego puede consultar el estado de la tarea.

* **Almacenamiento de archivos:** Los archivos físicos se guardan en disco utilizando nombres generados con `Guid`. La base de datos almacena únicamente la información necesaria para su gestión.

* **Manejo global de excepciones:** Un middleware centraliza la captura de excepciones y las transforma en respuestas JSON consistentes con sus respectivos códigos HTTP (`400`, `401`, `404`, `500`).

* **Autenticación desacoplada:** La autenticación se basa en JWT y las contraseñas se almacenan utilizando BCrypt. La información del usuario autenticado se expone mediante la abstracción `ICurrentUserService`, evitando dependencias directas con `HttpContext`.

---

##  Tecnologías utilizadas

* **Framework:** .NET 10 / ASP.NET Core Web API
* **Lenguaje:** C# 12
* **Persistencia:** Entity Framework Core 8 y SQL Server
* **Autenticación:** JWT Bearer Authentication y BCrypt.Net-Next
* **Validación:** FluentValidation
* **Procesamiento de PDFs:** PdfSharp
* **Conversión de documentos:** SautinSoft.Document

---

## Funcionalidades

### 1. Autenticación (`/api/auth`)

* Registro de usuarios con validación mediante FluentValidation.
* Inicio de sesión con generación de tokens JWT.

### 2. Conversión Word a PDF (`/api/conversions/word-to-pdf`)

* Carga de archivos `.doc` y `.docx`.
* Procesamiento en segundo plano y generación de PDF.

### 3. Fusión de PDFs (`/api/conversions/pdf-merge`)

* Carga de múltiples archivos PDF mediante `List<IFormFile>`.
* Combinación respetando el orden enviado por el cliente.

### 4. División de PDFs (`/api/conversions/pdf-split`)

* Extracción de rangos específicos de páginas (`StartPage` a `EndPage`).
* Generación de nuevos documentos a partir de las páginas seleccionadas.

### 5. Descarga de archivos (`/api/conversions/download/{jobId}`)

* Endpoint protegido para descargar el archivo procesado una vez que el trabajo se encuentra en estado `Completed`.

---

## Instalación y configuración

### Prerrequisitos

* .NET 10 SDK.
* Una instancia de SQL Server (LocalDB, Docker o servidor dedicado).

### 1. Clonar el repositorio

```bash
git clone https://github.com/martincodesenicen/docconverter.git
cd docconverter
```

### 2. Configurar la conexión a la base de datos

Editar `src/DocConverter.API/appsettings.json` y actualizar la cadena de conexión y la configuración JWT:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=TU_SERVIDOR;Database=DocConverterDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "Secret": "UnaClaveSuperSecretaYDeGranLongitudParaFirmarLosTokensJWT2026!",
    "Issuer": "DocConverterAPI",
    "Audience": "DocConverterClients"
  }
}
```

### 3. Aplicar las migraciones

```bash
dotnet ef database update --project src/DocConverter.Infrastructure --startup-project src/DocConverter.API
```

### 4. Ejecutar la aplicación

```bash
dotnet run --project src/DocConverter.API
```

### 5. Abrir Swagger

Una vez iniciada la API, acceder a:

```text
http://localhost:5000/swagger
```

O a la URL indicada en la consola.

---

## Licencia

Este proyecto es de código abierto bajo la licencia MIT. Siéntete libre de utilizarlo y modificarlo.
