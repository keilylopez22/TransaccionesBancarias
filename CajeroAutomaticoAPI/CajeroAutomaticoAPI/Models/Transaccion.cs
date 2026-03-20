namespace CajeroAutomaticoAPI.Models;

public class TransaccionRequest
{
    public int IdTipoTransaccion { get; set; }
    public string? NumeroCuenta { get; set; }
    public string? NumeroTarjeta { get; set; }
    public string? PIN { get; set; }
    public decimal Monto { get; set; }
    public string? NumeroCheque { get; set; }
    public DateTime? FechaCheque { get; set; }
    public string? DetallePersonalizado { get; set; }
}

public class DepositoRequest
{
    public string NumeroCuenta { get; set; } = null!;
    public decimal Monto { get; set; }
}

public class NotaCreditoRequest
{
    public string NumeroCuenta { get; set; } = null!;
    public decimal Monto { get; set; }
    public string Detalle { get; set; } = null!;
}

public class NotaDebitoRequest
{
    public string NumeroTarjeta { get; set; } = null!;
    public string PIN { get; set; } = null!;
    public decimal Monto { get; set; }
}

public class TransaccionResponse
{
    public string Mensaje { get; set; } = null!;
    public string? Cuenta { get; set; }
    public decimal? MontoAplicado { get; set; }
    public decimal? NuevoSaldo { get; set; }
}

public class TransaccionMasivaLinea
{
    public int Linea { get; set; }
    public int IdTipoTransaccion { get; set; }
    public string? NumeroCuenta { get; set; }
    public string? NumeroTarjeta { get; set; }
    public decimal Monto { get; set; }
    public string? NumeroCheque { get; set; }
    public DateTime? FechaCheque { get; set; }
    public string? Detalle { get; set; }
    public string? PIN { get; set; }
}

public class TransaccionMasivaResultado
{
    public int Linea { get; set; }
    public int IdTipoTransaccion { get; set; }
    public string? NumeroCuenta { get; set; }
    public decimal Monto { get; set; }
    public bool Exito { get; set; }
    public string Mensaje { get; set; } = null!;
    public decimal? NuevoSaldo { get; set; }
}

public class TransaccionMasivaResponse
{
    public int TotalLineas { get; set; }
    public int Exitosas { get; set; }
    public int Fallidas { get; set; }
    public long TiempoMs { get; set; }
    public List<TransaccionMasivaResultado> Resultados { get; set; } = [];
}
