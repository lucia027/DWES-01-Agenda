# 📒 DWES-01-Agenda

Aplicación de **gestión de una agenda de contactos** desarrollada en C# como proyecto de 2º de DAW.

El objetivo del proyecto es practicar una arquitectura básica de backend, el acceso a datos con SQLite, el uso de caché, testing, logging y una pequeña simulación del funcionamiento del protocolo HTTP desde consola.

---

## ✨ Funcionalidad

La aplicación permite gestionar contactos de una agenda mediante operaciones CRUD:

- `GET` → consultar contactos.
- `POST` → crear un contacto.
- `PUT` → modificar un contacto.
- `DELETE` → eliminar un contacto.

Cada contacto contiene la siguiente información principal:

- **Nombre**
- **Alias**
- **Teléfono**
- **Email**

Además, los contactos incluyen metadatos para controlar su estado dentro de la aplicación, como:

- Fecha de creación.
- Fecha de última modificación.
- Fecha de eliminación.
- Indicador de borrado lógico.

El programa simula respuestas similares a las utilizadas en HTTP:

| Código | Significado |
|---|---|
| `200 OK` | Operación realizada correctamente |
| `201 Created` | Recurso creado correctamente |
| `204 No Content` | Operación correcta sin contenido de respuesta |
| `400 Bad Request` | Petición incorrecta |
| `404 Not Found` | Recurso no encontrado |
| `405 Method Not Allowed` | Verbo no permitido |
| `500 Internal Server Error` | Error interno |

---

## 🧱 Estructura general

```text
Program
   │
   ▼
ContactoService
   │
   ├── CacheLru
   │
   ▼
ContactosEfcRepository
   │
   ▼
Entity Framework Core
   │
   ▼
SQLite
```

El `Program` se encarga de realizar las peticiones de prueba utilizando verbos HTTP.

El `ContactoService` contiene la lógica de negocio y se comunica con la caché y el repositorio.

El repositorio gestiona el acceso a SQLite mediante Entity Framework Core.

---

# 🛠️ Tecnologías y decisiones tomadas

## 📦 Records en lugar de clases para los modelos

Para representar los modelos se utilizan `record` en lugar de `class`.

Un `record` resulta adecuado cuando el objetivo principal de un objeto es almacenar datos, como ocurre con un contacto.

También permite crear copias modificadas de una forma sencilla mediante `with`:

```csharp
var actualizado = contacto with {
    Nombre = "Nuevo nombre"
};
```

Por este motivo encajan bien en este proyecto, donde los modelos representan principalmente información y estado.

---

## 🗄️ Entity Framework Core en lugar de Dapper o ADO.NET

Para acceder a la base de datos SQLite se utiliza **Entity Framework Core**.

Entity Framework Core funciona como ORM y permite trabajar con la base de datos utilizando objetos C# y consultas LINQ.

### ¿Por qué Entity Framework Core?

- Simplifica el acceso a SQLite.
- Permite utilizar LINQ.
- Reduce código repetitivo.
- Evita gestionar manualmente gran parte de las conexiones y comandos SQL.
- Facilita el mantenimiento del repositorio.
- Se integra de forma natural con los modelos y entidades de C#.

Con **ADO.NET** sería necesario escribir más código para abrir conexiones, crear comandos, ejecutar consultas y transformar manualmente los resultados.

**Dapper** también es una alternativa ligera y rápida, pero obliga a escribir directamente las consultas SQL.

Para este proyecto, Entity Framework Core permite mantener el repositorio más sencillo y legible.

---

## ⚡ Caché LRU

El proyecto utiliza una caché **LRU (Least Recently Used)** con una capacidad máxima configurable.

La caché almacena temporalmente los contactos utilizados recientemente.

Cuando la caché está llena y se necesita añadir un nuevo elemento, se elimina el contacto que lleva más tiempo sin utilizarse.

### Ventajas de utilizar LRU

- Mantiene una capacidad máxima controlada.
- Evita que la memoria utilizada crezca indefinidamente.
- Conserva los elementos consultados más recientemente.
- Reduce accesos innecesarios al repositorio y a SQLite.
- Permite mejorar el rendimiento en consultas repetidas.

La implementación utiliza:

```text
Dictionary
```

para acceder rápidamente a los elementos, y:

```text
LinkedList
```

para controlar el orden de uso.

---

## 🧪 FluentAssertions para los tests

Los tests utilizan **FluentAssertions** junto con NUnit.

