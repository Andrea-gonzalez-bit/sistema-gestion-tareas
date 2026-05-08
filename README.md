# Sistema de Gestión de Tareas

Aplicación full stack para administrar tareas y estados. La solución incluye una API RESTful en .NET 8 con autenticación JWT, manejo global de errores, persistencia en SQL Server con Entity Framework Core y una interfaz en React + TypeScript.

El sistema permite iniciar sesión, listar tareas y estados, crear, editar, eliminar, filtrar, ordenar, paginar y consultar detalles desde una interfaz web protegida.

## Tecnologías

Backend:

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- JWT Bearer Authentication
- Serilog
- Swagger / OpenAPI

Frontend:

- React
- TypeScript
- Vite
- Redux Toolkit
- react-router-dom
- react-hook-form
- CSS

## Estructura del proyecto

```txt
ApiGenerico.Application      # DTOs, contratos y servicios de aplicación
ApiGenerico.Domain           # Entidades, modelos y constantes de dominio
ApiGenerico.Infrastructure   # DbContext, configuraciones EF Core, migraciones y repositorios base
ApiGenerico.Utils            # Utilidades compartidas, incluyendo cifrado
ApiGenerico.WebAPI           # API, controladores, middleware, Swagger, JWT y configuración
ApiGenerico.Frontend         # Aplicación React + TypeScript + Vite
```

## Funcionalidades implementadas

Autenticación:

- Login con JWT.
- Rutas frontend protegidas.
- Credenciales de acceso precargadas.
- Validación controlada si las credenciales son modificadas.
- Confirmación antes de cerrar sesión.

Tareas:

- Listar tareas con paginación.
- Buscar por título o descripción.
- Filtrar por estado.
- Filtrar por rango de fechas.
- Ordenar por id, título, fecha límite, estado, creación o actualización.
- Crear tareas.
- Editar tareas.
- Eliminar tareas.
- Ver detalle de tarea en modal.

Estados:

- Listar estados.
- Crear estados.
- Editar estados.
- Eliminar estados solo si no tienen tareas asociadas.
- Ver detalle de estado en modal.
- Paginación local de 5 registros por página en frontend.

Experiencia de usuario:

- Menú lateral expandible y contraíble.
- Dashboard.
- Mensajes de éxito y error.
- Loaders.
- Estados vacíos.
- Modales internos para confirmación y detalle.
- Errores visibles en español.

## Base de datos

La base de datos esperada se llama:

```txt
TaskManagement
```

Tablas principales:

```txt
State
- Id
- Name
- CreatedAt
- UpdatedAt
```

```txt
Task
- Id
- Title
- Description
- DueDate
- StateId
- CreatedAt
- UpdatedAt
```

La migración inicial crea las tablas, la relación entre `Task` y `State`, índices para consultas y seed inicial con:

- Pendiente
- En Progreso
- Completado

La cadena de conexión está configurada en `ApiGenerico.WebAPI/appsettings.json` de forma encriptada y es desencriptada por la aplicación al iniciar.

## Credenciales de acceso

La prueba técnica entrega credenciales encriptadas. El frontend las carga automáticamente en el formulario de login.

```txt
User: 9gk7sPAj9hE=
Password: NsX8xEav35+BvurRn3x2bANt7lnq2RJ6odp/zr3HQ+k=
```

Importante: el frontend no vuelve a encriptar estas credenciales. Las envía exactamente como están escritas.

## Ejecución del backend

Requisitos:

- Visual Studio 2022
- .NET 8 SDK
- Acceso a SQL Server configurado en la cadena de conexión

Pasos:

1. Abrir la solución:

```txt
ApiGenerico.sln
```

2. Configurar como proyecto de inicio:

```txt
ApiGenerico.WebAPI
```

3. Seleccionar el perfil de ejecución.

Perfil `https`:

```txt
https://localhost:7147
```

Perfil `IIS Express`:

```txt
https://localhost:44391
```

