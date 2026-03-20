using CajeroBlazor.Models;
using System.Net.Http.Json;

namespace CajeroBlazor.Services;

public class ApiService
{
    private readonly HttpClient _http;

    public ApiService(HttpClient http) => _http = http;

    // Cuentahabiente
    public Task<List<Cuentahabiente>?> GetCuentahabientesAsync() =>
        _http.GetFromJsonAsync<List<Cuentahabiente>>("api/cuentahabiente");

    public Task<Cuentahabiente?> GetCuentahabienteAsync(int id) =>
        _http.GetFromJsonAsync<Cuentahabiente>($"api/cuentahabiente/{id}");

    public async Task<bool> CrearCuentahabienteAsync(CrearCuentahabienteRequest req)
    {
        var res = await _http.PostAsJsonAsync("api/cuentahabiente", req);
        return res.IsSuccessStatusCode;
    }

    // Cuenta
    public Task<List<Cuenta>?> GetCuentasPorCuentahabienteAsync(int id) =>
        _http.GetFromJsonAsync<List<Cuenta>>($"api/cuenta/cuentahabiente/{id}");

    public async Task<CrearCuentaResponse?> CrearCuentaAsync(CrearCuentaRequest req)
    {
        var res = await _http.PostAsJsonAsync("api/cuenta", req);
        return res.IsSuccessStatusCode ? await res.Content.ReadFromJsonAsync<CrearCuentaResponse>() : null;
    }

    // Tarjeta
    public Task<List<Tarjeta>?> GetTarjetasAsync(int? idCuenta = null) =>
        _http.GetFromJsonAsync<List<Tarjeta>>(idCuenta.HasValue ? $"api/tarjeta?idCuenta={idCuenta}" : "api/tarjeta");

    public async Task<CrearTarjetaResponse?> CrearTarjetaAsync(CrearTarjetaRequest req)
    {
        var res = await _http.PostAsJsonAsync("api/tarjeta", req);
        return res.IsSuccessStatusCode ? await res.Content.ReadFromJsonAsync<CrearTarjetaResponse>() : null;
    }

    public async Task<(string? mensaje, string? error)> DesbloquearTarjetaAsync(int id)
    {
        var res = await _http.PatchAsync($"api/tarjeta/{id}/desbloquear", null);
        if (res.IsSuccessStatusCode)
        {
            var ok = await res.Content.ReadFromJsonAsync<MensajeResponse>();
            return (ok?.Mensaje, null);
        }
        var err = await res.Content.ReadFromJsonAsync<ErrorResponse>();
        return (null, err?.Error ?? "Error desconocido");
    }

    // Bitacora
    public Task<List<BitacoraTransaccion>?> GetBitacoraAsync(string numeroCuenta, DateTime? desde = null, DateTime? hasta = null)
    {
        var url = $"api/bitacora/cuenta/{numeroCuenta}";
        var params_ = new List<string>();
        if (desde.HasValue) params_.Add($"fechaDesde={desde.Value:yyyy-MM-dd}");
        if (hasta.HasValue) params_.Add($"fechaHasta={hasta.Value:yyyy-MM-dd}");
        if (params_.Count > 0) url += "?" + string.Join("&", params_);
        return _http.GetFromJsonAsync<List<BitacoraTransaccion>>(url);
    }

    public Task<List<BitacoraTransaccion>?> GetBitacoraFiltradaAsync(int? idCuenta = null, int? idTipoTransaccion = null, bool? soloExitosos = null)
    {
        var params_ = new List<string>();
        if (idCuenta.HasValue) params_.Add($"idCuenta={idCuenta}");
        if (idTipoTransaccion.HasValue) params_.Add($"idTipoTransaccion={idTipoTransaccion}");
        if (soloExitosos.HasValue) params_.Add($"soloExitosos={soloExitosos}");
        var url = "api/bitacora" + (params_.Count > 0 ? "?" + string.Join("&", params_) : "");
        return _http.GetFromJsonAsync<List<BitacoraTransaccion>>(url);
    }

    // Transacciones
    public async Task<(TransaccionResponse? data, string? error)> DepositoAsync(DepositoRequest req)
    {
        var res = await _http.PostAsJsonAsync("api/transaccion/deposito", req);
        if (res.IsSuccessStatusCode) return (await res.Content.ReadFromJsonAsync<TransaccionResponse>(), null);
        var err = await res.Content.ReadFromJsonAsync<ErrorResponse>();
        return (null, err?.Error ?? "Error desconocido");
    }

    public async Task<(TransaccionResponse? data, string? error)> NotaCreditoAsync(NotaCreditoRequest req)
    {
        var res = await _http.PostAsJsonAsync("api/transaccion/nota-credito", req);
        if (res.IsSuccessStatusCode) return (await res.Content.ReadFromJsonAsync<TransaccionResponse>(), null);
        var err = await res.Content.ReadFromJsonAsync<ErrorResponse>();
        return (null, err?.Error ?? "Error desconocido");
    }

    public async Task<(TransaccionResponse? data, string? error)> NotaDebitoAsync(NotaDebitoRequest req)
    {
        var res = await _http.PostAsJsonAsync("api/transaccion/nota-debito", req);
        if (res.IsSuccessStatusCode) return (await res.Content.ReadFromJsonAsync<TransaccionResponse>(), null);
        var err = await res.Content.ReadFromJsonAsync<ErrorResponse>();
        return (null, err?.Error ?? "Error desconocido");
    }

    public async Task<(TransaccionResponse? data, string? error)> PagoChequeAsync(PagoChequeRequest req)
    {
        var payload = new
        {
            IdTipoTransaccion = 2,
            NumeroCuenta      = req.NumeroCuenta,
            Monto             = req.Monto,
            NumeroCheque      = req.NumeroCheque,
            FechaCheque       = req.FechaCheque
        };
        var res = await _http.PostAsJsonAsync("api/transaccion/cajero", payload);
        if (res.IsSuccessStatusCode) return (await res.Content.ReadFromJsonAsync<TransaccionResponse>(), null);
        var err = await res.Content.ReadFromJsonAsync<ErrorResponse>();
        return (null, err?.Error ?? "Error desconocido");
    }
}

public class ErrorResponse { public string Error { get; set; } = ""; }
public class MensajeResponse { public string Mensaje { get; set; } = ""; }
