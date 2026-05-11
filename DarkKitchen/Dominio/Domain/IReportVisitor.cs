namespace Domain;

public interface IReportVisitor
{
    void Visit(OrderReportData data);
}
