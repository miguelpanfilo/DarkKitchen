using Dominio;
using Dominio.Exceptions;

namespace Pruebas.Products;

[TestClass]
public class ProductTest
{
    private static Product CreateValidProduct()
    {
        return Product.MakeProduct(
            "COD123",
            "Milanesa al pan",
            "Milanesa al pan completa con fritas",
            "Minutas clásicas",
            "Sandwiches",
            100,
            ValidPhotos()
        );
    }

    private static List<ProductPhoto> ValidPhotos(int count = 1)
    {
        var photos = new List<ProductPhoto>();

        for (var i = 0; i < count; i++)
        {
            photos.Add(new ProductPhoto($"foto{i}.jpg"));
        }

        return photos;
    }

    [TestMethod]
    public void GivenValidData_WhenCreatingProduct_ThenProductIsCreated()
    {
        var product = Product.MakeProduct(
            "COD123",
            "Milanesa al pan",
            "Milanesa al pan completa",
            "Minutas clásicas",
            "Sandwiches",
            100,
            ValidPhotos());

        Assert.IsNotNull(product);
        Assert.AreEqual("COD123", product.Code);
        Assert.AreEqual("Milanesa al pan", product.Name);
        Assert.AreEqual("Milanesa al pan completa", product.Description);
        Assert.AreEqual("Minutas clásicas", product.CommercialLine);
        Assert.AreEqual("Sandwiches", product.Category);
        Assert.AreEqual(100m, product.Price);
        Assert.IsTrue(product.IsActive);
    }

    [TestMethod]
    public void GivenEmptyName_WhenCreatingProduct_ThenThrowsProductExceptionWithMessage()
    {
        var ex = Assert.ThrowsException<ProductException>(() =>
            Product.MakeProduct(
                "COD123",
                " ",
                "Milanesa al pan completa",
                "Minutas clásicas",
                "Sandwiches",
                100,
                ValidPhotos(0)));

        Assert.AreEqual("El nombre no puede estar vacío.", ex.Message);
    }

    [TestMethod]
    public void GivenInvalidCode_WhenCreatingProduct_ThenThrowsProductExceptionWithMessage()
    {
        var ex = Assert.ThrowsException<ProductException>(() =>
            Product.MakeProduct(
                " ",
                "Milanesa al pan",
                "Milanesa al pan completa",
                "Minutas clásicas",
                "Sandwiches",
                100,
                ValidPhotos(0)));

        Assert.AreEqual("El código no puede estar vacío.", ex.Message);
    }

    [TestMethod]
    public void GivenEmptyDescription_WhenCreatingProduct_ThenThrowsProductExceptionWithMessage()
    {
        var ex = Assert.ThrowsException<ProductException>(() =>
            Product.MakeProduct(
                "COD123",
                "Milanesa al pan",
                " ",
                "Minutas clásicas",
                "Sandwiches",
                100,
                ValidPhotos(0)));

        Assert.AreEqual("La descripción no puede estar vacío.", ex.Message);
    }

    [TestMethod]
    public void GivenEmptyCommercialLine_WhenCreatingProduct_ThenThrowsProductExceptionWithMessage()
    {
        var ex = Assert.ThrowsException<ProductException>(() =>
            Product.MakeProduct(
                "COD123",
                "Milanesa al pan",
                "Milanesa al pan completa",
                " ",
                "Sandwiches",
                100,
                ValidPhotos(0)));

        Assert.AreEqual("Línea comercial no puede estar vacío.", ex.Message);
    }

    [TestMethod]
    public void GivenEmptyCategory_WhenCreatingProduct_ThenThrowsProductExceptionWithMessage()
    {
        var ex = Assert.ThrowsException<ProductException>(() =>
            Product.MakeProduct(
                "COD123",
                "Milanesa al pan",
                "Milanesa al pan completa",
                "Minutas clásicas",
                " ",
                100,
                ValidPhotos(0)));

        Assert.AreEqual("La categoría no puede estar vacío.", ex.Message);
    }

    [TestMethod]
    public void GivenShortCode_WhenCreatingProduct_ThenThrowsProductExceptionWithMessage()
    {
        var code = new string('A', 4);

        var ex = Assert.ThrowsException<ProductException>(() =>
            Product.MakeProduct(
                code,
                "Milanesa al pan",
                "Milanesa al pan completa",
                "Minutas clásicas",
                "Sandwiches",
                100,
                ValidPhotos(0)));

        Assert.AreEqual("El código debe tener entre 5 y 20 caracteres.", ex.Message);
    }

