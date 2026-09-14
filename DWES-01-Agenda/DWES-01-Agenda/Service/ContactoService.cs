using CSharpFunctionalExtensions;

namespace DWES;

public class ContactoService(
        IContactoRepository repository,
        ICache<int, Contacto> cache
    ) : IContactoService {
    
    /// <inheritdoc cref="IContactoService.GetAll" />
    public IEnumerable<Contacto> GetAll(int pagina = 1, int tamPagina = 5) {
        return repository.GetAll(pagina, tamPagina);
    }

    /// <inheritdoc cref="IContactoService.GetById" />
    public Result<Contacto, DomainError> GetById(int id) {
        if (cache.Get(id) is {} cached) return Result.Success<Contacto, DomainError>(cached);

        var res = repository.GetById(id);
        if (res.IsFailure) return Result.Failure<Contacto, DomainError>(RepositoryErrors.IdNotFound(id));
        
        cache.Add(res.Value.Id, res.Value);
        return Result.Success<Contacto, DomainError>(res.Value);
    }

    /// <inheritdoc cref="IContactoService.GetByAlias" />
    public Result<Contacto, DomainError> GetByAlias(string aliasBusqueda = "") {
        var res = repository.GetByAlias(aliasBusqueda);
        if (res.IsFailure) return Result.Failure<Contacto, DomainError>(RepositoryErrors.AliasNotFound(aliasBusqueda));
        return Result.Success<Contacto, DomainError>(res.Value);
    }

    /// <inheritdoc cref="IContactoService.Create" />
    public Result<Contacto, DomainError> Create(Contacto contacto) {
        return repository.Create(contacto)
            .Tap(c => cache.Add(c.Id, c));
    }

    /// <inheritdoc cref="IContactoService.Update" />
    public Result<Contacto, DomainError> Update(int id, Contacto contacto) {
        return ComprobarExistencia(id)
            .Bind(c => repository.Update(id, contacto))
            .Tap(c => cache.Remove(id))
            .Tap(c => cache.Add(id, contacto));
    }

    /// <inheritdoc cref="IContactoService.Delete" />
    public Result<Contacto, DomainError> Delete(int id) {
        return ComprobarExistencia(id)
            .Bind(c => repository.Delete(id))
            .Tap(c => cache.Remove(id));    
    }
    
    /// <summary>
    /// Comprueba si ya hay un contacto registrado en el sistema con un id.
    /// </summary>
    /// <param name="id">Id a buscar.</param>
    /// <returns>El contactp si lo encuentra y error si no.</returns>
    private Result<Contacto, DomainError> ComprobarExistencia(int id) {
        var res = repository.GetById(id);
        return res.IsSuccess
            ? Result.Success<Contacto, DomainError>(res.Value)
            : Result.Failure<Contacto, DomainError>(RepositoryErrors.IdNotFound(id));
    }
}