using BusinessLogicsAbstractions.Exceptions;
using BusinessLogicsAbstractions.ShippingTypes;
using Contract.ShippingTypes;
using Contract.Users;
using DarkKitchen.Controllers;
using DarkKitchen.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.WebApi;

[TestClass]
public class ShippingTypesControllerTests
{
    private Mock<IShippingTypeService> _serviceMock = null!;
    private ShippingTypesController _controller = null!;

    [TestInitialize]
    public void Initialize()
    {
        _serviceMock = new Mock<IShippingTypeService>(MockBehavior.Strict);
        _controller = new ShippingTypesController(_serviceMock.Object);
    }

    private static CurrentUserDto MakeAdmin() => new CurrentUserDto
    {
        Id = 1,
        Email = "admin@test.com",
        Role = UserRolesDto.Administrative
    };

    private void SetCurrentUser(CurrentUserDto user)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Items["CurrentUser"] = user;
        _controller.ControllerContext = new ControllerContext { HttpContext = httpContext };
    }

    [TestMethod]
    public void Create_WhenAdmin_Returns201WithCreatedResource()
    {
        var request = new ShippingTypeRequestDto { Name = "Envío express", Cost = 150m };
        var response = new ShippingTypeResponseDto { Id = 1, Name = "Envío express", Cost = 150m };

        _serviceMock.Setup(s => s.CreateShippingType(request)).Returns(response);
        SetCurrentUser(MakeAdmin());

        var result = _controller.Create(request) as CreatedResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status201Created, result.StatusCode);
        Assert.AreEqual(response, result.Value);
        _serviceMock.Verify(s => s.CreateShippingType(request), Times.Once);
    }

    [TestMethod]
    public void Create_WhenServiceThrowsValidationException_PropagatesException()
    {
        var request = new ShippingTypeRequestDto { Name = string.Empty, Cost = 150m };

        _serviceMock
            .Setup(s => s.CreateShippingType(request))
            .Throws(new ValidationException("El nombre del tipo de envío no puede estar vacío."));

        Assert.ThrowsException<ValidationException>(() => _controller.Create(request));
    }

    [TestMethod]
    public void Create_HasHttpPostAttribute()
    {
        var method = typeof(ShippingTypesController).GetMethod(nameof(ShippingTypesController.Create));

        var attribute = method!.GetCustomAttributes(typeof(HttpPostAttribute), false)
            .Cast<HttpPostAttribute>()
            .SingleOrDefault();

        Assert.IsNotNull(attribute);
    }

    [TestMethod]
    public void Create_HasRoleAuthorizationAttributeWithAdministrative()
    {
        var method = typeof(ShippingTypesController).GetMethod(nameof(ShippingTypesController.Create));

        var attribute = method!.GetCustomAttributes(typeof(RoleAuthorizationAttribute), false)
            .Cast<RoleAuthorizationAttribute>()
            .SingleOrDefault();

        Assert.IsNotNull(attribute);
    }

    [TestMethod]
    public void Update_WhenAdmin_Returns200WithUpdatedResource()
    {
        var request = new ShippingTypePatchDto { Name = "Envío premium", Cost = 200m };
        var response = new ShippingTypeResponseDto { Id = 1, Name = "Envío premium", Cost = 200m };

        _serviceMock.Setup(s => s.UpdateShippingType(1, request)).Returns(response);
        SetCurrentUser(MakeAdmin());

        var result = _controller.Update(1, request) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
        Assert.AreEqual(response, result.Value);
        _serviceMock.Verify(s => s.UpdateShippingType(1, request), Times.Once);
    }

    [TestMethod]
    public void Update_WhenShippingTypeNotFound_PropagatesNotFoundException()
    {
        var request = new ShippingTypePatchDto { Name = "Envío premium" };

        _serviceMock
            .Setup(s => s.UpdateShippingType(99, request))
            .Throws(new NotFoundException("El tipo de envío no existe"));

        Assert.ThrowsException<NotFoundException>(() => _controller.Update(99, request));
    }

    [TestMethod]
    public void Update_HasHttpPatchAttributeWithIdTemplate()
    {
        var method = typeof(ShippingTypesController).GetMethod(nameof(ShippingTypesController.Update));

        var attribute = method!.GetCustomAttributes(typeof(HttpPatchAttribute), false)
            .Cast<HttpPatchAttribute>()
            .SingleOrDefault();

        Assert.IsNotNull(attribute);
        Assert.AreEqual("{id}", attribute.Template);
    }

    [TestMethod]
    public void Update_HasRoleAuthorizationAttributeWithAdministrative()
    {
        var method = typeof(ShippingTypesController).GetMethod(nameof(ShippingTypesController.Update));

        var attribute = method!.GetCustomAttributes(typeof(RoleAuthorizationAttribute), false)
            .Cast<RoleAuthorizationAttribute>()
            .SingleOrDefault();

        Assert.IsNotNull(attribute);
    }

    [TestMethod]
    public void GetAll_Returns200WithList()
    {
        var response = new List<ShippingTypeResponseDto>
        {
            new ShippingTypeResponseDto { Id = 1, Name = "Envío express", Cost = 150m },
            new ShippingTypeResponseDto { Id = 2, Name = "Envío estándar", Cost = 80m }
        };

        _serviceMock.Setup(s => s.GetAllShippingTypes()).Returns(response);

        var result = _controller.GetAll() as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
        Assert.AreEqual(response, result.Value);
        _serviceMock.VerifyAll();
    }

    [TestMethod]
    public void GetAll_HasHttpGetAttribute()
    {
        var method = typeof(ShippingTypesController).GetMethod(nameof(ShippingTypesController.GetAll));

        var attribute = method!.GetCustomAttributes(typeof(HttpGetAttribute), false)
            .Cast<HttpGetAttribute>()
            .SingleOrDefault();

        Assert.IsNotNull(attribute);
    }

    [TestMethod]
    public void GetAll_DoesNotHaveRoleAuthorizationAttribute()
    {
        var method = typeof(ShippingTypesController).GetMethod(nameof(ShippingTypesController.GetAll));

        var attribute = method!.GetCustomAttributes(typeof(RoleAuthorizationAttribute), false)
            .SingleOrDefault();

        Assert.IsNull(attribute);
    }
}
