namespace Contratos.Reportes;

public class PeriodoVentasDto
{
    public int Anio { get; set; }
    public int Mes { get; set; }
    public List<ClienteVentasDto> Clientes { get; set; } = [];
    public decimal TotalMes { get; set; }
}
