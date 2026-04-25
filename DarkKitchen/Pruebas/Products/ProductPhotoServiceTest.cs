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

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    public void GivenPhotoWithEmptyExtension_WhenSavingPhotos_ThenThrowsProductException(string extension)
    {
        var photos = new List<PhotoDTO>
        {
            new PhotoDTO("photo1", extension, 100, Convert.ToBase64String(new byte[] { 1, 2, 3 }))
        };

        var storageMock = new Mock<IProductPhotoStorage>(MockBehavior.Strict);
        var service = new ProductPhotoService(storageMock.Object);

        var exception = Assert.ThrowsException<ProductException>(() =>
            service.SavePhotos(photos));

        Assert.AreEqual("Las fotos deben estar en formato JPG", exception.Message);

        storageMock.Verify(s => s.Save(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [DataTestMethod]
    [DataRow(".png")]
    [DataRow("jpeg")]
    [DataRow(".webp")]
    public void GivenPhotoWithNonJpgExtension_WhenSavingPhotos_ThenThrowsProductException(string extension)
    {
        var photos = new List<PhotoDTO>
        {
            new PhotoDTO("photo1", extension, 100, Convert.ToBase64String(new byte[] { 1, 2, 3 }))
        };

        var storageMock = new Mock<IProductPhotoStorage>(MockBehavior.Strict);
        var service = new ProductPhotoService(storageMock.Object);

        var exception = Assert.ThrowsException<ProductException>(() =>
            service.SavePhotos(photos));

        Assert.AreEqual("Las fotos deben estar en formato JPG", exception.Message);

        storageMock.Verify(s => s.Save(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [DataTestMethod]
    [DataRow(".JPG")]
    [DataRow(" jpg ")]
    [DataRow(".jpg")]
    public void GivenPhotoWithValidJpgExtension_WhenSavingPhotos_ThenSavesPhoto(string extension)
    {
        var base64 = Convert.ToBase64String(new byte[] { 1, 2, 3 });

        var photos = new List<PhotoDTO>
        {
            new PhotoDTO("photo1", extension, 100, base64)
        };

        var storageMock = new Mock<IProductPhotoStorage>(MockBehavior.Strict);

        storageMock
            .Setup(s => s.Save(base64, extension))
            .Returns("uploads/photo1.jpg");

        var service = new ProductPhotoService(storageMock.Object);

        var result = service.SavePhotos(photos);

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("uploads/photo1.jpg", result[0].Path);

        storageMock.Verify(s => s.Save(base64, extension), Times.Once);
    }
}
