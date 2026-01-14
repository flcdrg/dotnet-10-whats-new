namespace webapp.Models;

public class Order
{
    private string _orderNumber = string.Empty;
    private List<CartItem> _items = new();
    private string _customerName = string.Empty;
    private string _customerEmail = string.Empty;
    private string _shippingAddress = string.Empty;
    private string _country = string.Empty;
    private string _state = string.Empty; // For Australia
    private string _shippingMethod = string.Empty;
    private decimal _subtotal;
    private decimal _shippingCost;
    private decimal _gstAmount;
    private decimal _total;
    private string _status = "Pending"; // Pending, Processing, Shipped, Delivered

    public int Id { get; set; }

    public string OrderNumber
    {
        get { return _orderNumber; }
        set { _orderNumber = string.IsNullOrEmpty(value) ? string.Empty : value; }
    }

    public List<CartItem> Items
    {
        get { return _items; }
        set { _items = value ?? new(); }
    }

    public string CustomerName
    {
        get { return _customerName; }
        set { _customerName = string.IsNullOrEmpty(value) ? string.Empty : value; }
    }

    public string CustomerEmail
    {
        get { return _customerEmail; }
        set { _customerEmail = string.IsNullOrEmpty(value) ? string.Empty : value; }
    }

    public string ShippingAddress
    {
        get { return _shippingAddress; }
        set { _shippingAddress = string.IsNullOrEmpty(value) ? string.Empty : value; }
    }

    public string Country
    {
        get { return _country; }
        set { _country = string.IsNullOrEmpty(value) ? string.Empty : value; }
    }

    public string State
    {
        get { return _state; }
        set { _state = string.IsNullOrEmpty(value) ? string.Empty : value; }
    }

    public string ShippingMethod
    {
        get { return _shippingMethod; }
        set { _shippingMethod = string.IsNullOrEmpty(value) ? string.Empty : value; }
    }

    public decimal Subtotal
    {
        get { return _subtotal; }
        set { _subtotal = value >= 0 ? value : 0; }
    }

    public decimal ShippingCost
    {
        get { return _shippingCost; }
        set { _shippingCost = value >= 0 ? value : 0; }
    }

    public decimal GstAmount
    {
        get { return _gstAmount; }
        set { _gstAmount = value >= 0 ? value : 0; }
    }

    public decimal Total
    {
        get { return _total; }
        set { _total = value >= 0 ? value : 0; }
    }

    public DateTime CreatedAt { get; set; }

    public DateTime LastModifiedAt { get; set; }

    public string Status
    {
        get { return _status; }
        set { _status = string.IsNullOrEmpty(value) ? "Pending" : value; }
    }
}
