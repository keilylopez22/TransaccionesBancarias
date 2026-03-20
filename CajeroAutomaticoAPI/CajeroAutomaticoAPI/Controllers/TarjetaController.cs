using CajeroAutomaticoAPI.Models;
using CajeroAutomaticoAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CajeroAutomaticoAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TarjetaController : ControllerBase
{
    private readonly TarjetaService _service;
    public TarjetaController(TarjetaService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? id, [FromQuery] int? idCuenta)
        => Ok(await _service.ObtenerAsync(id, idCuenta));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var lista = await _service.ObtenerAsync(id: id);
        return lista.Count == 0 ? NotFound(new { Mensaje = "Tarjeta no encontrada." }) : Ok(lista[0]);
    }

    [HttpGet("cuenta/{idCuenta}")]
    public async Task<IActionResult> GetByCuenta(int idCuenta)
        => Ok(await _service.ObtenerAsync(idCuenta: idCuenta));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CrearTarjetaRequest req)
        => Ok(await _service.CrearAsync(req));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ActualizarTarjetaRequest req)
        => Ok(new { Mensaje = await _service.ActualizarAsync(id, req) });

    [HttpPatch("{id}/desbloquear")]
    public async Task<IActionResult> Desbloquear(int id)
        => Ok(new { Mensaje = await _service.DesbloquearAsync(id) });

    
}
