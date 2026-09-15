using CSharpFunctionalExtensions;

namespace DWES;

/// <summary>
/// Contrato que contextualiza ICrudRepository para int y contactos.
/// </summary>
public interface IContactoRepository : ICrudRepository<int, Contacto> {

    /// <summary>
    /// Muestra todos los contactos que contengan el alias de forma paginada.
    /// </summary>
    /// <param name="aliasBusqueda">Alias por el que filtrar los contactos.</param>
    /// <returns></returns>
    Result<Contacto, DomainError> GetByAlias(string aliasBusqueda = "");
}