    [TestMethod]
    public void GivenLongCode_WhenCreatingProduct_ThenThrowsProductExceptionWithMessage()
    {
        var code = new string('A', 21);

        var ex = Assert.ThrowsException<ProductException>(() =>
            Product.MakeProduct(
                code,
                "Nombre válido",
                "Descripción suficientemente larga",
                "Minutas clásicas",
                "Sandwiches",
                100,
                ValidPhotos(0)));

        Assert.AreEqual("El código debe tener entre 5 y 20 caracteres.", ex.Message);
    }

    [TestMethod]
    public void GivenShortName_WhenCreatingProduct_ThenThrowsProductExceptionWithMessage()
    {
        var name = new string('A', 9);

        var ex = Assert.ThrowsException<ProductException>(() =>
            Product.MakeProduct(
                "COD123",
                name,
                "Descripción suficientemente larga",
                "Minutas clásicas",
                "Sandwiches",
                100,
                ValidPhotos(0)));

        Assert.AreEqual("El nombre debe tener entre 10 y 50 caracteres.", ex.Message);
    }

    [TestMethod]
    public void GivenLongName_WhenCreatingProduct_ThenThrowsProductExceptionWithMessage()
    {
        var name = new string('A', 51);

        var ex = Assert.ThrowsException<ProductException>(() =>
            Product.MakeProduct(
                "COD123",
                name,
                "Descripción suficientemente larga",
                "Minutas clásicas",
                "Sandwiches",
                100,
                ValidPhotos(0)));

        Assert.AreEqual("El nombre debe tener entre 10 y 50 caracteres.", ex.Message);
    }

    [TestMethod]
    public void GivenShortDescription_WhenCreatingProduct_ThenThrowsProductExceptionWithMessage()
    {
        var description = new string('A', 19);

        var ex = Assert.ThrowsException<ProductException>(() =>
            Product.MakeProduct(
                "COD123",
                "Nombre válido",
                description,
                "Minutas clásicas",
                "Sandwiches",
                100,
                ValidPhotos(0)));

        Assert.AreEqual("La descripción debe tener entre 20 y 500 caracteres.", ex.Message);
    }

    [TestMethod]
    public void GivenLongDescription_WhenCreatingProduct_ThenThrowsProductExceptionWithMessage()
    {
        var description = new string('A', 501);

        var ex = Assert.ThrowsException<ProductException>(() =>
            Product.MakeProduct(
                "COD123",
                "Nombre válido",
                description,
                "Minutas clásicas",
                "Sandwiches",
                100,
                ValidPhotos(0)));

        Assert.AreEqual("La descripción debe tener entre 20 y 500 caracteres.", ex.Message);
    }

    [TestMethod]
    public void GivenValidData_WhenCreatingProduct_ThenProductIsActiveByDefault()
    {
        var product = Product.MakeProduct(
            "ABC12",
            "Milanesa completa",
            "Milanesa completa con guarnicion y bebida incluida",
            "Minutas clasicas",
            "Fritos",
            100,
            ValidPhotos(1));

        Assert.IsTrue(product.IsActive);
    }

    [TestMethod]
    public void Deactivate_WhenCalled_ProductBecomesInactive()
    {
        var product = Product.MakeProduct(
            "ABC12",
            "Milanesa completa",
            "Milanesa completa con guarnicion y bebida incluida",
            "Minutas clasicas",
            "Fritos",
            100,
            ValidPhotos(1));

        product.Deactivate();

        Assert.IsFalse(product.IsActive);
    }

    [TestMethod]
    public void Activate_WhenCalled_ProductBecomesActive()
    {
        var product = Product.MakeProduct(
            "ABC12",
            "Milanesa completa",
            "Milanesa completa con guarnicion y bebida incluida",
            "Minutas clasicas",
            "Fritos",
            100,
            ValidPhotos(1));

        product.Deactivate();
        product.Activate();

        Assert.IsTrue(product.IsActive);
    }

