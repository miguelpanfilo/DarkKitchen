using Domain;
using FluentAssertions;
using Mappers;

namespace Tests.Orders;

[TestClass]
public sealed class OrderMapperTests
{
    private static Order MakeOrder(int clientId = 1, int itemCount = 1)
    {
        var address = Address.MakeAddress("Rivera", "1234");

        var items = Enumerable.Range(1, itemCount)
            .Select(i => new OrderItemData
            {
                ProductId = i,
                Quantity = 1,
                UnitPrice = 100m,
                DiscountApplied = 0m
            })
            .ToList();

        return Order.MakeOrder(clientId, DeliveryType.Express, address, items, 200m);
    }

    private static User MakeUser(int id = 1)
    {
        var phoneResult = Phone.TryMakePhone("099123456");
        phoneResult.Success.Should().BeTrue();

        var userResult = User.MakeClient(
            "Miguel",
            "Panfilo",
            $"miguel{id}@test.com",
            phoneResult.Value!,
            "PasswordSeguraee3!");

        userResult.Success.Should().BeTrue();

        var user = userResult.Value!;
        user.Id = id;
        return user;
    }

    #region ToOrderResponse
    #region Success

    [TestMethod]
    public void ToOrderResponse_WhenOrderHasItems_ShouldMapItemsCount()
    {
        var order = MakeOrder(itemCount: 3);
        var user = MakeUser(1);

        var result = order.ToOrderResponse(user);

        result.ItemsCount.Should().Be(3);
    }

    [TestMethod]
    public void ToOrderResponse_WhenOrderHasSingleItem_ShouldMapAllFields()
    {
        var order = MakeOrder(clientId: 1, itemCount: 1);
        var user = MakeUser(1);

        var result = order.ToOrderResponse(user);

        result.Should().NotBeNull();
        result.Client.Id.Should().Be(1);
        result.Client.Name.Should().Be("Miguel");
        result.Client.LastName.Should().Be("Panfilo");
        result.Client.Email.Should().Be("miguel1@test.com");
        result.DeliveryType.Should().Be(nameof(DeliveryType.Express));
        result.Status.Should().Be(nameof(OrderStatus.Pending));
        result.Items.Should().HaveCount(1);
        result.ItemsCount.Should().Be(1);
        result.Subtotal.Should().BeGreaterThan(0);
        result.Total.Should().BeGreaterThan(0);
    }

    [TestMethod]
    public void ToOrderResponse_ShouldMapAddressCorrectly()
    {
        var order = MakeOrder();
        var user = MakeUser();

        var result = order.ToOrderResponse(user);

        result.Address.Street.Should().Be("Rivera");
        result.Address.DoorNumber.Should().Be("1234");
    }

    [TestMethod]
    public void ToOrderResponse_ShouldMapOrderItemsCorrectly()
    {
        var order = MakeOrder(itemCount: 2);
        var user = MakeUser();

        var result = order.ToOrderResponse(user);

        result.Items.Should().HaveCount(2);
        result.Items[0].ProductId.Should().Be(1);
        result.Items[0].Quantity.Should().Be(1);
        result.Items[0].UnitPrice.Should().Be(100m);
        result.Items[0].DiscountApplied.Should().Be(0m);
        result.Items[0].LineTotal.Should().Be(100m);
    }

    #endregion
    #endregion
}
