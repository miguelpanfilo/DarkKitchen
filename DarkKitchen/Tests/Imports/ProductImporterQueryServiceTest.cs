using BusinessLogics;
using BusinessLogicsAbstractions;
using FluentAssertions;
using Moq;

namespace Tests.imports;

[TestClass]
public class ProductImporterQueryServiceTest
{
    [TestMethod]
    public void GetImporters_ShouldReturnAvailableImporters()
    {
        var jsonImporter = new Mock<IProductImporter>();
        jsonImporter.Setup(i => i.Id).Returns("JsonProductImporter");
        jsonImporter.Setup(i => i.Name).Returns("Importador JSON");

        var xmlImporter = new Mock<IProductImporter>();
        xmlImporter.Setup(i => i.Id).Returns("XmlProductImporter");
        xmlImporter.Setup(i => i.Name).Returns("Importador XML");

        var importers = new List<IProductImporter>
        {
            jsonImporter.Object,
            xmlImporter.Object
        };

        var importerProvider = new Mock<IProductImporterProvider>();

        importerProvider
            .Setup(p => p.GetImporters())
            .Returns(importers);

        var service = new ProductImporterQueryService(
            importerProvider.Object);

        var result = service.GetImporters();

        result.Should().HaveCount(2);

        result[0].Id.Should().Be("JsonProductImporter");
        result[0].Name.Should().Be("Importador JSON");

        result[1].Id.Should().Be("XmlProductImporter");
        result[1].Name.Should().Be("Importador XML");
    }
}
