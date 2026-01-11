namespace webapp.Models;

public class AustralianShippingRate
{
    private int _id;
    private string _state = string.Empty;
    private string _shippingMethod = string.Empty; // "AustraliaPost" or "Courier"
    private decimal _rate;

    public int Id
    {
        get { return _id; }
        set { _id = value; }
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

    public decimal Rate
    {
        get { return _rate; }
        set { _rate = value >= 0 ? value : 0; }
    }
}
