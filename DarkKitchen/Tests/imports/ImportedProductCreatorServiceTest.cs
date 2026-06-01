using BusinessLogics;
using BusinessLogicsAbstractions;
using Contract;
using FluentAssertions;
using Moq;

namespace Tests.imports;

[TestClass]
public class ImportedProductCreatorServiceTest
{
     [TestMethod]
    public void CreateImportedProduct_WhenImportedProductIsValid_ShouldCreateProduct()
    {
        var productCreatorServiceMock =
            new Mock<IProductCreatorService>(MockBehavior.Strict);

        var photoConverterMock =
            new Mock<IImportedProductPhotoConverter>(MockBehavior.Strict);

        var photoPaths = new List<string>
        {
            "C:\\photos\\burger.jpg"
        };

        var photos = new List<PhotoDTO>
        {
            new("burger.jpg", ".jpg", 3, "AQID")
        };

        var importedProduct = new ImportedProductDto
        {
            Code = "P001",
            Name = "Burger",
            Description = "Burger imported",
            CommercialLine = "Fast Food",
            Category = "Food",
            Price = 250,
            PhotoPaths = photoPaths
        };

        var expectedResponse = new ProductResponseDto
        {
            Code = "P001",
            Name = "Burger"
        };

        photoConverterMock.Setup(c => c.Convert(photoPaths))
            .Returns(photos);

        productCreatorServiceMock.Setup(p => p.Create(
                It.IsAny<ProductDTO>()))
            .Returns(expectedResponse);

        var service = new ImportedProductCreatorService(
            productCreatorServiceMock.Object,
            photoConverterMock.Object);

        var result = service.CreateImportedProduct(
            importedProduct);

        result.Should().Be(expectedResponse);

        photoConverterMock.Verify(
            c => c.Convert(photoPaths),
            Times.Once);

        productCreatorServiceMock.Verify(
            p => p.Create(
                It.Is<ProductDTO>(dto =>
                    dto.Code == "P001" &&
                    dto.Name == "Burger" &&
                    dto.Description == "Burger imported" &&
                    dto.CommercialLine == "Fast Food" &&
                    dto.Category == "Food" &&
                    dto.Price == 250 &&
                    dto.Photos == photos)),
            Times.Once);
    }
}
