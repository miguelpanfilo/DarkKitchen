using System.Xml.Serialization;
using BusinessLogicsAbstractions.Imports;
using Contract.Importers;

namespace ProductImporters;

public class XmlProductImporter : IProductImporter
{
    public string Id => "xml";

    public string Name => "XML";

    public List<ImportedProductDto> Import(Stream fileStream)
    {
        var serializer =
            new XmlSerializer(typeof(List<ImportedProductDto>));

        var products =
            serializer.Deserialize(fileStream)
                as List<ImportedProductDto>;

        return products ?? [];
    }
}
