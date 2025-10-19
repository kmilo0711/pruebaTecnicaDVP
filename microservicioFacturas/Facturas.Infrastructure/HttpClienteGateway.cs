using Facturas.Application;

namespace Facturas.Infrastructure;

public class HttpClienteGateway : IClienteGateway
{
    private readonly HttpClient _httpClient;
    private readonly string _clientesServiceUrl;

    public HttpClienteGateway(HttpClient httpClient, string clientesServiceUrl)
    {
        _httpClient = httpClient;
        _clientesServiceUrl = clientesServiceUrl;
    }

    public async Task<bool> ExisteClienteAsync(int clienteId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_clientesServiceUrl}/api/clientes/{clienteId}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            // En caso de error de comunicación, asumimos que el cliente no existe
            return false;
        }
    }
}
