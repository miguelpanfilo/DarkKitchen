namespace Contratos.Reportes;

public class ReporteVentasDto
{
    public List<PeriodoVentasDto> Periodos { get; set; } = [];
    public decimal TotalGeneral { get; set; }
}
