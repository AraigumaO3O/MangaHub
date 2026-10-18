Propuesta de caso de negocio

Caso: MangaHub, biblioteca digital de mangas

Problema

Quienes leen manga suelen perder el hilo de lo que estan leyendo: que titulos tienen pendientes, cuales estan leyendo y en que capitulo quedaron. Normalmente lo anotan en listas sueltas, en notas del celular o lo recuerdan de memoria. Un servicio que junte el catalogo y el avance de cada lector en un solo lugar resuelve ese problema.

Como funciona el negocio

La plataforma tiene un catalogo de mangas, cada uno con su autor y su categoria, que administra el equipo del servicio. Cada lector se registra, arma su biblioteca personal y va marcando cada manga como Pendiente, Leyendo o Terminado, guardando el capitulo en que va.

Usuarios

- Lector: consulta el catalogo y maneja su propia biblioteca.
- Administrador: mantiene el catalogo (crea, edita y elimina mangas).

Funcionalidades

- Registro e inicio de sesion
- Catalogo de mangas con autor y categoria
- Biblioteca personal con estado de lectura y capitulo actual
- Administracion del catalogo, solo para el administrador

Modelo de datos

- Usuario: email, contrasena cifrada y rol
- Autor
- Categoria
- Manga: pertenece a un autor y a una categoria
- EntradaBiblioteca: une a un usuario con un manga y guarda el estado y el capitulo actual

Alcance

Incluye la API REST con autenticacion JWT, dos roles, base de datos SQL Server y pruebas de integracion. No incluye aplicacion web ni movil, ni la lectura de los capitulos dentro de la plataforma.

Relacion con el proyecto del semestre anterior

El semestre pasado hice una aplicacion MVC con vistas sobre una idea parecida, sin API, sin autenticacion y sin patron Repository. Este trabajo se hizo desde cero como API REST y aplica los contenidos de la Unidad 2.
