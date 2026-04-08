using Contratos;
using Dominio;
using Dominio.Builders;

namespace Mapeo;

public class ProductMapper
{
    public static Product ToEntity(ProductDTO productDto)
    {
        return new ProductBuilder()
            .WithCode(productDto.Code)
            .WithName(productDto.Name)
            .WithDescription(productDto.Description)
            .WithCommercialLine(productDto.CommercialLine)
            .WithCategory(productDto.Category)
            .WithPrice(productDto.Price)
            .Build();
    }
}
