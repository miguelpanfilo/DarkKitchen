using Dominio;
using Dominio.Exceptions;

namespace Pruebas.Products;

[TestClass]
public class ProductTest
{
    [TestMethod]
    public void GivenValidData_WhenCreatingProduct_ThenProductIsCreated()
    {
        var product = new Product(
            "COD123",
            "Milanesa al pan",
            "Milanesa al pan completa",
            "Minutas clásicas",
            "Sandwiches"
        );

        Assert.IsNotNull(product);
        Assert.AreEqual("COD123", product.Code);
        Assert.AreEqual("Milanesa al pan", product.Name);
        Assert.AreEqual("Milanesa al pan completa", product.Description);
        Assert.AreEqual("Minutas clásicas", product.CommercialLine);
        Assert.AreEqual("Sandwiches", product.Category);
    }

    [TestMethod]
    public void GivenEmptyName_WhenCreatingProduct_ThenThrowsProductException()
    {
        Assert.ThrowsException<ProductException>(() =>
            new Product(
                "COD123",
                " ",
                "Milanesa al pan completa",
                "Minutas clásicas",
                "Sandwiches"
            )
        );
    }

    [TestMethod]
    public void GivenInvalidCode_WhenCreatingProduct_ThenThrowsProductException()
    {
        Assert.ThrowsException<ProductException>(() =>
            new Product(
                " ",
                "Milanesa al pan",
                "Milanesa al pan completa",
                "Minutas clásicas",
                "Sandwiches"
            )
        );
    }

    [TestMethod]
    public void GivenEmptyDescription_WhenCreatingProduct_ThenThrowsProductException()
    {
        Assert.ThrowsException<ProductException>(() =>
            new Product(
                "COD123",
                "Milanesa al pan",
                " ",
                "Minutas clásicas",
                "Sandwiches"
            )
        );
    }

    [TestMethod]
    public void GivenEmptyCommercialLine_WhenCreatingProduct_ThenThrowsProductException()
    {
        Assert.ThrowsException<ProductException>(() =>
            new Product(
                "COD123",
                "Milanesa al pan",
                "Milanesa al pan completa",
                " ",
                "Sandwiches"
            )
        );
    }

    [TestMethod]
    public void GivenEmptyCategory_WhenCreatingProduct_ThenThrowsProductException()
    {
        Assert.ThrowsException<ProductException>(() =>
            new Product(
                "COD123",
                "Milanesa al pan",
                "Milanesa al pan completa",
                "Minutas clásicas",
                " "
            )
        );
    }

    [TestMethod]
    public void GivenShortCode_WhenCreatingProduct_ThenThrowsProductException()
    {
        var code = new string('A', 4);

        Assert.ThrowsException<ProductException>(() =>
            new Product(
                new string('1', 4),
                "Milanesa al pan",
                "Milanesa al pan completa",
                "Minutas clásicas",
                "Sandwiches")
        );
    }

    [TestMethod]
    public void GivenLongCode_WhenCreatingProduct_ThenThrowsProductException()
    {
        var code = new string('A', 21);

        Assert.ThrowsException<ProductException>(() =>
            new Product(
                code,
                "Nombre válido",
                "Descripción suficientemente larga",
                "Minutas clásicas",
                "Sandwiches"
            )
        );
    }

    [TestMethod]
    public void GivenShortName_WhenCreatingProduct_ThenThrowsException()
    {
        var name = new string('A', 9);

        Assert.ThrowsException<ProductException>(() =>
            new Product(
                "COD123",
                name,
                "Descripción suficientemente larga",
                "Minutas clásicas",
                "Sandwiches"
            )
        );
    }

    [TestMethod]
    public void GivenLongName_WhenCreatingProduct_ThenThrowsException()
    {
        var name = new string('A', 51);

        Assert.ThrowsException<ProductException>(() =>
            new Product(
                "COD123",
                name,
                "Descripción suficientemente larga",
                "Minutas clásicas",
                "Sandwiches"
            )
        );
    }

    [TestMethod]
    public void GivenShortDescription_WhenCreatingProduct_ThenThrowsProductException()
    {
        var description = new string('A', 19);

        Assert.ThrowsException<ProductException>(() =>
            new Product(
                "COD123",
                "Nombre válido",
                description,
                "Minutas clásicas",
                "Sandwiches"
            )
        );
    }

    [TestMethod]
    public void GivenLongDescription_WhenCreatingProduct_ThenThrowsProductException()
    {
        var description = new string('A', 501);

        Assert.ThrowsException<ProductException>(() =>
            new Product(
                "COD123",
                "Nombre válido",
                description,
                "Minutas clásicas",
                "Sandwiches"
            )
        );
    }

    [TestMethod]
    public void Constructor_WhenDataIsValid_ProductIsActiveByDefault()
    {
        Product product = new Product(
            "ABC12",
            "Milanesa completa",
            "Milanesa completa con guarnicion y bebida incluida",
            "Minutas clasicas",
            "Fritos");

        Assert.IsTrue(product.IsActive);
    }

    [TestMethod]
    public void Deactivate_WhenCalled_ProductBecomesInactive()
    {
        Product product = new Product(
            "ABC12",
            "Milanesa completa",
            "Milanesa completa con guarnicion y bebida incluida",
            "Minutas clasicas",
            "Fritos");

        product.Deactivate();

        Assert.IsFalse(product.IsActive);
    }
}
