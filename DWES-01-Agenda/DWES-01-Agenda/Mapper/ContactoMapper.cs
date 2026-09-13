using System.Globalization;

namespace DWES;

/// <summary>
/// Clase estatica para las funciones de mapear un contacto a un contacto entity y viceversa.
/// </summary>
public static class ContactoMapper {

    /// <summary>
    /// Convierte un contacto entity a un contacto.
    /// </summary>
    /// <param name="entity">Contacto a convertir.</param>
    /// <returns>Contacto convertido.</returns>
    public static Contacto ToModel(this ContactoEntity entity) {
        return new Contacto {
            Id = entity.Id,
            Alias = entity.Alias,
            Nombre = entity.Nombre,
            Telefono = entity.Telefono,
            Email = entity.Email,
            CreateAt = entity.CreateAt,
            UpdateAt = entity.UpdateAt,
            DeleteAt = entity.DeleteAt,
            IsDelete = entity.IsDelete
        };
    }

    /// <summary>
    /// Convierte un contacto a un contacto entity.
    /// </summary>
    /// <param name="contacto">Contacto a convertir.</param>
    /// <returns>Contacto entity convertido.</returns>
    public static ContactoEntity ToEntity(this Contacto contacto) {
        return new ContactoEntity {
            Id = contacto.Id,
            Alias = contacto.Alias,
            Nombre = contacto.Nombre,
            Telefono = contacto.Telefono,
            Email = contacto.Email,
            CreateAt = contacto.CreateAt,
            UpdateAt = contacto.UpdateAt,
            DeleteAt = contacto.DeleteAt,
            IsDelete = contacto.IsDelete
        };
    }

    /// <summary>
    /// Convierte una coleccion de contactos entity a contactos.
    /// </summary>
    /// <param name="items">Coleccion de contactos entity.</param>
    /// <returns>Coleccion convertida.</returns>
    public static IEnumerable<Contacto> ToModel(this IEnumerable<ContactoEntity> items) {
        return items.Select(i => i.ToModel());
    }
}