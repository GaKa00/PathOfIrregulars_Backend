namespace PathOfIrregulars.API2.Contracts
{
    public class AccountDto ( int id, string username, int elo, List<DeckDto> decks)
    {

        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public int Elo { get; set; } = 1000;
        public List<DeckDto> Decks { get; set; } = new List<DeckDto>();
    }
}
