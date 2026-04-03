using Contratos;
using LogicaDeNegocio;
using LogicaDeNegocioAbstracciones;
using Moq;

namespace Pruebas.Usuarios;

[TestClass]
public class UsuarioServiceIntegracion
{
    [TestMethod]
    public void RegistrarCliente_LlamaAlServicioDeAltaUnaVez()
    {
        var altaMock = new Mock<IAltaUsuarioService>();
        var modificacionMock = new Mock<IModificacionUsuarioService>();
        var eliminacionMock = new Mock<IEliminacionUsuarioService>();
        var consultaMock = new Mock<IConsultaUsuarioService>();

        var service = new UsuarioService(
            altaMock.Object,
            modificacionMock.Object,
            eliminacionMock.Object,
            consultaMock.Object
        );

        var request = new RegistrarClienteRequest
        {
            Nombre = "Miguel",
            Apellido = "Lopez",
            Email = "miguel@gmail.com",
            Telefono = "099123456",
            Clave = "Password@2Ok6Abc"
        };

        service.RegistrarCliente(request);

        altaMock.Verify(
            x => x.RegistrarCliente(request),
            Times.Once
        );
    }

    [TestMethod]
    public void CrearUsuarioInterno_LlamaAlServicioDeAltaUnaVez()
    {
        var altaMock = new Mock<IAltaUsuarioService>();
        var modificacionMock = new Mock<IModificacionUsuarioService>();
        var eliminacionMock = new Mock<IEliminacionUsuarioService>();
        var consultaMock = new Mock<IConsultaUsuarioService>();

        var service = new UsuarioService(
            altaMock.Object,
            modificacionMock.Object,
            eliminacionMock.Object,
            consultaMock.Object
        );

        var request = new CrearUsuarioInternoRequest
        {
            Nombre = "Juan",
            Apellido = "Perez",
            Email = "admin@gmail.com",
            Telefono = "099123456",
            Clave = "PasswordSeguraee3!",
            Rol = "Administrativo"
        };

        service.CrearUsuarioInterno(1, request);

        altaMock.Verify(
            x => x.CrearUsuarioInterno(1, request),
            Times.Once
        );
    }

    [TestMethod]
    public void ModificarUsuarioInterno_LlamaAlServicioDeModificacionUnaVez()
    {
        var altaMock = new Mock<IAltaUsuarioService>();
        var modificacionMock = new Mock<IModificacionUsuarioService>();
        var eliminacionMock = new Mock<IEliminacionUsuarioService>();
        var consultaMock = new Mock<IConsultaUsuarioService>();

        var service = new UsuarioService(
            altaMock.Object,
            modificacionMock.Object,
            eliminacionMock.Object,
            consultaMock.Object
        );

        var request = new ModificarUsuarioRequest
        {
            Nombre = "Carlos"
        };

        service.ModificarUsuarioInterno(5, 99, request);

        modificacionMock.Verify(
            x => x.ModificarUsuarioInterno(5, 99, request),
            Times.Once
        );
    }

    [TestMethod]
    public void ModificarClaveUsuario_LlamaAlServicioDeModificacionUnaVez()
    {
        var altaMock = new Mock<IAltaUsuarioService>();
        var modificacionMock = new Mock<IModificacionUsuarioService>();
        var eliminacionMock = new Mock<IEliminacionUsuarioService>();
        var consultaMock = new Mock<IConsultaUsuarioService>();

        var service = new UsuarioService(
            altaMock.Object,
            modificacionMock.Object,
            eliminacionMock.Object,
            consultaMock.Object
        );

        var request = new ModificarClaveRequest
        {
            ClaveActual = "PasswordSeguraee3!",
            ClaveNueva = "ClaveNuevaSegura4!"
        };

        service.ModificarClaveUsuario(5, 99, request);

        modificacionMock.Verify(
            x => x.ModificarClaveUsuario(5, 99, request),
            Times.Once
        );
    }

    [TestMethod]
    public void EliminarUsuarioInterno_LlamaAlServicioDeEliminacionUnaVez()
    {
        var altaMock = new Mock<IAltaUsuarioService>();
        var modificacionMock = new Mock<IModificacionUsuarioService>();
        var eliminacionMock = new Mock<IEliminacionUsuarioService>();
        var consultaMock = new Mock<IConsultaUsuarioService>();

        var service = new UsuarioService(
            altaMock.Object,
            modificacionMock.Object,
            eliminacionMock.Object,
            consultaMock.Object
        );

        service.EliminarUsuarioInterno(5, 99);

        eliminacionMock.Verify(
            x => x.EliminarUsuarioInterno(5, 99),
            Times.Once
        );
    }

    [TestMethod]
    public void BuscarUsuarios_LlamaAlServicioDeConsultaUnaVez()
    {
        var altaMock = new Mock<IAltaUsuarioService>();
        var modificacionMock = new Mock<IModificacionUsuarioService>();
        var eliminacionMock = new Mock<IEliminacionUsuarioService>();
        var consultaMock = new Mock<IConsultaUsuarioService>();

        var service = new UsuarioService(
            altaMock.Object,
            modificacionMock.Object,
            eliminacionMock.Object,
            consultaMock.Object
        );

        var request = new BuscarUsuariosRequest
        {
            Nombre = "Juan",
            Apellido = "Perez"
        };

        service.BuscarUsuarios(99, request);

        consultaMock.Verify(
            x => x.BuscarUsuarios(99, request),
            Times.Once
        );
    }
}
