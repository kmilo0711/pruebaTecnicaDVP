namespace Clientes.Domain;

public class Cliente
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Identificacion { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;

    public void Validar()
    {
        if (string.IsNullOrWhiteSpace(Nombre))
            throw new ArgumentException("El nombre es requerido");

        if (string.IsNullOrWhiteSpace(Identificacion))
            throw new ArgumentException("La identificación es requerida");

        if (string.IsNullOrWhiteSpace(Correo))
            throw new ArgumentException("El correo es requerido");

        if (string.IsNullOrWhiteSpace(Direccion))
            throw new ArgumentException("La dirección es requerida");
    }
}
