using Contract;

namespace BusinessLogicsAbstractions;

public interface IShippingTypeService
{
    ShippingTypeResponseDto CreateShippingType(ShippingTypeRequestDto request);
    ShippingTypeResponseDto UpdateShippingType(int id, ShippingTypePatchDto request);
    List<ShippingTypeResponseDto> GetAllShippingTypes();
    ShippingTypeResponseDto? GetByName(string name);
}
