namespace CajeroAutomaticoAPI.Models;

public class Cuentahabiente
{
    public int Id { get; set; }
    public string CUI { get; set; } = null!;
    public string PrimerNombre { get; set; } = null!;
    public string? SegundoNombre { get; set; }
    public string PrimerApellido { get; set; } = null!;
    public string? SegundoApellido { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public DateTime FechaRegistro { get; set; }
}

public class CrearCuentahabienteRequest
{
    public string CUI { get; set; } = null!;
    public string PrimerNombre { get; set; } = null!;
    public string? SegundoNombre { get; set; }
    public string PrimerApellido { get; set; } = null!;
    public string? SegundoApellido { get; set; }
    public DateTime FechaNacimiento { get; set; }
}

public class ActualizarCuentahabienteRequest
{
    public string CUI { get; set; } = null!;
    public string PrimerNombre { get; set; } = null!;
    public string? SegundoNombre { get; set; }
    public string PrimerApellido { get; set; } = null!;
    public string? SegundoApellido { get; set; }
    public DateTime FechaNacimiento { get; set; }
}
