using Almacenamiento;
using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using Dominio.Excepciones;
using LogicaDeNegocio;
using LogicaDeNegocioAbstracciones;
using Moq;

namespace Pruebas.Usuarios;

[TestClass]
public class UserServiceTests
{
   [TestMethod]
    public void SignUpClient_ValidData_ReturnsResponseDto()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var validatorPhoneMock = new Mock<IPhoneValidator>();

        repoMock.Setup(r => r.EmailExist(It.IsAny<string>()))
            .Returns(false);

        validatorPhoneMock
            .Setup(v => v.ItsValid(It.IsAny<string>()))
            .Returns(true);

        repoMock.Setup(r => r.Add(It.IsAny<User>()))
            .Returns((User u) => u);

        var service = new SignUpService(repoMock.Object, validatorPhoneMock.Object);

        var request = new SignUpClientRequestDTO
        {
            Name = "Miguel",
            LastName = "Lopez",
            Email = "miguel@gmail.com",
            Phone = "099123456",
            Password = "Password@2Ok6Abc"
        };

        var result = service.SignUpClient(request);

        Assert.IsNotNull(result);
        Assert.AreEqual("Miguel", result.Name);
        Assert.AreEqual("Lopez", result.LastName);
        Assert.AreEqual("miguel@gmail.com", result.Email);

