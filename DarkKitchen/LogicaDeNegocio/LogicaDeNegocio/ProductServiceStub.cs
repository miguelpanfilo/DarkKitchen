using Dominio;
using LogicaDeNegocioAbstracciones;

namespace LogicaDeNegocio;

public class ProductServiceTemp : IProductService
{
    public Product GetById(int id) => throw new NotImplementedException("ProductRepository no está implementado aún");
}
