using AlmacenamientoAbstracciones;
using Contratos;
using Dominio.Exceptions;
using LogicaDeNegocio;
using Moq;

namespace Pruebas.Products;

[TestClass]
public class ProductPhotoServiceTest
{
    [TestMethod]
    public void GivenPhotoWithInvalidBase64_WhenSavingPhotos_ThenThrowsProductException()
    {
        var photos = new List<PhotoDTO>
        {
            new PhotoDTO("photo1", ".jpg", 100, "base64-invalido")
        };

        var storageMock = new Mock<IProductPhotoStorage>(MockBehavior.Strict);
        var service = new ProductPhotoService(storageMock.Object);

        var exception = Assert.ThrowsException<ProductException>(() =>
            service.SavePhotos(photos));

        Assert.AreEqual("La foto debe tener un base64 válido", exception.Message);

        storageMock.Verify(s => s.Save(It.IsAny<PhotoDTO>()), Times.Never);
    }

    [TestMethod]
    public void GivenPhotoWithEmptyBase64_WhenSavingPhotos_ThenThrowsProductException()
    {
        var photos = new List<PhotoDTO>
        {
            new PhotoDTO("photo1", ".jpg", 100, string.Empty)
        };

        var storageMock = new Mock<IProductPhotoStorage>(MockBehavior.Strict);
        var service = new ProductPhotoService(storageMock.Object);

        var exception = Assert.ThrowsException<ProductException>(() =>
            service.SavePhotos(photos));

        Assert.AreEqual("La foto debe tener contenido en base64", exception.Message);

        storageMock.Verify(s => s.Save(It.IsAny<PhotoDTO>()), Times.Never);
    }
}
