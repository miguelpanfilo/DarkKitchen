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
}
