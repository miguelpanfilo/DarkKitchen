namespace Contract;

public class OrderStatusResponse
{
    public int OrderId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime StatusUpdatedAt { get; set; }
}
