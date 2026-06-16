using System.Globalization;
using BusinessLogicsAbstractions;
using Contract;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using ValidationException =BusinessLogicsAbstractions.ValidationException;

namespace CsvProductImporter;

public class CsvImporter : IProductImporter
{
    public string Id => "csv";
    public string Name => "CSV";

    public List<ImportedProductDto> Import(Stream fileStream)
    {
        try
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HasHeaderRecord = true,
            };

            using var reader = new StreamReader(fileStream);
            using var csv = new CsvReader(reader, config);

            csv.Context.RegisterClassMap<ImportedProductDtoMap>();

            return csv.GetRecords<ImportedProductDto>().ToList();
        }
        catch (CsvHelperException)
        {
            throw new ValidationException("El archivo CSV tiene un formato inválido");
        }
    }
}

public class ImportedProductDtoMap : ClassMap<ImportedProductDto>
{
    public ImportedProductDtoMap()
    {
        Map(m => m.Code).Name("code");
        Map(m => m.Name).Name("name");
        Map(m => m.Description).Name("description");
        Map(m => m.CommercialLine).Name("commercialLine");
        Map(m => m.Category).Name("category");
        Map(m => m.Price).Name("price");
        Map(m => m.PhotoPaths).Name("photoPaths").TypeConverter<PhotoPathsConverter>();
    }
}

public class PhotoPathsConverter : DefaultTypeConverter
{
    public override object ConvertFromString(
        string? text,
        IReaderRow row,
        MemberMapData memberMapData)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return new List<string>();
        }

        return text.Split('|').ToList();
    }
}
