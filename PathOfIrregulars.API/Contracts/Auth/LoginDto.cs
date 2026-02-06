namespace PathOfIrregulars.API.Contracts.Auth
{
    public record LoginDto
    {
       public string Username { get; init; }
       public string Password { get; init; }
    }
}
