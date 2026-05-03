using BusinessLogics.Users;
using BusinessLogicsAbstractions.Exceptions;
using BusinessLogicsAbstractions.Users;
using Contract.Users;
using DataAccessAbstractions;
using Domain;
using Mappers;
using Moq;

namespace Tests.Usuarios;

[TestClass]
public class UserServiceTests
{
    private Mock<IRepositoryUser> _repoMock = null!;
    private Mock<IPhoneValidator> _validatorPhoneMock = null!;
    private Mock<IAdminValidationService> _adminValidationMock = null!;
    private SignUpService _service = null!;

    [TestInitialize]
    public void Initialize()
    {
        _repoMock = new Mock<IRepositoryUser>(MockBehavior.Strict);
        _validatorPhoneMock = new Mock<IPhoneValidator>(MockBehavior.Strict);
        _adminValidationMock = new Mock<IAdminValidationService>(MockBehavior.Strict);

        _service = new SignUpService(
            _repoMock.Object,
            _validatorPhoneMock.Object,
            _adminValidationMock.Object);
    }

    private static Phone CreateValidPhone(string number)
    {
        var phoneResult = Phone.TryMakePhone(number);
        Assert.IsTrue(phoneResult.Success);
        Assert.IsNotNull(phoneResult.Value);
        return phoneResult.Value!;
    }

    private static SignUpClientRequestDTO CreateValidSignUpRequest()
    {
        return new SignUpClientRequestDTO
        {
            Name = "Miguel",
            LastName = "Lopez",
            Email = "miguel@gmail.com",
            Phone = "099123456",
            Password = "Password@2Ok6Abc"
        };
    }

    [TestMethod]
    public void SignUpClient_ValidData_ReturnsResponseDto()
    {
        _repoMock
            .Setup(r => r.EmailExist(It.IsAny<string>()))
            .Returns(false);

        _validatorPhoneMock
            .Setup(v => v.ItsValid(It.IsAny<string>()))
            .Returns(true);

        _repoMock
            .Setup(r => r.Add(It.IsAny<User>()))
            .Returns((User u) => u);

        var request = CreateValidSignUpRequest();

        var result = _service.SignUpClient(request);

        Assert.IsNotNull(result);
        Assert.AreEqual("Miguel", result.Name);
        Assert.AreEqual("Lopez", result.LastName);
        Assert.AreEqual("miguel@gmail.com", result.Email);

        _repoMock.Verify(r => r.EmailExist("miguel@gmail.com"), Times.Once);
        _validatorPhoneMock.Verify(v => v.ItsValid("099123456"), Times.Once);
        _repoMock.Verify(r => r.Add(It.IsAny<User>()), Times.Once);
    }

    [TestMethod]
    public void SignUpClient_DuplicateEmail_ThrowsException_AndDoesNotValidatePhoneOrAdd()
    {
        _repoMock
            .Setup(r => r.EmailExist("miguel@gmail.com"))
            .Returns(true);

        var request = CreateValidSignUpRequest();

        var ex = Assert.ThrowsException<ConflictException>(() =>
            _service.SignUpClient(request));

        Assert.AreEqual("Email ya registrado", ex.Message);

        _validatorPhoneMock.Verify(v => v.ItsValid(It.IsAny<string>()), Times.Never);
        _repoMock.Verify(r => r.Add(It.IsAny<User>()), Times.Never);
    }

    [TestMethod]
    public void SignUpClient_InvalidPhoneAccordingToValidator_ThrowsException_AndDoesNotAdd()
    {
        _repoMock
            .Setup(r => r.EmailExist(It.IsAny<string>()))
            .Returns(false);

        _validatorPhoneMock
            .Setup(v => v.ItsValid("123"))
            .Returns(false);

        var request = new SignUpClientRequestDTO
        {
            Name = "Miguel",
            LastName = "Lopez",
            Email = "miguel@gmail.com",
            Phone = "123",
            Password = "Password@2026Ok"
        };

        var ex = Assert.ThrowsException<ValidationException>(() =>
            _service.SignUpClient(request));

        Assert.AreEqual("Telefono no valido", ex.Message);

        _validatorPhoneMock.Verify(v => v.ItsValid("123"), Times.Once);
        _repoMock.Verify(r => r.Add(It.IsAny<User>()), Times.Never);
    }

    [TestMethod]
    public void SignUpClient_EmptyPhone_ThrowsException_BeforeCallingValidator()
    {
        _repoMock
            .Setup(r => r.EmailExist(It.IsAny<string>()))
            .Returns(false);

        var request = new SignUpClientRequestDTO
        {
            Name = "Miguel",
            LastName = "Lopez",
            Email = "miguel@gmail.com",
            Phone = string.Empty,
            Password = "Password@2026Ok"
        };

        var ex = Assert.ThrowsException<ValidationException>(() =>
            _service.SignUpClient(request));

        Assert.AreEqual("Telefono no puede estar vacío", ex.Message);

        _validatorPhoneMock.Verify(v => v.ItsValid(It.IsAny<string>()), Times.Never);
        _repoMock.Verify(r => r.Add(It.IsAny<User>()), Times.Never);
    }

    [TestMethod]
    public void SignUpClient_InvalidEmailFormat_ThrowsException_AndDoesNotAdd()
    {
        _repoMock
            .Setup(r => r.EmailExist(It.IsAny<string>()))
            .Returns(false);

        _validatorPhoneMock
            .Setup(v => v.ItsValid("099123456"))
            .Returns(true);

        var request = new SignUpClientRequestDTO
        {
            Name = "Miguel",
            LastName = "Lopez",
            Email = "correo-invalido",
            Phone = "099123456",
            Password = "Password@2Ok6Abc"
        };

        var ex = Assert.ThrowsException<ValidationException>(() =>
            _service.SignUpClient(request));

        Assert.AreEqual("Formato de Email inválido", ex.Message);

        _repoMock.Verify(r => r.Add(It.IsAny<User>()), Times.Never);
    }

