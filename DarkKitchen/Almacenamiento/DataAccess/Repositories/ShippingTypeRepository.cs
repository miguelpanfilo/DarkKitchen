using DataAccessAbstractions;
using Domain;

namespace DataAccess.Repositories;

public class ShippingTypeRepository : IShippingTypeRepository
{
    private readonly SqlContext _context;

    public ShippingTypeRepository(SqlContext context)
    {
        _context = context;
    }

    public ShippingType Create(ShippingType shippingType)
    {
        _context.ShippingTypes.Add(shippingType);
        _context.SaveChanges();
        return shippingType;
    }

    public ShippingType? GetById(int id)
    {
        return _context.ShippingTypes.FirstOrDefault(s => s.Id == id);
    }

    public ShippingType Update(ShippingType shippingType)
    {
        _context.ShippingTypes.Update(shippingType);
        _context.SaveChanges();
        return shippingType;
    }

    public List<ShippingType> GetAll()
    {
        return _context.ShippingTypes.ToList();
    }
}
