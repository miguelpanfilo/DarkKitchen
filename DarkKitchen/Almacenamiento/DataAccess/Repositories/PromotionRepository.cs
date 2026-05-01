using DataAccessAbstractions;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

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
        return _context.Promotions
            .Include(p => p.PromotionProducts)
            .ToList();
    }

    public Promotion? GetById(int id)
    {
        return _context.Promotions
            .Include(p => p.PromotionProducts)
            .FirstOrDefault(p => p.Id == id);
    }

    public Promotion Update(Promotion promotion)
    {
        _context.Promotions.Update(promotion);
        _context.SaveChanges();
        return promotion;
    }
}
