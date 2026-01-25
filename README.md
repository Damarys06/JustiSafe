# ⚖️ JustiSafe - Sistema de Anonimización Judicial

JustiSafe es una aplicación web monolítica diseñada para garantizar la imparcialidad en el sistema judicial mediante la anonimización de expedientes y la comunicación segura entre jueces y soporte técnico.

## 🚀 Características Principales (Microservicios)

El sistema ha sido migrado a una arquitectura de **Microservicios** para mejorar la escalabilidad y mantenibilidad.

*   **Arquitectura de Microservicios:**
    *   **Identity Service:** Manejo de usuarios, roles y autenticación (JWT).
    *   **Cases Service:** Gestión de expedientes, sorteo de jueces y anonimización.
    *   **Chat Service:** (Integrado en Web/Gateway) Comunicación en tiempo real.
    *   **API Gateway (Ocelot):** Punto de entrada único para el frontend.
*   **Seguridad Avanzada:** Implementación de OAuth2/OpenID Connect con JWT.
*   **Anonimización Automática:** Lógica encapsulada en el servicio de Casos.
*   **Sorteo Aleatorio de Jueces:** Algoritmo aislado en su propio microservicio.
*   **DevOps:** Contenerización con Docker y orquestación con Docker Compose.

## 🛠️ Stack Tecnológico

* **Framework:** ASP.NET Core 8.0 (MVC)
* **Lenguaje:** C#
* **Base de Datos:** SQL Server (LocalDB / Developer)
* **ORM:** Entity Framework Core (Code-First)
* **Frontend:** Razor Views (.cshtml), Bootstrap 5, JavaScript.
* **WebSockets:** ASP.NET Core SignalR.

## 📋 Pre-requisitos

Para ejecutar este proyecto necesitas:
1.  [Visual Studio 2022](https://visualstudio.microsoft.com/) (con la carga de trabajo "Desarrollo ASP.NET y web").
2.  [SQL Server](https://www.microsoft.com/es-es/sql-server/sql-server-downloads) (Developer o Express).
3.  .NET 8.0 SDK.

## ⚙️ Guía de Instalación y Ejecución

Sigue estos pasos para desplegar la aplicación localmente:

### 1. Clonar el Repositorio
```bash
git clone https://github.com/Damarys06/JustiSafe.git
cd JustiSafe
```
### 2. Configurar la Base de Datos
Abre el archivo JustiSafe.sln con Visual Studio 2022.

Ve al proyecto JustiSafe.Web y abre el archivo appsettings.json.

Verifica que la cadena de conexión DefaultConnection apunte a tu instancia local de SQL Server.

Ejemplo: Server=localhost;Database=JustiSafeDb;Trusted_Connection=True;TrustServerCertificate=True;

### 3. Crear la Base de Datos (Code-First)
No necesitas ejecutar scripts SQL manuales. Usa las migraciones de EF Core:

En Visual Studio, ve a Herramientas > Administrador de paquetes NuGet > Consola del Administrador de paquetes.

Asegúrate de que el Proyecto predeterminado (menú desplegable en la consola) sea JustiSafe.Data.

Ejecuta el comando:
```bash
Add-Migration UpdateUserFields
```
```bash
Update-Database
```
(Esto creará la base de datos JustiSafeDb y todas las tablas automáticamente).

### 4. Ejecutar
Presiona F5 o el botón de Play (JustiSafe.Web) en Visual Studio.

👤 Usuarios y Roles (Cómo probar)
El sistema no tiene usuarios pre-cargados. Debes crearlos usando el flujo de registro:

## Para crear un Administrador (Consejo de la Judicatura):

Ve a "Registrarse".

Usuario: Admin (o admin).

Contraseña: La que desees (ej: admin123).

Capacidades: Puede ver el botón "Sortear Nuevo Caso", gestionar eliminaciones y responder chats como Soporte.

## Para crear un Juez:

Ve a "Registrarse".

Usuario: Cualquier otro nombre (ej: JuezPerez).

Contraseña: La que desees.

Capacidades: Solo verá sus casos asignados (con identidad protegida) y aparecerá como "Juez Anónimo" en el chat.
