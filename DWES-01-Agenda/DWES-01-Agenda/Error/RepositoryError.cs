namespace DWES;

public abstract record RepositoryError(string Message) : DomainError(Message) {
    public sealed record IdNotFound(int id)
        : RepositoryError($"No se ha podido encontrar el contacto con el id: {id}");
    
    public sealed record AliasNotFound(string alias)
        : RepositoryError($"No se ha podido encontrar el contacto con el alias: {alias}");
    
    public sealed record CreationError()
        : RepositoryError($"Ha surgido un error en la creacion de la nueva entidad.");
    
    public sealed record UpdatingError()
        : RepositoryError($"Ha surgido un error en la actualizacion de la entidad.");
    
    public sealed record DeletingError()
        : RepositoryError($"Ha surgido un error en la eliminacion de la entidad.");

}

public static class RepositoryErrors {
    public static DomainError IdNotFound(int id) {
        return new RepositoryError.IdNotFound(id);
    }
    
    public static DomainError AliasNotFound(string alias) {
        return new RepositoryError.AliasNotFound(alias);
    }
    
    public static DomainError CreationError() {
        return new RepositoryError.CreationError();
    }
    
    public static DomainError UpdatingError() {
        return new RepositoryError.UpdatingError();
    }
    
    public static DomainError DeletingError() {
        return new RepositoryError.DeletingError();
    }
}