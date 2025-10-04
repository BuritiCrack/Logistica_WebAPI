# 🚀 TalentStage - Sistema de Gestión de Eventos Logísticos

<div align="center">

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet)
![Blazor](https://img.shields.io/badge/Blazor-WebAssembly-512BD4?style=for-the-badge&logo=blazor)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC2927?style=for-the-badge&logo=microsoft-sql-server)
![Azure](https://img.shields.io/badge/Azure-Storage-0078D4?style=for-the-badge&logo=microsoft-azure)

**Sistema integral de gestión de eventos logísticos con autenticación, roles de usuario y aplicación de usuarios a eventos**

[Características](#-características-principales) •
[Tecnologías](#-tecnologías-utilizadas) •
[Instalación](#-instalación-y-configuración) •
[Uso](#-uso) •
[API](#-documentación-de-la-api)

</div>

---

## 📋 Descripción del Proyecto

**Logístico WebAPI** es una aplicación web full-stack desarrollada con **.NET 8** y **Blazor WebAssembly** que permite la gestión completa de eventos logísticos. El sistema facilita la organización de eventos, la aplicación de usuarios a estos eventos, y proporciona un panel de administración robusto para la gestión de usuarios, roles y estados de aplicaciones.

### 🎯 Propósito

El sistema está diseñado para empresas y organizaciones que necesitan:
- Gestionar eventos logísticos de manera eficiente
- Permitir que usuarios se postulen a eventos específicos
- Administrar aplicaciones con estados (Pendiente, Aceptado, Rechazado, Cancelado)
- Controlar acceso mediante autenticación JWT y roles
- Gestionar datos geográficos de Colombia (Estados y Ciudades)
- Almacenar y gestionar archivos e imágenes en la nube

---

## ✨ Características Principales

### 👥 Para Usuarios Generales

- ✅ **Registro y Autenticación**
  - Registro de nuevos usuarios con validación de email
  - Login seguro con JWT tokens
  - Recuperación de contraseña por correo electrónico
  - Confirmación de email con enlaces seguros
  
- ✅ **Gestión de Perfil**
  - Actualización de información personal
  - Cambio de contraseña
  - Carga de foto de perfil con almacenamiento en Azure Blob Storage
  
- ✅ **Eventos**
  - Visualización de eventos disponibles
  - Aplicación a eventos con comentarios
  - Seguimiento del estado de aplicaciones (Pendiente, Aceptado, Rechazado)
  - Cancelación de aplicaciones propias
  - Visualización de historial de aplicaciones

### 👨‍💼 Para Administradores

- ✅ **Gestión de Usuarios**
  - Visualización de todos los usuarios del sistema
  - Activación/Desactivación de cuentas de usuario
  - Visualización del estado de confirmación de email
  - Gestión de roles (Admin, User)
  
- ✅ **Gestión de Eventos**
  - Creación de nuevos eventos
  - Edición y eliminación de eventos
  - Visualización de todas las aplicaciones por evento
  - Gestión de estados de aplicaciones
  
- ✅ **Gestión de Aplicaciones**
  - Revisión de aplicaciones pendientes
  - Aceptación o rechazo de aplicaciones con comentarios
  - Visualización de estadísticas de eventos
  
- ✅ **Gestión Geográfica**
  - Administración de estados de Colombia
  - Administración de ciudades por estado
  - Datos precargados mediante SeedDb

---

## 🏗️ Arquitectura del Sistema

El proyecto sigue una **arquitectura en capas** con separación clara de responsabilidades:

### 📦 Estructura de Capas

```
Logistica_WebAPI/
│
├── 📁 LogisticoWebAPI.Backend/          # Capa de API REST
│   ├── Controllers/                      # Controladores de la API
│   ├── Data/                             # DbContext y configuraciones EF Core
│   ├── Helpers/                          # Utilidades y helpers
│   ├── Migrations/                       # Migraciones de Entity Framework
│   ├── Repositories/                     # Implementación del patrón Repository
│   │   ├── Interfaces/                   # Contratos de repositorios
│   │   └── Implementations/              # Implementaciones concretas
│   ├── UnitsOfWork/                      # Patrón Unit of Work
│   │   ├── Interfaces/                   # Contratos de UoW
│   │   └── Implementations/              # Implementaciones concretas
│   └── Program.cs                        # Configuración y arranque de la API
│
├── 📁 LogisticoWebAPI.Frontend/         # Capa de Presentación (Blazor WASM)
│   ├── Pages/                            # Páginas Razor
│   │   ├── Auth/                         # Autenticación y registro
│   │   ├── Events/                       # Gestión de eventos
│   │   ├── Users/                        # Gestión de usuarios
│   │   ├── States/                       # Gestión de estados
│   │   └── Cities/                       # Gestión de ciudades
│   ├── Repositories/                     # Repositorios HTTP del frontend
│   ├── Services/                         # Servicios del frontend
│   └── Program.cs                        # Configuración de Blazor WASM
│
└── 📁 LogisticoWebAPI.Shared/           # Capa de Entidades y DTOs Compartidos
    ├── Entities/                         # Modelos de dominio
    ├── DTOs/                             # Data Transfer Objects
    ├── Enums/                            # Enumeraciones
    └── Responses/                        # Objetos de respuesta estandarizados
```

### 🔄 Patrones de Diseño Implementados

- **Repository Pattern**: Abstracción de acceso a datos
- **Unit of Work Pattern**: Gestión de transacciones
- **Dependency Injection**: Inyección de dependencias nativa de .NET
- **Generic Repository**: Repositorio genérico para operaciones CRUD comunes
- **DTO Pattern**: Separación entre entidades de dominio y objetos de transferencia

---

## 🗄️ Modelo de Datos

### Entidades Principales

#### 👤 **User** (IdentityUser)
```csharp
- Id: string (GUID)
- FirstName: string
- LastName: string
- Document: string
- Address: string
- Photo: string (URL de Azure Blob)
- UserType: UserType (Admin, User)
- City: City
- IsActive: bool
- EmailConfirmed: bool
- Eps: string
- PensionFund: string
- Experience: string
```

#### 📅 **Event**
```csharp
- Id: int
- Name: string
- Description: string
- StartDate: DateTime
- EndDate: DateTime
- Location: string
- MaxParticipants: int
- ImageUrl: string
```

#### 🎫 **EventUser** (Relación Many-to-Many)
```csharp
- Id: int
- UserId: string
- EventId: int
- Status: ApplicationStatus
- RegistrationDate: DateTime
- UserComments: string
- AdminComments: string
- LastUpdated: DateTime
```

#### 🌎 **State** (Estados de Colombia)
```csharp
- Id: int
- Name: string
- Cities: List<City>
```

#### 🏙️ **City** (Ciudades de Colombia)
```csharp
- Id: int
- Name: string
- StateId: int
- State: State
```

### 📊 Enumeraciones

#### **ApplicationStatus**
```csharp
Pending = 0          // Estado inicial al aplicar
Accepted = 1         // Aceptado por administrador
Rejected = 2         // Rechazado por administrador
CancelledByUser = 3  // Cancelado por el usuario
```

#### **UserType**
```csharp
Admin = 0    // Administrador del sistema
User = 1     // Usuario estándar
```

---

## 🛠️ Tecnologías Utilizadas

### Backend (API)

| Tecnología | Versión | Propósito |
|------------|---------|-----------|
| **.NET** | 8.0 | Framework principal |
| **ASP.NET Core** | 8.0 | API REST |
| **Entity Framework Core** | 9.0.0 | ORM y acceso a datos |
| **SQL Server** | 2022 | Base de datos |
| **ASP.NET Core Identity** | 8.0 | Autenticación y autorización |
| **JWT Bearer** | 8.0 | Tokens de autenticación |
| **Azure Storage Blobs** | - | Almacenamiento de archivos |
| **MailKit** | - | Envío de correos electrónicos |
| **Swashbuckle** | 6.6.2 | Documentación Swagger/OpenAPI |

### Frontend (Web App)

| Tecnología | Versión | Propósito |
|------------|---------|-----------|
| **Blazor WebAssembly** | .NET 8.0 | SPA Framework |
| **Bootstrap** | 5.x | Framework CSS |
| **SweetAlert2** | - | Alertas y confirmaciones |
| **CurrieTechnologies.Razor.SweetAlert2** | - | Integración SweetAlert2 con Blazor |

### Herramientas y Servicios

- **GitHub** - Control de versiones
- **Azure Storage** - Almacenamiento en la nube
- **SMTP Gmail** - Servicio de correo electrónico
- **Swagger UI** - Documentación interactiva de API

---

## 📥 Instalación y Configuración

### Prerrequisitos

Asegúrate de tener instalado:

- ✅ [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) o superior
- ✅ [SQL Server 2022](https://www.microsoft.com/sql-server) o SQL Server LocalDB
- ✅ [Visual Studio 2022](https://visualstudio.microsoft.com/) o [VS Code](https://code.visualstudio.com/)
- ✅ [Git](https://git-scm.com/)
- ✅ Cuenta de [Azure Storage](https://azure.microsoft.com/) (opcional, para almacenamiento de archivos)

### 🔧 Configuración Paso a Paso

#### 1. Clonar el Repositorio

```bash
git clone https://github.com/BuritiCrack/Logistica.git
cd Logistica/Logistico_WebAPI
```

#### 2. Configurar la Cadena de Conexión

Edita el archivo `appsettings.json` en el proyecto Backend:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=LogisticoDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Para SQL Server LocalDB usa:
```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=LogisticoDB;Trusted_Connection=True;MultipleActiveResultSets=true"
```

#### 3. Configurar Azure Storage (Opcional)

Si quieres usar Azure Storage para las imágenes:

```json
{
  "ConnectionStrings": {
    "AzureStorage": "DefaultEndpointsProtocol=https;AccountName=tu_cuenta;AccountKey=tu_clave;EndpointSuffix=core.windows.net"
  }
}
```

#### 4. Configurar Email (Opcional)

Para envío de correos de confirmación:

```json
{
  "Mail": {
    "From": "tu_email@gmail.com",
    "Name": "Sistema Logístico",
    "Password": "tu_contraseña_de_aplicacion",
    "Smtp": "smtp.gmail.com",
    "Port": 587
  }
}
```

**Nota**: Para Gmail, necesitas crear una [contraseña de aplicación](https://myaccount.google.com/apppasswords).

#### 5. Restaurar Dependencias

```bash
# Backend
cd LogisticoWebAPI.Backend
dotnet restore

# Frontend
cd ../LogisticoWebAPI.Frontend
dotnet restore

# Shared
cd ../LogisticoWebAPI.Shared
dotnet restore
```

#### 6. Aplicar Migraciones a la Base de Datos

```bash
cd LogisticoWebAPI.Backend
dotnet ef database update
```

Esto creará la base de datos y aplicará todas las migraciones, incluyendo:
- ✅ Tabla de usuarios (Identity)
- ✅ Tabla de eventos
- ✅ Tabla de estados y ciudades de Colombia
- ✅ Tabla de aplicaciones (EventUsers)
- ✅ Datos semilla (Usuario admin, estados y ciudades)

#### 7. Ejecutar el Backend (API)

```bash
cd LogisticoWebAPI.Backend
dotnet run
```

La API estará disponible en:
- 🌐 HTTP: `http://localhost:5223`
- 🔒 HTTPS: `https://localhost:7172`
- 📋 Swagger: `http://localhost:5223/swagger`

#### 8. Ejecutar el Frontend (en otra terminal)

```bash
cd LogisticoWebAPI.Frontend
dotnet run
```

El frontend estará disponible en:
- 🌐 `http://localhost:5058`
- 🔒 `https://localhost:7241`

---

## 🎮 Uso

### Acceso Inicial

Al ejecutar la aplicación por primera vez, el sistema crea automáticamente un usuario administrador:

```
📧 Email: admin@yopmail.com
🔑 Contraseña: 123456
👤 Rol: Admin
```

**⚠️ IMPORTANTE**: Cambia esta contraseña después del primer inicio de sesión en un entorno de producción.

### Flujo de Trabajo Típico

#### Para Usuarios:

1. **Registro**
   - Ir a la página de registro
   - Completar el formulario con datos personales
   - Seleccionar estado y ciudad
   - Confirmar email mediante el enlace enviado (si está configurado el email)

2. **Aplicar a un Evento**
   - Navegar a la lista de eventos disponibles
   - Seleccionar un evento de interés
   - Click en "Aplicar a este evento"
   - Agregar comentarios opcionales
   - Confirmar aplicación
   - El estado inicial será "Pendiente"

3. **Ver Estado de Aplicaciones**
   - Ir a "Mis Aplicaciones"
   - Ver estado actual: Pendiente, Aceptado, Rechazado
   - Cancelar aplicación si es necesario (cambiará a "Cancelado por Usuario")

#### Para Administradores:

1. **Gestionar Eventos**
   - Crear nuevos eventos con información completa
   - Editar eventos existentes
   - Eliminar eventos
   - Ver todas las aplicaciones por evento

2. **Revisar Aplicaciones**
   - Ver lista de aplicaciones pendientes
   - Ver detalles del usuario que aplicó
   - Aceptar aplicación con comentarios del admin
   - Rechazar aplicación con motivo
   - Ver historial de aplicaciones por evento

3. **Gestionar Usuarios**
   - Ver lista completa de usuarios
   - Activar/Desactivar cuentas de usuario
   - Ver estado de confirmación de email
   - Ver información detallada de cada usuario

4. **Gestionar Geografía**
   - Administrar estados de Colombia
   - Administrar ciudades por estado
   - Crear, editar o eliminar ubicaciones

---

## 📡 Documentación de la API

### Endpoints Principales

#### 🔐 Autenticación (`/api/accounts`)

```http
POST   /api/accounts/register              # Registrar nuevo usuario
POST   /api/accounts/login                 # Iniciar sesión
GET    /api/accounts/ConfirmEmail          # Confirmar email
POST   /api/accounts/RecoverPassword       # Recuperar contraseña
POST   /api/accounts/ResetPassword         # Restablecer contraseña
PUT    /api/accounts/ChangePassword        # Cambiar contraseña
PUT    /api/accounts/ChangeStatus/{id}     # Activar/Desactivar usuario [Admin]
GET    /api/accounts/users                 # Listar usuarios [Admin]
GET    /api/accounts/user                  # Obtener usuario actual
```

#### 📅 Eventos (`/api/events`)

```http
GET    /api/events                         # Obtener todos los eventos
GET    /api/events/{id}                    # Obtener evento por ID
POST   /api/events                         # Crear evento [Admin]
PUT    /api/events                         # Actualizar evento [Admin]
DELETE /api/events/{id}                    # Eliminar evento [Admin]
GET    /api/events/totalPages              # Obtener total de páginas
```

#### 🎫 Aplicaciones a Eventos (`/api/eventapplications`)

```http
POST   /api/eventapplications/apply/{eventId}                    # Aplicar a evento
DELETE /api/eventapplications/cancel/{eventId}                   # Cancelar aplicación
GET    /api/eventapplications/myapplications                     # Ver mis aplicaciones
GET    /api/eventapplications/event/{eventId}/applications       # Ver aplicaciones de un evento [Admin]
PUT    /api/eventapplications/updatestatus                       # Actualizar estado [Admin]
GET    /api/eventapplications/{applicationId}                    # Ver detalles de aplicación
```

#### 🌎 Estados (`/api/states`)

```http
GET    /api/states                         # Obtener todos los estados
GET    /api/states/{id}                    # Obtener estado por ID
POST   /api/states                         # Crear estado [Admin]
PUT    /api/states                         # Actualizar estado [Admin]
DELETE /api/states/{id}                    # Eliminar estado [Admin]
GET    /api/states/combo                   # Obtener estados para combo
```

#### 🏙️ Ciudades (`/api/cities`)

```http
GET    /api/cities                         # Obtener todas las ciudades
GET    /api/cities/{id}                    # Obtener ciudad por ID
POST   /api/cities                         # Crear ciudad [Admin]
PUT    /api/cities                         # Actualizar ciudad [Admin]
DELETE /api/cities/{id}                    # Eliminar ciudad [Admin]
GET    /api/cities/combo/{stateId}         # Obtener ciudades por estado
```

### 📄 Swagger UI

Accede a la documentación interactiva de la API en:

```
http://localhost:5223/swagger
```

Desde Swagger puedes:
- ✅ Ver todos los endpoints disponibles
- ✅ Probar peticiones directamente
- ✅ Ver esquemas de datos (DTOs y Entities)
- ✅ Autenticarte con JWT tokens
- ✅ Ver respuestas de ejemplo

### 🔑 Autenticación en Swagger

1. Haz login en `/api/accounts/login`
2. Copia el token JWT de la respuesta
3. Click en el botón "Authorize" en Swagger
4. Ingresa: `Bearer {tu_token}`
5. Ahora puedes probar endpoints protegidos

---

## 🔒 Seguridad

### Autenticación JWT

El sistema utiliza **JSON Web Tokens (JWT)** para autenticación:

1. El usuario se autentica con email y contraseña
2. El servidor genera un token JWT válido por 24 horas
3. El cliente envía el token en el header `Authorization: Bearer {token}`
4. El servidor valida el token en cada petición protegida

### Roles y Autorización

- **Admin**: Acceso completo al sistema
  - Gestión de usuarios
  - Gestión de eventos
  - Gestión de aplicaciones
  - Gestión de estados y ciudades
  
- **User**: Acceso limitado
  - Ver eventos
  - Aplicar a eventos
  - Ver y cancelar sus propias aplicaciones
  - Gestionar su perfil

### Protección de Datos

- ✅ Contraseñas hasheadas con ASP.NET Core Identity
- ✅ Confirmación de email obligatoria (configurable)
- ✅ Tokens de recuperación de contraseña con expiración
- ✅ Validación de entrada en todos los endpoints
- ✅ CORS configurado para orígenes específicos
- ✅ HTTPS recomendado para producción

---

## 📁 Estructura de Archivos Clave

```
Logistica_WebAPI/
├── LogisticoWebAPI.Backend/
│   ├── Controllers/
│   │   ├── AccountsController.cs              # Autenticación y usuarios
│   │   ├── EventsController.cs                # Gestión de eventos
│   │   ├── EventApplicationsController.cs     # Aplicaciones a eventos
│   │   ├── StatesController.cs                # Gestión de estados
│   │   └── CitiesController.cs                # Gestión de ciudades
│   ├── Data/
│   │   ├── DataContext.cs                     # DbContext principal
│   │   └── SeedDb.cs                          # Datos semilla
│   ├── Helpers/
│   │   ├── IFileStorage.cs                    # Interfaz almacenamiento
│   │   └── FileStorage.cs                     # Implementación Azure Blob
│   ├── Repositories/
│   │   ├── Implementations/
│   │   │   ├── GenericRepository.cs           # Repositorio genérico
│   │   │   ├── UsersRepository.cs             # Repositorio de usuarios
│   │   │   ├── EventsRepository.cs            # Repositorio de eventos
│   │   │   ├── EventUsersRepository.cs        # Repositorio de aplicaciones
│   │   │   ├── StatesRepository.cs            # Repositorio de estados
│   │   │   └── CitiesRepository.cs            # Repositorio de ciudades
│   │   └── Interfaces/
│   │       ├── IGenericRepository.cs
│   │       ├── IUsersRepository.cs
│   │       ├── IEventsRepository.cs
│   │       └── IEventUsersRepository.cs
│   ├── UnitsOfWork/
│   │   ├── Implementations/
│   │   │   ├── GenericUnitOfWork.cs
│   │   │   ├── UsersUnitOfWork.cs
│   │   │   ├── EventsUnitOfWork.cs
│   │   │   ├── EventUsersUnitOfWork.cs
│   │   │   ├── StatesUnitOfWork.cs
│   │   │   └── CitiesUnitOfWork.cs
│   │   └── Interfaces/
│   │       ├── IGenericUnitOfWork.cs
│   │       ├── IUsersUnitOfWork.cs
│   │       └── IEventUsersUnitOfWork.cs
│   ├── Migrations/                            # Migraciones EF Core
│   ├── appsettings.json                       # Configuración
│   └── Program.cs                             # Configuración de la API
│
├── LogisticoWebAPI.Frontend/
│   ├── Pages/
│   │   ├── Auth/
│   │   │   ├── Login.razor                    # Página de login
│   │   │   ├── Register.razor                 # Registro de usuarios
│   │   │   ├── ConfirmEmail.razor             # Confirmación de email
│   │   │   └── ChangePassword.razor           # Cambio de contraseña
│   │   ├── Events/
│   │   │   ├── EventsIndex.razor              # Lista de eventos (Admin)
│   │   │   ├── EventCreate.razor              # Crear evento
│   │   │   ├── EventEdit.razor                # Editar evento
│   │   │   └── AvailableEvents.razor          # Eventos disponibles (User)
│   │   ├── Users/
│   │   │   └── UsersIndex.razor               # Gestión de usuarios
│   │   ├── States/
│   │   │   ├── StatesIndex.razor              # Lista de estados
│   │   │   └── StateEdit.razor                # Editar estado
│   │   └── Cities/
│   │       ├── CitiesIndex.razor              # Lista de ciudades
│   │       └── CityEdit.razor                 # Editar ciudad
│   ├── Repositories/
│   │   └── IRepository.cs                     # Interfaz de repositorio HTTP
│   ├── AuthenticationProviders/
│   │   └── AuthenticationProviderJWT.cs       # Proveedor de autenticación
│   └── Program.cs                             # Configuración de Blazor
│
└── LogisticoWebAPI.Shared/
    ├── Entities/
    │   ├── User.cs                            # Entidad de usuario
    │   ├── Event.cs                           # Entidad de evento
    │   ├── EventUser.cs                       # Entidad de aplicación
    │   ├── State.cs                           # Entidad de estado
    │   └── City.cs                            # Entidad de ciudad
    ├── DTOs/
    │   ├── LoginDTO.cs                        # DTO de login
    │   ├── UserDTO.cs                         # DTO de usuario
    │   ├── TokenDTO.cs                        # DTO de token
    │   ├── EventDTO.cs                        # DTO de evento
    │   ├── EventApplicationDTO.cs             # DTO de aplicación
    │   └── UpdateApplicationStatusDTO.cs      # DTO actualizar estado
    ├── Enums/
    │   ├── ApplicationStatus.cs               # Estados de aplicación
    │   └── UserType.cs                        # Tipos de usuario
    └── Responses/
        └── ActionResponse.cs                  # Respuesta estandarizada
```

---

## 🧪 Testing

### Ejecutar Tests

```bash
cd LogisticoWebAPI.Test
dotnet test
```

### Cobertura de Tests

El proyecto incluye tests para:
- ✅ Repositorios
- ✅ Units of Work
- ✅ Controladores
- ✅ Servicios

---

## 🤝 Contribuciones

Las contribuciones son bienvenidas. Por favor:

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

### Guía de Estilo

- Usa nombres descriptivos en inglés para código
- Comenta código complejo
- Sigue las convenciones de C# y .NET
- Escribe tests para nuevas funcionalidades
- Actualiza la documentación

---

## 📝 Licencia

Este proyecto es de código abierto y está disponible para uso educativo y comercial.

---

## 👨‍💻 Autores

- **Equipo de Desarrollo Logístico WebAPI**
- GitHub: [@BuritiCrack](https://github.com/BuritiCrack)
- Repositorio: [Logistica](https://github.com/BuritiCrack/Logistica)

---

## 📧 Contacto

Para preguntas o soporte:
- 💬 GitHub Issues: [Crear Issue](https://github.com/BuritiCrack/Logistica/issues)
- 📧 Email: [Contacto GitHub](https://github.com/BuritiCrack)

---

## 🎓 Recursos de Aprendizaje

### Documentación Oficial

- [.NET Documentation](https://docs.microsoft.com/dotnet/)
- [Blazor Documentation](https://docs.microsoft.com/aspnet/core/blazor/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [ASP.NET Core Identity](https://docs.microsoft.com/aspnet/core/security/authentication/identity)

### Tutoriales Relacionados

- [Building Web APIs with ASP.NET Core](https://docs.microsoft.com/aspnet/core/tutorials/first-web-api)
- [Blazor WebAssembly Authentication](https://docs.microsoft.com/aspnet/core/blazor/security/webassembly/)
- [Repository Pattern in .NET](https://docs.microsoft.com/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-design)

---

## 📊 Estadísticas del Proyecto

- **Lenguaje Principal**: C#
- **Framework**: .NET 8.0
- **Arquitectura**: N-Capas con Repository/UnitOfWork
- **Base de Datos**: SQL Server
- **Frontend**: Blazor WebAssembly
- **Autenticación**: JWT Bearer
- **ORM**: Entity Framework Core

---

## 🗺️ Roadmap

### Versión Actual (1.0)
- ✅ Sistema de autenticación completo
- ✅ Gestión de eventos
- ✅ Aplicaciones a eventos
- ✅ Panel de administración
- ✅ Gestión geográfica

### Próximas Características (2.0)
- 🔲 Notificaciones en tiempo real (SignalR)
- 🔲 Reportes y estadísticas avanzadas
- 🔲 Exportación de datos (Excel, PDF)
- 🔲 Sistema de calificaciones y comentarios
- 🔲 Integración con Google Maps
- 🔲 App móvil (MAUI)

---

<div align="center">

**⭐ Si este proyecto te fue útil, considera darle una estrella en GitHub ⭐**

Made with ❤️ by Logístico WebAPI Team

[![GitHub stars](https://img.shields.io/github/stars/BuritiCrack/Logistica?style=social)](https://github.com/BuritiCrack/Logistica)
[![GitHub forks](https://img.shields.io/github/forks/BuritiCrack/Logistica?style=social)](https://github.com/BuritiCrack/Logistica/fork)

---

**Última actualización**: Octubre 2025

</div>
