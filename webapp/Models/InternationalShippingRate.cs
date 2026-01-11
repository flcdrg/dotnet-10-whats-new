namespace webapp.Models;

public class InternationalShippingRate
{
    private int _id;
    private string _country = string.Empty; // "UK", "NewZealand", "Antarctica"
    private decimal _rate1To10Kg;
    private decimal _rateOver10Kg;

    public int Id
    {
        get { return _id; }
        set { _id = value; }
    }

    public string Country
    {
        get { return _country; }
        set { _country = string.IsNullOrEmpty(value) ? string.Empty : value; }
    }

    public decimal Rate1To10Kg
    {
        get { return _rate1To10Kg; }
        set { _rate1To10Kg = value >= 0 ? value : 0; }
    }

    public decimal RateOver10Kg
    {
        get { return _rateOver10Kg; }
        set { _rateOver10Kg = value >= 0 ? value : 0; }
    }

    public decimal GetRate(decimal weightKg)
    {
        return weightKg <= 10 ? _rate1To10Kg : _rateOver10Kg;
    }
}
