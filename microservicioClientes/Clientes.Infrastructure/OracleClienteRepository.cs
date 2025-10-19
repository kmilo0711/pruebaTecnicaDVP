using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using Clientes.Domain;
using Clientes.Application;

namespace Clientes.Infrastructure;

public class OracleClienteRepository : IClienteRepository
{
    private readonly string _connectionString;

    public OracleClienteRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<Cliente> CreateAsync(Cliente cliente)
    {
        const string sql = @"
            INSERT INTO CLIENTES (NOMBRE, IDENTIFICACION, CORREO, DIRECCION) 
            VALUES (:nombre, :identificacion, :correo, :direccion)
            RETURNING ID INTO :id";

        using var connection = new OracleConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new OracleCommand(sql, connection);
        
        command.Parameters.Add(":nombre", OracleDbType.Varchar2).Value = cliente.Nombre;
        command.Parameters.Add(":identificacion", OracleDbType.Varchar2).Value = cliente.Identificacion;
        command.Parameters.Add(":correo", OracleDbType.Varchar2).Value = cliente.Correo;
        command.Parameters.Add(":direccion", OracleDbType.Varchar2).Value = cliente.Direccion;
        
        var idParameter = command.Parameters.Add(":id", OracleDbType.Int32);
        idParameter.Direction = System.Data.ParameterDirection.Output;

        await command.ExecuteNonQueryAsync();

        cliente.Id = ((OracleDecimal)idParameter.Value).ToInt32();
        return cliente;
    }

    public async Task<Cliente?> GetByIdAsync(int id)
    {
        const string sql = @"
            SELECT ID, NOMBRE, IDENTIFICACION, CORREO, DIRECCION 
            FROM CLIENTES 
            WHERE ID = :id";

        using var connection = new OracleConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new OracleCommand(sql, connection);
        command.Parameters.Add(":id", OracleDbType.Int32).Value = id;

        using var reader = await command.ExecuteReaderAsync();
        
        if (!await reader.ReadAsync())
            return null;

        return new Cliente
        {
            Id = reader.GetInt32(0),
            Nombre = reader.GetString(1),
            Identificacion = reader.GetString(2),
            Correo = reader.GetString(3),
            Direccion = reader.GetString(4)
        };
    }

    public async Task<List<Cliente>> GetAllAsync()
    {
        const string sql = @"
            SELECT ID, NOMBRE, IDENTIFICACION, CORREO, DIRECCION 
            FROM CLIENTES 
            ORDER BY ID";

        using var connection = new OracleConnection(_connectionString);
        await connection.OpenAsync();

        using var command = new OracleCommand(sql, connection);
        using var reader = await command.ExecuteReaderAsync();

        var clientes = new List<Cliente>();

        while (await reader.ReadAsync())
        {
            clientes.Add(new Cliente
            {
                Id = reader.GetInt32(0),
                Nombre = reader.GetString(1),
                Identificacion = reader.GetString(2),
                Correo = reader.GetString(3),
                Direccion = reader.GetString(4)
            });
        }

        return clientes;
    }
}
