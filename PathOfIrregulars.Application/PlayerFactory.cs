using PathOfIrregulars.Domain.Entities;
using PathOfIrregulars.Application.Contracts;

using PathOfIrregulars.Domain.Enums;



namespace PathOfIrregulars.Application.Services
{


    namespace PathOfIrregulars.Application.Services
    {
        public class PlayerFactory
        {
            private readonly ICardRepository _cards;

            public PlayerFactory(ICardRepository cards)
            {
                _cards = cards;
            }

            public Player Create(AccountDto account, DeckDto deck)
            {

                var deckCards = deck.CardIds
                    .Select(id => _cards.GetCard(id))
                    .ToList();

                return new Player(account.Username, deckCards);
            }
        }
    }


}
