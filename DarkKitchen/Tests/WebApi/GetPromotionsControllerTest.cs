using BusinessLogicsAbstractions;
using Contract;
using DarkKitchen.Controllers;
using DarkKitchen.Filters;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.WebApi;

[TestClass]
public class GetPromotionsControllerTest
{
    [TestMethod]
    public void GetPromotions_WhenValidRequest_ShouldReturn200WithPromotions()
    {
        var serviceMock = new Mock<IPromotionService>(MockBehavior.Strict);

        var request = new PromotionSearchRequestDto
        {
            Date = new DateOnly(2026, 1, 26)
        };

        var response = new List<PromotionResponseDto>
        {
            new PromotionResponseDto(
                1,
                "Promo Test",
                10,
                new DateOnly(2026, 1, 25),
                new DateOnly(2026, 1, 30),
                new List<ProductSummaryDto>
                {
                    new ProductSummaryDto(1, "Producto 1")
                }
            )
        };

        serviceMock
            .Setup(s => s.GetPromotions(request))
            .Returns(response);

        var controller = new PromotionsController(serviceMock.Object);

        var result = controller.Get(request) as OkObjectResult;

        Assert.IsNotNull(result);
        Assert.AreEqual(200, result.StatusCode);
        Assert.AreEqual(response, result.Value);

        serviceMock.VerifyAll();
    }

    [TestMethod]
    public void GetPromotions_HasRoleAuthorizationAttribute()
    {
        var method = typeof(PromotionsController).GetMethod(nameof(PromotionsController.Get));

        Assert.IsNotNull(method);

        var attribute = method.GetCustomAttributes(typeof(RoleAuthorizationAttribute), false)
            .Cast<RoleAuthorizationAttribute>()
            .SingleOrDefault();

        Assert.IsNotNull(attribute);
    }

    [TestMethod]
    public void GetPromotions_HasHttpGetAttribute()
    {
        var method = typeof(PromotionsController).GetMethod(nameof(PromotionsController.Get));

        Assert.IsNotNull(method);

        var attribute = method.GetCustomAttributes(typeof(HttpGetAttribute), false)
            .Cast<HttpGetAttribute>()
            .SingleOrDefault();

        Assert.IsNotNull(attribute);
    }
}
