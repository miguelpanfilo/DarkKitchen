namespace Dominio;

public enum OrderStatus
{
    /// <summary>
    /// Order is pending.
    /// </summary>
    Pending,

    /// <summary>
    /// Order has been prepared.
    /// </summary>
    Prepared,

    /// <summary>
    /// Order is on the way.
    /// </summary>
    OnTheWay,

    /// <summary>
    /// Order has been delivered.
    /// </summary>
    Delivered,

    /// <summary>
    /// Order was not delivered.
    /// </summary>
    NotDelivered,

    /// <summary>
    /// Order has been cancelled.
    /// </summary>
    Cancelled
}
