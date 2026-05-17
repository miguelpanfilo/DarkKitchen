using BusinessLogics;
using BusinessLogicsAbstractions.Imports;
using Contract.Importers;
using Moq;

namespace Tests.imports;

[TestClass]
public class ProductImportServiceTest
{
    [TestMethod]
    public void ImportProducts_WhenImporterExists_ShouldUseImporterToReadFile()
    {
        var importerMock = new Mock<IProductImporter>();

        importerMock.Setup(i => i.Name)
            .Returns("CsvProductImporter");

        var importerProviderMock = new Mock<IProductImporterProvider>();

        importerProviderMock.Setup(p => p.GetImporters())
            .Returns([importerMock.Object]);

        var service = new ProductImportService(
            importerProviderMock.Object);

        var stream = new MemoryStream();

        var request = new ProductImportRequestDto
        {
            ImporterName = "CsvProductImporter",
            FileName = "products.csv",
            FileStream = stream
        };

        service.ImportProducts(request);

        importerMock.Verify(
            i => i.Import(stream),
            Times.Once);
    }
}
