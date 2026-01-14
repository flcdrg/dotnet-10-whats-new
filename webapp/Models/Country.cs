namespace webapp.Models;

public class Country
{
    private string _code = string.Empty;
    private string _name = string.Empty;

    public int Id { get; set; }

    public string Code
    {
        get => _code;
        set => _code = string.IsNullOrEmpty(value) ? string.Empty : value;
    }

    public string Name
    {
        get => _name;
        set => _name = string.IsNullOrEmpty(value) ? string.Empty : value;
    }
}
