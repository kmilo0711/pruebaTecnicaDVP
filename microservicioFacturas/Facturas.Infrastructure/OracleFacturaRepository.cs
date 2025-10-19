using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using Facturas.Domain;
using Facturas.Application;

namespace Facturas.Infrastructure;

public class OracleFacturaRepository : IFacturaRepository
{
    private readonly string _connectionString;

    public OracleFacturaRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<Factura> CreateAsync(Factura factura)
    {
        const string sql = @"
            INSERT INTO FACTURAS (CLIENTE_ID, FECHA_EMISION, MONTO_TOTAL) 
            VALUES (:clienteId, :fechaEmision, :montoTotal)
            RETURNING ID INTO :id";

        using var connection = new OracleConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new OracleCommand(sql, connection);
        
        command.Parameters.Add(":clienteId", OracleDbType.Int32).Value = factura.ClienteId;
        command.Parameters.Add(":fechaEmision", OracleDbType.Date).Value = factura.FechaEmision;
        command.Parameters.Add(":montoTotal", OracleDbType.Decimal).Value = factura.MontoTotal;
        
        var idParameter = command.Parameters.Add(":id", OracleDbType.Int32);
        idParameter.Direction = System.Data.ParameterDirection.Output;

        await command.ExecuteNonQueryAsync();

        factura.Id = ((OracleDecimal)idParameter.Value).ToInt32();
        return factura;
    }

    public async Task<Factura?> GetByIdAsync(int id)
    {
        const string sql = @"
            SELECT ID, CLIENTE_ID, FECHA_EMISION, MONTO_TOTAL 
            FROM FACTURAS 
            WHERE ID = :id";

        using var connection = new OracleConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new OracleCommand(sql, connection);
        command.Parameters.Add(":id", OracleDbType.Int32).Value = id;

        using var reader = await command.ExecuteReaderAsync();
        
        if (!await reader.ReadAsync())
            return null;

        return new Factura
        {
            Id = reader.GetInt32(0),
            ClienteId = reader.GetInt32(1),
            FechaEmision = reader.GetDateTime(2),
            MontoTotal = reader.GetDecimal(3)
        };
    }

    public async Task<List<Factura>> GetByDateRangeAsync(DateTime inicio, DateTime fin)
    {
        const string sql = @"
            SELECT ID, CLIENTE_ID, FECHA_EMISION, MONTO_TOTAL 
            FROM FACTURAS 
            WHERE FECHA_EMISION >= :inicio AND FECHA_EMISION <= :fin
            ORDER BY FECHA_EMISION DESC";

        using var connection = new OracleConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new OracleCommand(sql, connection);
        command.Parameters.Add(":inicio", OracleDbType.Date).Value = inicio;
        command.Parameters.Add(":fin", OracleDbType.Date).Value = fin;

        using var reader = await command.ExecuteReaderAsync();

        var facturas = new List<Factura>();

        while (await reader.ReadAsync())
        {
            facturas.Add(new Factura
            {
                Id = reader.GetInt32(0),
                ClienteId = reader.GetInt32(1),
                FechaEmision = reader.GetDateTime(2),
                MontoTotal = reader.GetDecimal(3)
            });
        }

        return facturas;
    }
}
