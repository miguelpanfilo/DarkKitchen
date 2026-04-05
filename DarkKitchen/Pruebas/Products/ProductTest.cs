using Dominio;
using Dominio.Excepciones;

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
        Assert.ThrowsException<ProductException>(() =>
            new Product(
                "1234",
                "Milanesa al pan",
                "Milanesa al pan completa",
                "Minutas clásicas",
                "Sandwiches")
        );
    }
}
