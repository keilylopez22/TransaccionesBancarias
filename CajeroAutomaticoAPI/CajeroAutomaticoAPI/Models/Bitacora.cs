namespace CajeroAutomaticoAPI.Models;

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
    public DateTime? FechaCheque { get; set; }
    public string? NumeroCheque { get; set; }
    public string? Usuario { get; set; }
    public string? Detalle { get; set; }
    public bool Exito { get; set; }
}

public class BitacoraFiltroRequest
{
    public int? IdCuenta { get; set; }
    public int? IdTarjeta { get; set; }
    public int? IdTipoTransaccion { get; set; }
    public DateTime? FechaDesde { get; set; }
    public DateTime? FechaHasta { get; set; }
    public bool? SoloExitosos { get; set; }
    public string? NumeroCuenta { get; set; }
}
