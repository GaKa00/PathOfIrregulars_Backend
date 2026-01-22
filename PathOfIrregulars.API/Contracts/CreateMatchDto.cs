using PathOfIrregulars.Application;

namespace PathOfIrregulars.API.Contracts
{
    public class CreateMatchDto
    {
        public int playerOneId { get; set; }
        public int playerTwoId { get; set; }
        public DeckDto playerOneDeck { get; set; } = new();
        public DeckDto playerTwoDeck { get; set; } = new();
    }
}
