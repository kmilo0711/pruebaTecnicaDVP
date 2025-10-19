using System.Text;
using System.Text.Json;

namespace Clientes.Infrastructure;

public class HttpAuditoriaClient
{
    private readonly HttpClient _httpClient;
    private readonly string _auditoriaServiceUrl;

    public HttpAuditoriaClient(HttpClient httpClient, string auditoriaServiceUrl)
    {
        _httpClient = httpClient;
        _auditoriaServiceUrl = auditoriaServiceUrl;
    }

    public async Task RegistrarEventoAsync(string servicio, string entidad, int entidadId, string accion, string detalles)
    {
        var evento = new
        {
            Servicio = servicio,
            Entidad = entidad,
            EntidadId = entidadId,
            Accion = accion,
            Detalles = detalles,
            Fecha = DateTime.UtcNow
        };

        var json = JsonSerializer.Serialize(evento);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync($"{_auditoriaServiceUrl}/auditoria", content);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            // Log the error but don't fail the main operation
            Console.WriteLine($"Error registrando evento de auditoría: {ex.Message}");
        }
    }
}
