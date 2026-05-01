using DataAccessAbstractions;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories;

public class RepositoryUser(SqlContext context) : IRepositoryUser
{
    private readonly SqlContext _context = context;
    public User Add(User user)
    {
        _context.Users.Add(user);
        _context.SaveChanges();
        return user;
    }

    public List<User> GetAll()
    {
        return _context.Users
            .AsNoTracking()
            .Where(u => u.Status == UserStatus.Active)
            .ToList();
    }

    public bool EmailExist(string email)
    {
        var emailResult = Email.TryMakeEmail(email);

        if(!emailResult.Success)
        {
            return false;
        }

        var searchingEmail = emailResult.Value!;

        return _context.Users
            .AsNoTracking()
            .Any(u => u.Email.Value == searchingEmail.Value);
    }

    public User? FindByEmail(string email)
    {
        var emailResult = Email.TryMakeEmail(email);

        if(!emailResult.Success)
        {
            return null;
        }

        var searchingEmail = emailResult.Value!;

        return _context.Users
            .AsNoTracking()
            .SingleOrDefault(u =>
                u.Email.Value == searchingEmail.Value &&
                u.Status == UserStatus.Active);
    }

    public User? FindById(int id)
    {
        return _context.Users
            .SingleOrDefault(u =>
                u.Id == id &&
                u.Status == UserStatus.Active);
    }

    public void Remove(int id)
    {
        var user = _context.Users.SingleOrDefault(u => u.Id == id);

        if(user != null)
        {
            user.Deactivate();
            _context.SaveChanges();
        }
    }

    public User Update(User user)
    {
        _context.Users.Update(user);
        _context.SaveChanges();
        return user;
    }

    public User? FindByIdIncludingInactive(int id)
    {
        return _context.Users
            .AsNoTracking()
            .SingleOrDefault(u => u.Id == id);
    }
}
