using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DWES;

public class ContactosEfcRepository : IContactoRepository {

    private readonly ILogger _logger = Log.ForContext<ContactosEfcRepository>();
    private readonly AppDbContext _context;

    public ContactosEfcRepository(AppDbContext context, bool dropData = false, bool seedData = false) {
        _context = context;
        
        if (dropData) _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();

        if (seedData) {
            foreach (var c in ContactosFactory.Seed()) Create(c);
        }
    }

    /// <inheritdoc cref="IContactoRepository.GetAll" />
    public IEnumerable<Contacto> GetAll(int pagina = 1, int tamPagina = 5) {
        var query = _context.Agenda.AsNoTracking();
        query = query
            .OrderBy(c => c.Nombre)
            .Skip((pagina - 1) * tamPagina)
            .Take(tamPagina);
        return query.AsEnumerable().ToModel();
    }

    /// <inheritdoc cref="IContactoRepository.GetById" />
    public Result<Contacto, DomainError> GetById(int id) {
        try {
            var entity = _context.Agenda.FirstOrDefault(c => c.Id == id);
            return Result.Success<Contacto, DomainError>(entity!.ToModel());
        } catch (Exception) {
            _logger.Error($"Error al intentar encontrar el contacto con el id: {id}.");
            return Result.Failure<Contacto, DomainError>(RepositoryErrors.IdNotFound(id));
        }
    }
    
    /// <inheritdoc cref="IContactoRepository.GetByAlias" />
    public Result<Contacto, DomainError> GetByAlias(string aliasBusqueda = "") {
        try {
            var entity = _context.Agenda.FirstOrDefault(c => c.Alias == aliasBusqueda);
            return Result.Success<Contacto, DomainError>(entity!.ToModel());
        } catch (Exception) {
            _logger.Error($"Error al intentar encontrar el contacto con el alias: {aliasBusqueda}.");
            return Result.Failure<Contacto, DomainError>(RepositoryErrors.AliasNotFound(aliasBusqueda));
        }
    }

    /// <inheritdoc cref="IContactoRepository.Create" />
    public Result<Contacto, DomainError> Create(Contacto entity) {
        try {
            var contacto = entity with { CreateAt = DateTime.Now, UpdateAt = null, DeleteAt = null, IsDelete = false };
            _context.Agenda.Add(contacto.ToEntity());
            _context.SaveChanges();
            
            _logger.Debug("Contacto creado correctamente");
            return Result.Success<Contacto, DomainError>(contacto);
        } catch (Exception) {
            _logger.Debug("Error al intentar crear un nuevo contacto");
            return Result.Failure<Contacto, DomainError>(RepositoryErrors.CreationError());
        }
    }

    /// <inheritdoc cref="IContactoRepository.Update" />
    public Result<Contacto, DomainError> Update(int id, Contacto entity) {
        try {
            if (GetById(id).IsFailure) {
                _logger.Debug($"No se ha podido actualizar el contacto con el id: {id}, no existe.");
                return Result.Failure<Contacto, DomainError>(RepositoryErrors.IdNotFound(id));
            }

            entity = entity with { UpdateAt = DateTime.Now, DeleteAt = null, IsDelete = false };
            _context.SaveChanges();
        
            _logger.Debug("Contacto actualizado correctamente");
            return Result.Success<Contacto, DomainError>(entity);
        } catch (Exception ) {
            _logger.Debug("Error al intentar actualizar el contacto");
            return Result.Failure<Contacto, DomainError>(RepositoryErrors.UpdatingError());
        }
    }

    /// <inheritdoc cref="IContactoRepository.Delete" />
    public Result<Contacto, DomainError> Delete(int id) {
        try {
            var eliminado = _context.Agenda.FirstOrDefault(c => c.Id == id);    
            if (eliminado == null) {
                _logger.Debug($"No se ha podido actualizar el contacto con el id: {id}, no existe.");
                return Result.Failure<Contacto, DomainError>(RepositoryErrors.IdNotFound(id));
            }

            eliminado = eliminado with { DeleteAt = DateTime.Now, IsDelete = true };
            _context.SaveChanges();
            return Result.Success<Contacto, DomainError>(eliminado.ToModel());

        } catch (Exception ) {
            _logger.Debug("Error al intentar eliminar el contacto");
            return Result.Failure<Contacto, DomainError>(RepositoryErrors.DeletingError());
        }
    }
}