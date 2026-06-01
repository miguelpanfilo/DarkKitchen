using System.Text;
using FluentAssertions;
using ProductImporters;

namespace Tests.Importers;

[TestClass]
public class CsvProductImporterTest
{
    [TestMethod]
    public void Import_WhenCsvIsValid_ShouldReturnProducts()
    {
        var csv = """
                  code,name,description,commercialLine,category,price,photoPaths
                  CSV001,Tarta de Espinaca,Tarta casera,Casera,Comidas,200.00,C:\photos\tarta.jpg
                  """;

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));

        var importer = new CsvImporter();

        var result = importer.Import(stream);

        result.Should().HaveCount(1);
        result[0].Code.Should().Be("CSV001");
        result[0].Name.Should().Be("Tarta de Espinaca");
    }

    [TestMethod]
    public void Import_WhenStreamIsEmpty_ShouldReturnEmptyList()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(string.Empty));

        var importer = new CsvImporter();

        var result = importer.Import(stream);

        result.Should().BeEmpty();
    }

    [TestMethod]
    public void Import_WhenProductHasMultiplePhotos_ShouldParseAllPaths()
    {
        var csv = """
                  code,name,description,commercialLine,category,price,photoPaths
                  CSV003,Milanesa,Milanesa napolitana,Casera,Comidas,180.00,C:\photos\mil1.jpg|C:\photos\mil2.jpg
                  """;

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));

        var importer = new CsvImporter();

        var result = importer.Import(stream);

        result[0].PhotoPaths.Should().HaveCount(2);
    }
}
