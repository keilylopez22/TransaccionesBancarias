using CajeroAutomaticoAPI.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CajeroAutomaticoAPI.DAOs;

public class CuentahabienteDAO
{
    private readonly string _connectionString;

    public CuentahabienteDAO(IConfiguration config)
        => _connectionString = config.GetConnectionString("DefaultConnection")!;

    private static Cuentahabiente MapCuentahabiente(SqlDataReader r) => new()
    {
        Id              = r.GetInt32(r.GetOrdinal("Id")),
        CUI             = r.GetString(r.GetOrdinal("CUI")),
        PrimerNombre    = r.GetString(r.GetOrdinal("PrimerNombre")),
        SegundoNombre   = r.IsDBNull(r.GetOrdinal("SegundoNombre"))   ? null : r.GetString(r.GetOrdinal("SegundoNombre")),
        PrimerApellido  = r.GetString(r.GetOrdinal("PrimerApellido")),
        SegundoApellido = r.IsDBNull(r.GetOrdinal("SegundoApellido")) ? null : r.GetString(r.GetOrdinal("SegundoApellido")),
        FechaNacimiento = r.GetDateTime(r.GetOrdinal("FechaNacimiento")),
        FechaRegistro   = r.GetDateTime(r.GetOrdinal("FechaRegistro"))
    };

    public async Task CrearAsync(CrearCuentahabienteRequest req)
    {
        using var conn = new SqlConnection(_connectionString);
        using var cmd  = new SqlCommand("SP_CrearCuentahabiente", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@CUI",             req.CUI);
        cmd.Parameters.AddWithValue("@PrimerNombre",    req.PrimerNombre);
        cmd.Parameters.AddWithValue("@SegundoNombre",   (object?)req.SegundoNombre   ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@PrimerApellido",  req.PrimerApellido);
        cmd.Parameters.AddWithValue("@SegundoApellido", (object?)req.SegundoApellido ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@FechaNacimiento", req.FechaNacimiento);
        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<List<Cuentahabiente>> ObtenerTodosAsync()
    {
        var lista = new List<Cuentahabiente>();
        using var conn = new SqlConnection(_connectionString);
        using var cmd  = new SqlCommand("SP_ObtenerCuentahabientes", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@Id", DBNull.Value);
        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync()) lista.Add(MapCuentahabiente(reader));
        return lista;
    }

    public async Task<Cuentahabiente?> ObtenerPorIdAsync(int id)
    {
        using var conn = new SqlConnection(_connectionString);
        using var cmd  = new SqlCommand("SP_ObtenerCuentahabientes", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@Id", id);
        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        return await reader.ReadAsync() ? MapCuentahabiente(reader) : null;
    }

    public async Task<string> ActualizarAsync(int id, ActualizarCuentahabienteRequest req)
    {
        using var conn = new SqlConnection(_connectionString);
        using var cmd  = new SqlCommand("SP_ActualizarCuentahabiente", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@Id",              id);
        cmd.Parameters.AddWithValue("@CUI",             req.CUI);
        cmd.Parameters.AddWithValue("@PrimerNombre",    req.PrimerNombre);
        cmd.Parameters.AddWithValue("@SegundoNombre",   (object?)req.SegundoNombre   ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@PrimerApellido",  req.PrimerApellido);
        cmd.Parameters.AddWithValue("@SegundoApellido", (object?)req.SegundoApellido ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@FechaNacimiento", req.FechaNacimiento);
        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();
        return reader.GetString(0);
    }

    public async Task<string> EliminarAsync(int id)
    {
        using var conn = new SqlConnection(_connectionString);
        using var cmd  = new SqlCommand("SP_EliminarCuentahabiente", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@Id", id);
        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();
        return reader.GetString(0);
    }
}
