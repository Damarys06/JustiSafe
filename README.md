# ⚖️ JustiSafe v2 - Sistema de Anonimización Judicial

JustiSafe es una plataforma basada en **Microservicios** para la gestión y anonimización de expedientes judiciales. Garantiza la imparcialidad mediante sorteos aleatorios de jueces y protege la identidad de los involucrados.

## 🚀 Arquitectura (Microservicios)

El sistema se compone de los siguientes servicios contenerizados:
*   **Identity Service:** Autentica usuarios y gestiona roles (JWT).
*   **Cases Service:** Administra expedientes y realiza la anonimización.
*   **API Gateway (Ocelot):** Enruta el tráfico y protege los endpoints.
*   **JustiSafe Web:** Interfaz de usuario (MVC) que consume los microservicios.
*   **SQL Server:** Base de datos persistente para cada servicio.

## 📋 Pre-requisitos

Para ejecutar esta versión (v2) solo necesitas:
1.  **[Docker Desktop](https://www.docker.com/products/docker-desktop/)** (Instalado y ejecutándose).
2.  **Git**.

> **Nota:** No necesitas instalar SQL Server ni Visual Studio localmente para ejecutar la aplicación, ya que todo el entorno se levanta en contenedores.

## ⚙️ Guía de Ejecución Rápida

Sigue estos pasos para desplegar la aplicación:

### 1. Clonar el Repositorio
```bash
git clone https://github.com/Damarys06/JustiSafe.git
cd JustiSafe
```

### 2. Desplegar con Docker Compose
Asegúrate de estar en la rama correcta (`JustiSafeII` si es la versión 2) y ejecuta:

```bash
docker-compose up --build
```

Espera unos momentos mientras se descargan las imágenes y se compilan los servicios. Verás logs de los diferentes contenedores iniciándose.

### 3. Acceder al Sistema
Una vez que los servicios estén activos, abre tu navegador e ingresa a:

👉 **http://localhost:8090**

## 👤 Usuarios y Roles (Cómo probar)

El sistema inicia con bases de datos limpias. Debes registrar usuarios para probar los flujos:

### Rol: Administrador (Soporte/Judicatura)
1.  Ve a **"Registrarse"**.
2.  **Usuario:** `Admin` (o cualquier nombre).
3.  **Contraseña:** Tu elección (ej: `Password123!`).
4.  **Capacidades:**
    *   Sorteo de nuevos casos.
    *   Gestión de casos.
    *   Chat de soporte.

### Rol: Juez
1.  Ve a **"Registrarse"**.
2.  **Usuario:** `JuezPerez` (o cualquier nombre).
3.  **Contraseña:** Tu elección.
4.  **Capacidades:**
    *   Visualización de casos asignados (anonimizados).
    *   Chat anónimo con soporte.

## 🛠️ Solución de Problemas

*   **Error de conexión a BD:** Si es la primera vez que ejecutas, espera unos segundos más; SQL Server puede tardar en inicializarse antes de que las APIs puedan conectarse.
*   **Puertos ocupados:** Asegúrate de que los puertos `8090` (Web), `5000` (Gateway) y `1433` (SQL) no estén en uso por otras aplicaciones.
