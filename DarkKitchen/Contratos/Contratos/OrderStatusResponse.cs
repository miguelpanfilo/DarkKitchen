namespace Contratos;

public class OrderStatusResponse
{
    public int PedidoId { get; set; }
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaCambioEstado { get; set; }
}
