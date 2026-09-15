using System.Text;
using Microsoft.EntityFrameworkCore;
using Serilog;
using DWES;


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .CreateLogger();


Console.Title = "Agenda de contactos - DWES-01-Agenda";
Console.OutputEncoding = Encoding.UTF8;
Console.Clear();

// Programa principal
Main();

// Limpieza de logs y salida
Log.CloseAndFlush();
Console.WriteLine("\n⌨️ Presiona una tecla para salir...");
Console.ReadKey();

// Programa principal
void Main() {

    // ---------------------------------------------------------
    // CONFIGURACIÓN
    // ---------------------------------------------------------

    Directory.CreateDirectory(Configuracion.DataFolder);

    var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseSqlite(Configuracion.ConnectionString)
        .Options;

    using var context = new AppDbContext(options);

    var repository = new ContactosEfcRepository(context, dropData: Configuracion.DropData, seedData: Configuracion.SeedData);
    var cache = new CacheLru<int, Contacto>(Configuracion.CacheSize);
    var service = new ContactoService(repository, cache);

    // ---------------------------------------------------------
    // PETICIONES
    // ---------------------------------------------------------
    
    // GET todos
    Ejecutar(Verbos.Get);

    // GET por ID
    Ejecutar(Verbos.Get, id: 1);
    
    // GET por alias
    Ejecutar(Verbos.Get, alias: "Laurita");

    // GET que provoca un 404
    Ejecutar(Verbos.Get, id: 999);

    // POST
    var nuevoContacto = new Contacto {
        Alias = "Pepe",
        Nombre = "Pepe Garcia",
        Telefono = "600123123",
        Email = "pepe@gmail.com"
    };
    Ejecutar(Verbos.Post, contacto: nuevoContacto);


    // PUT
    var contactoModificado = new Contacto {
        Alias = "Alex",
        Nombre = "Alejandro Garcia Modificado",
        Telefono = "600999999",
        Email = "alex.modificado@gmail.com"
    };
    Ejecutar(Verbos.Put, id: 3, contacto: contactoModificado);

    // DELETE
    Ejecutar(Verbos.Delete, id: 4);

    // DELETE que provoca un 404
    Ejecutar(Verbos.Delete, id: 999);
    
    
    // =========================================================
    // EJECUTAR PETICIÓN
    // =========================================================

    void Ejecutar(Verbos verbo, int? id = null, string? alias = null, Contacto? contacto = null) {

        Console.WriteLine();
        Console.WriteLine("----------------------------------------");
        Console.WriteLine($"{verbo.ToString().ToUpper()}");
        Console.WriteLine("----------------------------------------");
        
        switch (verbo) {

            // =================================================
            // GET
            // =================================================
            case Verbos.Get:
                
                // No tiene contenido -> GetAll
                if (id == null && alias == null) {
                    GetAll();
                    break;
                }
                
                // Tiene ID -> GetById
                if (id != null && alias == null) {
                    GetById(id.Value);
                    break;
                }
                
                // Tiene Alias -> GetByAlias
                if (alias != null && id == null) {
                    GetByAlias(alias);
                    break;
                }
                
                // Si tiene ID y Alias a la vez no sabemos
                // qué búsqueda quiere realizar.
                Console.WriteLine("400 Bad Request");
                break;

            // =================================================
            // POST
            // =================================================

            case Verbos.Post:
                if (contacto == null) {
                    Console.WriteLine("400 Bad Request");
                    break;
                }
                Post(contacto);
                break;

            // =================================================
            // PUT
            // =================================================

            case Verbos.Put:

                if (id == null || contacto == null) {
                    Console.WriteLine("400 Bad Request");
                    break;
                }
                Put(id.Value, contacto);
                break;

            // =================================================
            // DELETE
            // =================================================

            case Verbos.Delete:
                if (id == null) {
                    Console.WriteLine("400 Bad Request");
                    break;
                }
                Delete(id.Value);
                break;

            // =================================================
            // VERBO NO PERMITIDO
            // =================================================
            default:
                Console.WriteLine("405 Method Not Allowed");
                break;
        }
    }



    // =========================================================
    // GET ALL
    // =========================================================

    void GetAll() {
        var contactos = service.GetAll();
        Console.WriteLine("200 OK");
        foreach (var contacto in contactos) {
            MostrarContacto(contacto);
        }
    }



    // =========================================================
    // GET BY ID
    // =========================================================

    void GetById(int id) {
        var resultado = service.GetById(id);
        if (resultado.IsFailure) {
            Console.WriteLine("404 Not Found");
            Console.WriteLine(resultado.Error);
            return;
        }
        Console.WriteLine("200 OK");
        MostrarContacto(resultado.Value);
    }

    // =========================================================
    // GET BY ALIAS
    // =========================================================

    void GetByAlias(string alias) {
        var resultado = service.GetByAlias(alias);
        if (resultado.IsFailure) {
            Console.WriteLine("404 Not Found");
            Console.WriteLine(resultado.Error);
            return;
        }

        Console.WriteLine("200 OK");
        MostrarContacto(resultado.Value);
    }



    // =========================================================
    // POST
    // =========================================================

    void Post(Contacto contacto) {

        var resultado = service.Create(contacto);
        if (resultado.IsFailure) {
            Console.WriteLine("500 Internal Server Error");
            Console.WriteLine(resultado.Error);
            return;
        }
        
        Console.WriteLine("201 Created");

        MostrarContacto(resultado.Value);
    }

    // =========================================================
    // PUT
    // =========================================================

    void Put(int id, Contacto contacto) {

        var resultado = service.Update(id, contacto);


        if (resultado.IsFailure) {
            // Comprobamos si el contacto no existe.
            var existente = service.GetById(id);
            
            if (existente.IsFailure) {
                Console.WriteLine("404 Not Found");
                return;
            }

            Console.WriteLine("500 Internal Server Error");
            Console.WriteLine(resultado.Error);
            return;
        }
        
        Console.WriteLine("200 OK");
        MostrarContacto(resultado.Value);
    }
    
    // =========================================================
    // DELETE
    // =========================================================

    void Delete(int id) {

        var resultado = service.Delete(id);


        if (resultado.IsFailure) {

            var existente = service.GetById(id);
            if (existente.IsFailure) {
                Console.WriteLine("404 Not Found");
                return;
            }
            
            Console.WriteLine("500 Internal Server Error");
            Console.WriteLine(resultado.Error);
            return;
        }
        
        Console.WriteLine("204 No Content");
    }



    // =========================================================
    // MOSTRAR CONTACTO
    // =========================================================

    void MostrarContacto(Contacto contacto) {
        Console.WriteLine(
            $"ID: {contacto.Id} | " +
            $"Alias: {contacto.Alias} | " +
            $"Nombre: {contacto.Nombre} | " +
            $"Teléfono: {contacto.Telefono} | " +
            $"Email: {contacto.Email}"
        );
    }
}