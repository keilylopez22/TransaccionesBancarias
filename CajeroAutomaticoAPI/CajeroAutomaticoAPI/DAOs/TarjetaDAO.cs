using CajeroAutomaticoAPI.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CajeroAutomaticoAPI.DAOs;

public class TarjetaDAO
{
    private readonly string _connectionString;

    public TarjetaDAO(IConfiguration config)
        => _connectionString = config.GetConnectionString("DefaultConnection")!;

    private static Tarjeta MapTarjeta(SqlDataReader r) => new()
    {
        Id               = r.GetInt32(r.GetOrdinal("Id")),
        NumeroTarjeta    = r.GetString(r.GetOrdinal("NumeroTarjeta")),
        NombreEnTarjeta  = r.GetString(r.GetOrdinal("NombreEnTarjeta")),
        FechaExpiracion  = r.GetDateTime(r.GetOrdinal("FechaExpiracion")),
        IntentosFallidos = r.GetInt32(r.GetOrdinal("IntentosFallidos")),
        FechaBloqueo     = r.IsDBNull(r.GetOrdinal("FechaBloqueo")) ? null : r.GetDateTime(r.GetOrdinal("FechaBloqueo")),
        Estado           = r.GetBoolean(r.GetOrdinal("Estado")),
        FechaEmision     = r.GetDateTime(r.GetOrdinal("FechaEmision")),
        IdTipoTarjeta    = r.GetInt32(r.GetOrdinal("IdTipoTarjeta")),
        TipoTarjeta      = r.GetString(r.GetOrdinal("TipoTarjeta")),
        IdCuenta         = r.GetInt32(r.GetOrdinal("IdCuenta")),
        NumeroCuenta     = r.GetString(r.GetOrdinal("NumeroCuenta"))
    };

    public async Task<CrearTarjetaResponse> CrearAsync(CrearTarjetaRequest req)
    {
        using var conn = new SqlConnection(_connectionString);
        using var cmd  = new SqlCommand("SP_CrearTarjeta", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@IdCuentaHabiente", req.IdCuentahabiente);
        cmd.Parameters.AddWithValue("@IdCuenta",         req.IdCuenta);
        cmd.Parameters.AddWithValue("@IdTipoTarjeta",    req.IdTipoTarjeta);
        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();
        return new CrearTarjetaResponse
        {
            NumeroTarjeta = reader.GetString(reader.GetOrdinal("NumeroTarjeta")),
            PIN_Generado  = reader.GetString(reader.GetOrdinal("PIN_Generado")),
            CVV           = reader.GetString(reader.GetOrdinal("CVV")),
            Vence         = reader.GetDateTime(reader.GetOrdinal("Vence"))
        };
    }

    public async Task<List<Tarjeta>> ObtenerAsync(int? id = null, int? idCuenta = null)
    {
        var lista = new List<Tarjeta>();
        using var conn = new SqlConnection(_connectionString);
        using var cmd  = new SqlCommand("SP_ObtenerTarjetas", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@Id",       (object?)id       ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@IdCuenta", (object?)idCuenta ?? DBNull.Value);
        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync()) lista.Add(MapTarjeta(reader));
        return lista;
    }

    public async Task<string> ActualizarAsync(int id, ActualizarTarjetaRequest req)
    {
        using var conn = new SqlConnection(_connectionString);
        using var cmd  = new SqlCommand("SP_ActualizarTarjeta", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@Id",               id);
        cmd.Parameters.AddWithValue("@Estado",           req.Estado);
        cmd.Parameters.AddWithValue("@IntentosFallidos", req.IntentosFallidos);
        cmd.Parameters.AddWithValue("@FechaBloqueo",     (object?)req.FechaBloqueo ?? DBNull.Value);
        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();
        return reader.GetString(0);
    }

    public async Task<string> DesbloquearAsync(int id)
    {
        using var conn = new SqlConnection(_connectionString);
        using var cmd  = new SqlCommand("SP_DesbloquearTarjeta", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@Id",            id);
        cmd.Parameters.AddWithValue("@NumeroTarjeta", DBNull.Value);
        cmd.Parameters.AddWithValue("@Usuario",       DBNull.Value);
        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();
        return reader.GetString(reader.GetOrdinal("Mensaje"));
    }

    public async Task<string> EliminarAsync(int id)
    {
        using var conn = new SqlConnection(_connectionString);
        using var cmd  = new SqlCommand("SP_EliminarTarjeta", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@Id", id);
        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();
        return reader.GetString(0);
    }
}
