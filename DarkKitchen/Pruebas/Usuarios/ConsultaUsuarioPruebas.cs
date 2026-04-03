using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using Dominio.Excepciones;
using LogicaDeNegocio;
using Moq;

namespace Pruebas.Usuarios;

[TestClass]
public class ConsultaUsuarioPruebas
{
    [TestMethod]
    public void BuscarUsuariosPorNombre_NombreExistente_RetornaUsuariosCoincidentes()
    {
        var repoMock = new Mock<IRepositorioUsuario>();
        var usuario1 = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "juan1@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuario1.Id = 1;

        var usuario2 = Usuario.CrearInterno(
            "Juan",
            "Gomez",
            "juan2@gmail.com",
            Telefono.Crear("099111222"),
            "PasswordSeguraaa4!",
            RolUsuario.Preparador
        );
        usuario2.Id = 2;

        repoMock
            .Setup(r => r.GetAll())
            .Returns(new List<Usuario> { usuario1, usuario2 });
        var usuarioLogueado = Usuario.CrearInterno(
            "Admin",
            "Root",
            "root@gmail.com",
            Telefono.Crear("099999999"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        var service = new ConsultaUsuarioServicio(repoMock.Object);
        var request = new BuscarUsuariosRequest
        {
            Nombre = "Juan"
        };

        var resultado = service.BuscarUsuarios(99,request);
        Assert.AreEqual(2, resultado.Count);
        Assert.AreEqual("Juan", resultado[0].Nombre);
        Assert.AreEqual("Juan", resultado[1].Nombre);
    }

    [TestMethod]
    public void BuscarUsuariosPorNombre_NombreInexistente_RetornaListaVacia()
    {
        var repoMock = new Mock<IRepositorioUsuario>();

        var usuario = Usuario.CrearInterno(
            "Pedro",
            "Perez",
            "pedro@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuario.Id = 1;

        repoMock
            .Setup(r => r.GetAll())
            .Returns(new List<Usuario> { usuario });

        var service = new ConsultaUsuarioServicio(repoMock.Object);

        var request = new BuscarUsuariosRequest
        {
            Nombre = "Juan"
        };
        var usuarioLogueado = Usuario.CrearInterno(
            "Admin",
            "Root",
            "root@gmail.com",
            Telefono.Crear("099999999"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
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
        var repoMock = new Mock<IRepositorioUsuario>();

        var usuario1 = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "juan@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuario1.Id = 1;

        var usuario2 = Usuario.CrearInterno(
            "Pedro",
            "Gomez",
            "pedro@gmail.com",
            Telefono.Crear("099111222"),
            "PasswordSeguraaa4!",
            RolUsuario.Preparador
        );
        usuario2.Id = 2;

        repoMock
            .Setup(r => r.GetAll())
            .Returns(new List<Usuario> { usuario1, usuario2 });

        var service = new ConsultaUsuarioServicio(repoMock.Object);

        var request = new BuscarUsuariosRequest
        {
            Nombre = "Juan"
        };
        var usuarioLogueado = Usuario.CrearInterno(
            "Admin",
            "Root",
            "root@gmail.com",
            Telefono.Crear("099999999"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        var resultado = service.BuscarUsuarios(99,request);
        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual("Juan", resultado[0].Nombre);
        Assert.AreEqual("Perez", resultado[0].Apellido);
    }

    [TestMethod]
    public void BuscarUsuariosPorNombre_NombreConDistintoCase_RetornaCoincidencias()
    {
        var repoMock = new Mock<IRepositorioUsuario>();

        var usuario = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "juan@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuario.Id = 1;

        repoMock
            .Setup(r => r.GetAll())
            .Returns(new List<Usuario> { usuario });

        var service = new ConsultaUsuarioServicio(repoMock.Object);

        var request = new BuscarUsuariosRequest
        {
            Nombre = "juAn"
        };
        var usuarioLogueado = Usuario.CrearInterno(
            "Admin",
            "Root",
            "root@gmail.com",
            Telefono.Crear("099999999"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        var resultado = service.BuscarUsuarios(99,request);
        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual("Juan", resultado[0].Nombre);
    }

    [TestMethod]
    public void BuscarUsuariosPorNombre_SinUsuarios_RetornaListaVacia()
    {
        var repoMock = new Mock<IRepositorioUsuario>();

        repoMock
            .Setup(r => r.GetAll())
            .Returns(new List<Usuario>());

        var service = new ConsultaUsuarioServicio(repoMock.Object);

        var request = new BuscarUsuariosRequest
        {
            Nombre = "Juan"
        };
        var usuarioLogueado = Usuario.CrearInterno(
            "Admin",
            "Root",
            "root@gmail.com",
            Telefono.Crear("099999999"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
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
        var repoMock = new Mock<IRepositorioUsuario>();

        var usuario = Usuario.CrearCliente(
            "Juan",
            "Perez",
            "juan@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!"
        );
        usuario.Id = 100;

        repoMock
            .Setup(r => r.GetAll())
            .Returns(new List<Usuario> { usuario });

        var service = new ConsultaUsuarioServicio(repoMock.Object);

        var request = new BuscarUsuariosRequest
        {
            Nombre = "Juan"
        };
        var usuarioLogueado = Usuario.CrearInterno(
            "Admin",
            "Root",
            "root@gmail.com",
            Telefono.Crear("099999999"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        var resultado = service.BuscarUsuarios(99,request);
        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual(100, resultado[0].Id);
        Assert.AreEqual("Juan", resultado[0].Nombre);
        Assert.AreEqual("Perez", resultado[0].Apellido);
        Assert.AreEqual("juan@gmail.com", resultado[0].Email);
        Assert.AreEqual("Cliente", resultado[0].Rol);
        Assert.AreEqual("099123456", resultado[0].Telefono);
    }

    [TestMethod]
    public void BuscarUsuariosPorApellido_ApellidoExistente_RetornaUsuariosCoincidentes()
    {
        var repoMock = new Mock<IRepositorioUsuario>();

        var usuario1 = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "juan1@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuario1.Id = 1;

        var usuario2 = Usuario.CrearInterno(
            "Pedro",
            "Perez",
            "pedro@gmail.com",
            Telefono.Crear("099111222"),
            "PasswordSeguraaa4!",
            RolUsuario.Preparador
        );
        usuario2.Id = 2;

        repoMock
            .Setup(r => r.GetAll())
            .Returns(new List<Usuario> { usuario1, usuario2 });

        var service = new ConsultaUsuarioServicio(repoMock.Object);

        var request = new BuscarUsuariosRequest
        {
            Apellido = "Perez"
        };
        var usuarioLogueado = Usuario.CrearInterno(
            "Admin",
            "Root",
            "root@gmail.com",
            Telefono.Crear("099999999"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        var resultado = service.BuscarUsuarios(99,request);
        Assert.AreEqual(2, resultado.Count);
        Assert.AreEqual("Perez", resultado[0].Apellido);
        Assert.AreEqual("Perez", resultado[1].Apellido);
    }

    [TestMethod]
    public void BuscarUsuariosPorApellido_NoImportaMaysculasMinusculas_RetornaUsuariosCoincidentes()
    {
        var repoMock = new Mock<IRepositorioUsuario>();

        var usuario1 = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "juan1@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuario1.Id = 1;

        var usuario2 = Usuario.CrearInterno(
            "Pedro",
            "Perez",
            "pedro@gmail.com",
            Telefono.Crear("099111222"),
            "PasswordSeguraaa4!",
            RolUsuario.Preparador
        );
        usuario2.Id = 2;

        repoMock
            .Setup(r => r.GetAll())
            .Returns(new List<Usuario> { usuario1, usuario2 });

        var service = new ConsultaUsuarioServicio(repoMock.Object);

        var request = new BuscarUsuariosRequest
        {
            Apellido = "PeReZ"
        };
        var usuarioLogueado = Usuario.CrearInterno(
            "Admin",
            "Root",
            "root@gmail.com",
            Telefono.Crear("099999999"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        var resultado = service.BuscarUsuarios(99,request);
        Assert.AreEqual(2, resultado.Count);
        Assert.AreEqual("Perez", resultado[0].Apellido);
        Assert.AreEqual("Perez", resultado[1].Apellido);
    }

    [TestMethod]
    public void BuscarUsuariosPorApellido_ApellidoInexistente_RetornaListaVacia()
    {
        var repoMock = new Mock<IRepositorioUsuario>();

        var usuario = Usuario.CrearInterno(
            "Juan",
            "Gomez",
            "juan@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuario.Id = 1;

        repoMock
            .Setup(r => r.GetAll())
            .Returns(new List<Usuario> { usuario });

        var service = new ConsultaUsuarioServicio(repoMock.Object);

        var request = new BuscarUsuariosRequest
        {
            Apellido = "Perez"
        };
        var usuarioLogueado = Usuario.CrearInterno(
            "Admin",
            "Root",
            "root@gmail.com",
            Telefono.Crear("099999999"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
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
        var repoMock = new Mock<IRepositorioUsuario>();
        var usuario1 = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "juan@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuario1.Id = 1;

        var usuario2 = Usuario.CrearInterno(
            "Pedro",
            "Gomez",
            "pedro@gmail.com",
            Telefono.Crear("099111222"),
            "PasswordSeguraaa4!",
            RolUsuario.Preparador
        );
        usuario2.Id = 2;

        repoMock
            .Setup(r => r.GetAll())
            .Returns(new List<Usuario> { usuario1, usuario2 });

        var service = new ConsultaUsuarioServicio(repoMock.Object);

        var request = new BuscarUsuariosRequest
        {
            Apellido = "Perez"
        };
        var usuarioLogueado = Usuario.CrearInterno(
            "Admin",
            "Root",
            "root@gmail.com",
            Telefono.Crear("099999999"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        var resultado = service.BuscarUsuarios(99,request);
        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual("Juan", resultado[0].Nombre);
        Assert.AreEqual("Perez", resultado[0].Apellido);
        Assert.AreEqual("099123456", resultado[0].Telefono);
        Assert.AreEqual("juan@gmail.com", resultado[0].Email);
        Assert.AreEqual("Administrativo", resultado[0].Rol);
    }

    [TestMethod]
    public void BuscarUsuariosPorApellido_SinUsuarios_RetornaListaVacia()
    {
        var repoMock = new Mock<IRepositorioUsuario>();

        repoMock
            .Setup(r => r.GetAll())
            .Returns(new List<Usuario>());

        var service = new ConsultaUsuarioServicio(repoMock.Object);

        var request = new BuscarUsuariosRequest
        {
            Apellido = "Perez"
        };
        var usuarioLogueado = Usuario.CrearInterno(
            "Admin",
            "Root",
            "root@gmail.com",
            Telefono.Crear("099999999"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
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
        var repoMock = new Mock<IRepositorioUsuario>();

        var usuario = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "juan@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuario.Id = 1;

        repoMock
            .Setup(r => r.GetAll())
            .Returns(new List<Usuario> { usuario });

        var usuarioLogueado = Usuario.CrearInterno(
            "Admin",
            "Root",
            "root@gmail.com",
            Telefono.Crear("099999999"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        var service = new ConsultaUsuarioServicio(repoMock.Object);

        var request = new BuscarUsuariosRequest
        {
            Apellido = "  Perez   "
        };

        var resultado = service.BuscarUsuarios(99, request);
        Assert.AreEqual(1, resultado.Count);
    }

    [TestMethod]
    public void BuscarUsuariosPorNombre_ConEspacios()
    {
        var repoMock = new Mock<IRepositorioUsuario>();

        var usuario = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "juan@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuario.Id = 1;

        var usuarioLogueado = Usuario.CrearInterno(
            "Admin",
            "Root",
            "root@gmail.com",
            Telefono.Crear("099999999"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        repoMock
            .Setup(r => r.GetAll())
            .Returns(new List<Usuario> { usuario });

        var service = new ConsultaUsuarioServicio(repoMock.Object);

        var request = new BuscarUsuariosRequest
        {
            Nombre = "   Juan  "
        };

        var resultado = service.BuscarUsuarios(99, request);

        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual("Juan", resultado[0].Nombre);
    }

    [TestMethod]
    public void BuscarUsuarios_RequestConEspaciosYDistintoCase_RetornaCoincidencias()
    {
        var repoMock = new Mock<IRepositorioUsuario>();

        var usuario = Usuario.CrearInterno(
            "Juan",
            "Perez",
            "juan@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuario.Id = 1;

        var usuarioLogueado = Usuario.CrearInterno(
            "Admin",
            "Root",
            "root@gmail.com",
            Telefono.Crear("099999999"),
            "PasswordSeguraee3!",
            RolUsuario.Administrativo
        );
        usuarioLogueado.Id = 99;

        repoMock
            .Setup(r => r.FindById(99))
            .Returns(usuarioLogueado);

        repoMock
            .Setup(r => r.GetAll())
            .Returns(new List<Usuario> { usuario });

        var service = new ConsultaUsuarioServicio(repoMock.Object);

        var request = new BuscarUsuariosRequest
        {
            Nombre = " juan ",
            Apellido = " pErEz "
        };

        var resultado = service.BuscarUsuarios(99,request);

        Assert.AreEqual(1, resultado.Count);
        Assert.AreEqual("Juan", resultado[0].Nombre);
        Assert.AreEqual("Perez", resultado[0].Apellido);
    }

    [TestMethod]
    public void BuscarUsuarios_UsuarioLogueadoNoEsAdministrativo_LanzaExcepcionConMensajeCorrecto()
    {
        var repoMock = new Mock<IRepositorioUsuario>();

        var usuarioLogueado = Usuario.CrearCliente(
            "Juan",
            "Perez",
            "cliente@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!"
        );
        usuarioLogueado.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioLogueado);

        var service = new ConsultaUsuarioServicio(repoMock.Object);

        var request = new BuscarUsuariosRequest
        {
            Nombre = "Juan"
        };

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.BuscarUsuarios(5, request)
        );

        Assert.AreEqual("Solo un usuario administrativo puede realizar esta acción", ex.Message);
    }

    [TestMethod]
    public void BuscarUsuarios_UsuarioLogueadoNoEsAdministrativo_NoLlamaGetAllYLanzaExcepcionConMensajeCorrecto()
    {
        var repoMock = new Mock<IRepositorioUsuario>();

        var usuarioLogueado = Usuario.CrearCliente(
            "Juan",
            "Perez",
            "cliente@gmail.com",
            Telefono.Crear("099123456"),
            "PasswordSeguraee3!"
        );
        usuarioLogueado.Id = 5;

        repoMock
            .Setup(r => r.FindById(5))
            .Returns(usuarioLogueado);

        var service = new ConsultaUsuarioServicio(repoMock.Object);

        var request = new BuscarUsuariosRequest
        {
            Nombre = "Juan"
        };

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.BuscarUsuarios(5, request)
        );

        Assert.AreEqual("Solo un usuario administrativo puede realizar esta acción", ex.Message);

        repoMock.Verify(r => r.GetAll(), Times.Never);
    }
}
