using Contract.Importers;

namespace BusinessLogicsAbstractions;

public interface IProductImporterQueryService
{
    public List<ImporterDto> GetImporters();
}
