using Contract.Importers;

namespace BusinessLogicsAbstractions.Imports;

public interface IProductImportService
{
    public List<ImporterDto> GetImporters();
}
