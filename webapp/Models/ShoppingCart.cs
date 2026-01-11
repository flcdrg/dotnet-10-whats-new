namespace webapp.Models;

public class ShoppingCart
{
    private int _id;
    private List<CartItem> _items = new();
    private string _cartId = string.Empty;
    private DateTime _createdAt;
    private DateTime _lastModifiedAt;

    public int Id
    {
        get { return _id; }
        set { _id = value; }
    }

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

    public DateTime CreatedAt
    {
        get { return _createdAt; }
        set { _createdAt = value; }
    }

    public DateTime LastModifiedAt
    {
        get { return _lastModifiedAt; }
        set { _lastModifiedAt = value; }
    }

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
        _lastModifiedAt = DateTime.UtcNow;
    }

    public void RemoveItem(int petId)
    {
        _items.RemoveAll(ci => ci.PetId == petId);
        _lastModifiedAt = DateTime.UtcNow;
    }

    public void ClearCart()
    {
        _items.Clear();
        _lastModifiedAt = DateTime.UtcNow;
    }
}
