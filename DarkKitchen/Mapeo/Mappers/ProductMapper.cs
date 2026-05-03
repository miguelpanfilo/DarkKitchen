using Contract.Products;
using Domain;
using Domain.Builders;

namespace Mappers;

public class ProductMapper
{
    public static Product ToEntity(ProductDTO productDto, List<ProductPhoto> savedPhotos)
    {
        return new ProductBuilder()
            .WithCode(productDto.Code)
            .WithName(productDto.Name)
            .WithDescription(productDto.Description)
            .WithCommercialLine(productDto.CommercialLine)
            .WithCategory(productDto.Category)
            .WithPrice(productDto.Price)
            .WithPhotos(savedPhotos)
            .Build();
    }

    public static ProductResponseDto ToResponseDto(Product product)
    {
        return new ProductResponseDto
        {
            Id = product.Id,
            Code = product.Code,
            Name = product.Name,
            Description = product.Description,
            CommercialLine = product.CommercialLine,
            Category = product.Category,
            Price = product.Price,
            IsActive = product.IsActive,
            Photos = product.Photos
                .Select(photo => new PhotoResponseDto
                {
                    Path = photo.Path
                })
                .ToList()
        };
    }
}
