using Domain;

namespace DataAccessAbstractions;

public interface IShippingTypeRepository
{
    ShippingType Create(ShippingType shippingType);
    ShippingType? GetById(int id);
    ShippingType Update(ShippingType shippingType);
    List<ShippingType> GetAll();
}
