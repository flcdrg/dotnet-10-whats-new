namespace webapp.Models;

public class CartItem
{
    private int _id;
    private int _petId;
    private string _petName = string.Empty;
    private decimal _petPrice;
    private int _quantity;
    private string _imageUrl = string.Empty;

    public int Id
    {
        get { return _id; }
        set { _id = value; }
    }

    public int PetId
    {
        get { return _petId; }
        set { _petId = value; }
    }

    public string PetName
    {
        get { return _petName; }
        set { _petName = string.IsNullOrEmpty(value) ? string.Empty : value; }
    }

    public decimal PetPrice
    {
        get { return _petPrice; }
        set { _petPrice = value >= 0 ? value : 0; }
    }

    public int Quantity
    {
        get { return _quantity; }
        set { _quantity = value >= 0 ? value : 0; }
    }

    public string ImageUrl
    {
        get { return _imageUrl; }
        set { _imageUrl = string.IsNullOrEmpty(value) ? string.Empty : value; }
    }

    public decimal GetLineTotal()
    {
        return _petPrice * _quantity;
    }
}
