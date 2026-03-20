using CajeroAutomaticoAPI.Models;
using CajeroAutomaticoAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CajeroAutomaticoAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransaccionController : ControllerBase
{
    private readonly TransaccionService _service;

    public TransaccionController(TransaccionService service) => _service = service;

    [HttpPost("deposito")]
    public async Task<IActionResult> Deposito([FromBody] DepositoRequest req)
        => Ok(await _service.DepositoAsync(req));

    [HttpPost("nota-credito")]
    public async Task<IActionResult> NotaCredito([FromBody] NotaCreditoRequest req)
        => Ok(await _service.NotaCreditoAsync(req));

    [HttpPost("nota-debito")]
    public async Task<IActionResult> NotaDebito([FromBody] NotaDebitoRequest req)
        => Ok(await _service.NotaDebitoAsync(req));

    [HttpPost("cajero")]
    public async Task<IActionResult> CajeroMaestro([FromBody] TransaccionRequest req)
        => Ok(await _service.CajeroMaestroAsync(req));
}
