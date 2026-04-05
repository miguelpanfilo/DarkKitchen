using Contratos;
using LogicaDeNegocio;
using LogicaDeNegocioAbstracciones;
using Moq;

namespace Pruebas.Usuarios;

[TestClass]
public class UserServiceIntegrationTests
{
    [TestMethod]
    public void RegistrarCliente_LlamaAlServicioDeAltaUnaVez()
    {
        var altaMock = new Mock<ISignUpUserService>();
        var modificacionMock = new Mock<IModifyUserService>();
        var eliminacionMock = new Mock<IDeleteUserService>();
        var consultaMock = new Mock<IFilterUserService>();

        var service = new UserService(
            altaMock.Object,
            modificacionMock.Object,
            eliminacionMock.Object,
            consultaMock.Object);

        var request = new SignUpClientRequestDTO
        {
            Name = "Miguel",
            LastName = "Lopez",
            Email = "miguel@gmail.com",
            Phone = "099123456",
            Password = "Password@2Ok6Abc"
        };

        service.RegistrarCliente(request);

        altaMock.Verify(
            x => x.SignUpClient(request),
            Times.Once);
    }

    [TestMethod]
    public void CrearUsuarioInterno_LlamaAlServicioDeAltaUnaVez()
    {
        var altaMock = new Mock<ISignUpUserService>();
        var modificacionMock = new Mock<IModifyUserService>();
        var eliminacionMock = new Mock<IDeleteUserService>();
        var consultaMock = new Mock<IFilterUserService>();

        var service = new UserService(
            altaMock.Object,
            modificacionMock.Object,
            eliminacionMock.Object,
            consultaMock.Object);

        var request = new SingUpInternUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez",
            Email = "admin@gmail.com",
            Phone = "099123456",
            Password = "PasswordSeguraee3!",
            Role = "Administrativo"
        };

        service.CrearUsuarioInterno(1, request);

        altaMock.Verify(
            x => x.MakeInterUser(1, request),
            Times.Once);
    }

    [TestMethod]
    public void ModificarUsuarioInterno_LlamaAlServicioDeModificacionUnaVez()
    {
        var altaMock = new Mock<ISignUpUserService>();
        var modificacionMock = new Mock<IModifyUserService>();
        var eliminacionMock = new Mock<IDeleteUserService>();
        var consultaMock = new Mock<IFilterUserService>();

        var service = new UserService(
            altaMock.Object,
            modificacionMock.Object,
            eliminacionMock.Object,
            consultaMock.Object);

        var request = new ModificarUsuarioRequestDTO
        {
            Name = "Carlos"
        };

        service.ModificarUsuarioInterno(5, 99, request);

        modificacionMock.Verify(
            x => x.UpdateInternalUser(5, 99, request),
            Times.Once);
    }

    [TestMethod]
    public void ModificarClaveUsuario_LlamaAlServicioDeModificacionUnaVez()
    {
        var altaMock = new Mock<ISignUpUserService>();
        var modificacionMock = new Mock<IModifyUserService>();
        var eliminacionMock = new Mock<IDeleteUserService>();
        var consultaMock = new Mock<IFilterUserService>();

        var service = new UserService(
            altaMock.Object,
            modificacionMock.Object,
            eliminacionMock.Object,
            consultaMock.Object);

        var request = new ModificarClaveRequestDTO
        {
            OldPassword = "PasswordSeguraee3!",
            NewPassword = "ClaveNuevaSegura4!"
        };

        service.ModificarClaveUsuario(5, 99, request);

        modificacionMock.Verify(
            x => x.UpdateUserPassword(5, 99, request),
            Times.Once);
    }

    [TestMethod]
    public void EliminarUsuarioInterno_LlamaAlServicioDeEliminacionUnaVez()
    {
        var altaMock = new Mock<ISignUpUserService>();
        var modificacionMock = new Mock<IModifyUserService>();
        var eliminacionMock = new Mock<IDeleteUserService>();
        var consultaMock = new Mock<IFilterUserService>();

        var service = new UserService(
            altaMock.Object,
            modificacionMock.Object,
            eliminacionMock.Object,
            consultaMock.Object);

        service.EliminarUsuarioInterno(5, 99);

        eliminacionMock.Verify(
            x => x.DeleteInterUser(5, 99),
            Times.Once);
    }

    [TestMethod]
    public void BuscarUsuarios_LlamaAlServicioDeConsultaUnaVez()
    {
        var altaMock = new Mock<ISignUpUserService>();
        var modificacionMock = new Mock<IModifyUserService>();
        var eliminacionMock = new Mock<IDeleteUserService>();
        var consultaMock = new Mock<IFilterUserService>();

        var service = new UserService(
            altaMock.Object,
            modificacionMock.Object,
            eliminacionMock.Object,
            consultaMock.Object);

        var request = new FindUserRequestDTO
        {
            Name = "Juan",
            LastName = "Perez"
        };

        service.BuscarUsuarios(99, request);

        consultaMock.Verify(
            x => x.BuscarUsuarios(99, request),
            Times.Once);
    }
}
