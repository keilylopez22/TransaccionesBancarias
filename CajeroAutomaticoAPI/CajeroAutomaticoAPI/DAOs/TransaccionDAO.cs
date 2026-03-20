using CajeroAutomaticoAPI.Models;
using Microsoft.Data.SqlClient;

namespace CajeroAutomaticoAPI.DAOs;

public class TransaccionDAO
{
    private readonly string _connectionString;

    public TransaccionDAO(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")!;
    }

    public async Task<TransaccionResponse> DepositoAsync(DepositoRequest req)
    {
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("SP_Deposito", conn) { CommandType = System.Data.CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@NumeroCuenta", req.NumeroCuenta);
        cmd.Parameters.AddWithValue("@Monto", req.Monto);
        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();
        return new TransaccionResponse
        {
            Mensaje = "Depósito realizado con éxito",
            Cuenta = reader.GetString(reader.GetOrdinal("Cuenta")),
            MontoAplicado = reader.GetDecimal(reader.GetOrdinal("MontoDepositado")),
            NuevoSaldo = reader.GetDecimal(reader.GetOrdinal("NuevoSaldo"))
        };
    }

    public async Task<TransaccionResponse> NotaCreditoAsync(NotaCreditoRequest req)
    {
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("SP_NotaCredito", conn) { CommandType = System.Data.CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@NumeroCuenta", req.NumeroCuenta);
        cmd.Parameters.AddWithValue("@Monto", req.Monto);
        cmd.Parameters.AddWithValue("@Detalle", req.Detalle);
        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();
        return new TransaccionResponse
        {
            Mensaje = reader.GetString(reader.GetOrdinal("Mensaje")),
            MontoAplicado = reader.GetDecimal(reader.GetOrdinal("MontoAplicado")),
            NuevoSaldo = reader.GetDecimal(reader.GetOrdinal("NuevoSaldo"))
        };
    }

    public async Task<TransaccionResponse> NotaDebitoAsync(NotaDebitoRequest req)
    {
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("SP_NotaDebito", conn) { CommandType = System.Data.CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@PIN", req.PIN);
        cmd.Parameters.AddWithValue("@NumeroTarjeta", req.NumeroTarjeta);
        cmd.Parameters.AddWithValue("@MontoIngresado", req.Monto);
        await conn.OpenAsync();
        using var reader = await cmd.ExecuteReaderAsync();
        await reader.ReadAsync();
        return new TransaccionResponse
        {
            Mensaje = reader.GetString(reader.GetOrdinal("Mensaje")),
            NuevoSaldo = reader.GetDecimal(reader.GetOrdinal("SaldoRestante"))
        };
    }

    public async Task<TransaccionResponse> CajeroMaestroAsync(TransaccionRequest req)
    {
        using var conn = new SqlConnection(_connectionString);
        using var cmd = new SqlCommand("SP_CajeroMaestro", conn) { CommandType = System.Data.CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@IdTipoTransaccion", req.IdTipoTransaccion);
        cmd.Parameters.AddWithValue("@NumeroCuenta", (object?)req.NumeroCuenta ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@NumeroTarjeta", (object?)req.NumeroTarjeta ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@PIN", (object?)req.PIN ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Monto", req.Monto);
        cmd.Parameters.AddWithValue("@NumeroCheque", (object?)req.NumeroCheque ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@FechaCheque", (object?)req.FechaCheque ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@DetallePersonalizado", (object?)req.DetallePersonalizado ?? DBNull.Value);
        await conn.OpenAsync();
        await cmd.ExecuteNonQueryAsync();
        return new TransaccionResponse { Mensaje = "Transacción ejecutada con éxito" };
    }
}
