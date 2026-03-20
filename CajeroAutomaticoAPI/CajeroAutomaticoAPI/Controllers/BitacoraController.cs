using CajeroAutomaticoAPI.Models;
using CajeroAutomaticoAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CajeroAutomaticoAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BitacoraController : ControllerBase
{
    private readonly BitacoraService _service;
    public BitacoraController(BitacoraService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? idCuenta,
        [FromQuery] int? idTarjeta,
        [FromQuery] int? idTipoTransaccion,
        [FromQuery] DateTime? fechaDesde,
        [FromQuery] DateTime? fechaHasta,
        [FromQuery] bool? soloExitosos)
    {
        var filtro = new BitacoraFiltroRequest
        {
            IdCuenta          = idCuenta,
            IdTarjeta         = idTarjeta,
            IdTipoTransaccion = idTipoTransaccion,
            FechaDesde        = fechaDesde,
            FechaHasta        = fechaHasta,
            SoloExitosos      = soloExitosos
        };
        return Ok(await _service.ObtenerAsync(filtro));
    }

    [HttpGet("cuenta/{numeroCuenta}")]
    public async Task<IActionResult> GetByNumeroCuenta(
        string numeroCuenta,
        [FromQuery] DateTime? fechaDesde,
        [FromQuery] DateTime? fechaHasta)
        => Ok(await _service.ObtenerPorNumeroCuentaAsync(numeroCuenta, fechaDesde, fechaHasta));
}
