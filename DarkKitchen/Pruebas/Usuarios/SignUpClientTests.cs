using AlmacenamientoAbstracciones;
using Contratos;
using Dominio;
using LogicaDeNegocio;
using LogicaDeNegocioAbstracciones;
using LogicaDeNegocioAbstracciones.Exceptions;
using Mapeo;
using Moq;

namespace Pruebas.Usuarios;

[TestClass]
public class UserServiceTests
{
    private static Phone CreateValidPhone(string number)
    {
        var phoneResult = Phone.TryMakePhone(number);
        Assert.IsTrue(phoneResult.Success);
        Assert.IsNotNull(phoneResult.Value);
        return phoneResult.Value!;
    }

    [TestMethod]
    public void SignUpClient_ValidData_ReturnsResponseDto()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var validatorPhoneMock = new Mock<IPhoneValidator>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        repoMock
            .Setup(r => r.EmailExist(It.IsAny<string>()))
            .Returns(false);

        validatorPhoneMock
            .Setup(v => v.ItsValid(It.IsAny<string>()))
            .Returns(true);

        repoMock
            .Setup(r => r.Add(It.IsAny<User>()))
            .Returns((User u) => u);

        var service = new SignUpService(
            repoMock.Object,
            validatorPhoneMock.Object,
            adminValidationMock.Object);

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
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var validatorPhoneMock = new Mock<IPhoneValidator>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        repoMock
            .Setup(r => r.EmailExist("miguel@gmail.com"))
            .Returns(true);

        var service = new SignUpService(
            repoMock.Object,
            validatorPhoneMock.Object,
            adminValidationMock.Object);

        var request = new SignUpClientRequestDTO
        {
            Name = "Miguel",
            LastName = "Lopez",
            Email = "miguel@gmail.com",
            Phone = "099123456",
            Password = "Password@2Ok6Abc"
        };

        var ex = Assert.ThrowsException<ConflictException>(() =>
            service.SignUpClient(request));

        Assert.AreEqual("Email is already registered", ex.Message);

