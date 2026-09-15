using DWES;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Agenda.Test.Config;

[TestFixture]
public sealed class ConfiguracionTests {
    [Test]
    public void Configuration_AlInicializar_NoDebeSerNula() {
        // Act
        var configuration = Configuracion.Configuration;

        // Assert
        configuration.Should().NotBeNull();
    }

    [Test]
    public void DataFolder_RetornaRutaAbsoluta() {
        // Act
        var dataFolder = Configuracion.DataFolder;

        // Assert
        dataFolder.Should().NotBeNullOrWhiteSpace();
        Path.IsPathRooted(dataFolder).Should().BeTrue();
    }

    [Test]
    public void DataFolder_DebeUsarDirectorioConfigurado() {
        // Arrange
        var directory = Configuracion.Configuration.GetValue<string>("Repository:Directory") ?? "data";

        var rutaEsperada = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, directory);

        // Act
        var resultado = Configuracion.DataFolder;

        // Assert
        resultado.Should().Be(rutaEsperada);
    }

    [Test]
    public void ConnectionString_RetornaCadenaNoVacia() {
        // Act
        var connectionString = Configuracion.ConnectionString;

        // Assert
        connectionString.Should().NotBeNullOrWhiteSpace();
    }

    [Test]
    public void ConnectionString_DebeCoincidirConConfiguracion() {
        // Arrange
        var esperado = Configuracion.Configuration.GetValue<string>("Repository:ConnectionString") ?? "Data Source=data/DWES-01-Agenda.db";

        // Act
        var resultado = Configuracion.ConnectionString;

        // Assert
        resultado.Should().Be(esperado);
    }

    [Test]
    public void CacheSize_DebeCoincidirConConfiguracion() {
        // Arrange
        var esperado = Configuracion.Configuration.GetValue("Cache:Size", 10);

        // Act
        var resultado = Configuracion.CacheSize;

        // Assert
        resultado.Should().Be(esperado);
    }

    [Test]
    public void CacheSize_DebeSerMayorQueCero() {
        // Act
        var cacheSize = Configuracion.CacheSize;

        // Assert
        cacheSize.Should().BeGreaterThan(0);
    }

    [Test]
    public void DropData_DebeCoincidirConConfiguracion() {
        // Arrange
        var esperado = Configuracion.Configuration.GetValue("Repository:DropData", false);

        // Act
        var resultado = Configuracion.DropData;

        // Assert
        resultado.Should().Be(esperado);
    }

    [Test]
    public void SeedData_DebeCoincidirConConfiguracion() {
        // Arrange
        var esperado = Configuracion.Configuration.GetValue("Repository:SeedData", true);

        // Act
        var resultado = Configuracion.SeedData;

        // Assert
        resultado.Should().Be(esperado);
    }

    [Test]
    public void UseLogicalDelete_DebeCoincidirConConfiguracion() {
        // Arrange
        var esperado = Configuracion.Configuration.GetValue("Repository:UseLogicalDelete", true);

        // Act
        var resultado = Configuracion.UseLogicalDelete;

        // Assert
        resultado.Should().Be(esperado);
    }
}