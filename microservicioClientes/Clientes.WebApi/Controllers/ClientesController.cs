using Microsoft.AspNetCore.Mvc;
using Clientes.Application;
using Clientes.Application.DTOs;
using Clientes.Infrastructure;

namespace Clientes.WebApi.Controllers;

/// <summary>
/// Controlador para la gestión de clientes
/// </summary>
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

    /// <summary>
    /// Crear un nuevo cliente
    /// </summary>
    /// <param name="request">Datos del cliente a crear</param>
    /// <returns>Cliente creado</returns>
    [HttpPost]
    public async Task<ActionResult<ClienteResponse>> CrearCliente([FromBody] CreateClienteRequest request)
    {
        try
        {
            var cliente = await _clienteService.CrearAsync(request);
            
            // Registrar evento de auditoría
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
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
        }
    }

    /// <summary>
    /// Obtener un cliente por su ID
    /// </summary>
    /// <param name="id">ID del cliente</param>
    /// <returns>Cliente encontrado</returns>
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

    /// <summary>
    /// Obtener todos los clientes
    /// </summary>
    /// <returns>Lista de clientes</returns>
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
