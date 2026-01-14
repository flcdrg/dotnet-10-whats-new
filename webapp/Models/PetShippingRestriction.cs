namespace webapp.Models;

public class PetShippingRestriction
{
    public int Id { get; set; }

    public int PetId { get; set; }

    public int CountryId { get; set; }

    public Pet? Pet { get; set; }

    public Country? Country { get; set; }
}
