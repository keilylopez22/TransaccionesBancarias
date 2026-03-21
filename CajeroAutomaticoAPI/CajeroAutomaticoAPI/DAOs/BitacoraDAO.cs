using CajeroAutomaticoAPI.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace CajeroAutomaticoAPI.DAOs;

public class BitacoraDAO
{
    private readonly string _connectionString;

    public BitacoraDAO(IConfiguration config)
        => _connectionString = config.GetConnectionString("DefaultConnection")!;

    private static BitacoraTransaccion MapBitacora(SqlDataReader r) => new()
    {
        Id                 = r.GetInt64(r.GetOrdinal("Id")),
        IdCuenta           = r.GetInt32(r.GetOrdinal("IdCuenta")),
        NumeroCuenta       = r.GetString(r.GetOrdinal("NumeroCuenta")),
        IdTarjeta          = r.IsDBNull(r.GetOrdinal("IdTarjeta")) ? null : r.GetInt32(r.GetOrdinal("IdTarjeta")),
        IdTipoTransaccion  = r.GetInt32(r.GetOrdinal("IdTipoTransaccion")),
        TipoTransaccion    = r.GetString(r.GetOrdinal("TipoTransaccion")),
        Monto              = r.GetDecimal(r.GetOrdinal("Monto")),
        SaldoAnterior      = r.GetDecimal(r.GetOrdinal("SaldoAnterior")),
        SaldoNuevo         = r.GetDecimal(r.GetOrdinal("SaldoNuevo")),
        FechaMovimiento    = r.GetDateTime(r.GetOrdinal("FechaMovimiento")),
        FechaCheque        = r.IsDBNull(r.GetOrdinal("FechaCheque"))  ? null : r.GetDateTime(r.GetOrdinal("FechaCheque")),
        NumeroCheque       = r.IsDBNull(r.GetOrdinal("NumeroCheque")) ? null : r.GetString(r.GetOrdinal("NumeroCheque")),
        Usuario            = r.IsDBNull(r.GetOrdinal("Usuario"))      ? null : r.GetString(r.GetOrdinal("Usuario")),
        Detalle            = r.IsDBNull(r.GetOrdinal("Detalle"))      ? null : r.GetString(r.GetOrdinal("Detalle")),
        Exito              = r.GetBoolean(r.GetOrdinal("Exito"))
    };

    public async Task<List<BitacoraTransaccion>> ObtenerAsync(BitacoraFiltroRequest filtro)
    {
        var lista = new List<BitacoraTransaccion>();
        using var conn = new SqlConnection(_connectionString);
        using var cmd  = new SqlCommand("SP_ObtenerBitacora", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@IdCuenta",          (object?)filtro.IdCuenta          ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@IdTarjeta",         (object?)filtro.IdTarjeta         ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@IdTipoTransaccion", (object?)filtro.IdTipoTransaccion ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@FechaDesde",        (object?)filtro.FechaDesde        ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@FechaHasta",        (object?)filtro.FechaHasta        ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@SoloExitosos",      (object?)filtro.SoloExitosos      ?? DBNull.Value);
        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync()) lista.Add(MapBitacora(reader));
        return lista;
    }

    public async Task<List<BitacoraTransaccion>> ObtenerPorNumeroCuentaAsync(string numeroCuenta, DateTime? desde, DateTime? hasta)
    {
        var lista = new List<BitacoraTransaccion>();
        using var conn = new SqlConnection(_connectionString);
        using var cmd  = new SqlCommand("SP_ObtenerBitacoraPorNumeroCuenta", conn) { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@NumeroCuenta", numeroCuenta);
        cmd.Parameters.AddWithValue("@FechaDesde",   (object?)desde ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@FechaHasta",   (object?)hasta ?? DBNull.Value);
        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync()) lista.Add(MapBitacora(reader));
        return lista;
    }
}