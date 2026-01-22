namespace PathOfIrregulars.API.Contracts
{
    public class CreateDeckDto
    {
        public string Name { get; set; } = string.Empty;
        public List<string> CardIds { get; set; } = new();
    }
}
