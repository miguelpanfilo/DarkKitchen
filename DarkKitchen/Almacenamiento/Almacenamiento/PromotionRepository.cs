using AlmacenamientoAbstracciones;
using Dominio;

namespace Almacenamiento;

public class PromotionRepository : IPromotionRepository
{
    private readonly SqlContext _context;

    public PromotionRepository(SqlContext context)
    {
        _context = context;
    }

    public Promotion Create(Promotion promotion)
    {
        _context.Promotions.Add(promotion);
        _context.SaveChanges();
        return promotion;
    }

    public Promotion GetById(int id)
    {
        throw new NotImplementedException();
    }

    public Promotion Update(Promotion promotion)
    {
        throw new NotImplementedException();
    }
}
