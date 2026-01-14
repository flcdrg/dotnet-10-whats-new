using Mediator;
using webapp.Models;

namespace webapp.Application.Commands;

public class ProcessCheckoutCommand : IRequest<Order>
{
    public Order Order { get; set; } = new();
    public List<CartItem> Items { get; set; } = new();
    public string Country { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ShippingMethod { get; set; } = string.Empty;
}