    [DataTestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    public void GivenInvalidPrice_WhenCreatingProduct_ThenThrowsProductExceptionWithMessage(int price)
    {
        var ex = Assert.ThrowsException<ProductException>(() =>
            Product.MakeProduct(
                "COD123",
                "Milanesa al pan",
                "Milanesa al pan completa",
                "Minutas clásicas",
                "Sandwiches",
                price,
                ValidPhotos(0)));

        Assert.AreEqual("El precio debe ser mayor a 0.", ex.Message);
    }

    [DataTestMethod]
    [DataRow(1)]
    [DataRow(3)]
    public void GivenValidNumberOfPhotos_WhenCreatingProduct_ThenProductIsCreated(int photoCount)
    {
        var product = Product.MakeProduct(
            "COD123",
            "Milanesa al pan",
            "Milanesa al pan completa con fritas",
            "Minutas clásicas",
            "Sandwiches",
            100,
            ValidPhotos(photoCount)
        );

        Assert.IsNotNull(product);
        Assert.AreEqual(photoCount, product.Photos.Count);
    }

    [DataTestMethod]
    [DataRow(0)]
    [DataRow(4)]
    public void GivenInvalidNumberOfPhotos_WhenCreatingProduct_ThenThrowsProductException(int photoCount)
    {
        var exception = Assert.ThrowsException<ProductException>(() =>
            Product.MakeProduct(
                "COD123",
                "Milanesa al pan",
                "Milanesa al pan completa con fritas",
                "Minutas clásicas",
                "Sandwiches",
                100,
                ValidPhotos(photoCount)
            )
        );

        Assert.AreEqual("El producto debe tener entre 1 y 3 fotos.", exception.Message);
    }

    [TestMethod]
    public void GivenNullPhotos_WhenCreatingProduct_ThenThrowsProductException()
    {
        var exception = Assert.ThrowsException<ProductException>(() =>
            Product.MakeProduct(
                "COD123",
                "Milanesa al pan",
                "Milanesa al pan completa con fritas",
                "Minutas clásicas",
                "Sandwiches",
                100,
                null!
            )
        );

        Assert.AreEqual("El producto debe tener entre 1 y 3 fotos.", exception.Message);
    }

    [TestMethod]
    public void GivenValidName_WhenUpdatingName_ThenNameIsUpdated()
    {
        var product = CreateValidProduct();

        var newName = "Chivito completo";

        product.UpdateName(newName);

        Assert.AreEqual(newName, product.Name);
    }

    [TestMethod]
    public void GivenEmptyName_WhenUpdatingName_ThenThrowsProductExceptionWithMessage()
    {
        var product = CreateValidProduct();

        var ex = Assert.ThrowsException<ProductException>(() => product.UpdateName(string.Empty));

        Assert.AreEqual("El nombre no puede estar vacío.", ex.Message);
    }

    [DataTestMethod]
    [DataRow("corto")]
    [DataRow("este nombre es extremadamente largo y supera los cincuenta caracteres permitidos")]
    public void GivenInvalidLengthName_WhenUpdatingName_ThenThrowsProductExceptionWithMessage(string invalidName)
    {
        var product = CreateValidProduct();

        var ex = Assert.ThrowsException<ProductException>(() => product.UpdateName(invalidName));

        Assert.AreEqual("El nombre debe tener entre 10 y 50 caracteres.", ex.Message);
    }

    [TestMethod]
    public void GivenValidCode_WhenUpdatingCode_ThenUpdatesProductCode()
    {
        var product = CreateValidProduct();
        var newCode = "COD999";

        product.UpdateCode(newCode);

        Assert.AreEqual(newCode, product.Code);
    }

    [DataTestMethod]
    [DataRow("", "El código no puede estar vacío.")]
    [DataRow("abc", "El código debe tener entre 5 y 20 caracteres.")]
    [DataRow("este-codigo-es-demasiado-largo-para-ser-valido", "El código debe tener entre 5 y 20 caracteres.")]
    public void GivenInvalidCode_WhenUpdatingCode_ThenThrowsProductExceptionWithExpectedMessage(
        string invalidCode,
        string expectedMessage)
    {
        var product = CreateValidProduct();

        var exception = Assert.ThrowsException<ProductException>(() => product.UpdateCode(invalidCode));

        Assert.AreEqual(expectedMessage, exception.Message);
    }

    [TestMethod]
    public void GivenValidDescription_WhenUpdatingDescription_ThenUpdatesProductDescription()
    {
        var product = CreateValidProduct();
        var newDescription = "Esta es una descripción válida con más de veinte caracteres.";

        product.UpdateDescription(newDescription);

        Assert.AreEqual(newDescription, product.Description);
    }
}
