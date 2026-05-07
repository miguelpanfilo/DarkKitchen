using BusinessLogics.Promotions;
using BusinessLogicsAbstractions;
using BusinessLogicsAbstractions.Promotions;
using Contract.Promotions;
using DataAccessAbstractions;
using Domain;
using Moq;

namespace Tests.AuditLogs;

[TestClass]
public class PromotionServiceAuditTests
{
    [TestMethod]
    public void Create_ValidPromotion_RegistersAuditLog()
    {
        var promotionRepoMock = new
            Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepoMock = new
            Mock<IProductRepository>(MockBehavior.Strict);
        var auditMock = new
            Mock<IAuditLogService>(MockBehavior.Strict);

        promotionRepoMock
            .Setup(r => r.Create(It.IsAny<Promotion>()))
            .Returns((Promotion p) => p);

        auditMock
            .Setup(a => a.Register(AuditedEntities.Promotion,
                It.IsAny<int>(), AuditDescriptions.PromotionCreated, It.IsAny<string>()));

        var service = new PromotionService(promotionRepoMock.Object, productRepoMock.Object, auditMock.Object);

        service.Create(new PromotionRequestDto
        {
            Name = "Black Friday",
            DiscountPercentage = 10,
            StartDate = new DateOnly(2026, 1, 25),
            EndDate = new DateOnly(2026, 1, 30)
        }, "admin@dark.com");

        auditMock.Verify(a => a.Register(AuditedEntities.Promotion,
            It.IsAny<int>(), AuditDescriptions.PromotionCreated, It.IsAny<string>()));
    }

    [TestMethod]
    public void Update_ValidPromotion_RegistersAuditLog()
    {
        var promotionRepoMock = new
            Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepoMock = new
            Mock<IProductRepository>(MockBehavior.Strict);
        var auditMock = new Mock<IAuditLogService>(MockBehavior.Strict);

        var promotion = Promotion.MakePromotion(
            "Black Friday",
            10,
            new DateOnly(2026, 6, 1),
            new DateOnly(2026, 6, 30));

        promotionRepoMock
            .Setup(r => r.GetById(It.IsAny<int>()))
            .Returns(promotion);

        promotionRepoMock
            .Setup(r => r.Update(It.IsAny<Promotion>()))
            .Returns((Promotion p) => p);

        auditMock
            .Setup(a => a.Register(AuditedEntities.Promotion,
                It.IsAny<int>(), AuditDescriptions.PromotionModified, It.IsAny<string>()));

        var service = new PromotionService(promotionRepoMock.Object,
            productRepoMock.Object, auditMock.Object);

        service.Update(1, new PromotionPatchDto
        { Name = "Nuevo nombre" }, "admin@dark.com");
        auditMock.Verify(a => a.Register(AuditedEntities.Promotion,
            It.IsAny<int>(), AuditDescriptions.PromotionModified, It.IsAny<string>()));
    }

    [TestMethod]
    public void Delete_ValidPromotion_RegistersAuditLog()
    {
        var promotionRepoMock = new
            Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepoMock = new
            Mock<IProductRepository>(MockBehavior.Strict);
        var auditMock = new Mock<IAuditLogService>(MockBehavior.Strict);

        var promotion = Promotion.MakePromotion(
            "Black Friday",
            10,
            new DateOnly(2026, 6, 1),
            new DateOnly(2026, 6, 30));

        promotionRepoMock
            .Setup(r => r.GetById(It.IsAny<int>()))
            .Returns(promotion);

        promotionRepoMock
            .Setup(r => r.Update(It.IsAny<Promotion>()))
            .Returns((Promotion p) => p);

        auditMock
            .Setup(a => a.Register(AuditedEntities.Promotion,
                It.IsAny<int>(), AuditDescriptions.PromotionDeleted, It.IsAny<string>()));

        var service = new PromotionService(promotionRepoMock.Object,
            productRepoMock.Object, auditMock.Object);

            service.Delete(1, "admin@dark.com");
        auditMock.Verify(a => a.Register(AuditedEntities.Promotion,
            It.IsAny<int>(), AuditDescriptions.PromotionDeleted, It.IsAny<string>()));
    }
}
