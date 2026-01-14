namespace webapp.Models;

public class Pet
{
    private string _name = string.Empty;
    private string _description = string.Empty;
    private decimal _price;
    private string _imageUrl = string.Empty;
    private int _stockQuantity;
    private string _species = string.Empty;

    public int Id { get; set; }

    public string Name
    {
        get { return _name; }
        set { _name = string.IsNullOrEmpty(value) ? string.Empty : value; }
    }

    public string Description
    {
        get { return _description; }
        set { _description = string.IsNullOrEmpty(value) ? string.Empty : value; }
    }

    public decimal Price
    {
        get { return _price; }
        set { _price = value >= 0 ? value : 0; }
    }

    public string ImageUrl
    {
        get { return _imageUrl; }
        set { _imageUrl = string.IsNullOrEmpty(value) ? string.Empty : value; }
    }

    public int StockQuantity
    {
        get { return _stockQuantity; }
        set { _stockQuantity = value >= 0 ? value : 0; }
    }

    public string Species
    {
        get { return _species; }
        set { _species = string.IsNullOrEmpty(value) ? string.Empty : value; }
    }

    public DateTime CreatedAt { get; set; }
}
