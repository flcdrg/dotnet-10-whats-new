using Mediator;
using webapp.Models;

namespace webapp.Application.Commands;

public class ValidateCheckoutCommand : IRequest<ValidateCheckoutResult>
{
    public Order Order { get; set; } = new();
    public string Country { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ShippingMethod { get; set; } = string.Empty;
    public List<CartItem> Items { get; set; } = new();
}

public class ValidateCheckoutResult
{
    public bool IsValid { get; set; }
    public Dictionary<string, string> Errors { get; set; } = new();
}
