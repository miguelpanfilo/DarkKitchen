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
        Assert.AreNotEqual(Guid.Empty, shippingType.Id);
        Assert.AreEqual("Envío express", shippingType.Name);
        Assert.AreEqual(150m, shippingType.Cost);
    }

    [TestMethod]
    public void GivenTwoShippingTypes_WhenCreated_ThenHaveDifferentIds()
    {
        var first = ShippingType.MakeShippingType("Envío express", 150m);
        var second = ShippingType.MakeShippingType("Envío estándar", 80m);

        Assert.AreNotEqual(first.Id, second.Id);
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

    [DataTestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    public void GivenInvalidCost_WhenCreatingShippingType_ThenThrowsShippingTypeException(int invalidCost)
    {
        var ex = Assert.ThrowsException<ShippingTypeException>(() =>
            ShippingType.MakeShippingType("Envío express", invalidCost));

        Assert.AreEqual("El costo del tipo de envío debe ser mayor a 0.", ex.Message);
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

    [DataTestMethod]
    [DataRow(0.0)]
    [DataRow(-50.0)]
    public void GivenInvalidCost_WhenUpdatingCost_ThenThrowsShippingTypeException(double invalidCost)
    {
        var shippingType = CreateValidShippingType();

        var ex = Assert.ThrowsException<ShippingTypeException>(() =>
            shippingType.UpdateCost((decimal)invalidCost));

        Assert.AreEqual("El costo del tipo de envío debe ser mayor a 0.", ex.Message);
    }
}
