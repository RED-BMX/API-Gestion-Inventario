# API Gestión de Inventario

REST API desarrollada con C# y ASP.NET Core para la gestión de productos, categorías y movimientos de inventario.

El proyecto implementa autenticación mediante JWT, autorización basada en roles, persistencia con PostgreSQL, Entity Framework Core, Swagger/OpenAPI, pruebas unitarias y un entorno de desarrollo completamente contenedorizado con Docker Compose.

## 🚀 Tecnologías
- C#
- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- PostgreSQL 16
- Npgsql
- JWT Bearer Authentication
- BCrypt
- Swagger / OpenAPI
- xUnit
- Docker
- Docker Compose
- Git / GitHub

## 📋 Funcionalidades

### Autenticación y autorización
- Registro de usuarios.
- Inicio de sesión.
- Contraseñas almacenadas mediante hashing con BCrypt.
- Autenticación mediante JWT.
- Autorización basada en roles.
- Roles:
  - `Admin`
  - `User`

### Categorías
- Crear categorías.
- Consultar todas las categorías.
- Consultar una categoría por ID.
- Actualizar categorías.
- Eliminar categorías.

### Productos
- Crear productos.
- Consultar todos los productos.
- Consultar un producto por ID.
- Actualizar productos.
- Eliminar productos.
- Asociación entre productos y categorías.
- Control de stock mínimo.

### Movimientos de inventario
- Registrar entradas de inventario.
- Registrar salidas de inventario.
- Actualización automática del stock.
- Validación de stock disponible.
- Registro de fecha y descripción del movimiento.
- Uso de transacciones para mantener la consistencia de los datos.

## 🏗️ Arquitectura

El proyecto utiliza una arquitectura por capas:

```text
Controller
    ↓
Service
    ↓
Repository
    ↓
DbContext
    ↓
PostgreSQL
Capas principales
Controllers: reciben y procesan las solicitudes HTTP.
Services: contienen la lógica de negocio.
Repositories: gestionan el acceso a los datos.
Data: contiene el DbContext de Entity Framework Core.
Models: entidades principales de la aplicación.
DTOs: objetos utilizados para las solicitudes y respuestas de la API.
Middleware: manejo global de excepciones.
Migrations: migraciones de Entity Framework Core.
📁 Estructura del proyecto
API-Gestion-Inventario/
│
├── Controllers/
│   ├── AuthController.cs
│   ├── CategoriesController.cs
│   ├── InventoryMovementsController.cs
│   └── ProductsController.cs
│
├── Data/
│   └── ApplicationDbContext.cs
│
├── DTOs/
│   ├── Auth/
│   ├── Categories/
│   ├── InventoryMovements/
│   └── Products/
│
├── Middleware/
│   └── ExceptionMiddleware.cs
│
├── Migrations/
│
├── Models/
│
├── Repositories/
│
├── Services/
│
├── Dockerfile
├── docker-compose.yml
├── .dockerignore
├── Program.cs
└── API-Gestion-Inventario.csproj
```

⚙️ Requisitos

Para ejecutar el proyecto directamente con .NET:

- .NET SDK 8
- PostgreSQL 16
- Git

Para ejecutarlo mediante Docker:

- Docker
- Docker Compose

## 🐳 Ejecución con Docker

La forma recomendada de ejecutar el proyecto es mediante Docker Compose.

Clona el repositorio:

```bash
git clone git@github.com:RED-BMX/API-Gestion-Inventario.git
cd API-Gestion-Inventario
```

Inicia los servicios:

```bash
docker compose up --build
```

Esto inicia:
- La API ASP.NET Core.
- PostgreSQL 16.
- La red interna entre ambos servicios.
- La base de datos persistente mediante un volumen Docker.

La API estará disponible en:

```
http://localhost:8080
```

Swagger estará disponible en:

```
http://localhost:8080/swagger
```

Para detener los contenedores:

```bash
docker compose down
```

El volumen de PostgreSQL se mantiene al utilizar docker compose down, por lo que los datos no se eliminan.

Para eliminar también los datos de la base de datos:

```bash
docker compose down -v
```

Este comando elimina el volumen de PostgreSQL y todos los datos almacenados en él.

## 🔐 Configuración

El entorno Docker utiliza variables de entorno para configurar:

- `ConnectionStrings__DefaultConnection`
- `Jwt__Key`
- `Jwt__Issuer`
- `Jwt__Audience`

Los valores incluidos actualmente en `docker-compose.yml` son únicamente para desarrollo local.

No deben utilizarse en producción. Para producción se deben utilizar secretos y credenciales seguras.

## 🗄️ Base de datos

