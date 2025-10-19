using Clientes.Domain;

namespace Clientes.Application;

public interface IClienteRepository
{
    Task<Cliente> CreateAsync(Cliente cliente);
    Task<Cliente?> GetByIdAsync(int id);
    Task<List<Cliente>> GetAllAsync();
}
