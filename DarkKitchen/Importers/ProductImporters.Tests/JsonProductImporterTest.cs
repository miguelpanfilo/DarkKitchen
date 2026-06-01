using System.Text;
using BusinessLogicsAbstractions;
using FluentAssertions;

namespace ProductImporters.Tests;

[TestClass]
public class JsonProductImporterTest
{
    [TestMethod]
    public void Import_WhenJsonIsValid_ShouldReturnProducts()
    {
        var json = """
                   [
                       {
                           "code": "P001",
                           "name": "Burger",
                           "description": "Burger imported",
                           "commercialLine": "Fast Food",
                           "category": "Food",
                           "price": 250,
                           "photoPaths": [ "C:\\photos\\burger.jpg" ]
                       }
                   ]
                   """;

        using var stream = new MemoryStream(
            Encoding.UTF8.GetBytes(json));

        var importer = new JsonProductImporter();

        var result = importer.Import(stream);

        result.Should().HaveCount(1);
        result[0].Code.Should().Be("P001");
        result[0].Name.Should().Be("Burger");
        result[0].Description.Should().Be("Burger imported");
        result[0].CommercialLine.Should().Be("Fast Food");
        result[0].Category.Should().Be("Food");
        result[0].Price.Should().Be(250);
        result[0].PhotoPaths.Should().ContainSingle()
            .Which.Should().Be("C:\\photos\\burger.jpg");
    }

    [TestMethod]
    public void Import_WhenJsonIsEmptyArray_ShouldReturnEmptyList()
    {
        var json = "[]";

        using var stream = new MemoryStream(
            Encoding.UTF8.GetBytes(json));

        var importer = new JsonProductImporter();

        var result = importer.Import(stream);

        result.Should().BeEmpty();
    }

    [TestMethod]
    public void Import_WhenJsonIsInvalid_ShouldThrowValidationException()
    {
        var json = "{ invalid json }";

        using var stream = new MemoryStream(
            Encoding.UTF8.GetBytes(json));

        var importer = new JsonProductImporter();

        Action act = () => importer.Import(stream);

        act.Should().Throw<ValidationException>()
            .WithMessage("*formato inválido*");
    }
}
