using System.Xml.Serialization;
using BusinessLogicsAbstractions;
using Contract;

namespace XmlProductImporter;

public class XmlImporter : IProductImporter
{
    public string Id => "xml";
    public string Name => "XML";

    public List<ImportedProductDto> Import(Stream fileStream)
    {
        try
        {
            var serializer = new XmlSerializer(typeof(List<ImportedProductDto>));
            var products = serializer.Deserialize(fileStream) as List<ImportedProductDto>;
            return products ?? [];
        }
        catch (InvalidOperationException)
        {
            throw new ValidationException("El archivo XML tiene un formato inválido");
        }
    }
}
