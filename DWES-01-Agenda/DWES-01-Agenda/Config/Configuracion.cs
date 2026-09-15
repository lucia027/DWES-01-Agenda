using Microsoft.Extensions.Configuration;

namespace DWES;

/// <summary>
/// Clase de configuracion que se encarga de leer el appsettings.json
/// </summary>
public static class Configuracion {
    static Configuracion() {
        Configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json",  false,  true)
            .Build();
    }

    public static IConfiguration Configuration { get; }
    
    public static string DataFolder => Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        Configuration.GetValue<string>("Repository:Directory") ?? "data");
    
    public static string ConnectionString => Configuration.GetValue<string>("Repository:ConnectionString") ?? "Data Source=data/DWES-01-Agenda.db";
    
    public static int CacheSize => Configuration.GetValue("Cache:Size", 10);
    
    public static bool DropData => Configuration.GetValue("Repository:DropData", false);
    
    public static bool SeedData => Configuration.GetValue("Repository:SeedData", true);
    
    public static bool UseLogicalDelete => Configuration.GetValue("Repository:UseLogicalDelete", true);
}