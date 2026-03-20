using CajeroAutomaticoAPI.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CajeroAutomaticoAPI.DAOs;

public class CuentaDAO
{
    private readonly string _connectionString;

    public CuentaDAO(IConfiguration config)
        => _connectionString = config.GetConnectionString("DefaultConnection")!;

    private static Cuenta MapCuenta(SqlDataReader r) => new()
    {
        Id               = r.GetInt32(r.GetOrdinal("Id")),
        NumeroCuenta     = r.GetString(r.GetOrdinal("NumeroCuenta")),
        Saldo            = r.GetDecimal(r.GetOrdinal("Saldo")),
        FechaApertura    = r.GetDateTime(r.GetOrdinal("FechaApertura")),
        Estado           = r.GetBoolean(r.GetOrdinal("Estado")),
        IdCuentahabiente = r.GetInt32(r.GetOrdinal("IdCuentahabiente")),
        IdTipoCuenta     = r.GetInt32(r.GetOrdinal("IdTipoCuenta")),
        TipoCuenta       = r.GetString(r.GetOrdinal("TipoCuenta")),
        Titular          = r.GetString(r.GetOrdinal("Titular"))
    };

    private SqlCommand BuildObtenerCmd(SqlConnection conn, int? id, int? idCuentahabiente, string? numeroCuenta)
    {
        var cmd = new SqlCommand("SP_ObtenerCuentas", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@Id",               (object?)id               ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@IdCuentahabiente", (object?)idCuentahabiente ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@NumeroCuenta",     (object?)numeroCuenta     ?? DBNull.Value);
        return cmd;
    }

    public async Task<CrearCuentaResponse> CrearAsync(CrearCuentaRequest req)
    {
        using var conn = new SqlConnection(_connectionString);
        using var cmd  = new SqlCommand("SP_CrearCuentaBancaria", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@IdCuentahabiente", req.IdCuentahabiente);
        cmd.Parameters.AddWithValue("@IdTipoCuenta",     req.IdTipoCuenta);
        cmd.Parameters.AddWithValue("@SaldoInicial",     req.SaldoInicial);
        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();
        return new CrearCuentaResponse
        {
            IdCuenta     = reader.GetInt32(reader.GetOrdinal("IdCuenta")),
            NumeroCuenta = reader.GetString(reader.GetOrdinal("NumeroCuenta"))
        };
    }

    public async Task<List<Cuenta>> ObtenerAsync(int? id = null, int? idCuentahabiente = null, string? numeroCuenta = null)
    {
        var lista = new List<Cuenta>();
        using var conn = new SqlConnection(_connectionString);
        using var cmd  = BuildObtenerCmd(conn, id, idCuentahabiente, numeroCuenta);
        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync()) lista.Add(MapCuenta(reader));
        return lista;
    }

    public async Task<string> ActualizarAsync(int id, ActualizarCuentaRequest req)
    {
        using var conn = new SqlConnection(_connectionString);
        using var cmd  = new SqlCommand("SP_ActualizarCuenta", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@Id",           id);
        cmd.Parameters.AddWithValue("@IdTipoCuenta", req.IdTipoCuenta);
        cmd.Parameters.AddWithValue("@Estado",       req.Estado);
        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();
        return reader.GetString(0);
    }

   public async Task<string> EliminarAsync(int id)
    {
        using var conn = new SqlConnection(_connectionString);
        using var cmd  = new SqlCommand("SP_EliminarCuenta", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@Id", id);
        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();
        return reader.GetString(0);
    }
}
