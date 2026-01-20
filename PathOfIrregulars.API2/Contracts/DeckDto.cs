namespace PathOfIrregulars.API2.Contracts
{
    public class DeckDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<CardDto> Cards { get; set; } = new();
    }
}
