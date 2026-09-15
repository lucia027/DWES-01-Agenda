using CSharpFunctionalExtensions;

namespace DWES;

/// <summary>
/// Interfaz que define los metodos del servicio.
/// </summary>
public interface IContactoService {

    /// <summary>
    /// Obtiene un enumerable de la agenda paginada y ordenada por nombre.
    /// </summary>
    /// <param name="pagina">Número de página que mostrar.</param>
    /// <param name="tamPagina">Número de elementos a mostrar por páginas.</param>
    /// <returns>Enumerable de agenda.</returns>
    IEnumerable<Contacto> GetAll(int pagina = 1, int tamPagina = 5);
    
    /// <summary>
    /// Obtiene un contacto según su id.
    /// </summary>
    /// <param name="id">Id a buscar.</param>
    /// <returns>El contacto que encuentre o error.</returns>
    Result<Contacto, DomainError> GetById(int id);
    
    /// <summary>
    /// Obtiene un contacto según su alias.
    /// </summary>
    /// <param name="aliasBusqueda">Alias a buscar.</param>
    /// <returns>El contacto que encuentre o error.</returns>
    Result<Contacto, DomainError> GetByAlias(string aliasBusqueda = "");
    
    /// <summary>
    /// Registra un nuevo contacto en el sistema.
    /// </summary>
    /// <param name="contacto">Contacto a crear.</param>
    /// <returns>Contacto creado.</returns>
    Result<Contacto, DomainError> Create(Contacto contacto);
    
    /// <summary>
    /// Actualiza un contacto en el sistema.
    /// </summary>
    /// <param name="id">Id del contacto a actualizar.</param>
    /// <param name="contacto">Contacto actualizado</param>
    /// <returns>Contacto actualizado o error.</returns>
    Result<Contacto, DomainError> Update(int id, Contacto contacto);
    
    /// <summary>
    /// Elimina un contacto en el sistema.
    /// </summary>
    /// <param name="id">Id del contacto a eliminar.</param>
    /// <returns>Contacto eliminado o error.</returns>
    Result<Contacto, DomainError> Delete(int id);
}