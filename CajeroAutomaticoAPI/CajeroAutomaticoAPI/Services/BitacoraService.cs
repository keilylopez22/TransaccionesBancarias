using CajeroAutomaticoAPI.DAOs;
using CajeroAutomaticoAPI.Models;

namespace CajeroAutomaticoAPI.Services;

public class BitacoraService
{
    private readonly BitacoraDAO _dao;
    public BitacoraService(BitacoraDAO dao) => _dao = dao;

    public Task<List<BitacoraTransaccion>> ObtenerAsync(BitacoraFiltroRequest filtro)
        => _dao.ObtenerAsync(filtro);

    public Task<List<BitacoraTransaccion>> ObtenerPorNumeroCuentaAsync(string numeroCuenta, DateTime? desde, DateTime? hasta)
        => _dao.ObtenerPorNumeroCuentaAsync(numeroCuenta, desde, hasta);
}
