using Contract.ShippingTypes;

namespace BusinessLogicsAbstractions.ShippingTypes;

public interface IShippingTypeService
{
    ShippingTypeResponseDto CreateShippingType(ShippingTypeRequestDto request);
}
