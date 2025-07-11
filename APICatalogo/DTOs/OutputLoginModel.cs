namespace APICatalogo.DTOs;

public class OutputLoginModel
{
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime Expiration { get; set; }

}
