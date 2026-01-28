using PathOfIrregulars.Domain.Entities;
using PathOfIrregulars.Infrastructure.Database.Models;
using PathOfIrregulars.Infrastructure.Persistence;

namespace PathOfIrregulars.API.Mappers
{
    public static class DeckMapper
    {
        public static List<CardInstance> ToCardInstances(
            List<Infrastructure.Database.Models.Card> dbCards,
            CardRepository cardRepo
        )
        {
            var result = new List<CardInstance>();

            foreach (var dbCard in dbCards)
            {
                var definition = cardRepo.GetCardById(dbCard.CardId);

                for (int i = 0; i < dbCard.Amount; i++)
                {
                    result.Add(new CardInstance
                    {
                        Definition = definition,
                        Power = definition.Power
                    });
                }
            }

            return result;
        }
    }
}

