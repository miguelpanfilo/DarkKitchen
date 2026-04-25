namespace Contratos.Reportes;

public class TopProductoReporteItemDto
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int CantidadVendida { get; set; }
    public List<string> Imagenes { get; set; } = [];
}
