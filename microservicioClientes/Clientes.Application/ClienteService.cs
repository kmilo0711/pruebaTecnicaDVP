using Clientes.Domain;
using Clientes.Application.DTOs;

namespace Clientes.Application;

public class ClienteService
{
    private readonly IClienteRepository _clienteRepository;

    public ClienteService(IClienteRepository clienteRepository)
    {
        _clienteRepository = clienteRepository;
    }

    public async Task<ClienteResponse> CrearAsync(CreateClienteRequest request)
    {
        var cliente = new Cliente
        {
            Nombre = request.Nombre,
            Identificacion = request.Identificacion,
            Correo = request.Correo,
            Direccion = request.Direccion
        };

        cliente.Validar();

        var clienteCreado = await _clienteRepository.CreateAsync(cliente);

        return new ClienteResponse
        {
            Id = clienteCreado.Id,
            Nombre = clienteCreado.Nombre,
            Identificacion = clienteCreado.Identificacion,
            Correo = clienteCreado.Correo,
            Direccion = clienteCreado.Direccion
        };
    }

    public async Task<ClienteResponse?> ObtenerPorIdAsync(int id)
    {
        var cliente = await _clienteRepository.GetByIdAsync(id);
        
        if (cliente == null)
            return null;

        return new ClienteResponse
        {
            Id = cliente.Id,
            Nombre = cliente.Nombre,
            Identificacion = cliente.Identificacion,
            Correo = cliente.Correo,
            Direccion = cliente.Direccion
        };
    }

    public async Task<List<ClienteResponse>> ObtenerTodosAsync()
    {
        var clientes = await _clienteRepository.GetAllAsync();

        return clientes.Select(c => new ClienteResponse
        {
            Id = c.Id,
            Nombre = c.Nombre,
            Identificacion = c.Identificacion,
            Correo = c.Correo,
            Direccion = c.Direccion
        }).ToList();
    }
}
