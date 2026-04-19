using Contratos;

namespace LogicaDeNegocioAbstracciones;

public interface IProductService
{
    ProductResponseDto GetById(int id);
}
