using Domain;

namespace DataAccessAbstractions;

public interface IShippingTypeRepository
{
    public ShippingType Create(ShippingType shippingType);
    public ShippingType? GetById(int id);
    public ShippingType Update(ShippingType shippingType);
    public List<ShippingType> GetAll();
}
