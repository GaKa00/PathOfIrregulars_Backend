namespace PathOfIrregulars.API.Contracts.Auth
{
    public record RegisterDto
    {
        public string Username { get; init; }
       public  string Password { get; init; }
    }
}
