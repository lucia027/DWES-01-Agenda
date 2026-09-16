using System.Text.Json;

namespace DWES;

/// <summary>
/// Controller encargado de recibir las peticiones,
/// procesar su contenido JSON y comunicarse con el servicio.
/// </summary>
public class ContactoController(ContactoService service) {
    
    private readonly JsonSerializerOptions _jsonOptions = new() {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };
    
    /// <summary>
    /// Procesa una petición formada por un verbo y un contenido JSON.
    /// </summary>
    /// <param name="verbo">Verbo de la petición.</param>
    /// <param name="contenido">Contenido en formato JSON.</param>
    /// <returns>Código HTTP y respuesta.</returns>
    public (CodigoHttp codigo, string respuesta) Procesar(Verbos verbo, string contenido = "{}") {
        try {
            return verbo switch {
                Verbos.Get => Get(contenido),
                Verbos.Post => Post(contenido),
                Verbos.Put => Put(contenido),
                Verbos.Delete => Delete(contenido),
                _ => (CodigoHttp.MethodNotAllowed, "Método no permitido.")
            };
        } catch (JsonException) {
            return (CodigoHttp.BadRequest, "El contenido enviado no es un JSON válido.");
        } catch (Exception ex) {
            return (CodigoHttp.InternalServerError, ex.Message);
        }
    }

    private (CodigoHttp codigo, string respuesta) Get(string contenido) {
        
        using var json = JsonDocument.Parse(contenido);
        var root = json.RootElement;

        if (!root.TryGetProperty("id", out _) && !root.TryGetProperty("alias", out _)) {
            var contactos = service.GetAll().ToList();
            return (CodigoHttp.Ok, JsonSerializer.Serialize(contactos, _jsonOptions));
        }

        if (root.TryGetProperty("id", out var idJson)) {
            var id = idJson.GetInt32();
            var resultado = service.GetById(id);
            
            if (resultado.IsFailure) return (CodigoHttp.NotFound, resultado.Error.ToString());
            return (CodigoHttp.Ok, JsonSerializer.Serialize(resultado.Value, _jsonOptions));
        }
        
        if (root.TryGetProperty("alias", out var aliasJson)) {
            var alias = aliasJson.GetString();
            if (string.IsNullOrWhiteSpace(alias)) return (CodigoHttp.BadRequest, "El alias no es válido."); 
            
            var resultado = service.GetByAlias(alias);
            if (resultado.IsFailure) return (CodigoHttp.NotFound, resultado.Error.ToString());
            
            return (CodigoHttp.Ok, JsonSerializer.Serialize(resultado.Value, _jsonOptions));
        }
        
        return (CodigoHttp.BadRequest, "Contenido GET incorrecto.");
    }

    private (CodigoHttp codigo, string respuesta) Post(string contenido) {
        var contacto = JsonSerializer.Deserialize<Contacto>(contenido, _jsonOptions);
        
        if (contacto == null)  return (CodigoHttp.BadRequest, "No se ha recibido un contacto válido."); 
        var resultado = service.Create(contacto);
        
        if (resultado.IsFailure) return (CodigoHttp.InternalServerError, resultado.Error.ToString());
        return (CodigoHttp.Created, JsonSerializer.Serialize(resultado.Value, _jsonOptions));
    }


    private (CodigoHttp codigo, string respuesta) Put(string contenido) {
        using var json = JsonDocument.Parse(contenido);
        var root = json.RootElement;

        if (!root.TryGetProperty("id", out var idJson)) return (CodigoHttp.BadRequest, "PUT necesita recibir un ID.");

        if (!root.TryGetProperty("contacto", out var contactoJson))
            return (CodigoHttp.BadRequest, "PUT necesita recibir un contacto.");

        var id = idJson.GetInt32();

        var contacto = JsonSerializer.Deserialize<Contacto>(contactoJson.GetRawText(), _jsonOptions);
        if (contacto == null) return (CodigoHttp.BadRequest, "El contacto recibido no es válido.");

        var existente = service.GetById(id);
        if (existente.IsFailure) return (CodigoHttp.NotFound, $"No existe ningún contacto con ID {id}.");

        var resultado = service.Update(id, contacto);
        if (resultado.IsFailure) return (CodigoHttp.InternalServerError, resultado.Error.ToString());

        return (CodigoHttp.Ok, JsonSerializer.Serialize(resultado.Value, _jsonOptions));

    }
    private (CodigoHttp codigo, string respuesta) Delete(string contenido) {
        using var json = JsonDocument.Parse(contenido);
        var root = json.RootElement;
        
        if (!root.TryGetProperty("id", out var idJson))  return (CodigoHttp.BadRequest, "DELETE necesita recibir un ID."); 
        
        var id = idJson.GetInt32();
        var existente = service.GetById(id);
        if (existente.IsFailure) return (CodigoHttp.NotFound, $"No existe ningún contacto con ID {id}.");
        
        var resultado = service.Delete(id);
        if (resultado.IsFailure) return (CodigoHttp.InternalServerError, resultado.Error.ToString());
        
        return (CodigoHttp.NoContent, string.Empty);
    }
}