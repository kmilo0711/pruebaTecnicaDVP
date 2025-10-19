using Facturas.Domain;

namespace Facturas.Application;

public interface IFacturaRepository
{
    Task<Factura> CreateAsync(Factura factura);
    Task<Factura?> GetByIdAsync(int id);
    Task<List<Factura>> GetByDateRangeAsync(DateTime inicio, DateTime fin);
}
