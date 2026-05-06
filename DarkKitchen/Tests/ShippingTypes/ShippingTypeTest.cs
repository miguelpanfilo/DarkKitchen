using Domain;
using Domain.Exceptions;

namespace Tests.ShippingTypes;

[TestClass]
public class ShippingTypeTest
{
    private static ShippingType CreateValidShippingType()
    {
        return ShippingType.MakeShippingType("Envío express", 150m);
    }

    [TestMethod]
    public void GivenValidData_WhenCreatingShippingType_ThenShippingTypeIsCreated()
    {
        var shippingType = ShippingType.MakeShippingType("Envío express", 150m);

        Assert.IsNotNull(shippingType);
        Assert.AreEqual("Envío express", shippingType.Name);
        Assert.AreEqual(150m, shippingType.Cost);
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow("   ")]
    public void GivenEmptyOrWhitespaceName_WhenCreatingShippingType_ThenThrowsShippingTypeException(string invalidName)
    {
        var ex = Assert.ThrowsException<ShippingTypeException>(() =>
            ShippingType.MakeShippingType(invalidName, 150m));

        Assert.AreEqual("El nombre del tipo de envío no puede estar vacío.", ex.Message);
    }

    [TestMethod]
    public void GivenZeroCost_WhenCreatingShippingType_ThenShippingTypeIsCreated()
    {
        var shippingType = ShippingType.MakeShippingType("Envío gratis", 0m);

        Assert.IsNotNull(shippingType);
        Assert.AreEqual(0m, shippingType.Cost);
    }

    [TestMethod]
    public void GivenNegativeCost_WhenCreatingShippingType_ThenThrowsShippingTypeException()
    {
        var ex = Assert.ThrowsException<ShippingTypeException>(() =>
            ShippingType.MakeShippingType("Envío express", -1m));

        Assert.AreEqual("El costo del tipo de envío no puede ser negativo.", ex.Message);
    }

    [TestMethod]
    public void GivenValidName_WhenUpdatingName_ThenNameIsUpdated()
    {
        var shippingType = CreateValidShippingType();

        shippingType.UpdateName("Envío estándar");

        Assert.AreEqual("Envío estándar", shippingType.Name);
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow("   ")]
    public void GivenEmptyOrWhitespaceName_WhenUpdatingName_ThenThrowsShippingTypeException(string invalidName)
    {
        var shippingType = CreateValidShippingType();

        var ex = Assert.ThrowsException<ShippingTypeException>(() =>
            shippingType.UpdateName(invalidName));

        Assert.AreEqual("El nombre del tipo de envío no puede estar vacío.", ex.Message);
    }

    [TestMethod]
    public void GivenValidCost_WhenUpdatingCost_ThenCostIsUpdated()
    {
        var shippingType = CreateValidShippingType();

        shippingType.UpdateCost(200m);

        Assert.AreEqual(200m, shippingType.Cost);
    }

    [TestMethod]
    public void GivenZeroCost_WhenUpdatingCost_ThenCostIsUpdated()
    {
        var shippingType = CreateValidShippingType();

        shippingType.UpdateCost(0m);

        Assert.AreEqual(0m, shippingType.Cost);
    }

    [TestMethod]
    public void GivenNegativeCost_WhenUpdatingCost_ThenThrowsShippingTypeException()
    {
        var shippingType = CreateValidShippingType();

        var ex = Assert.ThrowsException<ShippingTypeException>(() =>
            shippingType.UpdateCost(-50m));

        Assert.AreEqual("El costo del tipo de envío no puede ser negativo.", ex.Message);
    }
}
