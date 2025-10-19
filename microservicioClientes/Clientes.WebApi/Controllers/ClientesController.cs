using Microsoft.AspNetCore.Mvc;
using Clientes.Application;
using Clientes.Application.DTOs;
using Clientes.Infrastructure;
using Oracle.ManagedDataAccess.Client;

namespace Clientes.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientesController : ControllerBase
{
    private readonly ClienteService _clienteService;
    private readonly HttpAuditoriaClient _auditoriaClient;

    public ClientesController(ClienteService clienteService, HttpAuditoriaClient auditoriaClient)
    {
        _clienteService = clienteService;
        _auditoriaClient = auditoriaClient;
    }

    [HttpPost]
    public async Task<ActionResult<ClienteResponse>> CrearCliente([FromBody] CreateClienteRequest request)
    {
        try
        {
            var cliente = await _clienteService.CrearAsync(request);
            await _auditoriaClient.RegistrarEventoAsync(
                "Clientes", 
                "Cliente", 
                cliente.Id, 
                "Crear", 
                $"Cliente creado: {cliente.Nombre} - {cliente.Identificacion}"
            );

            return CreatedAtAction(nameof(ObtenerClientePorId), new { id = cliente.Id }, cliente);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (OracleException ex) when (ex.Number == 1)
        {
            return Conflict(new { message = "Ya existe un cliente con la identificación proporcionada." });
        }
        catch (OracleException ex)
        {
            return StatusCode(500, new { message = "Error al acceder a la base de datos", details = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClienteResponse>> ObtenerClientePorId(int id)
    {
        try
        {
            var cliente = await _clienteService.ObtenerPorIdAsync(id);
            
            if (cliente == null)
                return NotFound(new { message = $"Cliente con ID {id} no encontrado" });

            return Ok(cliente);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<ClienteResponse>>> ObtenerTodosLosClientes()
    {
        try
        {
            var clientes = await _clienteService.ObtenerTodosAsync();
            return Ok(clientes);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
        }
    }
}
