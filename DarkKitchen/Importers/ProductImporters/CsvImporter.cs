using BusinessLogicsAbstractions;
using Contract;

namespace ProductImporters;

public class CsvImporter : IProductImporter
{
    public string Id => "csv";
    public string Name => "CSV";

    public List<ImportedProductDto> Import(Stream fileStream)
    {
        using var reader = new StreamReader(fileStream);
        var lines = reader.ReadToEnd()
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(l => l.Trim())
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .ToList();

        if (lines.Count == 0)
        {
            return [];
        }

        lines.RemoveAt(0);

        return lines.Select(ParseLine).ToList();
    }

    private static ImportedProductDto ParseLine(string line)
    {
        var parts = line.Split(';');

        return new ImportedProductDto
        {
            Code = parts[0],
            Name = parts[1],
            Description = parts[2],
            CommercialLine = parts[3],
            Category = parts[4],
            Price = decimal.Parse(parts[5]),
            PhotoPaths = [.. parts[6].Split('|')]
        };
    }
}
