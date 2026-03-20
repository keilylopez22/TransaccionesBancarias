namespace CajeroAutomaticoAPI.Models;

public class Tarjeta
{
    public int Id { get; set; }
    public string NumeroTarjeta { get; set; } = null!;
    public string NombreEnTarjeta { get; set; } = null!;
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

public class CrearTarjetaRequest
{
    public int IdCuentahabiente { get; set; }
    public int IdCuenta { get; set; }
    public int IdTipoTarjeta { get; set; }
}

public class CrearTarjetaResponse
{
    public string NumeroTarjeta { get; set; } = null!;
    public string PIN_Generado { get; set; } = null!;
    public string CVV { get; set; } = null!;
    public DateTime Vence { get; set; }
}

public class ActualizarTarjetaRequest
{
    public bool Estado { get; set; }
    public int IntentosFallidos { get; set; } = 0;
    public DateTime? FechaBloqueo { get; set; }
}
