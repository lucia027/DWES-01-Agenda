using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DWES;

/// <summary>
/// Entidad para los contactos del sistema.
/// </summary>
[Table("Agenda")]
[Index(nameof(Telefono), IsUnique = true)]
[Index(nameof(Email), IsUnique = true)]
public record ContactoEntity {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; init; }
    
    [Required]
    [MaxLength(20)]
    public string Alias { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(20)]
    public string Nombre { get; set; } =  string.Empty;
    
    [Required]
    [MaxLength(9)]
    public string Telefono { get; set; } =  string.Empty;
    
    [Required]
    [MaxLength(20)]
    public string Email { get; set; } =  string.Empty;
    
    [Column(TypeName = "datetime2")]
    public DateTime CreateAt { get; set; }
    
    [Column(TypeName = "datetime2")]
    public DateTime? UpdateAt { get; set; }
    
    [Column(TypeName = "datetime2")]
    public DateTime? DeleteAt { get; set; }
    
    [Column(TypeName = "datetime2")]
    public bool IsDelete { get; set; }
}