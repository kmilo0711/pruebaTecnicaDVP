namespace Facturas.Application.DTOs;

public class FacturaResponse
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public DateTime FechaEmision { get; set; }
    public decimal MontoTotal { get; set; }
}
