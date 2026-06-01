using Contract.Importers;

namespace BusinessLogicsAbstractions;

public interface IProductImporter
{
    string Id { get; }

    string Name { get; }

    List<ImportedProductDto> Import(Stream fileStream);
}
