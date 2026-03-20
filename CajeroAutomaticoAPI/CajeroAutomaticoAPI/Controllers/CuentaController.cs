using CajeroAutomaticoAPI.Models;
using CajeroAutomaticoAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CajeroAutomaticoAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CuentaController : ControllerBase
{
    private readonly CuentaService _service;
    public CuentaController(CuentaService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? id,
        [FromQuery] int? idCuentahabiente,
        [FromQuery] string? numeroCuenta)
        => Ok(await _service.ObtenerAsync(id, idCuentahabiente, numeroCuenta));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var lista = await _service.ObtenerAsync(id: id);
        return lista.Count == 0 ? NotFound(new { Mensaje = "Cuenta no encontrada." }) : Ok(lista[0]);
    }

    [HttpGet("cuentahabiente/{idCuentahabiente}")]
    public async Task<IActionResult> GetByCuentahabiente(int idCuentahabiente)
        => Ok(await _service.ObtenerAsync(idCuentahabiente: idCuentahabiente));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CrearCuentaRequest req)
        => Ok(await _service.CrearAsync(req));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ActualizarCuentaRequest req)
        => Ok(new { Mensaje = await _service.ActualizarAsync(id, req) });

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
        => Ok(new { Mensaje = await _service.EliminarAsync(id) });
}
