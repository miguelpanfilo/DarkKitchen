namespace Domain;

public class OrderReportData : IReportVisitable
{
    public List<Order> Orders { get; }

    public OrderReportData(List<Order> orders)
    {
        Orders = orders;
    }

    public void Accept(IReportVisitor visitor)
    {
        visitor.Visit(this);
    }
}
