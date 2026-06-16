using BusinessLogics;
using BusinessLogicsAbstractions;
using Contract;
using Moq;

namespace Tests.Users;

[TestClass]
public class UserServiceDelegationTests
{
    private Mock<ISignUpUserService> _signUpMock = null!;
    private Mock<IModifyUserService> _modifyMock = null!;
    private Mock<IDeleteUserService> _deleteMock = null!;
    private Mock<IFilterUserService> _filterMock = null!;
    private UserService _service = null!;

    [TestInitialize]
    public void Initialize()
    {
        _signUpMock = new Mock<ISignUpUserService>(MockBehavior.Strict);
        _modifyMock = new Mock<IModifyUserService>(MockBehavior.Strict);
        _deleteMock = new Mock<IDeleteUserService>(MockBehavior.Strict);
        _filterMock = new Mock<IFilterUserService>(MockBehavior.Strict);
        _service = new UserService(
            _signUpMock.Object,
            _modifyMock.Object,
            _deleteMock.Object,
            _filterMock.Object);
    }

    [TestMethod]
    public void MakeUserIntern_DelegatesTo_SignUpUserService()
    {
        var request = new SignUpInternUserRequestDTO();
        var expected = new UserResponseDTO();
        _signUpMock.Setup(s => s.MakeInterUser(1, request)).Returns(expected);

        var result = _service.MakeUserIntern(1, request);

        Assert.AreEqual(expected, result);
        _signUpMock.Verify(s => s.MakeInterUser(1, request), Times.Once);
    }

    [TestMethod]
    public void ModifyUserIntern_DelegatesTo_ModifyUserService()
    {
        var request = new ModifyUserRequestDTO();
        var expected = new UserResponseDTO();
        _modifyMock.Setup(s => s.UpdateInternalUser(2, 1, request)).Returns(expected);

        var result = _service.ModifyUserIntern(2, 1, request);

        Assert.AreEqual(expected, result);
        _modifyMock.Verify(s => s.UpdateInternalUser(2, 1, request), Times.Once);
    }

    [TestMethod]
    public void ModifyPasswordUser_DelegatesTo_ModifyUserService()
    {
        var request = new ModifyPasswordRequestDTO();
        _modifyMock.Setup(s => s.UpdateUserPassword(2, 1, request));

        _service.ModifyPasswordUser(2, 1, request);

        _modifyMock.Verify(s => s.UpdateUserPassword(2, 1, request), Times.Once);
    }

    [TestMethod]
    public void DeleteUser_DelegatesTo_DeleteUserService()
    {
        _deleteMock.Setup(s => s.DeleteInterUser(2, 1));

        _service.DeleteUser(2, 1);

        _deleteMock.Verify(s => s.DeleteInterUser(2, 1), Times.Once);
    }

    [TestMethod]
    public void FindUserOnSystem_DelegatesTo_FilterUserService()
    {
        var request = new FindUserRequestDTO();
        var expected = new List<UserResponseDTO>();
        _filterMock.Setup(s => s.FindUser(1, request)).Returns(expected);

        var result = _service.FindUserOnSystem(1, request);

        Assert.AreEqual(expected, result);
        _filterMock.Verify(s => s.FindUser(1, request), Times.Once);
    }

    [TestMethod]
    public void GetByIdIncludingInactive_DelegatesTo_FilterUserService()
    {
        var expected = new UserResponseDTO { Id = 5 };
        _filterMock.Setup(s => s.GetByIdIncludingInactive(5)).Returns(expected);

        var result = _service.GetByIdIncludingInactive(5);

        Assert.AreEqual(expected, result);
        _filterMock.Verify(s => s.GetByIdIncludingInactive(5), Times.Once);
    }
}
