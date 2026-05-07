using BusinessLogics.Shared;
using BusinessLogicsAbstractions.ShippingTypes;
using Contract.ShippingTypes;
using DataAccessAbstractions;
using Domain;
using Domain.Exceptions;
using Mappers;

namespace BusinessLogics.ShippingTypes;

public class ShippingTypeService(IShippingTypeRepository shippingTypeRepository) : IShippingTypeService
{
    private readonly IShippingTypeRepository _shippingTypeRepository = shippingTypeRepository;

    public ShippingTypeResponseDto CreateShippingType(ShippingTypeRequestDto request)
    {
        var shippingType = ServiceExceptionHandler.ExecuteWithValidationHandling<ShippingType, ShippingTypeException>(()
            => ShippingTypeMapper.ToEntity(request));

        var created = _shippingTypeRepository.Create(shippingType);

        return ShippingTypeMapper.ToResponseDto(created);
    }
}