        repoMock.Verify(r => r.EmailExist("miguel@gmail.com"), Times.Once);
        validatorPhoneMock.Verify(v => v.ItsValid("099123456"), Times.Once);
        repoMock.Verify(r => r.Add(It.IsAny<User>()), Times.Once);
    }

    [TestMethod]
    public void SignUpClient_DuplicateEmail_ThrowsException_AndDoesNotValidatePhoneOrAdd()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var validatorPhoneMock = new Mock<IPhoneValidator>();

        repoMock.Setup(r => r.EmailExist("miguel@gmail.com"))
            .Returns(true);

        var service = new SignUpService(repoMock.Object, validatorPhoneMock.Object);

        var request = new SignUpClientRequestDTO
        {
            Name = "Miguel",
            LastName = "Lopez",
            Email = "miguel@gmail.com",
            Phone = "099123456",
            Password = "Password@2Ok6Abc"
        };

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.SignUpClient(request)
        );

        Assert.AreEqual("Email is already registered", ex.Message);

        validatorPhoneMock.Verify(v => v.ItsValid(It.IsAny<string>()), Times.Never);
        repoMock.Verify(r => r.Add(It.IsAny<User>()), Times.Never);
    }

    [TestMethod]
    public void SignUpClient_InvalidPhoneAccordingToValidator_ThrowsException_AndDoesNotAdd()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var validatorPhoneMock = new Mock<IPhoneValidator>();

        repoMock.Setup(r => r.EmailExist(It.IsAny<string>()))
            .Returns(false);

        validatorPhoneMock
            .Setup(v => v.ItsValid("123"))
            .Returns(false);

        var service = new SignUpService(repoMock.Object, validatorPhoneMock.Object);

        var request = new SignUpClientRequestDTO
        {
            Name = "Miguel",
            LastName = "Lopez",
            Email = "miguel@gmail.com",
            Phone = "123",
            Password = "Password@2026Ok"
        };

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.SignUpClient(request)
        );

        Assert.AreEqual("Phone is not valid", ex.Message);

        validatorPhoneMock.Verify(v => v.ItsValid("123"), Times.Once);
        repoMock.Verify(r => r.Add(It.IsAny<User>()), Times.Never);
    }

    [TestMethod]
    public void SignUpClient_EmptyPhone_ThrowsException_BeforeCallingValidator()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var validatorPhoneMock = new Mock<IPhoneValidator>();

        repoMock.Setup(r => r.EmailExist(It.IsAny<string>()))
            .Returns(false);

        var service = new SignUpService(repoMock.Object, validatorPhoneMock.Object);

        var request = new SignUpClientRequestDTO
        {
            Name = "Miguel",
            LastName = "Lopez",
            Email = "miguel@gmail.com",
            Phone = string.Empty,
            Password = "Password@2026Ok"
        };

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            service.SignUpClient(request)
        );

        Assert.AreEqual("Phone cannot be empty", ex.Message);

        validatorPhoneMock.Verify(v => v.ItsValid(It.IsAny<string>()), Times.Never);
        repoMock.Verify(r => r.Add(It.IsAny<User>()), Times.Never);
    }

    [TestMethod]
    public void SignUpClient_ValidUser_Saved()
    {
        var repoUSer = new RepositoryUser();

        var user = User.MakeClient(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            Phone.MakePhone("099123456"),
            "Password@2Ok6Abc");

        repoUSer.Add(user);

        var userFromRepo = repoUSer.GetAll();

        Assert.AreEqual(1, userFromRepo.Count);
        Assert.AreEqual("Miguel", userFromRepo[0].Name);
        Assert.AreEqual("Lopez", userFromRepo[0].LastName);
    }

    [TestMethod]
    [ExpectedException(typeof(UsuarioExcepcion))]
    public void SignUpCliente_NoName_Exception()
    {
        var user = User.MakeClient(
            string.Empty,
            "Lopez",
            "miguel@gmail.com",
            Phone.MakePhone("099123456"),
            "Password@2026Ok");
    }

    [TestMethod]
    public void SignUpCliente_MinNameValid()
    {
        var usuario = User.MakeClient(
            "M",
            "Lopez",
            "miguel@gmail.com",
            Phone.MakePhone("099123456"),
            "Password@2Ok6Abc"
        );

        Assert.IsNotNull(usuario);
    }

    [TestMethod]
    [ExpectedException(typeof(UsuarioExcepcion))]
    public void SignUpCliente_NullName_ReturnException()
    {
        User.MakeClient(
            null!,
            "Lopez",
            "miguel@gmail.com",
            Phone.MakePhone("099123456"),
            "Password@2Ok6Abc"
        );
    }

    [TestMethod]
    [ExpectedException(typeof(UsuarioExcepcion))]
    public void SignClient_WithOutLastname_Exception()
    {
        User.MakeClient(
            "Miguel",
            string.Empty,
            "miguel@gmail.com",
            Phone.MakePhone("099123456"),
            "Password@2026Ok");
    }

    [TestMethod]
    [ExpectedException(typeof(UsuarioExcepcion))]
    public void SignupCliente_WithMoreOf25Chars()
    {
        var lastnameLenghtIncorrect = 26;
        var notValidLastname = new string('a', lastnameLenghtIncorrect);

        var user = User.MakeClient(
            "Miguel",
            notValidLastname,
            "miguel@gmail.com",
            Phone.MakePhone("099123456"),
            "Password@2026Ok");
    }

    [TestMethod]
    [ExpectedException(typeof(UsuarioExcepcion))]
    public void SignupClient_WithLessOf3Chars()
    {
        var lastnameLenghtIncorrect = 2;
        var smallLastname = new string('a', lastnameLenghtIncorrect);

        var user = User.MakeClient(
            "Miguel",
            smallLastname,
            "miguel@gmail.com",
            Phone.MakePhone("099123456"),
            "Password@2026Ok");
    }

    [TestMethod]
    public void SignUp_Client_MaxLastnameValid()
    {
        var lastname = new string('a', 25);

        var user = User.MakeClient(
            "Miguel",
            lastname,
            "miguel@gmail.com",
            Phone.MakePhone("099123456"),
            "Password@2Ok6Abc"
        );

        Assert.IsNotNull(user);
    }

    [TestMethod]
    public void SignUpCliente_LastnameWithMinimunValid()
    {
        var user = User.MakeClient(
            "Miguel",
            "Lop",
            "miguel@gmail.com",
            Phone.MakePhone("099123456"),
            "Password@2Ok6Abc"
        );

        Assert.IsNotNull(user);
    }

    [TestMethod]
    public void Properties_SetValues_CorrectlyAssigned()
    {
        var dto = new SignUpClientResponseDTO
        {
            Name = "Miguel",
            LastName = "Lopez",
            Email = "miguel@gmail.com"
        };

        Assert.AreEqual("Miguel", dto.Name);
        Assert.AreEqual("Lopez", dto.LastName);
        Assert.AreEqual("miguel@gmail.com", dto.Email);
    }

    [TestMethod]
    public void RegisterClient_ValidUruguayanPhone_SuccessfulRegistration()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var validatorPhoneMock = new Mock<IPhoneValidator>();

        validatorPhoneMock
            .Setup(v => v.ItsValid(It.IsAny<string>()))
            .Returns(true);

        repoMock.Setup(r => r.Add(It.IsAny<User>()))
            .Returns((User u) => u);

        var signUpService = new SignUpService(repoMock.Object, validatorPhoneMock.Object);

        var signUpRequest = new SignUpClientRequestDTO
        {
            Name = "Miguel",
            LastName = "Lopez",
            Email = "miguel@gmail.com",
            Phone = "099123456",
            Password = "Password@2Ok6Abc"
        };

        var clientResult = signUpService.SignUpClient(signUpRequest);

        Assert.IsNotNull(clientResult);
        Assert.AreEqual("Miguel", clientResult.Name);
    }

    [TestMethod]
    public void EmailExists_EmailAlreadyExists_ReturnsTrue()
    {
        var repo = new RepositoryUser();

        var user = User.MakeClient(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            Phone.MakePhone("099123456"),
            "PPassword@2Ok6Abc"
        );

        repo.Add(user);

        var exist = repo.EmailExist("MIGUEL@gmail.com");

        Assert.IsTrue(exist);
    }

    [TestMethod]
    public void Add_NewUser_AssignsIncrementalId()
    {
        var repo = new RepositoryUser();

        var user1 = User.MakeClient(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            Phone.MakePhone("099123456"),
            "Password@2Ok6Abc"
        );

        var user2 = User.MakeClient(
            "Juan",
            "Perez",
            "juan@gmail.com",
            Phone.MakePhone("099999999"),
            "Password@2Ok6Abc"
        );

        var userAddedFirst = repo.Add(user1);
        var userAddedSecond = repo.Add(user2);

        var firstAddInTheRow = 1;
        var secondAddInTheRow = 2;

        Assert.AreEqual(firstAddInTheRow, userAddedFirst.Id);
        Assert.AreEqual(secondAddInTheRow, userAddedSecond.Id);
    }

    [TestMethod]
    public void Add_User_AssignsIdGreaterThanZero()
    {
        var repo = new RepositoryUser();

        var user = User.MakeClient(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            Phone.MakePhone("099123456"),
            "Password@2Ok6Abc"
        );

        var userNuevo = repo.Add(user);

        Assert.IsTrue(userNuevo.Id > 0);
    }

    [TestMethod]
    public void CreateUser_EmailIsNormalized_Correctly()
    {
        var user = User.MakeClient(
            "Miguel",
            "Lopez",
            "  MIGUEL@gmail.com  ",
            Phone.MakePhone("099123456"),
            "Password@2Ok6Abc"
        );

        Assert.AreEqual("miguel@gmail.com", user.Email.ToString());
    }

    [TestMethod]
    public void RegisterClient_EmailNotDuplicated_SuccessfulRegistration()
    {
        var repoMock = new Mock<IRepositoryUser>();
        var validatorPhoneMock = new Mock<IPhoneValidator>();

        repoMock.Setup(r => r.EmailExist(It.IsAny<string>()))
            .Returns(false);

        repoMock.Setup(r => r.Add(It.IsAny<User>()))
            .Returns((User u) => u);

        validatorPhoneMock
            .Setup(v => v.ItsValid(It.IsAny<string>()))
            .Returns(true);

        var singUpService = new SignUpService(repoMock.Object, validatorPhoneMock.Object);

        var signUpRequest = new SignUpClientRequestDTO
        {
            Name = "Miguel",
            LastName = "Lopez",
            Email = "miguel@gmail.com",
            Phone = "099123456",
            Password = "Password@2Ok6Abc"
        };

        var signUpDTO = singUpService.SignUpClient(signUpRequest);

        Assert.IsNotNull(signUpDTO);
        Assert.AreEqual("miguel@gmail.com", signUpDTO.Email);
    }

    [TestMethod]
    public void CreateUser_EmptyPassword_ThrowsException_WithCorrectMessage()
    {
        var password = string.Empty;

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            User.MakeClient(
                "Miguel",
                "Lopez",
                "miguel@gmail.com",
                Phone.MakePhone("099123456"),
                password
            )
        );

        Assert.AreEqual("Password format is not valid", ex.Message);
    }

    [TestMethod]
    public void CreateUser_PasswordShorterThan15_ThrowsException_WithCorrectMessage()
    {
        var lengthLessThanMin = 14;
        var passwordWithLengthLessThanMin = new string('a', lengthLessThanMin);

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            User.MakeClient(
                "Miguel",
                "Lopez",
                "miguel@gmail.com",
                Phone.MakePhone("099123456"),
                passwordWithLengthLessThanMin
            )
        );

        Assert.AreEqual("Password format is not valid", ex.Message);
    }

    [TestMethod]
    public void CreateUser_PasswordLongerThan25_ThrowsException_WithCorrectMessage()
    {
        var lengthGreaterThanLimit = 26;
        var passwordGreaterThanLimit = new string('a', lengthGreaterThanLimit);

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            User.MakeClient(
                "Miguel",
                "Lopez",
                "miguel@gmail.com",
                Phone.MakePhone("099123456"),
                passwordGreaterThanLimit
            )
        );

        Assert.AreEqual("Password format is not valid", ex.Message);
    }

    [TestMethod]
    public void CreateUser_PasswordWithoutUppercase_ThrowsException_WithCorrectMessage()
    {
        var password = "password!abc9xy";

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            User.MakeClient(
                "Miguel",
                "Lopez",
                "miguel@gmail.com",
                Phone.MakePhone("099123456"),
                password
            )
        );

        Assert.AreEqual("Password format is not valid", ex.Message);
    }

    [TestMethod]
    public void CreateUser_PasswordWithoutLowercase_ThrowsException_WithCorrectMessage()
    {
        var password = "PASSWORD!ABC9XY";

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            User.MakeClient(
                "Miguel",
                "Lopez",
                "miguel@gmail.com",
                Phone.MakePhone("099123456"),
                password
            )
        );

        Assert.AreEqual("Password format is not valid", ex.Message);
    }

    [TestMethod]
    public void CreateUser_PasswordWithoutNumber_ThrowsException_WithCorrectMessage()
    {
        var password = "Password!AbcXyz";

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            User.MakeClient(
                "Miguel",
                "Lopez",
                "miguel@gmail.com",
                Phone.MakePhone("099123456"),
                password
            )
        );

        Assert.AreEqual("Password format is not valid", ex.Message);
    }

    [TestMethod]
    public void CreateUser_PasswordWithoutSymbol_ThrowsException_WithCorrectMessage()
    {
        var password = "PasswordAbc123Xy";

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            User.MakeClient(
                "Miguel",
                "Lopez",
                "miguel@gmail.com",
                Phone.MakePhone("099123456"),
                password
            )
        );

        Assert.AreEqual("Password format is not valid", ex.Message);
    }

    [TestMethod]
    public void CreateUser_PasswordWithNumberSequence_ThrowsException_WithCorrectMessage()
    {
        var password = "Password!Ab12Xy";

        var ex = Assert.ThrowsException<UsuarioExcepcion>(() =>
            User.MakeClient(
                "Miguel",
                "Lopez",
                "miguel@gmail.com",
                Phone.MakePhone("099123456"),
                password
            )
        );

        Assert.AreEqual("Password format is not valid", ex.Message);
    }

    [TestMethod]
    public void CreateUser_PasswordWith15Characters_IsValid_DoesNotThrowException()
    {
        var user = User.MakeClient(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            Phone.MakePhone("099123456"),
            "Password!Abc9Xy"
        );

        Assert.IsNotNull(user);
    }

    [TestMethod]
    public void CreateUser_PasswordWith25Characters_IsValid_DoesNotThrowException()
    {
        var user = User.MakeClient(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            Phone.MakePhone("099123456"),
            "Password!Abc9XyzKlmNoPq"
        );

        Assert.IsNotNull(user);
    }

    [TestMethod]
    public void CreateClient_ValidData_AssignsClientRole()
    {
        var user = User.MakeClient(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            Phone.MakePhone("099123456"),
            "aaClaveSegura#1a"
        );

        Assert.AreEqual(UserRoles.Client, user.Role);
    }
}
