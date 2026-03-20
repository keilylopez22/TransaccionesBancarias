using CajeroAutomaticoAPI.DAOs;
using CajeroAutomaticoAPI.Models;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace CajeroAutomaticoAPI.Services;

public class TransaccionMasivaService
{
    private readonly TransaccionDAO _dao;
    // Máximo de hilos concurrentes — ajustable según el pool de conexiones SQL
    private const int MaxConcurrencia = 10;

    public TransaccionMasivaService(TransaccionDAO dao) => _dao = dao;

    public async Task<TransaccionMasivaResponse> ProcesarAsync(Stream archivo)
    {
        var lineas = await ParsearArchivoAsync(archivo);
        var resultados = new ConcurrentBag<TransaccionMasivaResultado>();
        var semaforo = new SemaphoreSlim(MaxConcurrencia);
        var sw = Stopwatch.StartNew();

        var tareas = lineas.Select(async linea =>
        {
            await semaforo.WaitAsync();
            try
            {
                var resultado = await EjecutarLineaAsync(linea);
                resultados.Add(resultado);
            }
            finally
            {
                semaforo.Release();
            }
        });

        await Task.WhenAll(tareas);
        sw.Stop();

        var lista = resultados.OrderBy(r => r.Linea).ToList();

        return new TransaccionMasivaResponse
        {
            TotalLineas = lista.Count,
            Exitosas    = lista.Count(r => r.Exito),
            Fallidas    = lista.Count(r => !r.Exito),
            TiempoMs    = sw.ElapsedMilliseconds,
            Resultados  = lista
        };
    }

    private static async Task<List<TransaccionMasivaLinea>> ParsearArchivoAsync(Stream archivo)
    {
        var lineas = new List<TransaccionMasivaLinea>();
        using var reader = new StreamReader(archivo);
        int numeroLinea = 0;

        while (!reader.EndOfStream)
        {
            var linea = await reader.ReadLineAsync();
            numeroLinea++;

            // Ignorar líneas vacías y comentarios (#)
            if (string.IsNullOrWhiteSpace(linea) || linea.TrimStart().StartsWith('#'))
                continue;

            var partes = linea.Split('|');
            if (partes.Length < 3) continue;

            if (!int.TryParse(partes[0].Trim(), out int tipo)) continue;
            if (!decimal.TryParse(partes[2].Trim(), System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out decimal monto)) continue;

            var item = new TransaccionMasivaLinea
            {
                Linea              = numeroLinea,
                IdTipoTransaccion  = tipo,
                Monto              = monto
            };

            switch (tipo)
            {
                case 1: // Depósito: TIPO|CUENTA|MONTO
                    item.NumeroCuenta = partes[1].Trim();
                    break;

                case 2: // Pago Cheque: TIPO|CUENTA|MONTO|NUM_CHEQUE|FECHA_CHEQUE
                    item.NumeroCuenta  = partes[1].Trim();
                    item.NumeroCheque  = partes.Length > 3 ? partes[3].Trim() : null;
                    item.FechaCheque   = partes.Length > 4 && DateTime.TryParse(partes[4].Trim(), out var fc)
                                         ? fc : DateTime.Today;
                    break;

                case 3: // Nota Crédito: TIPO|CUENTA|MONTO|DETALLE
                    item.NumeroCuenta = partes[1].Trim();
                    item.Detalle      = partes.Length > 3 ? partes[3].Trim() : "Nota de crédito masiva";
                    break;

                case 4: // Nota Débito: TIPO|TARJETA|MONTO|PIN
                    item.NumeroTarjeta = partes[1].Trim();
                    item.PIN           = partes.Length > 3 ? partes[3].Trim() : null;
                    break;
            }

            lineas.Add(item);
        }

        return lineas;
    }

    private async Task<TransaccionMasivaResultado> EjecutarLineaAsync(TransaccionMasivaLinea linea)
    {
        var resultado = new TransaccionMasivaResultado
        {
            Linea             = linea.Linea,
            IdTipoTransaccion = linea.IdTipoTransaccion,
            NumeroCuenta      = linea.NumeroCuenta ?? linea.NumeroTarjeta,
            Monto             = linea.Monto
        };

        try
        {
            switch (linea.IdTipoTransaccion)
            {
                case 1:
                    var dep = await _dao.DepositoAsync(new DepositoRequest
                    {
                        NumeroCuenta = linea.NumeroCuenta!,
                        Monto        = linea.Monto
                    });
                    resultado.Exito     = true;
                    resultado.Mensaje   = dep.Mensaje;
                    resultado.NuevoSaldo = dep.NuevoSaldo;
                    break;

                case 2:
                    var cheque = await _dao.CajeroMaestroAsync(new TransaccionRequest
                    {
                        IdTipoTransaccion = 2,
                        NumeroCuenta      = linea.NumeroCuenta,
                        Monto             = linea.Monto,
                        NumeroCheque      = linea.NumeroCheque,
                        FechaCheque       = linea.FechaCheque
                    });
                    resultado.Exito   = true;
                    resultado.Mensaje = cheque.Mensaje;
                    break;

                case 3:
                    var credito = await _dao.NotaCreditoAsync(new NotaCreditoRequest
                    {
                        NumeroCuenta = linea.NumeroCuenta!,
                        Monto        = linea.Monto,
                        Detalle      = linea.Detalle ?? "Nota de crédito masiva"
                    });
                    resultado.Exito      = true;
                    resultado.Mensaje    = credito.Mensaje;
                    resultado.NuevoSaldo = credito.NuevoSaldo;
                    break;

                case 4:
                    var debito = await _dao.NotaDebitoAsync(new NotaDebitoRequest
                    {
                        NumeroTarjeta = linea.NumeroTarjeta!,
                        PIN           = linea.PIN!,
                        Monto         = linea.Monto
                    });
                    resultado.Exito      = true;
                    resultado.Mensaje    = debito.Mensaje;
                    resultado.NuevoSaldo = debito.NuevoSaldo;
                    break;

                default:
                    resultado.Exito   = false;
                    resultado.Mensaje = $"Tipo de transacción {linea.IdTipoTransaccion} no reconocido.";
                    break;
            }
        }
        catch (Exception ex)
        {
            resultado.Exito   = false;
            resultado.Mensaje = ex.Message;
        }

        return resultado;
    }
}
