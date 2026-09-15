namespace DWES;

/// <summary>
/// Representa unb contacto de la agenda dentro del sistema.
/// </summary>
public record Contacto {
    public int Id { get; init; }
    public string Alias { get; set; } = string.Empty;
    public string Nombre { get; set; } =  string.Empty;
    public string Telefono { get; set; } =  string.Empty;
    public string Email { get; set; } =  string.Empty;
    public DateTime CreateAt { get; set; }
    public DateTime? UpdateAt { get; set; }
    public DateTime? DeleteAt { get; set; }
    public bool IsDelete { get; set; }
}