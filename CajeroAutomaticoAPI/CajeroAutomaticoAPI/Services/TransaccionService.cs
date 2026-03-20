using CajeroAutomaticoAPI.DAOs;
using CajeroAutomaticoAPI.Models;

namespace CajeroAutomaticoAPI.Services;

public class TransaccionService
{
    private readonly TransaccionDAO _dao;

    public TransaccionService(TransaccionDAO dao) => _dao = dao;

    public Task<TransaccionResponse> DepositoAsync(DepositoRequest req) => _dao.DepositoAsync(req);
    public Task<TransaccionResponse> NotaCreditoAsync(NotaCreditoRequest req) => _dao.NotaCreditoAsync(req);
    public Task<TransaccionResponse> NotaDebitoAsync(NotaDebitoRequest req) => _dao.NotaDebitoAsync(req);
    public Task<TransaccionResponse> CajeroMaestroAsync(TransaccionRequest req) => _dao.CajeroMaestroAsync(req);
}
