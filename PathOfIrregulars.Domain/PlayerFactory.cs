using PathOfIrregulars.Domain.Entities;
using PathOfIrregulars.Domain.Enums;



namespace PathOfIrregulars.Application.Services
{
    public static class PlayerFactory
    {

        // converts account to player entity used in actual game logic
        public static Player Create(string name, List<CardInstance> deck)
        {
            return new Player
            {
                Name = name,
                Deck = deck,
                Hand = new List<CardInstance>(),
                Graveyard = new List<CardInstance>(),
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
