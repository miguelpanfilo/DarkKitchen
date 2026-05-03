namespace BusinessLogicsAbstractions.Users;

public interface IDeleteUserService
{
    void DeleteInterUser(int idToDelete, int idUserLogged);
}
