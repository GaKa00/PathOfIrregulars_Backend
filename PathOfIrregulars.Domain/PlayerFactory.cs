using PathOfIrregulars.Domain.Entities;
using PathOfIrregulars.Domain.Enums;



namespace PathOfIrregulars.Application.Services
{
    public static class PlayerFactory
    {
        public static Player Create(string name, List<Card> deck)
        {
            return new Player
            {
                Name = name,
                Deck = deck,
                Hand = new List<Card>(),
                Graveyard = new List<Card>(),
                Lanes = new Lane[]
                {
                new Lane(LaneType.Vanguard),
                new Lane(LaneType.Midrange),
                new Lane(LaneType.Backline)
                }
            };
        }
    }

}
