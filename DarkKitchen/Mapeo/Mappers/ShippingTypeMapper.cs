using Contract.ShippingTypes;
using Domain;

namespace Mappers;

public class ShippingTypeMapper
{
    public static ShippingType ToEntity(ShippingTypeRequestDto requestDto)
    {
        return ShippingType.MakeShippingType(requestDto.Name, requestDto.Cost);
    }

    public static ShippingTypeResponseDto ToResponseDto(ShippingType shippingType)
    {
        return new ShippingTypeResponseDto
        {
            Id = shippingType.Id,
            Name = shippingType.Name,
            Cost = shippingType.Cost
        };
    }
}
