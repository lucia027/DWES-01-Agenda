namespace DWES;

/// <summary>
///  Clase abstracta para todos los errores del sistema.
/// </summary>
/// <param name="Message"></param>
public abstract record DomainError(string Message) { }