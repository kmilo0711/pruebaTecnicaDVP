using Facturas.Domain;
using Facturas.Application.DTOs;

namespace Facturas.Application;

public class FacturaService
{
    private readonly IFacturaRepository _facturaRepository;
    private readonly IClienteGateway _clienteGateway;

    public FacturaService(IFacturaRepository facturaRepository, IClienteGateway clienteGateway)
    {
        _facturaRepository = facturaRepository;
        _clienteGateway = clienteGateway;
    }

    public async Task<FacturaResponse> CrearAsync(CreateFacturaRequest request)
    {
        var factura = new Factura
        {
            ClienteId = request.ClienteId,
            FechaEmision = request.FechaEmision,
            MontoTotal = request.MontoTotal
        };

        factura.Validar();

        // Validar que el cliente existe
        var clienteExiste = await _clienteGateway.ExisteClienteAsync(request.ClienteId);
        if (!clienteExiste)
            throw new ArgumentException($"El cliente con ID {request.ClienteId} no existe");

        var facturaCreada = await _facturaRepository.CreateAsync(factura);

        return new FacturaResponse
        {
            Id = facturaCreada.Id,
            ClienteId = facturaCreada.ClienteId,
            FechaEmision = facturaCreada.FechaEmision,
            MontoTotal = facturaCreada.MontoTotal
        };
    }

    public async Task<FacturaResponse?> ObtenerPorIdAsync(int id)
    {
        var factura = await _facturaRepository.GetByIdAsync(id);
        
        if (factura == null)
            return null;

        return new FacturaResponse
        {
            Id = factura.Id,
            ClienteId = factura.ClienteId,
            FechaEmision = factura.FechaEmision,
            MontoTotal = factura.MontoTotal
        };
    }

    public async Task<List<FacturaResponse>> ObtenerPorRangoFechasAsync(DateTime fechaInicio, DateTime fechaFin)
    {
        var facturas = await _facturaRepository.GetByDateRangeAsync(fechaInicio, fechaFin);

        return facturas.Select(f => new FacturaResponse
        {
            Id = f.Id,
            ClienteId = f.ClienteId,
            FechaEmision = f.FechaEmision,
            MontoTotal = f.MontoTotal
        }).ToList();
    }
}
