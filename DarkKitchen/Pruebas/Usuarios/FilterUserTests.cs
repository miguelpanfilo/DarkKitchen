using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using Dominio.Excepciones;
using LogicaDeNegocio;
using Moq;

namespace Pruebas.Usuarios;

[TestClass]
public class FilterUserTests
{
    [TestMethod]
    public void BuscarUsuariosPorNombre_NombreExistente_RetornaUsuariosCoincidentes()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var usuario1 = User.MakeIntern(
            "Juan",
            "Perez",
            "juan1@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        usuario1.Id = 1;

        var usuario2 = User.MakeIntern(
            "Juan",
            "Gomez",
            "juan2@gmail.com",
            Phone.MakePhone("099111222"),
            "PasswordSeguraaa4!",
            UserRoles.Preparer
        );
        usuario2.Id = 2;

        repoMock
            .Setup(r => r.GetAll())
            .Returns(new List<User> { usuario1, usuario2 });
        var usuarioLogueado = User.MakeIntern(
            "Admin",
            "Root",
            "root@gmail.com",
            Phone.MakePhone("099999999"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        var service = new FilterUserService(repoMock.Object);
        var request = new FindUserRequestDTO
        {
            Name = "Juan"
        };

        var resultado = service.BuscarUsuarios(99,request);
        Assert.AreEqual(2, resultado.Count);
        Assert.AreEqual("Juan", resultado[0].Name);
        Assert.AreEqual("Juan", resultado[1].Name);
    }

    [TestMethod]
    public void BuscarUsuariosPorNombre_NombreInexistente_RetornaListaVacia()
    {
        var repoMock = new Mock<IRepositoryUser>();

        var usuario = User.MakeIntern(
            "Pedro",
            "Perez",
            "pedro@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        usuario.Id = 1;

        repoMock
            .Setup(r => r.GetAll())
            .Returns(new List<User> { usuario });

        var service = new FilterUserService(repoMock.Object);

        var request = new FindUserRequestDTO
        {
            Name = "Juan"
        };
        var usuarioLogueado = User.MakeIntern(
            "Admin",
            "Root",
            "root@gmail.com",
            Phone.MakePhone("099999999"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        var resultado = service.BuscarUsuarios(99,request);
        Assert.AreEqual(0, resultado.Count);
    }

    [TestMethod]
    public void BuscarUsuariosPorNombre_FiltraSoloLosUsuariosConEseNombre()
    {
        var repoMock = new Mock<IRepositoryUser>();

        var usuario1 = User.MakeIntern(
            "Juan",
            "Perez",
            "juan@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        usuario1.Id = 1;

        var usuario2 = User.MakeIntern(
            "Pedro",
            "Gomez",
            "pedro@gmail.com",
            Phone.MakePhone("099111222"),
            "PasswordSeguraaa4!",
            UserRoles.Preparer
        );
        usuario2.Id = 2;

        repoMock
            .Setup(r => r.GetAll())
            .Returns(new List<User> { usuario1, usuario2 });

        var service = new FilterUserService(repoMock.Object);

        var request = new FindUserRequestDTO
        {
            Name = "Juan"
        };
        var usuarioLogueado = User.MakeIntern(
            "Admin",
            "Root",
            "root@gmail.com",
            Phone.MakePhone("099999999"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        var resultado = service.BuscarUsuarios(99,request);
        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual("Juan", resultado[0].Name);
        Assert.AreEqual("Perez", resultado[0].LastName);
    }

    [TestMethod]
    public void BuscarUsuariosPorNombre_NombreConDistintoCase_RetornaCoincidencias()
    {
        var repoMock = new Mock<IRepositoryUser>();

        var usuario = User.MakeIntern(
            "Juan",
            "Perez",
            "juan@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        usuario.Id = 1;

        repoMock
            .Setup(r => r.GetAll())
            .Returns(new List<User> { usuario });

        var service = new FilterUserService(repoMock.Object);

        var request = new FindUserRequestDTO
        {
            Name = "juAn"
        };
        var usuarioLogueado = User.MakeIntern(
            "Admin",
            "Root",
            "root@gmail.com",
            Phone.MakePhone("099999999"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        var resultado = service.BuscarUsuarios(99,request);
        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual("Juan", resultado[0].Name);
    }

    [TestMethod]
    public void BuscarUsuariosPorNombre_SinUsuarios_RetornaListaVacia()
    {
        var repoMock = new Mock<IRepositoryUser>();

        repoMock
            .Setup(r => r.GetAll())
            .Returns(new List<User>());

        var service = new FilterUserService(repoMock.Object);

        var request = new FindUserRequestDTO
        {
            Name = "Juan"
        };
        var usuarioLogueado = User.MakeIntern(
            "Admin",
            "Root",
            "root@gmail.com",
            Phone.MakePhone("099999999"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        var resultado = service.BuscarUsuarios(99,request);
        Assert.AreEqual(0, resultado.Count);
    }

    [TestMethod]
    public void BuscarUsuariosPorNombre_MapeaCorrectamenteLosDatosDelUsuario()
    {
        var repoMock = new Mock<IRepositoryUser>();

        var usuario = User.MakeClient(
            "Juan",
            "Perez",
            "juan@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!"
        );
        usuario.Id = 100;

        repoMock
            .Setup(r => r.GetAll())
            .Returns(new List<User> { usuario });

        var service = new FilterUserService(repoMock.Object);

        var request = new FindUserRequestDTO
        {
            Name = "Juan"
        };
        var usuarioLogueado = User.MakeIntern(
            "Admin",
            "Root",
            "root@gmail.com",
            Phone.MakePhone("099999999"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        var resultado = service.BuscarUsuarios(99,request);
        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual(100, resultado[0].Id);
        Assert.AreEqual("Juan", resultado[0].Name);
        Assert.AreEqual("Perez", resultado[0].LastName);
        Assert.AreEqual("juan@gmail.com", resultado[0].Email);
        Assert.AreEqual("Client", resultado[0].Role);
        Assert.AreEqual("099123456", resultado[0].Phone);
    }

    [TestMethod]
    public void BuscarUsuariosPorApellido_ApellidoExistente_RetornaUsuariosCoincidentes()
    {
        var repoMock = new Mock<IRepositoryUser>();

        var usuario1 = User.MakeIntern(
            "Juan",
            "Perez",
            "juan1@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        usuario1.Id = 1;

        var usuario2 = User.MakeIntern(
            "Pedro",
            "Perez",
            "pedro@gmail.com",
            Phone.MakePhone("099111222"),
            "PasswordSeguraaa4!",
            UserRoles.Preparer
        );
        usuario2.Id = 2;

        repoMock
            .Setup(r => r.GetAll())
            .Returns(new List<User> { usuario1, usuario2 });

        var service = new FilterUserService(repoMock.Object);

        var request = new FindUserRequestDTO
        {
            LastName = "Perez"
        };
        var usuarioLogueado = User.MakeIntern(
            "Admin",
            "Root",
            "root@gmail.com",
            Phone.MakePhone("099999999"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        var resultado = service.BuscarUsuarios(99,request);
        Assert.AreEqual(2, resultado.Count);
        Assert.AreEqual("Perez", resultado[0].LastName);
        Assert.AreEqual("Perez", resultado[1].LastName);
    }

    [TestMethod]
    public void BuscarUsuariosPorApellido_NoImportaMaysculasMinusculas_RetornaUsuariosCoincidentes()
    {
        var repoMock = new Mock<IRepositoryUser>();

        var usuario1 = User.MakeIntern(
            "Juan",
            "Perez",
            "juan1@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        usuario1.Id = 1;

        var usuario2 = User.MakeIntern(
            "Pedro",
            "Perez",
            "pedro@gmail.com",
            Phone.MakePhone("099111222"),
            "PasswordSeguraaa4!",
            UserRoles.Preparer
        );
        usuario2.Id = 2;

        repoMock
            .Setup(r => r.GetAll())
            .Returns(new List<User> { usuario1, usuario2 });

        var service = new FilterUserService(repoMock.Object);

        var request = new FindUserRequestDTO
        {
            LastName = "PeReZ"
        };
        var usuarioLogueado = User.MakeIntern(
            "Admin",
            "Root",
            "root@gmail.com",
            Phone.MakePhone("099999999"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        var resultado = service.BuscarUsuarios(99,request);
        Assert.AreEqual(2, resultado.Count);
        Assert.AreEqual("Perez", resultado[0].LastName);
        Assert.AreEqual("Perez", resultado[1].LastName);
    }

    [TestMethod]
    public void BuscarUsuariosPorApellido_ApellidoInexistente_RetornaListaVacia()
    {
        var repoMock = new Mock<IRepositoryUser>();

        var usuario = User.MakeIntern(
            "Juan",
            "Gomez",
            "juan@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        usuario.Id = 1;

        repoMock
            .Setup(r => r.GetAll())
            .Returns(new List<User> { usuario });

        var service = new FilterUserService(repoMock.Object);

        var request = new FindUserRequestDTO
        {
            LastName = "Perez"
        };
        var usuarioLogueado = User.MakeIntern(
            "Admin",
            "Root",
            "root@gmail.com",
            Phone.MakePhone("099999999"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        var resultado = service.BuscarUsuarios(99, request);
        Assert.AreEqual(0, resultado.Count);
    }

    [TestMethod]
    public void BuscarUsuariosPorApellido_FiltraSoloLosUsuariosConEseApellido()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var usuario1 = User.MakeIntern(
            "Juan",
            "Perez",
            "juan@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        usuario1.Id = 1;

        var usuario2 = User.MakeIntern(
            "Pedro",
            "Gomez",
            "pedro@gmail.com",
            Phone.MakePhone("099111222"),
            "PasswordSeguraaa4!",
            UserRoles.Preparer
        );
        usuario2.Id = 2;

        repoMock
            .Setup(r => r.GetAll())
            .Returns(new List<User> { usuario1, usuario2 });

        var service = new FilterUserService(repoMock.Object);

        var request = new FindUserRequestDTO
        {
            LastName = "Perez"
        };
        var usuarioLogueado = User.MakeIntern(
            "Admin",
            "Root",
            "root@gmail.com",
            Phone.MakePhone("099999999"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        var resultado = service.BuscarUsuarios(99,request);
        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual("Juan", resultado[0].Name);
        Assert.AreEqual("Perez", resultado[0].LastName);
        Assert.AreEqual("099123456", resultado[0].Phone);
        Assert.AreEqual("juan@gmail.com", resultado[0].Email);
        Assert.AreEqual("Administrative", resultado[0].Role);
    }

    [TestMethod]
    public void BuscarUsuariosPorApellido_SinUsuarios_RetornaListaVacia()
    {
        var repoMock = new Mock<IRepositoryUser>();

        repoMock
            .Setup(r => r.GetAll())
            .Returns(new List<User>());

        var service = new FilterUserService(repoMock.Object);

        var request = new FindUserRequestDTO
        {
            LastName = "Perez"
        };
        var usuarioLogueado = User.MakeIntern(
            "Admin",
            "Root",
            "root@gmail.com",
            Phone.MakePhone("099999999"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        var resultado = service.BuscarUsuarios(99,request);
        Assert.AreEqual(0, resultado.Count);
    }

    [TestMethod]
    public void BuscarUsuariosPorApellido_ConEspacios()
    {
        var repoMock = new Mock<IRepositoryUser>();

        var usuario = User.MakeIntern(
            "Juan",
            "Perez",
            "juan@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        usuario.Id = 1;

        repoMock
            .Setup(r => r.GetAll())
            .Returns(new List<User> { usuario });

        var usuarioLogueado = User.MakeIntern(
            "Admin",
            "Root",
            "root@gmail.com",
            Phone.MakePhone("099999999"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        var service = new FilterUserService(repoMock.Object);

        var request = new FindUserRequestDTO
        {
            LastName = "  Perez   "
        };

        var resultado = service.BuscarUsuarios(99, request);
        Assert.AreEqual(1, resultado.Count);
    }

    [TestMethod]
    public void BuscarUsuariosPorNombre_ConEspacios()
    {
        var repoMock = new Mock<IRepositoryUser>();

        var usuario = User.MakeIntern(
            "Juan",
            "Perez",
            "juan@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        usuario.Id = 1;

        var usuarioLogueado = User.MakeIntern(
            "Admin",
            "Root",
            "root@gmail.com",
            Phone.MakePhone("099999999"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        repoMock
            .Setup(r => r.GetAll())
            .Returns(new List<User> { usuario });

        var service = new FilterUserService(repoMock.Object);

        var request = new FindUserRequestDTO
        {
            Name = "   Juan  "
        };

        var resultado = service.BuscarUsuarios(99, request);

        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual("Juan", resultado[0].Name);
    }

    [TestMethod]
    public void BuscarUsuarios_RequestConEspaciosYDistintoCase_RetornaCoincidencias()
    {
        var repoMock = new Mock<IRepositoryUser>();

        var usuario = User.MakeIntern(
            "Juan",
            "Perez",
            "juan@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        usuario.Id = 1;

        var usuarioLogueado = User.MakeIntern(
            "Admin",
            "Root",
            "root@gmail.com",
            Phone.MakePhone("099999999"),
            "PasswordSeguraee3!",
            UserRoles.Administrative
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        repoMock
            .Setup(r => r.GetAll())
            .Returns(new List<User> { usuario });

        var service = new FilterUserService(repoMock.Object);

        var request = new FindUserRequestDTO
        {
            Name = " juan ",
            LastName = " pErEz "
        };

        var resultado = service.BuscarUsuarios(99,request);

        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual("Juan", resultado[0].Name);
        Assert.AreEqual("Perez", resultado[0].LastName);
    }

    [TestMethod]
    public void BuscarUsuarios_UsuarioLogueadoNoEsAdministrativo_LanzaExcepcionConMensajeCorrecto()
    {
        var repoMock = new Mock<IRepositoryUser>();

        var usuarioLogueado = User.MakeClient(
            "Juan",
            "Perez",
            "cliente@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!"
        );
        usuarioLogueado.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioLogueado);

        var service = new FilterUserService(repoMock.Object);

        var request = new FindUserRequestDTO
        {
            Name = "Juan"
        };

        var ex = Assert.ThrowsException<UserException>(() =>
            service.BuscarUsuarios(5, request)
        );

        Assert.AreEqual("Only an administrative user can perform this action", ex.Message);
    }

    [TestMethod]
    public void BuscarUsuarios_UsuarioLogueadoNoEsAdministrativo_NoLlamaGetAllYLanzaExcepcionConMensajeCorrecto()
    {
        var repoMock = new Mock<IRepositoryUser>();

        var usuarioLogueado = User.MakeClient(
            "Juan",
            "Perez",
            "cliente@gmail.com",
            Phone.MakePhone("099123456"),
            "PasswordSeguraee3!"
        );
        usuarioLogueado.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioLogueado);

        var service = new FilterUserService(repoMock.Object);

        var request = new FindUserRequestDTO
        {
            Name = "Juan"
        };

        var ex = Assert.ThrowsException<UserException>(() =>
            service.BuscarUsuarios(5, request)
        );

        Assert.AreEqual("Only an administrative user can perform this action", ex.Message);

        repoMock.Verify(r => r.GetAll(), Times.Never);
    }
}
