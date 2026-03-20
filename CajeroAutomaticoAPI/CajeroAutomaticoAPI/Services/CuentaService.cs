using CajeroAutomaticoAPI.DAOs;
using CajeroAutomaticoAPI.Models;

namespace CajeroAutomaticoAPI.Services;

public class CuentaService
{
    private readonly CuentaDAO _dao;
    public CuentaService(CuentaDAO dao) => _dao = dao;

    public Task<CrearCuentaResponse> CrearAsync(CrearCuentaRequest req)                  => _dao.CrearAsync(req);
    public Task<List<Cuenta>> ObtenerAsync(int? id = null, int? idCuentahabiente = null, string? numeroCuenta = null)
                                                                                          => _dao.ObtenerAsync(id, idCuentahabiente, numeroCuenta);
    public Task<string> ActualizarAsync(int id, ActualizarCuentaRequest req)             => _dao.ActualizarAsync(id, req);
    public Task<string> EliminarAsync(int id)                                            => _dao.EliminarAsync(id);
}
