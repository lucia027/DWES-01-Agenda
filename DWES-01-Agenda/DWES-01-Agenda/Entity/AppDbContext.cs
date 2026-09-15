using Microsoft.EntityFrameworkCore;

namespace DWES;

/// <summary>
/// Contexto para la base de datos de Entity Framework Core.
/// </summary>
public class AppDbContext : DbContext{

    private readonly String _connectionString;

    public AppDbContext(string connectionString) {
        _connectionString = connectionString;
    }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
        _connectionString = "";
    }
    
    public DbSet<ContactoEntity> Agenda { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        if (!optionsBuilder.IsConfigured) optionsBuilder.UseSqlite(_connectionString);
    }
}