MangaHub API

API REST hecha en ASP.NET Core para el Trabajo 2 de Construccion de Software.
Sirve para llevar un catalogo de mangas y la biblioteca de cada lector. Cada lector
puede marcar un manga como Pendiente, Leyendo o Terminado y guardar en que capitulo va.
El administrador es el que agrega, edita y elimina los mangas del catalogo.

Tecnologias

- .NET 8 con controladores
- Entity Framework Core con SQL Server (migraciones)
- Patron Repository
- JWT para el login, con roles Admin y Lector
- Versionamiento por URL (/api/v1)
- Swagger
- Un middleware propio que registra cada peticion
- Pruebas de integracion con xUnit

Estructura

- Controllers: los endpoints
- Dtos: los datos que entran y salen de la API
- Repositories: acceso a la base de datos
- Services: creacion del token JWT
- Middleware: registro de peticiones
- Entities y Data: tablas y DbContext
- Migrations: migraciones de la base de datos
- MangaHub.Tests: pruebas de integracion

Configuracion

Los datos secretos no estan en el repositorio, se configuran con variables de entorno:

- ConnectionStrings__Default: cadena de conexion a SQL Server
- Jwt__Key: llave para firmar el token (minimo 32 caracteres)
- Jwt__Issuer: emisor del token
- Jwt__Audience: audiencia del token
- Seed__AdminEmail: email del administrador (opcional)
- Seed__AdminPassword: contrasena del administrador (opcional)

Un ejemplo de cadena de conexion es: Server=localhost;Database=MangaHubDb;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False

Como ejecutarlo

Dentro de la carpeta MangaHub.Api se ejecutan estos dos comandos, en este orden:

- dotnet ef database update
- dotnet run

Esto crea la base de datos con algunos autores, categorias y mangas de ejemplo.
Despues se abre Swagger en http://localhost:5018/swagger

Si se definen Seed__AdminEmail y Seed__AdminPassword antes de ejecutar, se crea el
usuario administrador al iniciar.

Usuarios y roles

- Cualquiera puede registrarse con POST /api/v1/Auth/register y queda como Lector.
- El administrador se crea con las variables Seed__ de arriba.
- El Lector puede ver el catalogo y manejar su propia biblioteca.
- Solo el Admin puede crear, editar y eliminar mangas.

Endpoints

- POST /api/v1/Auth/register (publico)
- POST /api/v1/Auth/login (publico, devuelve el token)
- GET /api/v1/Mangas (usuario con token)
- GET /api/v1/Mangas/{id} (usuario con token)
- POST /api/v1/Mangas (solo Admin)
- PUT /api/v1/Mangas/{id} (solo Admin)
- DELETE /api/v1/Mangas/{id} (solo Admin)
- GET /api/v1/Autores (usuario con token)
- GET /api/v1/Categorias (usuario con token)
- GET /api/v1/Biblioteca (usuario con token, solo la suya)
- POST /api/v1/Biblioteca (usuario con token)
- GET /api/v1/Biblioteca/{mangaId} (usuario con token)
- PUT /api/v1/Biblioteca/{mangaId} (usuario con token)
- DELETE /api/v1/Biblioteca/{mangaId} (usuario con token)

Para usar los endpoints que piden token, se hace login, se copia el token y se pega en el
boton Authorize de Swagger.

Pruebas

Desde la carpeta de la solucion se ejecuta: dotnet test MangaHub.Tests

Las pruebas usan una base de datos en memoria, no tocan la base real.

Documentacion

El archivo docs/swagger.json tiene la especificacion de la API exportada desde Swagger.
