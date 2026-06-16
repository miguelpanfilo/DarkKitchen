using Contract;

namespace BusinessLogicsAbstractions;

public interface IProductImporterQueryService
{
    public List<ImporterDto> GetImporters();
}
