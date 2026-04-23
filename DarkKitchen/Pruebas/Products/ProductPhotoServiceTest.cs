using AlmacenamientoAbstracciones;
using Contratos.Products;
using Dominio.Exceptions;
using LogicaDeNegocio.Products;
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

        storageMock.Verify(s => s.Save(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
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

        Assert.AreEqual("La foto debe tener un base64 válido", exception.Message);

        storageMock.Verify(s => s.Save(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [TestMethod]
    public void GivenPhotoWithRealSizeGreaterThan500KB_WhenSavingPhotos_ThenThrowsProductException()
    {
        var bytes = new byte[512001];
        var base64 = Convert.ToBase64String(bytes);

        var photos = new List<PhotoDTO>
        {
            new PhotoDTO("photo1", ".jpg", 512001, base64)
        };

        var storageMock = new Mock<IProductPhotoStorage>(MockBehavior.Strict);
        var service = new ProductPhotoService(storageMock.Object);

        var exception = Assert.ThrowsException<ProductException>(() =>
            service.SavePhotos(photos));

        Assert.AreEqual("La foto no puede superar los 500 KB", exception.Message);

        storageMock.Verify(s => s.Save(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}
