using CajeroAutomaticoAPI.DAOs;
using CajeroAutomaticoAPI.Models;

namespace CajeroAutomaticoAPI.Services;

public class TarjetaService
{
    private readonly TarjetaDAO _dao;
    public TarjetaService(TarjetaDAO dao) => _dao = dao;

    public Task<CrearTarjetaResponse> CrearAsync(CrearTarjetaRequest req)          => _dao.CrearAsync(req);
    public Task<List<Tarjeta>> ObtenerAsync(int? id = null, int? idCuenta = null)  => _dao.ObtenerAsync(id, idCuenta);
    public Task<string> ActualizarAsync(int id, ActualizarTarjetaRequest req)      => _dao.ActualizarAsync(id, req);
    public Task<string> DesbloquearAsync(int id)                                   => _dao.DesbloquearAsync(id);
    public Task<string> EliminarAsync(int id)                                      => _dao.EliminarAsync(id);
}