    [TestMethod]
    public void SignUpClient_EmptyEmail_ThrowsException_AndDoesNotAdd()
    {
        _repoMock
            .Setup(r => r.EmailExist(It.IsAny<string>()))
            .Returns(false);

        _validatorPhoneMock
            .Setup(v => v.ItsValid("099123456"))
            .Returns(true);

        var request = new SignUpClientRequestDTO
        {
            Name = "Miguel",
            LastName = "Lopez",
            Email = string.Empty,
            Phone = "099123456",
            Password = "Password@2Ok6Abc"
        };

        var ex = Assert.ThrowsException<ValidationException>(() =>
            _service.SignUpClient(request));

        Assert.AreEqual("Email no puede estar vacío", ex.Message);

        _repoMock.Verify(r => r.Add(It.IsAny<User>()), Times.Never);
    }

    [TestMethod]
    public void SignUpClient_NameWithNumbers_ThrowsException_AndDoesNotAdd()
    {
        _repoMock
            .Setup(r => r.EmailExist(It.IsAny<string>()))
            .Returns(false);

        _validatorPhoneMock
            .Setup(v => v.ItsValid("099123456"))
            .Returns(true);

        var request = new SignUpClientRequestDTO
        {
            Name = "Mig4el",
            LastName = "Lopez",
            Email = "miguel@gmail.com",
            Phone = "099123456",
            Password = "Password@2Ok6Abc"
        };

        var ex = Assert.ThrowsException<ValidationException>(() =>
            _service.SignUpClient(request));

        Assert.AreEqual("Nombre no puede contener números", ex.Message);

        _repoMock.Verify(r => r.Add(It.IsAny<User>()), Times.Never);
    }

    [TestMethod]
    public void SignUpClient_DoesNotUseAdminValidationService()
    {
        _repoMock
            .Setup(r => r.EmailExist("miguel@gmail.com"))
            .Returns(false);

        _validatorPhoneMock
            .Setup(v => v.ItsValid("099123456"))
            .Returns(true);

        _repoMock
            .Setup(r => r.Add(It.IsAny<User>()))
            .Returns((User u) => u);

        var request = CreateValidSignUpRequest();

        var result = _service.SignUpClient(request);

        Assert.IsNotNull(result);

        _adminValidationMock.Verify(v => v.EnsureUserIsAdmin(It.IsAny<int>()), Times.Never);
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
        Assert.AreEqual("Nombre no puede estar vacío", result.ErrorMessage);
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
        Assert.AreEqual("Nombre no puede estar vacío", result.ErrorMessage);
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
        Assert.AreEqual("El apellido no puede estar vacío", result.ErrorMessage);
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
        Assert.AreEqual("El apellido debe tener entre 3 y 25 caracteres", result.ErrorMessage);
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
        Assert.AreEqual("El apellido debe tener entre 3 y 25 caracteres", result.ErrorMessage);
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
        _repoMock
            .Setup(r => r.EmailExist(It.IsAny<string>()))
            .Returns(false);

        _validatorPhoneMock
            .Setup(v => v.ItsValid(It.IsAny<string>()))
            .Returns(true);

        _repoMock
            .Setup(r => r.Add(It.IsAny<User>()))
            .Returns((User u) => u);

        var signUpRequest = CreateValidSignUpRequest();

        var clientResult = _service.SignUpClient(signUpRequest);

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
        _repoMock
            .Setup(r => r.EmailExist(It.IsAny<string>()))
            .Returns(false);

        _repoMock
            .Setup(r => r.Add(It.IsAny<User>()))
            .Returns((User u) => u);

        _validatorPhoneMock
            .Setup(v => v.ItsValid(It.IsAny<string>()))
            .Returns(true);

        var signUpRequest = CreateValidSignUpRequest();

        var signUpDTO = _service.SignUpClient(signUpRequest);

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
        Assert.AreEqual("Formato de clave inválida", result.ErrorMessage);
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
        Assert.AreEqual("Formato de clave inválida", result.ErrorMessage);
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
        Assert.AreEqual("Formato de clave inválida", result.ErrorMessage);
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
        Assert.AreEqual("Formato de clave inválida", result.ErrorMessage);
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
        Assert.AreEqual("Formato de clave inválida", result.ErrorMessage);
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
        Assert.AreEqual("Formato de clave inválida", result.ErrorMessage);
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
        Assert.AreEqual("Formato de clave inválida", result.ErrorMessage);
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
        Assert.AreEqual("Formato de clave inválida", result.ErrorMessage);
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

    [TestMethod]
    public void CreateUser_WhenUserIsCreated_StatusIsActive()
    {
        var phone = CreateValidPhone("099123456");

        var result = User.MakeClient(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            phone,
            "Password@2Ok6Abc");

        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Value);
        Assert.AreEqual(UserStatus.Active, result.Value!.Status);
    }

    [TestMethod]
    public void DeactivateUser_WhenCalled_StatusChangesToInactive()
    {
        var phone = CreateValidPhone("099123456");

        var result = User.MakeClient(
            "Miguel",
            "Lopez",
            "miguel@gmail.com",
            phone,
            "Password@2Ok6Abc");

        Assert.IsTrue(result.Success);
        Assert.IsNotNull(result.Value);

        var user = result.Value!;

        user.Deactivate();

        Assert.AreEqual(UserStatus.Inactive, user.Status);
    }
}