4. Ejecutar el proyecto desde Visual Studio.

5. Abrir Swagger según el perfil usado:

```txt
https://localhost:7147/swagger
```

```txt
https://localhost:44391/swagger
```

## Ejecución del frontend

Requisitos:

- Node.js
- npm

Entrar a la carpeta del frontend:

```bash
cd ApiGenerico.Frontend
```

Instalar dependencias si aún no existen:

```bash
npm install
```

Ejecutar en modo desarrollo:

```bash
npm run dev
```

Abrir:

```txt
http://localhost:5173/
```

## Configuración de URL de la API

El frontend usa la variable:

```env
VITE_API_BASE_URL
```

Archivo:

```txt
ApiGenerico.Frontend/.env
```

Si el backend se ejecuta con perfil `https`, usar:

```env
VITE_API_BASE_URL=https://localhost:7147
```

Si el backend se ejecuta con `IIS Express`, usar:

```env
VITE_API_BASE_URL=https://localhost:44391
```

Después de cambiar el `.env`, reiniciar el servidor de Vite.

## Build del frontend

Desde `ApiGenerico.Frontend`:

```bash
npm run build
```

Este comando ejecuta TypeScript y genera la versión de producción con Vite.

## Endpoints principales

Autenticación:

```txt
POST /api/token/authentication
```

Tareas:

```txt
GET    /api/tasks
GET    /api/tasks/{id}
POST   /api/tasks
PUT    /api/tasks/{id}
DELETE /api/tasks/{id}
GET    /api/tasks/states
```

Estados:

```txt
GET    /api/states
GET    /api/states/{id}
POST   /api/states
PUT    /api/states/{id}
DELETE /api/states/{id}
```

Todos los endpoints de tareas y estados están protegidos con JWT.

## Decisiones técnicas

- Se mantuvo una arquitectura por capas para separar responsabilidades.
- Se usaron DTOs para evitar exponer directamente las entidades.
- Se configuró JWT para proteger los endpoints funcionales.
- Se implementó middleware global para errores de negocio y errores no controlados.
- Se usaron Data Annotations para validar entrada en DTOs.
- Se usaron proyecciones con `Select` para retornar solo los datos necesarios.
- Se agregaron índices en `StateId` y `DueDate` para apoyar consultas frecuentes.
- Se usó `AsNoTracking` en consultas de lectura.
- Se usaron transacciones en operaciones de escritura.
- En frontend se centralizaron llamadas HTTP y manejo de sesión con Redux Toolkit.
- Las confirmaciones nativas del navegador fueron reemplazadas por modales internos.
- Se mantuvo la autenticación original sin modificar el esquema de credenciales encriptadas.

## Validaciones y manejo de errores

Backend:

- Validación de modelos con Data Annotations.
- Validación de rango de fechas.
- Validación de estado existente al crear o editar tareas.
- Validación de duplicados en estados.
- Bloqueo de eliminación de estados con tareas asociadas.
- Manejo global de excepciones.

Frontend:

- Validación de campos requeridos.
- Mensajes de error visibles en pantalla.
- Mensje controlado si la API no está disponible.
- Mensaje controlado si las credenciales de login son incorrectas.
- Modales para confirmar eliminaciones y cierre de sesión.

## Flujo recomendado de validación manual

1. Ejecutar backend.
2. Abrir Swagger y confirmar que carga.
3. Ejecutar frontend.
4. Iniciar sesión con las credenciales precargadas.
5. Crear una tarea.
6. Editar la tarea.
7. Ver detalle de la tarea.
8. Filtrar tareas por texto, estado y fecha.
9. Cambiar ordenamiento.
10. Validar paginación.
11. Eliminar la tarea.
12. Crear un estado.
13. Editar el estado.
14. Ver detalle del estado.
15. Intentar eliminar un estado con tareas asociadas y validar el mensaje de error.
16. Eliminar un estado sin tareas asociadas.
17. Apagar el backend y validar el mensaje de conexión en español.


