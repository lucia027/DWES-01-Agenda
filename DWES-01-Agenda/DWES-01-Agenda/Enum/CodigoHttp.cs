namespace DWES;

/// <summary>
/// Códigos HTTP utilizados por la aplicación.
/// </summary>
public enum CodigoHttp {
    Ok = 200,
    Created = 201,
    NoContent = 204,

    BadRequest = 400,
    NotFound = 404,
    MethodNotAllowed = 405,

    InternalServerError = 500
}