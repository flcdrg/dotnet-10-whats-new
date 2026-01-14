namespace webapp.Models;

public class ShoppingCart
{
    private List<CartItem> _items = [];
    private string _cartId = string.Empty;

    public int Id { get; set; }

    public string CartId
    {
        get { return _cartId; }
        set { _cartId = string.IsNullOrEmpty(value) ? string.Empty : value; }
    }

    public List<CartItem> Items
    {
        get { return _items; }
        set { _items = value ?? new(); }
    }

    public DateTime CreatedAt { get; set; }

    public DateTime LastModifiedAt { get; set; }

    public int GetItemCount()
    {
        return _items.Sum(item => item.Quantity);
    }

    public decimal GetSubtotal()
    {
        return _items.Sum(item => item.GetLineTotal());
    }

    public void AddItem(CartItem item)
    {
        var existingItem = _items.FirstOrDefault(ci => ci.PetId == item.PetId);
        if (existingItem != null)
        {
            existingItem.Quantity += item.Quantity;
        }
        else
        {
            _items.Add(item);
        }
        LastModifiedAt = DateTime.UtcNow;
    }

    public void RemoveItem(int petId)
    {
        _items.RemoveAll(ci => ci.PetId == petId);
        LastModifiedAt = DateTime.UtcNow;
    }

    public void ClearCart()
    {
        _items.Clear();
        LastModifiedAt = DateTime.UtcNow;
    }
}
