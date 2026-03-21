namespace CajeroBlazor.Models;

public class Cuentahabiente
{
    public int Id { get; set; }
    public string CUI { get; set; } = "";
    public string PrimerNombre { get; set; } = "";
    public string? SegundoNombre { get; set; }
    public string PrimerApellido { get; set; } = "";
    public string? SegundoApellido { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public DateTime FechaRegistro { get; set; }
    public string NombreCompleto => $"{PrimerNombre} {SegundoNombre} {PrimerApellido} {SegundoApellido}".Replace("  ", " ").Trim();
}

public class CrearCuentahabienteRequest
{
    public string CUI { get; set; } = "";
    public string PrimerNombre { get; set; } = "";
    public string? SegundoNombre { get; set; }
    public string PrimerApellido { get; set; } = "";
    public string? SegundoApellido { get; set; }
    public DateTime FechaNacimiento { get; set; } = DateTime.Today.AddYears(-18);
}

public class Cuenta
{
    public int Id { get; set; }
    public string NumeroCuenta { get; set; } = "";
    public decimal Saldo { get; set; }
    public DateTime FechaApertura { get; set; }
    public bool Estado { get; set; }
    public int IdCuentahabiente { get; set; }
    public int IdTipoCuenta { get; set; }
}

public class CrearCuentaRequest
{
    public int IdCuentahabiente { get; set; }
    public int IdTipoCuenta { get; set; }
    public decimal SaldoInicial { get; set; }
}

public class CrearCuentaResponse
{
    public int IdCuenta { get; set; }
    public string NumeroCuenta { get; set; } = "";
}

public class CrearTarjetaRequest
{
    public int IdCuentahabiente { get; set; }
    public int IdCuenta { get; set; }
    public int IdTipoTarjeta { get; set; }
}

public class CrearTarjetaResponse
{
    public string NumeroTarjeta { get; set; } = "";
    public string PIN_Generado { get; set; } = "";
    public string CVV { get; set; } = "";
    public DateTime Vence { get; set; }
}

public class DepositoRequest
{
    public string NumeroCuenta { get; set; } = "";
    public decimal Monto { get; set; }
}

public class NotaCreditoRequest
{
    public string NumeroCuenta { get; set; } = "";
    public decimal Monto { get; set; }
    public string Detalle { get; set; } = "";
    public string NumeroDocumento { get; set; } = "";
}

public class NotaDebitoRequest
{
    public string NumeroTarjeta { get; set; } = "";
    public decimal Monto { get; set; }
    public string? Detalle { get; set; }
    public string? NumeroDocumento { get; set; } = "";
}

public class PagoChequeRequest
{
    public string NumeroCuenta { get; set; } = "";
    public decimal Monto { get; set; }
    public string NumeroCheque { get; set; } = "";
    public DateTime FechaCheque { get; set; } = DateTime.Today;
}

public class TransaccionResponse
{
    public string Mensaje { get; set; } = "";
    public string? Cuenta { get; set; }
    public decimal? MontoAplicado { get; set; }
    public decimal? NuevoSaldo { get; set; }
    public string NumeroDocumento { get; set; } = "";
}

public class Tarjeta
{
    public int Id { get; set; }
    public string NumeroTarjeta { get; set; } = "";
    public string NombreEnTarjeta { get; set; } = "";
    public DateTime FechaExpiracion { get; set; }
    public int IntentosFallidos { get; set; }
    public DateTime? FechaBloqueo { get; set; }
    public bool Estado { get; set; }
    public DateTime FechaEmision { get; set; }
    public int IdTipoTarjeta { get; set; }
    public string? TipoTarjeta { get; set; }
    public int IdCuenta { get; set; }
    public string? NumeroCuenta { get; set; }
}

public class BitacoraTransaccion
{
    public long Id { get; set; }
    public int IdCuenta { get; set; }
    public string? NumeroCuenta { get; set; }
    public int? IdTarjeta { get; set; }
    public int IdTipoTransaccion { get; set; }
    public string? TipoTransaccion { get; set; }
    public decimal Monto { get; set; }
    public decimal SaldoAnterior { get; set; }
    public decimal SaldoNuevo { get; set; }
    public DateTime FechaMovimiento { get; set; }
    public string? Usuario { get; set; }
    public string? Detalle { get; set; }
    public bool Exito { get; set; }
}
