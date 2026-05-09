using Contract.Importers;

namespace BusinessLogicsAbstractions.Imports;

public interface IProductImporterQueryService
{
    public List<ImporterDto> GetImporters();
}
