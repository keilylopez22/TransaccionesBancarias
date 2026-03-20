using CajeroAutomaticoAPI.Models;
using CajeroAutomaticoAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CajeroAutomaticoAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransaccionController : ControllerBase
{
    private readonly TransaccionService _service;
    private readonly TransaccionMasivaService _masivaService;

    public TransaccionController(TransaccionService service, TransaccionMasivaService masivaService)
    {
        _service       = service;
        _masivaService = masivaService;
    }

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

    [HttpPost("masiva")]
    [RequestSizeLimit(10 * 1024 * 1024)] // 10 MB máximo
    public async Task<IActionResult> Masiva(IFormFile archivo)
    {
        if (archivo == null || archivo.Length == 0)
            return BadRequest(new { Error = "Debe adjuntar un archivo .txt con las transacciones." });

        if (!archivo.FileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new { Error = "Solo se aceptan archivos .txt" });

        using var stream = archivo.OpenReadStream();
        var resultado = await _masivaService.ProcesarAsync(stream);
        return Ok(resultado);
    }
}
