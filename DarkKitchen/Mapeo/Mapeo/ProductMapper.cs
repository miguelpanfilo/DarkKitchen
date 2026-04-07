using Contratos;
using Dominio;

namespace Mapeo;

public class ProductMapper
{
    public static Product ToEntity(ProductDTO productDto)
    {
        return new Product(
            productDto.Code,
            productDto.Name,
            productDto.Description,
            productDto.CommercialLine,
            productDto.Category
        );
    }
}
