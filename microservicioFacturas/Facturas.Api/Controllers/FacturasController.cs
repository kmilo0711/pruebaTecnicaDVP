using Microsoft.AspNetCore.Mvc;
using Facturas.Application;
using Facturas.Application.DTOs;
using Facturas.Infrastructure;

namespace Facturas.Api.Controllers;

/// <summary>
/// Controlador para la gestión de facturas
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class FacturasController : ControllerBase
{
    private readonly FacturaService _facturaService;
    private readonly HttpAuditoriaClient _auditoriaClient;

    public FacturasController(FacturaService facturaService, HttpAuditoriaClient auditoriaClient)
    {
        _facturaService = facturaService;
        _auditoriaClient = auditoriaClient;
    }

    /// <summary>
    /// Crear una nueva factura
    /// </summary>
    /// <param name="request">Datos de la factura a crear</param>
    /// <returns>Factura creada</returns>
    [HttpPost]
    public async Task<ActionResult<FacturaResponse>> CrearFactura([FromBody] CreateFacturaRequest request)
    {
        try
        {
            var factura = await _facturaService.CrearAsync(request);
            
            // Registrar evento de auditoría
            await _auditoriaClient.RegistrarEventoAsync(
                "Facturas", 
                "Factura", 
                factura.Id, 
                "Crear", 
                $"Factura creada para cliente {factura.ClienteId} por monto ${factura.MontoTotal}"
            );

            return CreatedAtAction(nameof(ObtenerFacturaPorId), new { id = factura.Id }, factura);
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
    /// Obtener una factura por su ID
    /// </summary>
    /// <param name="id">ID de la factura</param>
    /// <returns>Factura encontrada</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<FacturaResponse>> ObtenerFacturaPorId(int id)
    {
        try
        {
            var factura = await _facturaService.ObtenerPorIdAsync(id);
            
            if (factura == null)
                return NotFound(new { message = $"Factura con ID {id} no encontrada" });

            return Ok(factura);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
        }
    }

    /// <summary>
    /// Obtener facturas por rango de fechas
    /// </summary>
    /// <param name="fechaInicio">Fecha de inicio del rango</param>
    /// <param name="fechaFin">Fecha de fin del rango</param>
    /// <returns>Lista de facturas en el rango</returns>
    [HttpGet]
    public async Task<ActionResult<List<FacturaResponse>>> ObtenerFacturasPorRango(
        [FromQuery] DateTime fechaInicio, 
        [FromQuery] DateTime fechaFin)
    {
        try
        {
            if (fechaInicio > fechaFin)
                return BadRequest(new { message = "La fecha de inicio no puede ser mayor a la fecha de fin" });

            var facturas = await _facturaService.ObtenerPorRangoFechasAsync(fechaInicio, fechaFin);
            return Ok(facturas);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error interno del servidor", details = ex.Message });
        }
    }
}
