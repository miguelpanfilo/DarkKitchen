using System.Text.Json;
using BusinessLogicsAbstractions.Exceptions;
using BusinessLogicsAbstractions.Imports;
using Contract.Importers;

namespace ProductImporters;

public class JsonProductImporter : IProductImporter
{
    public string Id => "json";

    public string Name => "JSON";

    public List<ImportedProductDto> Import(Stream fileStream)
    {
        try
        {
            using var reader = new StreamReader(fileStream);

            var json = reader.ReadToEnd();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var products =
                JsonSerializer.Deserialize<List<ImportedProductDto>>(
                    json,
                    options);

            return products ?? [];
        }
        catch(JsonException)
        {
            throw new ValidationException(
                "El archivo JSON tiene un formato inválido");
        }
    }
}
