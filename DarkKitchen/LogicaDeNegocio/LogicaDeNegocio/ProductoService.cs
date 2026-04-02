using AlmacenamientoAbstracciones;
using Dominio;

namespace LogicaDeNegocio;

public class ProductoService
{
    private readonly IProductoRepositorio _repositorio;
    public ProductoService(IProductoRepositorio repositorio)
    {
        _repositorio = repositorio;
    }

    public Producto Crear(Producto producto)
    {
        return _repositorio.Crear(producto);
    }
}
