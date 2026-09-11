namespace DWES;

public record Agenda {
    public int Id {get; init;}
    public string Alias { get; init; } = string.Empty;
    public string Nombre {get; init;} =  string.Empty;
    public string Telefono {get; init;} =  string.Empty;
    public string Email {get; init;} =  string.Empty;
}