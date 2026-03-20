using CajeroAutomaticoAPI.Models;
using CajeroAutomaticoAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CajeroAutomaticoAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CuentahabienteController : ControllerBase
{
    private readonly CuentahabienteService _service;
    public CuentahabienteController(CuentahabienteService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.ObtenerTodosAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.ObtenerPorIdAsync(id);
        return result is null ? NotFound(new { Mensaje = "Cuentahabiente no encontrado." }) : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CrearCuentahabienteRequest req)
    {
        await _service.CrearAsync(req);
        return StatusCode(201, new { Mensaje = "Cuentahabiente creado exitosamente." });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ActualizarCuentahabienteRequest req)
        => Ok(new { Mensaje = await _service.ActualizarAsync(id, req) });

    
}
