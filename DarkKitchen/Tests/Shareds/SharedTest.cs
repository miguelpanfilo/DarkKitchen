using BusinessLogics;
using BusinessLogicsAbstractions.Exceptions;
using Domain.Exceptions;

namespace Tests.Shareds;

[TestClass]
public class SharedTest
{
    [TestMethod]
    public void GivenValidAction_WhenExecutingWithConflictHandling_ThenDoesNotThrow()
    {
        ServiceExceptionHandler.ExecuteWithConflictHandling<ProductException>(() => { });

        Assert.IsTrue(true);
    }

    [TestMethod]
    public void GivenProductException_WhenExecutingWithConflictHandling_ThenThrowsConflictException()
    {
        var exception = Assert.ThrowsException<ConflictException>(() =>
            ServiceExceptionHandler.ExecuteWithConflictHandling<ProductException>(() =>
                throw new ProductException("Error de producto")));

        Assert.AreEqual("Error de producto", exception.Message);
    }

    [TestMethod]
    public void GivenWatchSystem_WhenGettingNow_ThenReturnsCurrentDateTime()
    {
        var watch = new WatchSystem();
        var uruguayZone = TimeZoneInfo.FindSystemTimeZoneById("America/Montevideo");

        var before = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
            uruguayZone);
        var result = watch.Now();
        var after = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
            uruguayZone);

        Assert.IsTrue(result >= before);
        Assert.IsTrue(result <= after);
    }
}
