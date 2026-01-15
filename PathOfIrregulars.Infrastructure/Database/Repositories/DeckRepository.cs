using PathOfIrregulars.Infrastructure.Database.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PathOfIrregulars.Application.Contracts;
using Microsoft.EntityFrameworkCore;

namespace PathOfIrregulars.Infrastructure.Database.Repositories
{
    public class DeckRepository : IDeckRepository
    {

        private readonly GameDbContext _context;

        public DeckRepository(GameDbContext context)
        {
            _context = context;
        }

        public DeckDto GetDeck(Guid deckId)
        {
            var deck = _context.Decks
                .Include(d => d.Cards)
                .First(d => d.Id == deckId);

            return new DeckDto
            {
                Id = deck.Id,
                Name = deck.Name,
                CardIds = deck.Cards.Select(dc => dc.CardId).ToList()
            };
        }


    }
}
