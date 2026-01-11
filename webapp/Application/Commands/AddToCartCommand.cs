using Mediator;
using webapp.Models;

namespace webapp.Application.Commands;

public class AddToCartCommand : IRequest<bool>
{
    public CartItem Item { get; set; }

    public AddToCartCommand(CartItem item)
    {
        Item = item;
    }
}

public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, bool>
{
    public AddToCartCommandHandler()
    {
    }

    public async ValueTask<bool> Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        // Cart is handled in session, this is a placeholder for future database persistence
        await Task.CompletedTask;
        return true;
    }
}
