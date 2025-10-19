using Clientes.Domain;

namespace Clientes.Tests;

public class ClienteTests
{
    [Fact]
    public void Validar_ClienteValido_NoLanzaExcepcion()
    {
        // Arrange
        var cliente = new Cliente
        {
            Nombre = "Juan Pérez",
            Identificacion = "12345678",
            Correo = "juan@email.com",
            Direccion = "Calle 123 #45-67"
        };

        // Act & Assert
        var exception = Record.Exception(() => cliente.Validar());
        Assert.Null(exception);
    }

    [Fact]
    public void Validar_NombreVacio_LanzaExcepcion()
    {
        // Arrange
        var cliente = new Cliente
        {
            Nombre = "",
            Identificacion = "12345678",
            Correo = "juan@email.com",
            Direccion = "Calle 123 #45-67"
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => cliente.Validar());
        Assert.Equal("El nombre es requerido", exception.Message);
    }

    [Fact]
    public void Validar_IdentificacionVacia_LanzaExcepcion()
    {
        // Arrange
        var cliente = new Cliente
        {
            Nombre = "Juan Pérez",
            Identificacion = "",
            Correo = "juan@email.com",
            Direccion = "Calle 123 #45-67"
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => cliente.Validar());
        Assert.Equal("La identificación es requerida", exception.Message);
    }

    [Fact]
    public void Validar_CorreoVacio_LanzaExcepcion()
    {
        // Arrange
        var cliente = new Cliente
        {
            Nombre = "Juan Pérez",
            Identificacion = "12345678",
            Correo = "",
            Direccion = "Calle 123 #45-67"
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => cliente.Validar());
        Assert.Equal("El correo es requerido", exception.Message);
    }

    [Fact]
    public void Validar_DireccionVacia_LanzaExcepcion()
    {
        // Arrange
        var cliente = new Cliente
        {
            Nombre = "Juan Pérez",
            Identificacion = "12345678",
            Correo = "juan@email.com",
            Direccion = ""
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => cliente.Validar());
        Assert.Equal("La dirección es requerida", exception.Message);
    }
}