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

    public List<Promotion> GetAll()
    {
        return _context.Promotions.ToList();
    }

    public Promotion? GetById(int id)
    {
        return _context.Promotions.FirstOrDefault(p => p.Id == id)!;
    }

    public Promotion Update(Promotion promotion)
    {
        _context.Promotions.Update(promotion);
        _context.SaveChanges();
        return promotion;
    }
}
