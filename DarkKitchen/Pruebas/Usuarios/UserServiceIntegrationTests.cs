using Contratos;
using LogicaDeNegocio;
using LogicaDeNegocioAbstracciones;
using Moq;

namespace Pruebas.Usuarios;

[TestClass]
public class UserServiceIntegrationTests
{
    [TestMethod]
    public void RegisterClient_CallsSignUpServiceOnce()
    {
        var signUpServiceMock = new Mock<ISignUpUserService>(MockBehavior.Strict);
        var modifyUserServiceMock = new Mock<IModifyUserService>(MockBehavior.Strict);
        var deleteUserServiceMock = new Mock<IDeleteUserService>(MockBehavior.Strict);
        var filterUserServiceMock = new Mock<IFilterUserService>(MockBehavior.Strict);

        var request = new SignUpClientRequestDTO
        {
            Name = "Miguel",
            LastName = "Lopez",
            Email = "miguel@gmail.com",
            Phone = "099123456",
            Password = "Password@2Ok6Abc"
        };

        signUpServiceMock
            .Setup(x => x.SignUpClient(request))
            .Returns(new SignUpClientResponseDTO
            {
                Name = "Miguel",
                LastName = "Lopez",
                Email = "miguel@gmail.com"
            });

        var service = new UserService(
            signUpServiceMock.Object,
            modifyUserServiceMock.Object,
            deleteUserServiceMock.Object,
            filterUserServiceMock.Object);

        service.RegistrarCliente(request);

        signUpServiceMock.Verify(
            x => x.SignUpClient(request),
            Times.Once);
    }

    [TestMethod]
    public void CreateInternalUser_CallsSignUpServiceOnce()
    {
        var signUpServiceMock = new Mock<ISignUpUserService>(MockBehavior.Strict);
        var modifyUserServiceMock = new Mock<IModifyUserService>(MockBehavior.Strict);
        var deleteUserServiceMock = new Mock<IDeleteUserService>(MockBehavior.Strict);
        var filterUserServiceMock = new Mock<IFilterUserService>(MockBehavior.Strict);

        var request = new SingUpInternUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "admin@gmail.com",
            Phone = "099123456",
            Password = "PasswordSeguraee3!",
            Role = "Administrativo"
        };

        signUpServiceMock
            .Setup(x => x.MakeInterUser(1, request))
            .Returns(new UsuarioResponseDTO()
            {
                Id = 10,
                Name = "Juan",
                LastName = "Perez",
                Email = "admin@gmail.com",
                Role = "Administrative"
            });

        var service = new UserService(
            signUpServiceMock.Object,
            modifyUserServiceMock.Object,
            deleteUserServiceMock.Object,
            filterUserServiceMock.Object);

        service.CrearUsuarioInterno(1, request);

        signUpServiceMock.Verify(
            x => x.MakeInterUser(1, request),
            Times.Once);
    }

    [TestMethod]
    public void UpdateInternalUser_CallsModifyServiceOnce()
    {
        var signUpServiceMock = new Mock<ISignUpUserService>(MockBehavior.Strict);
        var modifyUserServiceMock = new Mock<IModifyUserService>(MockBehavior.Strict);
        var deleteUserServiceMock = new Mock<IDeleteUserService>(MockBehavior.Strict);
        var filterUserServiceMock = new Mock<IFilterUserService>(MockBehavior.Strict);

        var request = new ModificarUsuarioRequestDTO
        {
            Name = "Carlos"
        };

        modifyUserServiceMock
            .Setup(x => x.UpdateInternalUser(5, 99, request))
            .Returns(new UsuarioResponseDTO()
            {
                Id = 5,
                Name = "Carlos",
                LastName = "Perez",
                Email = "admin@gmail.com",
                Phone = "099123456",
                Role = "Administrative"
            });

        var service = new UserService(
            signUpServiceMock.Object,
            modifyUserServiceMock.Object,
            deleteUserServiceMock.Object,
            filterUserServiceMock.Object);

        service.ModificarUsuarioInterno(5, 99, request);

        modifyUserServiceMock.Verify(
            x => x.UpdateInternalUser(5, 99, request),
            Times.Once);
    }

   [TestMethod]
    public void UpdateUserPassword_CallsModifyServiceOnce()
    {
        var signUpServiceMock = new Mock<ISignUpUserService>(MockBehavior.Strict);
        var modifyUserServiceMock = new Mock<IModifyUserService>(MockBehavior.Strict);
        var deleteUserServiceMock = new Mock<IDeleteUserService>(MockBehavior.Strict);
        var filterUserServiceMock = new Mock<IFilterUserService>(MockBehavior.Strict);

        var request = new ModificarClaveRequestDTO
        {
            OldPassword = "PasswordSeguraee3!",
            NewPassword = "ClaveNuevaSegura4!"
        };

        modifyUserServiceMock
            .Setup(x => x.UpdateUserPassword(5, 99, request));

        var service = new UserService(
            signUpServiceMock.Object,
            modifyUserServiceMock.Object,
            deleteUserServiceMock.Object,
            filterUserServiceMock.Object);

        service.ModificarClaveUsuario(5, 99, request);

        modifyUserServiceMock.Verify(
            x => x.UpdateUserPassword(5, 99, request),
            Times.Once);
    }

    [TestMethod]
    public void DeleteInternalUser_CallsDeleteServiceOnce()
    {
        var signUpServiceMock = new Mock<ISignUpUserService>(MockBehavior.Strict);
        var modifyUserServiceMock = new Mock<IModifyUserService>(MockBehavior.Strict);
        var deleteUserServiceMock = new Mock<IDeleteUserService>(MockBehavior.Strict);
        var filterUserServiceMock = new Mock<IFilterUserService>(MockBehavior.Strict);

        deleteUserServiceMock
            .Setup(x => x.DeleteInterUser(5, 99));

        var service = new UserService(
            signUpServiceMock.Object,
            modifyUserServiceMock.Object,
            deleteUserServiceMock.Object,
            filterUserServiceMock.Object);

        service.EliminarUsuarioInterno(5, 99);

        deleteUserServiceMock.Verify(
            x => x.DeleteInterUser(5, 99),
            Times.Once);
    }

    [TestMethod]
    public void SearchUsers_CallsFilterServiceOnce()
    {
        var signUpServiceMock = new Mock<ISignUpUserService>(MockBehavior.Strict);
        var modifyUserServiceMock = new Mock<IModifyUserService>(MockBehavior.Strict);
        var deleteUserServiceMock = new Mock<IDeleteUserService>(MockBehavior.Strict);
        var filterUserServiceMock = new Mock<IFilterUserService>(MockBehavior.Strict);

        var request = new FindUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez"
        };

        filterUserServiceMock
            .Setup(x => x.BuscarUsuarios(99, request))
            .Returns(new List<UsuarioResponseDTO>());

        var service = new UserService(
            signUpServiceMock.Object,
            modifyUserServiceMock.Object,
            deleteUserServiceMock.Object,
            filterUserServiceMock.Object);

        service.BuscarUsuarios(99, request);

        filterUserServiceMock.Verify(
            x => x.BuscarUsuarios(99, request),
            Times.Once);
    }
}
