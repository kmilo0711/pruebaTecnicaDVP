using Facturas.Domain;
using Facturas.Application;
using Facturas.Application.DTOs;
using Moq;

namespace Facturas.Tests;

public class FacturaTests
{
    [Fact]
    public void Validar_FacturaValida_NoLanzaExcepcion()
    {
        // Arrange
        var factura = new Factura
        {
            ClienteId = 1,
            FechaEmision = DateTime.Now.AddDays(-1),
            MontoTotal = 100.50m
        };

        // Act & Assert
        var exception = Record.Exception(() => factura.Validar());
        Assert.Null(exception);
    }

    [Fact]
    public void Validar_ClienteIdMenorOIgualACero_LanzaExcepcion()
    {
        // Arrange
        var factura = new Factura
        {
            ClienteId = 0,
            FechaEmision = DateTime.Now.AddDays(-1),
            MontoTotal = 100.50m
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => factura.Validar());
        Assert.Equal("El ClienteId debe ser mayor a cero", exception.Message);
    }

    [Fact]
    public void Validar_MontoTotalMenorOIgualACero_LanzaExcepcion()
    {
        // Arrange
        var factura = new Factura
        {
            ClienteId = 1,
            FechaEmision = DateTime.Now.AddDays(-1),
            MontoTotal = 0
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => factura.Validar());
        Assert.Equal("El MontoTotal debe ser mayor a cero", exception.Message);
    }

    [Fact]
    public void Validar_FechaEmisionFutura_LanzaExcepcion()
    {
        // Arrange
        var factura = new Factura
        {
            ClienteId = 1,
            FechaEmision = DateTime.Now.AddDays(1),
            MontoTotal = 100.50m
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => factura.Validar());
        Assert.Equal("La FechaEmision no puede ser futura", exception.Message);
    }

    [Fact]
    public async Task CrearAsync_ClienteNoExiste_LanzaExcepcion()
    {
        // Arrange
        var mockFacturaRepository = new Mock<IFacturaRepository>();
        var mockClienteGateway = new Mock<IClienteGateway>();
        
        mockClienteGateway.Setup(x => x.ExisteClienteAsync(It.IsAny<int>()))
                         .ReturnsAsync(false);

        var facturaService = new FacturaService(mockFacturaRepository.Object, mockClienteGateway.Object);
        
        var request = new CreateFacturaRequest
        {
            ClienteId = 999,
            FechaEmision = DateTime.Now.AddDays(-1),
            MontoTotal = 100.50m
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ArgumentException>(() => facturaService.CrearAsync(request));
        Assert.Equal("El cliente con ID 999 no existe", exception.Message);
    }

    [Fact]
    public async Task CrearAsync_ClienteExiste_CreaFacturaCorrectamente()
    {
        // Arrange
        var mockFacturaRepository = new Mock<IFacturaRepository>();
        var mockClienteGateway = new Mock<IClienteGateway>();
        
        mockClienteGateway.Setup(x => x.ExisteClienteAsync(It.IsAny<int>()))
                         .ReturnsAsync(true);
        
        var facturaCreada = new Factura
        {
            Id = 1,
            ClienteId = 1,
            FechaEmision = DateTime.Now.AddDays(-1),
            MontoTotal = 100.50m
        };
        
        mockFacturaRepository.Setup(x => x.CreateAsync(It.IsAny<Factura>()))
                            .ReturnsAsync(facturaCreada);

        var facturaService = new FacturaService(mockFacturaRepository.Object, mockClienteGateway.Object);
        
        var request = new CreateFacturaRequest
        {
            ClienteId = 1,
            FechaEmision = DateTime.Now.AddDays(-1),
            MontoTotal = 100.50m
        };

        // Act
        var result = await facturaService.CrearAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal(1, result.ClienteId);
        Assert.Equal(100.50m, result.MontoTotal);
    }
}