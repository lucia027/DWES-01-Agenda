namespace DWES;

/// <summary>
/// Representa unb contacto de la agenda dentro del sistema.
/// </summary>
public record Contacto {
    public int Id {get; init;}
    public string Alias { get; init; } = string.Empty;
    public string Nombre {get; init;} =  string.Empty;
    public string Telefono {get; init;} =  string.Empty;
    public string Email {get; init;} =  string.Empty;
}