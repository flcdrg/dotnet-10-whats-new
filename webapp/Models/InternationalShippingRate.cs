namespace webapp.Models;

public class InternationalShippingRate
{
    private decimal _rate1To10Kg;
    private decimal _rateOver10Kg;

    public int Id { get; set; }

    public int CountryId { get; set; }

    public Country? Country { get; set; }

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
