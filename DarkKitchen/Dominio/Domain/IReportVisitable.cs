namespace Domain;
public interface IReportVisitable
{
    void Accept(IReportVisitor visitor);
}
