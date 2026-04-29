namespace LogicaDeNegocioAbstracciones.Users;

public interface IDeleteUserService
{
    void DeleteInterUser(int idToDelete, int idUserLogged);
}