El proyecto utiliza PostgreSQL como sistema de gestión de base de datos.

Entidades principales:
- Users
- Roles
- Categories
- Products
- InventoryMovements

Entity Framework Core se utiliza para:
- Modelado de entidades.
- Relaciones entre entidades.
- Configuración de restricciones.
- Migraciones.
- Persistencia de datos.

En el entorno Docker, las migraciones pendientes se aplican automáticamente cuando la aplicación se ejecuta en el entorno Development.

Los roles iniciales son:
- Admin
- User

## 📚 Swagger / OpenAPI

La API incluye documentación interactiva mediante Swagger.

Desde Swagger se pueden:
- Consultar los endpoints.
- Probar solicitudes HTTP.
- Visualizar DTOs.
- Consultar códigos de respuesta.
- Autenticarse mediante JWT.
- Probar endpoints protegidos.

Acceso:

```
http://localhost:8080/swagger
```

Para utilizar endpoints protegidos:
1. Ejecutar el endpoint de login.
2. Copiar el JWT obtenido.
3. Pulsar Authorize.
4. Introducir: `Bearer {token}`
5. Ejecutar los endpoints autorizados.

## 🔑 Roles

- **Admin**: Tiene permisos para realizar operaciones administrativas como:
  - Crear categorías.
  - Actualizar categorías.
  - Eliminar categorías.
  - Crear productos.
  - Actualizar productos.
  - Eliminar productos.
  - Registrar movimientos de inventario.

- **User**: Puede acceder a los recursos protegidos permitidos por la API, pero no puede realizar operaciones exclusivas del administrador.

## 🌐 Endpoints principales

- **Auth**
  - `POST /api/Auth/register`
  - `POST /api/Auth/login`

- **Categories**
  - `GET    /api/Categories`
  - `GET    /api/Categories/{id}`
  - `POST   /api/Categories`
  - `PUT    /api/Categories/{id}`
  - `DELETE /api/Categories/{id}`

- **Products**
  - `GET    /api/Products`
  - `GET    /api/Products/{id}`
  - `POST   /api/Products`
  - `PUT    /api/Products/{id}`
  - `DELETE /api/Products/{id}`

- **Inventory Movements**
  - `GET  /api/InventoryMovements`
  - `GET  /api/InventoryMovements/{id}`
  - `POST /api/InventoryMovements`

## 📦 Ejemplo de movimiento de inventario
Una entrada de inventario puede registrarse utilizando:

```json
{
  "productId": 1,
  "type": 0,
  "quantity": 5,
  "description": "Entrada de mercancía"
}
```

Una salida utiliza:

```json
{
  "productId": 1,
  "type": 1,
  "quantity": 3,
  "description": "Salida de mercancía"
}
```

La API actualiza automáticamente el stock del producto. También evita realizar una salida cuando no existe suficiente stock disponible.

## 🧪 Pruebas

El proyecto utiliza xUnit para las pruebas automatizadas.

Ejecutar las pruebas:

```bash
dotnet test
```

Estado actual:
- 31 tests passed
- 0 failed
- 0 skipped

## 🔁 Migraciones

Para crear una nueva migración:

```bash
dotnet ef migrations add NombreMigracion
```

Para aplicar las migraciones:

```bash
dotnet ef database update
```

En el entorno Docker de desarrollo, las migraciones pendientes se aplican automáticamente al iniciar la API.

## 🛡️ Buenas prácticas implementadas

- Arquitectura por capas.
- Separación de responsabilidades.
- DTOs para entrada y salida de datos.
- Validaciones mediante Data Annotations.
- Autenticación JWT.
- Autorización mediante roles.
- Hashing de contraseñas con BCrypt.
- Entity Framework Core.
- Migraciones de base de datos.
- PostgreSQL.
- Manejo global de excepciones.
- Uso de transacciones en movimientos de inventario.
- Documentación OpenAPI.
- Pruebas automatizadas.
- Contenedorización con Docker.
- `.dockerignore` para reducir el contexto de construcción.
- Variables de entorno para configuración.

## 📌 Estado del proyecto

Completado — versión de portafolio

El proyecto cumple con las funcionalidades principales de una API de gestión de inventario y cuenta con:
- API REST funcional.
- PostgreSQL.
- Entity Framework Core.
- JWT.
- Roles.
- Swagger/OpenAPI.
- Pruebas automatizadas.
- Docker.
- Docker Compose.
- Migraciones.
- Lógica de movimientos de inventario.

👨‍💻 Autor
RED-BMX

Proyecto desarrollado como parte de un portafolio de desarrollo backend con C# y .NET.