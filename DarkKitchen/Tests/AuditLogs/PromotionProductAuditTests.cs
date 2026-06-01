using BusinessLogics;
using BusinessLogicsAbstractions;
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

        var currentUserMock = new Mock<ICurrentUserService>(MockBehavior.Strict);
        currentUserMock.Setup(s => s.GetEmail()).Returns("admin@dark.com");
        var service = new PromotionService(promotionRepoMock.Object, productRepoMock.Object,
            auditMock.Object, currentUserMock.Object);

        service.Create(new PromotionRequestDto
        {
            Name = "Black Friday",
            DiscountPercentage = 10,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(5),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(10)
        });

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

        var currentUserMock = new Mock<ICurrentUserService>(MockBehavior.Strict);
        currentUserMock.Setup(s => s.GetEmail()).Returns("admin@dark.com");
        var service = new PromotionService(promotionRepoMock.Object,
            productRepoMock.Object, auditMock.Object, currentUserMock.Object);

        service.Update(1, new PromotionPatchDto { Name = "Nuevo nombre" });
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

        var currentUserMock = new Mock<ICurrentUserService>(MockBehavior.Strict);
        currentUserMock.Setup(s => s.GetEmail()).Returns("admin@dark.com");
        var service = new PromotionService(promotionRepoMock.Object,
            productRepoMock.Object, auditMock.Object, currentUserMock.Object);

        service.Delete(1);
        auditMock.Verify(a => a.Register(AuditedEntities.Promotion,
            It.IsAny<int>(), AuditDescriptions.PromotionDeleted, It.IsAny<string>()));
    }

    [TestMethod]
    public void Create_ValidPromotion_GetsResponsibleUserFromCurrentUserService()
    {
        var promotionRepoMock = new
            Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepoMock = new
            Mock<IProductRepository>(MockBehavior.Strict);
        var auditMock = new Mock<IAuditLogService>(MockBehavior.Strict);
        var currentUserMock = new
            Mock<ICurrentUserService>(MockBehavior.Strict);

        currentUserMock
            .Setup(s => s.GetEmail())
            .Returns("admin@dark.com");

        promotionRepoMock
            .Setup(r => r.Create(It.IsAny<Promotion>()))
            .Returns((Promotion p) => p);

        auditMock
            .Setup(a => a.Register(AuditedEntities.Promotion,
                It.IsAny<int>(), AuditDescriptions.PromotionCreated,
                "admin@dark.com"));

        var service = new PromotionService(promotionRepoMock.Object,
            productRepoMock.Object,
            auditMock.Object, currentUserMock.Object);

        service.Create(new PromotionRequestDto
        {
            Name = "Black Friday",
            DiscountPercentage = 10,
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(5),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(10)
        });

        currentUserMock.Verify(s => s.GetEmail(), Times.Once);
        auditMock.Verify(a => a.Register(AuditedEntities.Promotion,
            It.IsAny<int>(), AuditDescriptions.PromotionCreated,
            "admin@dark.com"), Times.Once);
    }

    [TestMethod]
    public void Update_ValidPromotion_GetsResponsibleUserFromCurrentUserService()
    {
        var promotionRepoMock = new
            Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepoMock = new
            Mock<IProductRepository>(MockBehavior.Strict);
        var auditMock = new Mock<IAuditLogService>(MockBehavior.Strict);
        var currentUserMock = new
            Mock<ICurrentUserService>(MockBehavior.Strict);

        currentUserMock
            .Setup(s => s.GetEmail())
            .Returns("admin@dark.com");

        var promotion = Promotion.MakePromotion(
            "Black Friday",
            10,
            new DateOnly(2026, 6, 1),
            new DateOnly(2026, 6, 30));

        promotionRepoMock
            .Setup(r => r.GetById(1))
            .Returns(promotion);

        promotionRepoMock
            .Setup(r => r.Update(It.IsAny<Promotion>()))
            .Returns((Promotion p) => p);

        auditMock
            .Setup(a => a.Register(AuditedEntities.Promotion,
                It.IsAny<int>(), AuditDescriptions.PromotionModified,
                "admin@dark.com"));

        var service = new PromotionService(promotionRepoMock.Object,
            productRepoMock.Object,
            auditMock.Object, currentUserMock.Object);

        service.Update(1, new PromotionPatchDto { Name = "Nuevo nombre"});

        currentUserMock.Verify(s => s.GetEmail(), Times.Once);
        auditMock.Verify(a => a.Register(AuditedEntities.Promotion,
            It.IsAny<int>(), AuditDescriptions.PromotionModified,
            "admin@dark.com"), Times.Once);
    }

    [TestMethod]
    public void Delete_ValidPromotion_GetsResponsibleUserFromCurrentUserService()
    {
        var promotionRepoMock = new
            Mock<IPromotionRepository>(MockBehavior.Strict);
        var productRepoMock = new
            Mock<IProductRepository>(MockBehavior.Strict);
        var auditMock = new Mock<IAuditLogService>(MockBehavior.Strict);
        var currentUserMock = new
            Mock<ICurrentUserService>(MockBehavior.Strict);

        currentUserMock
            .Setup(s => s.GetEmail())
            .Returns("admin@dark.com");

        var promotion = Promotion.MakePromotion(
            "Black Friday",
            10,
            new DateOnly(2026, 6, 1),
            new DateOnly(2026, 6, 30));

        promotionRepoMock
            .Setup(r => r.GetById(1))
            .Returns(promotion);

        promotionRepoMock
            .Setup(r => r.Update(It.IsAny<Promotion>()))
            .Returns((Promotion p) => p);

        auditMock
            .Setup(a => a.Register(AuditedEntities.Promotion,
                It.IsAny<int>(), AuditDescriptions.PromotionDeleted,
                "admin@dark.com"));

        var service = new PromotionService(promotionRepoMock.Object,
            productRepoMock.Object,
            auditMock.Object, currentUserMock.Object);

        service.Delete(1);

        currentUserMock.Verify(s => s.GetEmail(), Times.Once);
        auditMock.Verify(a => a.Register(AuditedEntities.Promotion,
            It.IsAny<int>(), AuditDescriptions.PromotionDeleted,
            "admin@dark.com"), Times.Once);
    }
}
