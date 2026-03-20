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
    public async Task<CrearTarjetaResponse?> CrearTarjetaAsync(CrearTarjetaRequest req)
    {
        var res = await _http.PostAsJsonAsync("api/tarjeta", req);
        return res.IsSuccessStatusCode ? await res.Content.ReadFromJsonAsync<CrearTarjetaResponse>() : null;
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
}

public class ErrorResponse { public string Error { get; set; } = ""; }
