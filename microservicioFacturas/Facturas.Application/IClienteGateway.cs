namespace Facturas.Application;

public interface IClienteGateway
{
    Task<bool> ExisteClienteAsync(int clienteId);
}
