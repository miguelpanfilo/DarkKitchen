using AlmacenamientoAbstracciones;
using Dominio;

namespace Almacenamiento;

public class RepositoryUser : IRepositoryUser
{
    private readonly List<User> _users = [];

    private int _lastId = 0;

    public User Add(User user)
    {
        _lastId++;
        user.Id = _lastId;
        _users.Add(user);
        return user;
    }

    public List<User> GetAll()
    {
        return _users;
    }

    public bool EmailExist(string email)
    {
        var emailResult = Email.TryMakeEmail(email);

        if(!emailResult.Success)
        {
            return false;
        }

        var searchingEmail = emailResult.Value!;

        return _users.Any(u => u.Email.IsTheSame(searchingEmail));
    }

    public User? FindByEmail(string email)
    {
        var emailResult = Email.TryMakeEmail(email);

        if(!emailResult.Success)
        {
            return null;
        }

        var searchingEmail = emailResult.Value!;

        return _users.FirstOrDefault(u => u.Email.IsTheSame(searchingEmail));
    }

    public User? FindById(int id)
    {
        return _users.FirstOrDefault(u => u.Id == id);
    }

    public void Remove(int id)
    {
        var user = _users.FirstOrDefault(u => u.Id == id);

        if(user != null)
        {
            _users.Remove(user);
        }
    }
}