Su principal ventaja es que permite escribir comprobaciones más claras y fáciles de leer.

Por ejemplo:

```csharp
resultado.IsSuccess.Should().BeTrue();
resultado.Value.Should().BeEquivalentTo(contacto);
```

Esto resulta más legible que realizar todas las comprobaciones únicamente con las aserciones tradicionales.

Además, FluentAssertions genera mensajes de error bastante descriptivos cuando una prueba falla.

### Cobertura

Actualmente el proyecto cuenta con aproximadamente un **67 % de cobertura de tests**.

Se prueban tanto casos correctos como casos de error en operaciones como:

- Obtener contactos.
- Buscar por ID.
- Buscar por alias.
- Crear.
- Actualizar.
- Eliminar.
- Uso de la caché.

---

## 📝 Serilog para los logs

Para el sistema de logging se utiliza **Serilog**.

Serilog permite registrar información con diferentes niveles:

- `Verbose`
- `Debug`
- `Information`
- `Warning`
- `Error`
- `Fatal`

En el proyecto se utiliza especialmente el nivel **Debug** para poder seguir el funcionamiento interno de la aplicación durante el desarrollo.

Por ejemplo:

```csharp
_logger.Debug(
    "Se ha encontrado la clave: {Key} en la cache.",
    key
);
```

Los logs permiten comprobar fácilmente acciones como:

- Accesos a la caché.
- Inserciones y eliminaciones.
- Errores del repositorio.
- Operaciones sobre contactos.
- Flujo interno de la aplicación.

Serilog también permite utilizar **logging estructurado**, haciendo que los mensajes sean más fáciles de filtrar, analizar y reutilizar.

Otra ventaja es que el destino de los logs puede cambiarse fácilmente en el futuro, por ejemplo para guardarlos en archivos además de mostrarlos por consola.

---

## ⚙️ Configuración mediante `appsettings.json`

La configuración de la aplicación se encuentra separada del código mediante un archivo `appsettings.json`.

Ejemplo:

```json
{
  "Cache": {
    "Size": 5
  },
  "Repository": {
    "Directory": "data",
    "ConnectionString": "Data Source=data/DWES-01-Agenda.db",
    "DropData": true,
    "SeedData": true,
  }
}
```

### ¿Por qué utilizar `appsettings.json`?

Permite cambiar la configuración de la aplicación sin modificar directamente el código fuente.

Desde este archivo se pueden configurar aspectos como:

- Tamaño máximo de la caché.
- Directorio de datos.
- Cadena de conexión de SQLite.
- Eliminación de la base de datos al iniciar.
- Carga automática de datos iniciales.

La clase `Configuracion` centraliza el acceso a estos valores.

Por ejemplo:

```csharp
var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlite(Configuracion.ConnectionString)
    .Options;

var repository = new ContactosEfcRepository(
    context,
    dropData: Configuracion.DropData,
    seedData: Configuracion.SeedData
);

var cache = new CacheLru<int, Contacto>(
    Configuracion.CacheSize
);
```

Esto permite separar claramente:

```text
Configuración
      ↓
Lógica de la aplicación
```

y evita tener valores como rutas, tamaños o cadenas de conexión escritos directamente en el `Program`.

---

## 🌐 Simulación del protocolo HTTP

Aunque el proyecto se ejecuta por consola, el `Program` está planteado para practicar la lógica básica de una API.

Por ejemplo:

```csharp
Ejecutar(Verbos.Get);

Ejecutar(Verbos.Get, id: 1);

Ejecutar(Verbos.Get, alias: "Laurita");

Ejecutar(Verbos.Post, contacto: nuevoContacto);

Ejecutar(
    Verbos.Put,
    id: 3,
    contacto: contactoModificado
);

Ejecutar(Verbos.Delete, id: 4);
```

Esto permite relacionar cada operación CRUD con su verbo HTTP correspondiente antes de trabajar con una API web real.

---

## 🎯 Objetivo del proyecto

El objetivo principal de **DWES-01-Agenda** es practicar una arquitectura sencilla de backend separando correctamente las distintas responsabilidades:

- Modelos.
- Servicio.
- Repositorio.
- Persistencia.
- Caché.
- Configuración.
- Logging.
- Testing.
- Gestión básica de peticiones y códigos HTTP.

El resultado es una pequeña agenda de contactos fácil de entender, probar y ampliar durante el desarrollo de la asignatura.