        validatorPhoneMock.Verify(v => v.ItsValid(It.IsAny<string>()), Times.Never);
        repoMock.Verify(r => r.Add(It.IsAny<User>()), Times.Never);
    }

    [TestMethod]
    public void SignUpClient_InvalidPhoneAccordingToValidator_ThrowsException_AndDoesNotAdd()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var validatorPhoneMock = new Mock<IPhoneValidator>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        repoMock
            .Setup(r => r.EmailExist(It.IsAny<string>()))
            .Returns(false);

        validatorPhoneMock
            .Setup(v => v.ItsValid("123"))
            .Returns(false);

        var service = new SignUpService(
            repoMock.Object,
            validatorPhoneMock.Object,
            adminValidationMock.Object);

        var request = new SignUpClientRequestDTO
        {
            Name = "Miguel",
            LastName = "Lopez",
            Email = "miguel@gmail.com",
            Phone = "123",
            Password = "Password@2026Ok"
        };

        var ex = Assert.ThrowsException<ValidationException>(() =>
            service.SignUpClient(request));

        Assert.AreEqual("Phone is not valid", ex.Message);

        validatorPhoneMock.Verify(v => v.ItsValid("123"), Times.Once);
        repoMock.Verify(r => r.Add(It.IsAny<User>()), Times.Never);
    }

    [TestMethod]
    public void SignUpClient_EmptyPhone_ThrowsException_BeforeCallingValidator()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var validatorPhoneMock = new Mock<IPhoneValidator>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        repoMock
            .Setup(r => r.EmailExist(It.IsAny<string>()))
            .Returns(false);

        var service = new SignUpService(
            repoMock.Object,
            validatorPhoneMock.Object,
            adminValidationMock.Object);

        var request = new SignUpClientRequestDTO
        {
            Name = "Miguel",
            LastName = "Lopez",
            Email = "miguel@gmail.com",
            Phone = string.Empty,
            Password = "Password@2026Ok"
        };

        var ex = Assert.ThrowsException<ValidationException>(() =>
            service.SignUpClient(request));

        Assert.AreEqual("Phone cannot be empty", ex.Message);

        validatorPhoneMock.Verify(v => v.ItsValid(It.IsAny<string>()), Times.Never);
        repoMock.Verify(r => r.Add(It.IsAny<User>()), Times.Never);
    }

    [TestMethod]
    public void SignUpClient_InvalidEmailFormat_ThrowsException_AndDoesNotAdd()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var validatorPhoneMock = new Mock<IPhoneValidator>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        repoMock
            .Setup(r => r.EmailExist(It.IsAny<string>()))
            .Returns(false);

        validatorPhoneMock
            .Setup(v => v.ItsValid("099123456"))
            .Returns(true);

        var service = new SignUpService(
            repoMock.Object,
            validatorPhoneMock.Object,
            adminValidationMock.Object);

        var request = new SignUpClientRequestDTO
        {
            Name = "Miguel",
            LastName = "Lopez",
            Email = "correo-invalido",
            Phone = "099123456",
            Password = "Password@2Ok6Abc"
        };

        var ex = Assert.ThrowsException<ValidationException>(() =>
            service.SignUpClient(request));

        Assert.AreEqual("Email format is not valid", ex.Message);

        repoMock.Verify(r => r.Add(It.IsAny<User>()), Times.Never);
    }

    [TestMethod]
    public void SignUpClient_EmptyEmail_ThrowsException_AndDoesNotAdd()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var validatorPhoneMock = new Mock<IPhoneValidator>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        repoMock
            .Setup(r => r.EmailExist(It.IsAny<string>()))
            .Returns(false);

        validatorPhoneMock
            .Setup(v => v.ItsValid("099123456"))
            .Returns(true);

        var service = new SignUpService(
            repoMock.Object,
            validatorPhoneMock.Object,
            adminValidationMock.Object);

        var request = new SignUpClientRequestDTO
        {
            Name = "Miguel",
            LastName = "Lopez",
            Email = string.Empty,
            Phone = "099123456",
            Password = "Password@2Ok6Abc"
        };

        var ex = Assert.ThrowsException<ValidationException>(() =>
            service.SignUpClient(request));

        Assert.AreEqual("Email cannot be empty", ex.Message);

        repoMock.Verify(r => r.Add(It.IsAny<User>()), Times.Never);
    }

    [TestMethod]
    public void SignUpClient_NameWithNumbers_ThrowsException_AndDoesNotAdd()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var validatorPhoneMock = new Mock<IPhoneValidator>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        repoMock
            .Setup(r => r.EmailExist(It.IsAny<string>()))
            .Returns(false);

        validatorPhoneMock
            .Setup(v => v.ItsValid("099123456"))
            .Returns(true);

        var service = new SignUpService(
            repoMock.Object,
            validatorPhoneMock.Object,
            adminValidationMock.Object);

        var request = new SignUpClientRequestDTO
        {
            Name = "Mig4el",
            LastName = "Lopez",
            Email = "miguel@gmail.com",
            Phone = "099123456",
            Password = "Password@2Ok6Abc"
        };

        var ex = Assert.ThrowsException<ValidationException>(() =>
            service.SignUpClient(request));

        Assert.AreEqual("Name cannot contain numbers", ex.Message);

        repoMock.Verify(r => r.Add(It.IsAny<User>()), Times.Never);
    }

    [TestMethod]
    public void SignUpClient_DoesNotUseAdminValidationService()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var validatorPhoneMock = new Mock<IPhoneValidator>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        repoMock
            .Setup(r => r.EmailExist("miguel@gmail.com"))
            .Returns(false);

        validatorPhoneMock
            .Setup(v => v.ItsValid("099123456"))
            .Returns(true);

        repoMock
            .Setup(r => r.Add(It.IsAny<User>()))
            .Returns((User u) => u);

        var service = new SignUpService(
            repoMock.Object,
            validatorPhoneMock.Object,
            adminValidationMock.Object);

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

        adminValidationMock.Verify(v => v.EnsureUserIsAdmin(It.IsAny<int>()), Times.Never);
    }

    [TestMethod]
    public void SignUpCliente_NoName_Exception()
    {
        var phone = CreateValidPhone("099123456");

        var result = User.MakeClient(
            string.Empty,
            "Lopez",
            "miguel@gmail.com",
            phone,
            "Password@2026Ok");

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Name cannot be empty", result.ErrorMessage);
    }

    [TestMethod]
    public void SignUpCliente_MinNameValid()
    {
        var phone = CreateValidPhone("099123456");

        var result = User.MakeClient(
            "M",
            "Lopez",
            "miguel@gmail.com",
            phone,
            "Password@2Ok6Abc");

        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Value);
    }

    [TestMethod]
    public void SignUpCliente_NullName_ReturnException()
    {
        var phone = CreateValidPhone("099123456");

        var result = User.MakeClient(
            null!,
            "Lopez",
            "miguel@gmail.com",
            phone,
            "Password@2Ok6Abc");

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Name cannot be empty", result.ErrorMessage);
    }

    [TestMethod]
    public void SignClient_WithOutLastname_Exception()
    {
        var phone = CreateValidPhone("099123456");

        var result = User.MakeClient(
            "Miguel",
            string.Empty,
            "miguel@gmail.com",
            phone,
            "Password@2026Ok");

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Last name cannot be empty", result.ErrorMessage);
    }

    [TestMethod]
    public void SignupCliente_WithMoreOf25Chars()
    {
        var lastnameLenghtIncorrect = 26;
        var notValidLastname = new string('a', lastnameLenghtIncorrect);
        var phone = CreateValidPhone("099123456");

        var result = User.MakeClient(
            "Miguel",
            notValidLastname,
            "miguel@gmail.com",
            phone,
            "Password@2026Ok");

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Last name must be between 3 and 25 characters", result.ErrorMessage);
    }

    [TestMethod]
    public void SignupClient_WithLessOf3Chars()
    {
        var lastnameLenghtIncorrect = 2;
        var smallLastname = new string('a', lastnameLenghtIncorrect);
        var phone = CreateValidPhone("099123456");

        var result = User.MakeClient(
            "Miguel",
            smallLastname,
            "miguel@gmail.com",
            phone,
            "Password@2026Ok");

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Last name must be between 3 and 25 characters", result.ErrorMessage);
    }

    [TestMethod]
    public void SignUp_Client_MaxLastnameValid()
    {
        var lastname = new string('a', 25);
        var phone = CreateValidPhone("099123456");

        var user = User.MakeClient(
            "Miguel",
            lastname,
            "miguel@gmail.com",
            phone,
            "Password@2Ok6Abc");

        Assert.IsTrue(user.Success);
        Assert.IsNotNull(user.Value);
    }

    [TestMethod]
    public void SignUpCliente_LastnameWithMinimunValid()
    {
        var phone = CreateValidPhone("099123456");

        var user = User.MakeClient(
            "Miguel",
            "Lop",
            "miguel@gmail.com",
            phone,
            "Password@2Ok6Abc");

        Assert.IsTrue(user.Success);
        Assert.IsNotNull(user.Value);
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
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var validatorPhoneMock = new Mock<IPhoneValidator>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        repoMock
            .Setup(r => r.EmailExist(It.IsAny<string>()))
            .Returns(false);

        validatorPhoneMock
            .Setup(v => v.ItsValid(It.IsAny<string>()))
            .Returns(true);

        repoMock
            .Setup(r => r.Add(It.IsAny<User>()))
            .Returns((User u) => u);

        var signUpService = new SignUpService(
            repoMock.Object,
            validatorPhoneMock.Object,
            adminValidationMock.Object);

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
    public void CreateUser_EmailIsNormalized_Correctly()
    {
        var phone = CreateValidPhone("099123456");

        var userResult = User.MakeClient(
            "Miguel",
            "Lopez",
            "  MIGUEL@gmail.com  ",
            phone,
            "Password@2Ok6Abc");

        Assert.IsTrue(userResult.Success);
        Assert.IsNotNull(userResult.Value);
        Assert.AreEqual("miguel@gmail.com", userResult.Value!.Email.ToString());
    }

    [TestMethod]
    public void RegisterClient_EmailNotDuplicated_SuccessfulRegistration()
    {
        var repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        var validatorPhoneMock = new Mock<IPhoneValidator>(MockBehavior.Strict);
        var adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        repoMock
            .Setup(r => r.EmailExist(It.IsAny<string>()))
            .Returns(false);

        repoMock
            .Setup(r => r.Add(It.IsAny<User>()))
            .Returns((User u) => u);

        validatorPhoneMock
            .Setup(v => v.ItsValid(It.IsAny<string>()))
            .Returns(true);

        var singUpService = new SignUpService(
            repoMock.Object,
            validatorPhoneMock.Object,
            adminValidationMock.Object);

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
        var phone = CreateValidPhone("099123456");

        var result = User.MakeClient(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            phone,
            password);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Password format is not valid", result.ErrorMessage);
    }

    [TestMethod]
    public void CreateUser_PasswordShorterThan15_ThrowsException_WithCorrectMessage()
    {
        var lengthLessThanMin = 14;
        var passwordWithLengthLessThanMin = new string('a', lengthLessThanMin);
        var phone = CreateValidPhone("099123456");

        var result = User.MakeClient(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            phone,
            passwordWithLengthLessThanMin);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Password format is not valid", result.ErrorMessage);
    }

    [TestMethod]
    public void CreateUser_PasswordLongerThan25_ThrowsException_WithCorrectMessage()
    {
        var lengthGreaterThanLimit = 26;
        var passwordGreaterThanLimit = new string('a', lengthGreaterThanLimit);
        var phone = CreateValidPhone("099123456");

        var result = User.MakeClient(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            phone,
            passwordGreaterThanLimit);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Password format is not valid", result.ErrorMessage);
    }

    [TestMethod]
    public void CreateUser_PasswordWithoutUppercase_ThrowsException_WithCorrectMessage()
    {
        var password = "password!abc9xy";
        var phone = CreateValidPhone("099123456");

        var result = User.MakeClient(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            phone,
            password);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Password format is not valid", result.ErrorMessage);
    }

    [TestMethod]
    public void CreateUser_PasswordWithoutLowercase_ThrowsException_WithCorrectMessage()
    {
        var password = "PASSWORD!ABC9XY";
        var phone = CreateValidPhone("099123456");

        var result = User.MakeClient(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            phone,
            password);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Password format is not valid", result.ErrorMessage);
    }

    [TestMethod]
    public void CreateUser_PasswordWithoutNumber_ThrowsException_WithCorrectMessage()
    {
        var password = "Password!AbcXyz";
        var phone = CreateValidPhone("099123456");

        var result = User.MakeClient(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            phone,
            password);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Password format is not valid", result.ErrorMessage);
    }

    [TestMethod]
    public void CreateUser_PasswordWithoutSymbol_ThrowsException_WithCorrectMessage()
    {
        var password = "PasswordAbc123Xy";
        var phone = CreateValidPhone("099123456");

        var result = User.MakeClient(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            phone,
            password);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Password format is not valid", result.ErrorMessage);
    }

    [TestMethod]
    public void CreateUser_PasswordWithNumberSequence_ThrowsException_WithCorrectMessage()
    {
        var password = "Password!Ab12Xy";
        var phone = CreateValidPhone("099123456");

        var result = User.MakeClient(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            phone,
            password);

        Assert.IsFalse(result.Success);
        Assert.AreEqual("Password format is not valid", result.ErrorMessage);
    }

    [TestMethod]
    public void CreateUser_PasswordWith15Characters_IsValid_DoesNotThrowException()
    {
        var phone = CreateValidPhone("099123456");

        var user = User.MakeClient(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            phone,
            "Password!Abc9Xy");

        Assert.IsTrue(user.Success);
        Assert.IsNotNull(user.Value);
    }

    [TestMethod]
    public void CreateUser_PasswordWith25Characters_IsValid_DoesNotThrowException()
    {
        var phone = CreateValidPhone("099123456");

        var user = User.MakeClient(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            phone,
            "Password!Abc9XyzKlmNoPq");

        Assert.IsTrue(user.Success);
        Assert.IsNotNull(user.Value);
    }

    [TestMethod]
    public void CreateClient_ValidData_AssignsClientRole()
    {
        var phone = CreateValidPhone("099123456");

        var user = User.MakeClient(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            phone,
            "aaClaveSegura#1a");

        Assert.IsTrue(user.Success);
        Assert.IsNotNull(user.Value);
        Assert.AreEqual(UserRoles.Client, user.Value!.Role);
    }

    [TestMethod]
    public void MappingUserToResponseDTO_NullUser_ThrowsUserException()
    {
        User? user = null;

        var ex = Assert.ThrowsException<ArgumentNullException>(() =>
            user!.MappingUserToResponseDTO());

        Assert.AreEqual("user", ex.ParamName);
    }
}
