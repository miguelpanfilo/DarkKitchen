using AlmacenamientoAbstracciones;
using Dominio;
using LogicaDeNegocio;
using Moq;

namespace Pruebas.ProductoPruebas;

[TestClass]
public class ProductoServiceTests
{
    [TestMethod]
    public void Crear_Producto_Retorna_Producto_Test_Ok()
    {
        var producto = new Producto("XXXX");

        var repoMock = new Mock<IProductoRepositorio>();
        repoMock
            .Setup(repocitorio => repocitorio.Crear(producto))
            .Returns(producto);

        var service = new ProductoService(repoMock.Object);

        var productoCreado = service.Crear(producto);

        Assert.IsNotNull(productoCreado);
        Assert.AreEqual("XXXX", productoCreado.Nombre);
    }
}
