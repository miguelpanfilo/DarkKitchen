using Contract.ShippingTypes;

namespace BusinessLogicsAbstractions.ShippingTypes;

public interface IShippingTypeService
{
    ShippingTypeResponseDto CreateShippingType(ShippingTypeRequestDto request);
    ShippingTypeResponseDto UpdateShippingType(int id, ShippingTypePatchDto request);
    List<ShippingTypeResponseDto> GetAllShippingTypes();
}
