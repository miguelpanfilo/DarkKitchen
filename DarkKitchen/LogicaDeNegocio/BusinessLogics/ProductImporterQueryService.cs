using BusinessLogicsAbstractions.Imports;
using Contract.Importers;

namespace BusinessLogics;

public class ProductImporterQueryService(
    IProductImporterProvider importerProvider)
    : IProductImporterQueryService
{
    public List<ImporterDto> GetImporters()
    {
        return importerProvider.GetImporters()
            .Select(i => new ImporterDto
            {
                Id = i.Id,
                Name = i.Name
            })
            .ToList();
    }
}
