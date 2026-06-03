using System.Text;
using BusinessLogicsAbstractions;
using CsvProductImporter;
using FluentAssertions;

namespace Tests.Importers;

[TestClass]
public class CsvProductImporterTest
{
    [TestMethod]
    public void Import_WhenCsvIsValid_ShouldReturnProducts()
    {
        var csv = """
                  code;name;description;commercialLine;category;price;photoPaths
                  CSV001;Tarta de Espinaca;Tarta casera;Casera;Comidas;200.00;C:\photos\tarta.jpg
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
                  code;name;description;commercialLine;category;price;photoPaths
                  CSV003;Milanesa;Milanesa napolitana;Casera;Comidas;180.00;C:\photos\mil1.jpg|C:\photos\mil2.jpg
                  """;

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));

        var importer = new CsvImporter();

        var result = importer.Import(stream);

        result[0].PhotoPaths.Should().HaveCount(2);
    }

    [TestMethod]
    public void Import_WhenCsvHasMultipleProducts_ShouldReturnAllProducts()
    {
        var csv = """
                  code;name;description;commercialLine;category;price;photoPaths
                  CSV001;Tarta de Espinaca;Tarta casera;Casera;Comidas;200.00;C:\photos\tarta.jpg
                  CSV002;Empanada de Carne;Empanada casera;Casera;Comidas;150.00;C:\photos\empanada.jpg
                  """;

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));

        var importer = new CsvImporter();

        var result = importer.Import(stream);

        result.Should().HaveCount(2);
        result[1].Code.Should().Be("CSV002");
    }

    [TestMethod]
    public void Import_WhenCsvIsMalformed_ShouldThrowValidationException()
    {
        var csv = """
                  code;name;description;commercialLine;category;price;photoPaths
                  MALFORMED_LINE_SIN_COLUMNAS
                  """;

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));

        var importer = new CsvImporter();

        var act = () => importer.Import(stream);

        act.Should().Throw<ValidationException>()
            .WithMessage("El archivo CSV tiene un formato inválido");
    }

    [TestMethod]
    public void Import_WhenProductHasEmptyPhotoPaths_ShouldReturnEmptyPhotoList()
    {
        var csv = """
                  code;name;description;commercialLine;category;price;photoPaths
                  CSV005;Empanada;Desc;Casera;Comidas;150.00;
                  """;

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));

        var importer = new CsvImporter();

        var result = importer.Import(stream);

        result.Should().HaveCount(1);
        result[0].PhotoPaths.Should().BeEmpty();
    }
}
