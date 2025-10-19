namespace Facturas.Domain;

public class Factura
{
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public DateTime FechaEmision { get; set; }
    public decimal MontoTotal { get; set; }

    public void Validar()
    {
        if (ClienteId <= 0)
            throw new ArgumentException("El ClienteId debe ser mayor a cero");

        if (MontoTotal <= 0)
            throw new ArgumentException("El MontoTotal debe ser mayor a cero");

        if (FechaEmision > DateTime.Now)
            throw new ArgumentException("La FechaEmision no puede ser futura");
    }
}
