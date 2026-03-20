namespace CajeroAutomaticoAPI.Models;

public class Cuenta
{
    public int Id { get; set; }
    public string NumeroCuenta { get; set; } = null!;
    public decimal Saldo { get; set; }
    public DateTime FechaApertura { get; set; }
    public bool Estado { get; set; }
    public int IdCuentahabiente { get; set; }
    public int IdTipoCuenta { get; set; }
    public string? TipoCuenta { get; set; }
    public string? Titular { get; set; }
}

public class CrearCuentaRequest
{
    public int IdCuentahabiente { get; set; }
    public int IdTipoCuenta { get; set; }
    public decimal SaldoInicial { get; set; } = 0;
}

public class CrearCuentaResponse
{
    public int IdCuenta { get; set; }
    public string NumeroCuenta { get; set; } = null!;
}

public class ActualizarCuentaRequest
{
    public int IdTipoCuenta { get; set; }
    public bool Estado { get; set; }
}
