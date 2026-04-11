using Dominio;
using Dominio.Exceptions;

namespace Pruebas.Products;

[TestClass]
public class ProductTest
{
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
            new List<string>());

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
                new List<string>()));

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
                new List<string>()));

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
                new List<string>()));

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
                new List<string>()));

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
                new List<string>()));

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
                new List<string>()));

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
                new List<string>()));

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
                new List<string>()));

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
                new List<string>()));

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
                new List<string>()));

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
                new List<string>()));

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
            new List<string>());

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
            new List<string>());

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
            new List<string>());

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
                new List<string>()));

        Assert.AreEqual("El precio debe ser mayor a 0.", ex.Message);
    }
}
