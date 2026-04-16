using Dominio;

namespace AlmacenamientoAbstracciones;

public interface IRepositoryUser
{
    User Add(User user);
    List<User> GetAll();
    bool EmailExist(string email);
    User? FindByEmail(string email);
    User? FindById(int id);
    void Remove(int id);
    User Update(User user);
    List<User> Find(string? name, string? lastName);
}
