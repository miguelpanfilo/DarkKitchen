using BusinessLogicsAbstractions;
using Contract;

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
