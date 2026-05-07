using BusinessLogics.Shared;
using BusinessLogicsAbstractions.Exceptions;
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

    public ShippingTypeResponseDto UpdateShippingType(int id, ShippingTypePatchDto request)
    {
        var shippingType = GetExistingShippingType(id);

        if (request.Name != null)
        {
            ServiceExceptionHandler.ExecuteWithValidationHandling<ShippingTypeException>(()
                => shippingType.UpdateName(request.Name));
        }

        if (request.Cost != null)
        {
            ServiceExceptionHandler.ExecuteWithValidationHandling<ShippingTypeException>(()
                => shippingType.UpdateCost(request.Cost.Value));
        }

        var updated = _shippingTypeRepository.Update(shippingType);

        return ShippingTypeMapper.ToResponseDto(updated);
    }

    private ShippingType GetExistingShippingType(int id)
    {
        var shippingType = _shippingTypeRepository.GetById(id);

        if (shippingType == null)
        {
            throw new NotFoundException("El tipo de envío no existe");
        }

        return shippingType;
    }
}
