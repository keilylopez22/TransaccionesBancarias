using CajeroAutomaticoAPI.DAOs;
using CajeroAutomaticoAPI.Models;

namespace CajeroAutomaticoAPI.Services;

public class CuentahabienteService
{
    private readonly CuentahabienteDAO _dao;
    public CuentahabienteService(CuentahabienteDAO dao) => _dao = dao;

    public Task CrearAsync(CrearCuentahabienteRequest req)                          => _dao.CrearAsync(req);
    public Task<List<Cuentahabiente>> ObtenerTodosAsync()                           => _dao.ObtenerTodosAsync();
    public Task<Cuentahabiente?> ObtenerPorIdAsync(int id)                          => _dao.ObtenerPorIdAsync(id);
    public Task<string> ActualizarAsync(int id, ActualizarCuentahabienteRequest req) => _dao.ActualizarAsync(id, req);
    public Task<string> EliminarAsync(int id)                                        => _dao.EliminarAsync(id);
}
