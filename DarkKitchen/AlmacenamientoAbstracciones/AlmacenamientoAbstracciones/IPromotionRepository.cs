using Dominio;

namespace AlmacenamientoAbstracciones;

public interface IPromotionRepository
{
    public Promotion Create(Promotion promotion);
    List<Promotion> GetAll();
    public Promotion? GetById(int id);
    public Promotion Update(Promotion promotion);
}
