namespace Facturas.Application.DTOs;

public class CreateFacturaRequest
{
    public int ClienteId { get; set; }
    public DateTime FechaEmision { get; set; }
    public decimal MontoTotal { get; set; }
}
