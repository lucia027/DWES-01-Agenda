using System;
using System.Globalization;
using System.Text;
using Serilog;


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug() 
    .WriteTo.Console() 
    .CreateLogger();


Console.Title = "Agenda de contactos - DWES-01-Agenda";
Console.OutputEncoding = Encoding.UTF8;
Console.Clear();


// Programa principal
Main(args);

// Limpieza de logs y salida
Log.CloseAndFlush(); // Asegura que todos los logs pendientes se escriban.
Console.WriteLine("\n⌨️ Presiona una tecla para salir...");
Console.ReadKey();

// Programa principal
void Main(string[] args) {

}

