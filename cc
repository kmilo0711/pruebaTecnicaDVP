Implementa el código completo del microservicio de Facturas en la estructura ya creada en facturas-service/.

ESTRUCTURA EXISTENTE:
- Facturas.Domain/
- Facturas.Application/
- Facturas.Infrastructure/
- Facturas.Api/
- Facturas.Tests/

IMPLEMENTAR:

1. *Facturas.Domain/Factura.cs*
   - Propiedades: Id (int), ClienteId (int), FechaEmision (DateTime), MontoTotal (decimal)
   - Método Validar() que valide: ClienteId > 0, MontoTotal > 0, FechaEmision no futura

2. *Facturas.Application/IFacturaRepository.cs*
   - Interface con: Task<Factura> CreateAsync(Factura), Task<Factura> GetByIdAsync(int), Task<List<Factura>> GetByDateRangeAsync(DateTime inicio, DateTime fin)

3. *Facturas.Application/IClienteGateway.cs*
   - Interface para comunicación HTTP con servicio de Clientes
   - Task<bool> ExisteClienteAsync(int clienteId)

4. *Facturas.Application/DTOs/*
   - CreateFacturaRequest.cs (ClienteId, FechaEmision, MontoTotal)
   - FacturaResponse.cs (Id, ClienteId, FechaEmision, MontoTotal)

5. *Facturas.Application/FacturaService.cs*
   - Usa IFacturaRepository, IClienteGateway
   - CrearAsync: valida que cliente exista llamando a IClienteGateway antes de crear
   - ObtenerPorIdAsync, ObtenerPorRangoFechasAsync

6. *Facturas.Infrastructure/OracleFacturaRepository.cs*
   - Implementa IFacturaRepository
   - Usa Oracle.ManagedDataAccess.Core
   - Queries SQL para tabla FACTURAS (ID, CLIENTE_ID, FECHA_EMISION, MONTO_TOTAL)
   - INSERT con RETURNING para obtener ID generado

7. *Facturas.Infrastructure/HttpClienteGateway.cs*
   - Implementa IClienteGateway
   - HttpClient a http://localhost:5100/api/clientes/{id}
   - ExisteClienteAsync: retorna true si GET devuelve 200, false si 404

8. *Facturas.Infrastructure/HttpAuditoriaClient.cs*
   - HttpClient para enviar eventos a http://localhost:5003/auditoria
   - Método RegistrarEventoAsync(servicio, entidad, entidadId, accion, detalles)

9. *Facturas.Api/Controllers/FacturasController.cs*
   - [ApiController] [Route("api/[controller]")]
   - POST /api/facturas → Crear factura (valida cliente) y registrar en auditoría
   - GET /api/facturas/{id} → Obtener factura por ID
   - GET /api/facturas?fechaInicio&fechaFin → Listar facturas por rango de fechas

10. *Facturas.Api/Program.cs*
    - Configurar inyección de dependencias
    - AddScoped para IFacturaRepository, FacturaService
    - AddHttpClient para HttpClienteGateway y HttpAuditoriaClient
    - Leer ConnectionStrings:OracleDb, ClientesServiceUrl, AuditoriaServiceUrl de appsettings.json
    - Configurar Swagger
    - Puerto 5002 en launchSettings.json

11. *Facturas.Api/appsettings.json*
    - ConnectionStrings:OracleDb = "User Id=system;Password=oracle123;Data Source=localhost:1521/XEPDB1;"
    - ClientesServiceUrl = "http://localhost:5100"
    - AuditoriaServiceUrl = "http://localhost:5003"

12. *Facturas.Tests/FacturaTests.cs*
    - Tests unitarios para método Validar() de la entidad Factura
    - Tests para FacturaService (usando mocks de IClienteGateway)
    - Test: factura válida no lanza excepción
    - Test: ClienteId <= 0 lanza excepción
    - Test: MontoTotal <= 0 lanza excepción
    - Test: FechaEmision futura lanza excepción
    - Test: CrearAsync lanza excepción si cliente no existe

Modifica TODOS los archivos necesarios en la estructura existente. Usa Oracle.ManagedDataAccess.Core y HttpClient.