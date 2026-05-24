using BusinessLogics;
using BusinessLogicsAbstractions;
using BusinessLogicsAbstractions.Exceptions;
using BusinessLogicsAbstractions.Imports;
using Contract.Importers;
using FluentAssertions;
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

        var productCreatorServiceMock = new Mock<IImportedProductCreatorService>();

        importerProviderMock.Setup(p => p.GetImporters())
            .Returns([importerMock.Object]);

        var service = new ProductImportService(
            importerProviderMock.Object,
            productCreatorServiceMock.Object);

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

    [TestMethod]
    public void ImportProducts_WhenImporterReturnsProducts_ShouldCreateProducts()
    {
        var stream = new MemoryStream();

        var importedProducts = new List<ImportedProductDto>
        {
            new()
            {
                Code = "P001",
                Name = "Burger",
                Description = "Burger imported",
                CommercialLine = "Fast Food",
                Category = "Food",
                Price = 250,
                PhotoPaths = ["C:\\photos\\burger.jpg"]
            }
        };

        var importerMock = new Mock<IProductImporter>();

        importerMock.Setup(i => i.Name)
            .Returns("CsvProductImporter");

        importerMock.Setup(i => i.Import(stream))
            .Returns(importedProducts);

        var importerProviderMock = new Mock<IProductImporterProvider>();

        importerProviderMock.Setup(p => p.GetImporters())
            .Returns([importerMock.Object]);

        var productCreatorServiceMock = new Mock<IImportedProductCreatorService>();

        var service = new ProductImportService(
            importerProviderMock.Object,
            productCreatorServiceMock.Object);

        var request = new ProductImportRequestDto
        {
            ImporterName = "CsvProductImporter",
            FileName = "products.csv",
            FileStream = stream
        };

        service.ImportProducts(request);

        productCreatorServiceMock.Verify(
            p => p.CreateImportedProduct(
                It.Is<ImportedProductDto>(dto =>
                    dto.Code == "P001" &&
                    dto.Name == "Burger" &&
                    dto.Description == "Burger imported" &&
                    dto.CommercialLine == "Fast Food" &&
                    dto.Category == "Food" &&
                    dto.Price == 250 &&
                    dto.PhotoPaths.Count == 1)),
            Times.Once);
    }

    [TestMethod]
    public void ImportProducts_WhenImporterDoesNotExist_ShouldThrowNotFoundException()
    {
        var importerProviderMock = new Mock<IProductImporterProvider>();
        var productCreatorServiceMock = new Mock<IImportedProductCreatorService>();

        importerProviderMock.Setup(p => p.GetImporters())
            .Returns([]);

        var service = new ProductImportService(
            importerProviderMock.Object,
            productCreatorServiceMock.Object);

        var request = new ProductImportRequestDto
        {
            ImporterName = "CsvProductImporter",
            FileName = "products.csv",
            FileStream = new MemoryStream()
        };

        Action act = () => service.ImportProducts(request);

        act.Should().Throw<NotFoundException>();
    }

    [TestMethod]
    public void ImportProducts_WhenRequestIsNull_ShouldThrowValidationException()
    {
        var importerProviderMock =
            new Mock<IProductImporterProvider>(MockBehavior.Strict);

        var productCreatorServiceMock =
            new Mock<IImportedProductCreatorService>(MockBehavior.Strict);

        var service = new ProductImportService(
            importerProviderMock.Object,
            productCreatorServiceMock.Object);

        ProductImportRequestDto request = null!;

        var act = () => service.ImportProducts(request);

        act.Should().Throw<ValidationException>()
            .WithMessage("*importación*");
    }

    [TestMethod]
    public void ImportProducts_WhenImporterNameIsEmpty_ShouldThrowValidationException()
    {
        var importerProviderMock =
            new Mock<IProductImporterProvider>(MockBehavior.Strict);

        var productCreatorServiceMock =
            new Mock<IImportedProductCreatorService>(MockBehavior.Strict);

        var service = new ProductImportService(
            importerProviderMock.Object,
            productCreatorServiceMock.Object);

        var request = new ProductImportRequestDto
        {
            ImporterName = string.Empty,
            FileStream = new MemoryStream()
        };

        var act = () => service.ImportProducts(request);

        act.Should().Throw<ValidationException>()
            .WithMessage("*importador*");
    }
}
